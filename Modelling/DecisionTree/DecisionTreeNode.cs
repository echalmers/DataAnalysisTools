using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Modelling.DecisionTree
{
    /// <summary>
    /// Interface from which other types of classification tree nodes derive
    /// </summary>
    public interface ClassificationTreeNode
    {
        /// <summary>
        /// The total number of nodes (including this one) in this node's subtree
        /// </summary>
        int TreeSize();

        /// <summary>
        /// Use this node and its subtree to infer the class probabilities associated with a given instance
        /// </summary>
        /// <param name="testX">The instance for which class probabilities should be inferred</param>
        /// <returns>A ClassProbabilities object representing the inferred class probability</returns>
        ClassProbabilities Evaluate(double[] testX);

        /// <summary>
        /// Returns a list of all the leaf nodes in this node's subtree
        /// </summary>
        List<LeafNode> AllDescendantLeaves();

        /// <summary>
        /// Perform pruning of the classification tree in the C4.5 style
        /// </summary>
        /// <param name="errorCI">(optional) The confidence interval of classification error to use in pruning</param>
        void PruneSubtree(double errorCI = 0.5);

        /// <summary>
        /// Converts this node and its subtree into a human-readable string
        /// </summary>
        /// <returns></returns>
        List<string> PrintTree();

        /// <summary>
        /// Create a copy of this ClassificationTree object
        /// </summary>
        /// <returns></returns>
        ClassificationTreeNode Copy();
    }

    /// <summary>
    /// Class representing a classification tree leaf node
    /// </summary>
    [Serializable()]
    public class LeafNode: ClassificationTreeNode
    {
        int nodeDepth;
        ClassProbabilities p;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trainingY">The outputs (i.e. predicted variables) for the training instances associated with this leaf node </param>
        /// <param name="NodeDepth">The depth level of this leaf node</param>
        public LeafNode(double[] trainingY, int NodeDepth)
        {
            nodeDepth = NodeDepth;
            p = new ClassProbabilities(trainingY);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="classProbability">a classProbability object describing the class probabilities at this leaf node</param>
        /// <param name="NodeDepth">The depth level of this leaf node</param>
        public LeafNode(ClassProbabilities classProbability, int NodeDepth)
        {
            nodeDepth = NodeDepth;
            p = classProbability;
        }

        /// <summary>
        /// The total number of nodes (including this one) in this node's subtree
        /// </summary>
        public int TreeSize()
        {
            return 1;
        }

        /// <summary>
        /// Infer the class probabilities associated with a given instance
        /// </summary>
        /// <param name="testX">The instance for which class probabilities should be inferred</param>
        /// <returns>a ClassProbabilities object for the leaf node</returns>
        public ClassProbabilities Evaluate(double[] testX)
        {
            return p;
        }

        /// <summary>
        /// Return the class probabilities associated with this leaf node
        /// </summary>
        public ClassProbabilities Evaluate()
        {
            return p;
        }

        /// <summary>
        /// Perform pruning of the classification tree in the C4.5 style (not applicable to leaf nodes)
        /// </summary>
        public void PruneSubtree(double errorCI = 0.5)
        {
        }

        /// <summary>
        /// Return the list of all leaf nodes in this node's subtree. For leaf nodes, simply return 'this'.
        /// </summary>
        /// <returns></returns>
        public List<LeafNode> AllDescendantLeaves()
        {
            List<LeafNode> thisLeaf = new List<LeafNode>();
            thisLeaf.Add(this);
            return thisLeaf;
        }

        public override string ToString()
        {
            return p.ToString();
        }

        /// <summary>
        /// Return a human-readable string version of this leaf node's class probabilities
        /// </summary>
        /// <returns></returns>
        public List<string> PrintTree()
        {
            List<string> printedNode = new List<string>();
            printedNode.Add(p.ToString());
            return printedNode;
        }

        /// <summary>
        /// Create a copy of the ClassificationTreeNode object
        /// </summary>
        /// <returns></returns>
        public ClassificationTreeNode Copy()
        {
            ClassificationTreeNode newNode = new LeafNode(new ClassProbabilities(p), nodeDepth);
            return newNode;
        }
    }

    /// <summary>
    /// Class representing a classification tree branch node
    /// </summary>
    [Serializable()]
    public class BranchNode : ClassificationTreeNode
    {
        #region attributes
        int nodeDepth;
        string[] featureNames;
        BranchingFunction branchingFunction;

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

        ClassificationTreeNode[] children;
        /// <summary>
        /// Array of this node's child nodes
        /// </summary>
        public ClassificationTreeNode[] Children
        {
            get { return children; }
            set { children = value; }
        }
        #endregion

        
        #region constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BranchNode()
        {
            nodeDepth = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BranchNode(int NodeDepth, int MaximumBranches, int MinimumBranchNodeSupport, int MaximumTestThresholds, int MaximumTreeDepth)
        {
            nodeDepth = NodeDepth;
            maxBranches = MaximumBranches;
            minBranchNodeSupport = MinimumBranchNodeSupport;
            maxTestThresholds = MaximumTestThresholds;
            maxTreeDepth = MaximumTreeDepth;
        }
        #endregion

        /// <summary>
        /// Grow a subtree from this branch node using the information gain criteria
        /// </summary>
        /// <param name="TrainingX">Training instances</param>
        /// <param name="TrainingY">Classes associated with the training instances</param>
        /// <param name="FeatureNames">Names of each feature in the training data</param>
        public void GrowSubtree(double[][] TrainingX, double[] TrainingY, string[] FeatureNames)
        {
            List<int> availableFeatures = new List<int>();
            for (int i=0; i<FeatureNames.Length; i++)
            {
                availableFeatures.Add(i);
            }
            GrowSubtree(TrainingX, TrainingY, FeatureNames, availableFeatures);
        }

        /// <summary>
        /// Grow a subtree from this branch node using the information gain criteria
        /// </summary>
        /// <param name="TrainingX">Training instances</param>
        /// <param name="TrainingY">Classes associated with the training instances</param>
        /// <param name="FeatureNames">Names of each feature in the training data</param>
        /// <param name="availableFeatures">A list of feature indexes which are allowed for consideration as split features</param>
        public void GrowSubtree(double[][] TrainingX, double[] TrainingY, string[] FeatureNames, List<int> availableFeatures)
        {
            featureNames = FeatureNames;

            double globalEntropy = H(TrainingX, TrainingY);

            // consider each feature
            int bestFeature = 0;
            double? bestObScore = null;
            foreach (int splitFeature in availableFeatures) //for (int splitFeature = 0; splitFeature < TrainingX[0].Length; splitFeature++)
            {
                //Console.WriteLine("testing feature " + featureNames[splitFeature]);
                // list all values of this feature
                List<double> values = new List<double>();
                foreach (double[] instance in TrainingX)
                {
                    values.Add(instance[splitFeature]);
                }

                // is this feature discrete or continuous?
                List<double> distinctValues = values.Distinct().ToList();

                if (distinctValues.Count <= maxBranches) // if the feature is discrete
                {
                    DiscreteValueBranchingFunction testBranchingFunc = new DiscreteValueBranchingFunction(distinctValues, splitFeature, featureNames[splitFeature]);
                    
                    double splitInfo;
                    double infoGain = globalEntropy - H(TrainingX, TrainingY, testBranchingFunc, out splitInfo);
                    double ob = infoGain == 0 ? 0 : infoGain / splitInfo;

                    //Console.WriteLine("Feature " + splitFeature + " gainRatio: " + ob);
                    if (ob > bestObScore || bestObScore == null)
                    {
                        bestFeature = splitFeature; bestObScore = ob;
                        branchingFunction = new DiscreteValueBranchingFunction(testBranchingFunc);
                    }
                }
                else // if the feature is continuous
                {
                    //if (distinctValues.Count > maxTestThresholds)
                        distinctValues = continuousValueThresholds(values, maxTestThresholds);
                    //else
                    //    distinctValues.Remove(distinctValues.Min());
                    //Console.WriteLine("    testing " + distinctValues.Count + " thresholds");
                    foreach(double threshold in distinctValues)
                    {
                        ContinuousValueBranchFunction testBranchingFunc = new ContinuousValueBranchFunction(threshold, splitFeature, featureNames[splitFeature]);

                        double splitInfo;
                        double infoGain = globalEntropy - H(TrainingX, TrainingY, testBranchingFunc, out splitInfo);
                        double ob = infoGain == 0 ? 0 : infoGain / splitInfo;

                        //Console.WriteLine("Feature " + splitFeature + " gainRatio: " + ob);
                        if (ob > bestObScore || bestObScore == null)
                        {
                            bestFeature = splitFeature; bestObScore = ob;
                            branchingFunction = new ContinuousValueBranchFunction(testBranchingFunc);
                        }
                    }
                }
            }
            //Console.WriteLine("best f: " + featureNames[bestFeature]);
            availableFeatures.Remove(bestFeature);
            
            //create the training datasets for child nodes
            Dictionary<int, List<double[]>> BranchTrainingXsets = new Dictionary<int, List<double[]>>();
            Dictionary<int, List<double>> BranchTrainingYsets = new Dictionary<int, List<double>>();
            
            for (int i=0; i<TrainingX.Length; i++)
            {
                int branch = branchingFunction.Branch(TrainingX[i]);
                if (!BranchTrainingXsets.ContainsKey(branch))
                {
                    BranchTrainingXsets.Add(branch, new List<double[]>());
                    BranchTrainingYsets.Add(branch, new List<double>());
                }

                BranchTrainingXsets[branch].Add(TrainingX[i]);
                BranchTrainingYsets[branch].Add(TrainingY[i]);
            }

            // create child nodes
            children = new ClassificationTreeNode[BranchTrainingXsets.Keys.Count];
            foreach(int index in BranchTrainingYsets.Keys)
            {
                List<double> thisY = BranchTrainingYsets[index];
                // check for base cases: too few instances to support another branch, no more features, or max tree depth reached
                if (thisY.Count < minBranchNodeSupport || availableFeatures.Count == 0 || nodeDepth >= maxTreeDepth)
                {
                    LeafNode newLeaf = new LeafNode(thisY.ToArray(), nodeDepth + 1);
                    children[index] = newLeaf;
                    continue;
                }
                // check for base case: all instances belong to one class
                double[] thisYDistinct = thisY.Distinct().ToArray();
                if (thisYDistinct.Length < 2)
                {
                    LeafNode newLeaf = new LeafNode(thisY.ToArray(), nodeDepth + 1);
                    children[index] = newLeaf;
                    continue;
                }
                // otherwise create a new branch node
                BranchNode newBranchNode = new BranchNode(nodeDepth + 1, maxBranches, minBranchNodeSupport, maxTestThresholds, maxTreeDepth);
                newBranchNode.GrowSubtree(BranchTrainingXsets[index].ToArray(), BranchTrainingYsets[index].ToArray(), featureNames, availableFeatures);
                children[index] = newBranchNode;
            }
        }

        /// <summary>
        /// Generates a list of thresholds to test when splitting on a continuous feature
        /// </summary>
        /// <param name="values">The list of all values of this feature in the training data</param>
        /// <param name="numThresholds">Number of thresholds (max) to return</param>
        private List<double> continuousValueThresholds(List<double> values, int numThresholds)
        {
            List<double> thresholds = new List<double>();

            values.Sort();
            int numToRemove = Math.Min(minBranchNodeSupport / 2, values.Count / 2 - 1);
            values.RemoveRange(0, numToRemove);
            values.RemoveRange(values.Count - numToRemove, numToRemove);

            int instancesPerThreshold = Math.Max(values.Count / numThresholds,1);
            for (int i = 0; i < values.Count; i += instancesPerThreshold)
            {
                if (!thresholds.Contains(values[i]))
                    thresholds.Add(values[i]);
            }

            return thresholds;
        }

        /// <summary>
        /// Classify a test instance using this decision tree node and it's subtree
        /// </summary>
        /// <param name="testX">The test instance</param>
        /// <returns>The predicted class probability for the test instance</returns>
        public ClassProbabilities Evaluate(double[] testX)
        {
            int childIndex = branchingFunction.Branch(testX);
            return children[childIndex].Evaluate(testX);
        }

        /// <summary>
        /// Returns the number of nodes in this subtree (including this node)
        /// </summary>
        public int TreeSize()
        {
            int number = 1; // this node
            foreach (ClassificationTreeNode n in children)
            {
                number += n.TreeSize();
            }
            return number;
        }

        /// <summary>
        /// Perform C4.5-style pruning. But rather than move from leaves to root as in the traditional C4.5 algorithm, this method moves from root to leaves.
        /// </summary>
        /// <param name="errorCI">The binomial confidence interval to use when calculating pessimistic error rates</param>
        public void PruneSubtree(double errorCI = 0.5)
        {
            for (int i=0; i<children.Length; i++)
            {
                if (children[i].GetType() == typeof(LeafNode))
                    continue;

                double errorTree = 0; ClassProbabilities cpLeaf = new ClassProbabilities();
                foreach (LeafNode leaf in children[i].AllDescendantLeaves())
                {
                    ClassProbabilities thisP = leaf.Evaluate();
                    errorTree += thisP.nTotal * upperErrorRateEstimate(thisP, errorCI);
                    cpLeaf = cpLeaf.Combine(thisP);
                }
                double errorLeaf = cpLeaf.nTotal * upperErrorRateEstimate(cpLeaf, errorCI);

                if (errorLeaf <= errorTree)
                {
                    //Console.WriteLine("pruned " + children[i].ToString());
                    children[i] = new LeafNode(cpLeaf, nodeDepth + 1);
                }
                else
                {
                    children[i].PruneSubtree(errorCI);
                }
            }
        }

        /// <summary>
        /// Gets a list of all leaves which are decendants of this node in the tree
        /// </summary>
        public List<LeafNode> AllDescendantLeaves()
        {
            List<LeafNode> leaves = new List<LeafNode>();
            foreach(ClassificationTreeNode node in children)
            {
                leaves.AddRange(node.AllDescendantLeaves());
            }
            return leaves;
        }

        /// <summary>
        /// Estimates the upper limit of an error rate given a specified binomial confidence interval. NOTE: Uses a normal approximation that may become innacurate for small sample sizes.
        /// </summary>
        /// <param name="classProbability">the classProbability object representing the error rate</param>
        /// <param name="errorCI">Binomial distribution confidence interval to use</param>
        private double upperErrorRateEstimate(ClassProbabilities classProbability, double errorCI = 0.5)
        {
            int nErrors = classProbability.nErrors();
            int nTotal = classProbability.nTotal;

            // estimate the z parameter, which is the 1-0.5(alpha) percentile of a standard normal distribution
            // use a cubic polynomial approximation to estimate z from error CI
            double z = 3.4622 * (errorCI * errorCI * errorCI) - 3.6799 * (errorCI * errorCI) + 2.5038 * errorCI - 0.1088;

            // calculate upper error estimate for the binomial distribution
            double p = (double)nErrors / nTotal;
            return p + z * Math.Sqrt(p * (1 - p) / nTotal);
        }

        /// <summary>
        /// Calculate entropy
        /// </summary>
        /// <param name="X">Instances</param>
        /// <param name="Y">Classes associated with the supplied instances</param>
        /// <param name="branchFunc">The BranchingFunction used to split these instances into subsets</param>
        /// <param name="splitInfo">The split information associated with the split effected by the branching function</param>
        private double H(double[][] X, double[] Y, BranchingFunction branchFunc, out double splitInfo)
        {
            Dictionary<int,Dictionary<double, int>> classCountsInBranch = new Dictionary<int,Dictionary<double, int>>();
            Dictionary<int, int> N = new Dictionary<int,int>();

            // scan through the instances that meet the given condition and count members of each class
            for (int i = 0; i < Y.Length; i++ )
            {
                int branch = branchFunc.Branch(X[i]);
                if (N.ContainsKey(branch))
                    N[branch]++;
                else
                {
                    N[branch] = 1;
                    classCountsInBranch[branch] = new Dictionary<double, int>();
                }
                
                if (classCountsInBranch[branch].ContainsKey(Y[i]))
                    classCountsInBranch[branch][Y[i]]++;
                else
                    classCountsInBranch[branch][Y[i]] = 1;
            }

            // calculate entropy and split info
            double H = 0;
            splitInfo = 0;
            foreach (int branch in classCountsInBranch.Keys)
            {
                // calculate split info
                if (N[branch] > 0)
                {
                    double prevalence = (double)N[branch]/Y.Length;
                    splitInfo -= (prevalence == 0) ? 0 : (prevalence * Math.Log(prevalence, 2));
                }

                // calculate entropy
                foreach (double c in classCountsInBranch[branch].Keys)
                {
                    double p_c = (double)classCountsInBranch[branch][c] / N[branch];
                    if (p_c == 0)
                        continue;
                    H -= p_c * Math.Log(p_c, 2) * N[branch];
                }
            }

            return H / Y.Length;
        }

        /// <summary>
        /// Calculate global entropy
        /// </summary>
        /// <param name="X">Instances</param>
        /// <param name="Y">Classes associated with the supplied instances</param>
        private double H(double[][] X, double[] Y)
        {
            // create a dummy branch function for use in calculating global entropy, and a dummy variable for the split info
            ContinuousValueBranchFunction dummyBranchFunc = new ContinuousValueBranchFunction(double.NegativeInfinity, 0, string.Empty);
            double discard;

            return H(X, Y, dummyBranchFunc, out discard);
        }

        /// <summary>
        /// Returns a human-readable representation of this branch node's decision function
        /// </summary>
        public override string ToString()
        {
            return branchingFunction.Print();
        }

        /// <summary>
        /// Returns a human-readable string of this node and it's subtree
        /// </summary>
        /// <returns></returns>
        public List<string> PrintTree()
        {
            List<string> thisTree = new List<string>();
            thisTree.Add(this.ToString());

            for (int j = 0; j < children.Length; j++ )
            {
                thisTree.Add("Branch: " + j);
                List<string> childTree = children[j].PrintTree();
                for (int i = 0; i < childTree.Count; i++)
                {
                    childTree[i] = "\t" + childTree[i];
                }
                thisTree.AddRange(childTree);
            }
            return thisTree;
        }

        /// <summary>
        /// Creates a (deep) copy of this branch node
        /// </summary>
        public ClassificationTreeNode Copy()
        {
            BranchNode newNode = new BranchNode();
            if (featureNames != null)
            {
                newNode.featureNames = new string[featureNames.Length];
                Array.Copy(featureNames, newNode.featureNames, featureNames.Length);
            }
            if (branchingFunction != null)
                newNode.branchingFunction = branchingFunction.Copy();
            newNode.nodeDepth = nodeDepth; 
            newNode.maxBranches = maxBranches;
            newNode.minBranchNodeSupport = minBranchNodeSupport;
            newNode.maxTestThresholds = maxTestThresholds;
            newNode.maxTreeDepth = maxTreeDepth; 
            if (children != null)
            {
                newNode.children = new ClassificationTreeNode[children.Length];
                for (int i = 0; i < children.Length; i++)
                {
                    newNode.children[i] = children[i].Copy();
                }   
            }
            return newNode;
        }
    }

    
}
