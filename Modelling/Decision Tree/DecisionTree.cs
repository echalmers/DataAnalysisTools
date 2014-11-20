//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Modelling.FunctionTree;

//namespace Modelling.Decision_Tree
//{
//    public class DecisionTreeBranchNode : BranchNode
//    {
//        public void GrowSubtree(double[][] trainingX, double[] trainingY)
//        {
//            int N = trainingY.Length;

//            Dictionary<double, int> classInstanceCounts = new Dictionary<double,int>();
//            foreach (double d in trainingY)
//            {
//                if (!classInstanceCounts.ContainsKey(d))
//                    classInstanceCounts.Add(d,1);
//                else
//                    classInstanceCounts[d]++;
//            }

//            // calculate global entropy H(S)
//            double H_S = 0;
//            foreach (double i in classInstanceCounts.Keys)
//            {
//                double P_i = (double)classInstanceCounts[i] / (double)N;
//                H_S -= (P_i * Math.Log(P_i, 2));
//            }

            
//        }

//        private double InfoGain(double H_S, double[][] X, double Y, int AttributeIndex, double Threshold)
//        {
//            int[] S_v = new int[2];
//            Dictionary<bool, Dictionary<double, int>> N_i = new Dictionary<bool, Dictionary<double, int>>();
 
//        }

        
//    }
//}
