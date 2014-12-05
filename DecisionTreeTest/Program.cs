using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelling.DecisionTree;

namespace DecisionTreeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            double[][] X = new double[12][] 
            {
                new double[5] {0,0,1,0,0},
                new double[5] {0,0,1,0,3},
                new double[5] {1,0,0,0,0},
                new double[5] {0,1,1,0,1},
                new double[5] {0,1,0,0,6},
                new double[5] {1,0,1,1,0},
                new double[5] {1,0,0,1,0},
                new double[5] {0,0,1,1,0},
                new double[5] {1,1,0,1,6},
                new double[5] {1,1,1,0,1},
                new double[5] {0,0,0,0,0},
                new double[5] {1,1,1,0,3}
            };
            double[] Y = {1,0,1,1,0,1,0,1,0,0,0,1};
            string[] featureNames = { "bar", "fri/sat", "hungry", "rain", "estimate" };

            DecisionTreeNode root = new DecisionTreeNode();
            double h = root.H(X, Y, (instance) => (int)instance[4]);

        }
    }
}
