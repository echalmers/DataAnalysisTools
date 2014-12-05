using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelling.DecisionTree
{
    class ClassProbability
    {
        int classInstances;
        int totalInstances;

        public double probability()
        {
            return classInstances / totalInstances;
        }
    }

    public class DecisionTreeNode
    {
        #region attributes
        string[] featureNames;
        int? splitFeatureIndex;
        double? splitThreshold;

        List<DecisionTreeNode> children = new List<DecisionTreeNode>();
        public List<DecisionTreeNode> Children
        {
            get { return children; }
            set { children = value; }
        }
        #endregion

        #region constructors
        public DecisionTreeNode()
        {
        }
        #endregion

        public void growSubtree(double[][] TrainingX, double[] TrainingY, string[] FeatureNames)
        {
            featureNames = FeatureNames;

            // consider each feature
            for (int f = 0; f < TrainingX[0].Length; f++)
            {
                // enumerate all possible thresholds

            }
        }


        public int NumChildren()
        {
            int number = 1; // this node
            foreach (DecisionTreeNode n in children)
            {
                number += n.NumChildren();
            }
            return number;
        }



        public double H(double[][] X, double[] Y, Func<double[], int> branchingFunction)
        {
            Dictionary<int,Dictionary<double, int>> classCountsInBranch = new Dictionary<int,Dictionary<double, int>>();
            Dictionary<int, int> N = new Dictionary<int,int>();

            // scan through the instances that meet the given condition and count members of each class
            for (int i = 0; i < Y.Length; i++ )
            {
                int branch = branchingFunction(X[i]);
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

            // calculate entropy
            double H = 0;
            foreach (int branch in classCountsInBranch.Keys)
            {
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

        public double H(double[][] X, double[] Y)
        {
            return H(X, Y, (dummy) => 0);
        }

        

    }


}
