using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelling.DecisionTree
{
    // ******* Comments coming soon! ********************** 
    [Serializable()]
    public class RandomForest : LearnerInterface
    {
        public ClassificationTree[] forest { get; set; }
        int rndSeed;
        Random rnd;
        int nRndFeatures;
        public List<int> availableFeatures = new List<int>();

        int maxBranches;
        /// <summary>
        /// Maximum number of branches allowed from a single branch node
        /// </summary>
        public int MaxBranches
        {
            get { return maxBranches; }
            set { maxBranches = value; }
        }

        int minBranchNodeSupport;
        /// <summary>
        /// Minimum number of instances required to support a branch. If fewer instances are available, the growSubtree algorithm will terminate this branch with a leaf node.
        /// </summary>
        public int MinBranchNodeSupport
        {
            get { return minBranchNodeSupport; }
            set { minBranchNodeSupport = value; }
        }

        int maxTestThresholds;
        /// <summary>
        /// The maximum number of test thresholds to apply to continuous variables
        /// </summary>
        public int MaxTestThresholds
        {
            get { return maxTestThresholds; }
            set { maxTestThresholds = value; }
        }

        int maxTreeDepth;
        /// <summary>
        /// The maximum depth (counting the root node as depth zero) of the tree
        /// </summary>
        public int MaxTreeDepth
        {
            get { return maxTreeDepth; }
            set { maxTreeDepth = value; }
        }


        public RandomForest(int forestSize, int numRndFeatures, int RndSeed, int MaximumBranches, int MinimumBranchNodeSupport, int MaximumTestThresholds, int MaximumTreeDepth)
        {
            rndSeed = RndSeed;
            rnd = new Random(rndSeed);
            nRndFeatures = numRndFeatures;

            maxBranches = MaximumBranches;
            minBranchNodeSupport = MinimumBranchNodeSupport;
            maxTestThresholds = MaximumTestThresholds;
            maxTreeDepth = MaximumTreeDepth;

            // initialize the forest
            forest = new ClassificationTree[forestSize];
            //for (int i=0; i<forestSize; i++)
            //{
            //    forest[i] = new ClassificationTree();
            //}

            
        }

        public void train(double[][] trainingX, double[] trainingY)
        {
            string[] featureNames = new string[trainingX[0].Length];
            for(int i=0; i<featureNames.Length; i++)
            {
                featureNames[i] = i.ToString();
            }
            train(trainingX, trainingY, featureNames);
        }

        public void train(double[][] trainingX, double[] trainingY, string[] featureNames)
        {
            int numFeatures = trainingX[0].Length;
            int numInstances = trainingX.Length;

            if (availableFeatures.Count==0)
            {
                for (int i = 0; i < numFeatures; i++)
                {
                    availableFeatures.Add(i);
                }
            }

            for (int t = 0; t<forest.Length; t++)
            {
                // choose random features
                List<int> unselectedFeatures = new List<int>(availableFeatures);
                List<int> selectedFeatures = new List<int>();
                for (int i=0; i<Math.Min(numFeatures,nRndFeatures); i++)
                {
                    int index = rnd.Next(unselectedFeatures.Count);
                    selectedFeatures.Add(unselectedFeatures[index]);
                    unselectedFeatures.RemoveAt(index);
                }

                // create a bagged dataset
                double[][] baggedX = new double[numInstances][];
                double[] baggedY = new double[numInstances];
                for (int i=0; i<numInstances; i++)
                {
                    int index = rnd.Next(numInstances);
                    baggedX[i] = trainingX[i];//ndex];
                    baggedY[i] = trainingY[i];//ndex];
                }

                // train the tree
                forest[t] = new ClassificationTree(maxBranches, minBranchNodeSupport, maxTestThresholds, maxTreeDepth);
                forest[t].availableFeatures = selectedFeatures;
                forest[t].train(baggedX, baggedY, featureNames);
            }
        }


        public double[] predict(double[][] X) // maybe try getting double[] predictions from each tree instead, and combine those instead of combining classProbabilities
        {
            double[] predictions = new double[X.Length];

            for (int i = 0; i < predictions.Length; i++)
            {
                ClassProbabilities p = new ClassProbabilities();
                foreach (ClassificationTree tree in forest)
                {
                    p = p.Combine(tree.predict(X[i]));
                }
                predictions[i] = p.ProbabilityOf(1);
            }
            return predictions;
        }

        /// <summary>
        /// Get the predictions of every tree in the forest
        /// </summary>
        /// <param name="X">The instances to be classified</param>
        /// <returns>the complete predictions in a double[][], where element [n][m] is the nth tree's prediction for the mth instance</returns>
        public double[][] allPredictions(double[][] X)
        {
            double[][] predictions = new double[forest.Length][];

            for (int i=0; i<forest.Length; i++)
            {
                predictions[i] = forest[i].predict(X);
            }

            return predictions;
        }

        /// <summary>
        /// Method intended to produce a copy of a learner
        /// The leaner's thread safety may depend on whether this is a deep or a shallow copy
        /// </summary>
        /// <returns>A copy of the object</returns>
        public LearnerInterface Copy()
        {
            return new RandomForest(forest.Length, nRndFeatures, rndSeed + 1, maxBranches, minBranchNodeSupport, maxTestThresholds, maxTreeDepth);
        }

    }
}
