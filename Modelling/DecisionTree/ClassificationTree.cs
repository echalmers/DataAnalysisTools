using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelling.DecisionTree
{
    [Serializable()]
    public class ClassificationTree : LearnerInterface
    {
        BranchNode root = new BranchNode();

        /// <summary>
        /// Maximum number of branches allowed from a single branch node
        /// </summary>
        public int MaxBranches
        {
            set { root.MaxBranches = value; }
        }

        /// <summary>
        /// Minimum number of instances required to support a branch. If fewer instances are available, the growSubtree algorithm will terminate this branch with a leaf node.
        /// </summary>
        public int MinBranchNodeSupport
        {
            set { root.MinBranchNodeSupport = value; }
        }

        /// <summary>
        /// The maximum number of test thresholds to apply to continuous variables
        /// </summary>
        public int MaxTestThresholds
        {
            set { root.MaxTestThresholds = value; }
        }

        /// <summary>
        /// The maximum depth (counting the root node as depth zero) of the tree
        /// </summary>
        public int MaxTreeDepth
        {
            set { root.MaxTreeDepth = value; }
        }

        double errorCI;
        /// <summary>
        /// The binomial distribution confidence interval used during C4.5-style pruning
        /// </summary>
        public double ErrorCI
        {
            get { return errorCI; }
            set { errorCI = value; }
        }

        /// <summary>
        /// A list of the feature indexes allowed for consideration as split features in the decision tree
        /// </summary>
        public List<int> availableFeatures = new List<int>();
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MaximumBranches">Maximum number of branches allowed from a single branch node</param>
        /// <param name="MinimumBranchNodeSupport">Minimum number of instances required to support a branch. If fewer instances are available, the growSubtree algorithm will terminate this branch with a leaf node.</param>
        /// <param name="MaximumTestThresholds">The maximum number of test thresholds to apply to continuous variables</param>
        /// <param name="MaximumTreeDepth">The maximum depth (counting the root node as depth zero) of the tree</param>
        public ClassificationTree(int MaximumBranches, int MinimumBranchNodeSupport, int MaximumTestThresholds, int MaximumTreeDepth)
        {
            root.MaxBranches = MaximumBranches;
            root.MinBranchNodeSupport = MinimumBranchNodeSupport;
            root.MaxTestThresholds = MaximumTestThresholds;
            root.MaxTreeDepth = MaximumTreeDepth;
        }

        /// <summary>
        /// Train the classification tree model
        /// </summary>
        /// <param name="trainingX">Training instances</param>
        /// <param name="trainingY">Class assignments associated with the training instances</param>
        public void train(double[][] trainingX, double[] trainingY)
        {
            train(trainingX, trainingY, new string[trainingX[0].Length]);
        }

        /// <summary>
        /// Train the classification tree model
        /// </summary>
        /// <param name="trainingX">Training instances</param>
        /// <param name="trainingY">Class assignments associated with the training instances</param>
        /// <param name="featureNames">Names of the features in the training data</param>
        public void train(double[][] trainingX, double[] trainingY, string[] featureNames)
        {
            if (availableFeatures.Count == 0)
                root.GrowSubtree(trainingX, trainingY, featureNames);
            else
                root.GrowSubtree(trainingX, trainingY, featureNames, availableFeatures);

            root.PruneSubtree(errorCI);
        }

        /// <summary>
        /// Predict probability of class "1" for test instances
        /// </summary>
        /// <param name="X">Test instances</param>
        public double[] predict(double[][] X)
        {
            double[] predictions = new double[X.Length];
            for (int i=0; i<predictions.Length; i++)
            {
                ClassProbabilities cp = root.Evaluate(X[i]);
                predictions[i] = cp.ProbabilityOf(1);
            }
            return predictions;
        }

        /// <summary>
        /// Predict class probability for a single test instance
        /// </summary>
        /// <param name="X">test instance</param>
        public ClassProbabilities predict(double[] X)
        {
            return root.Evaluate(X);
        }

        /// <summary>
        /// Create a (deep) copy of this classification tree object 
        /// </summary>
        public LearnerInterface Copy()
        {
            ClassificationTree newTree = new ClassificationTree(root.MaxBranches, root.MinBranchNodeSupport, root.MaxTestThresholds, root.MaxTreeDepth);
            newTree.root = (BranchNode)root.Copy();
            newTree.errorCI = errorCI;
            newTree.availableFeatures = new List<int>(availableFeatures);
            return newTree;
        }

        /// <summary>
        /// Returns a human-readable string representation of the tree
        /// </summary>
        public string PrintTree()
        {
            string tree = "";
            List<string> nodes = root.PrintTree();
            foreach (string s in nodes)
            {
                tree += s + Environment.NewLine;
            }
            return tree;
        }
    }
}
