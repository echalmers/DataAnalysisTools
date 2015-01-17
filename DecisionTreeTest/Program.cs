using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelling.DecisionTree;

namespace DecisionTreeTest
{
    /// <summary>
    /// This program demonstrates the use of the ClassificationTree class
    /// </summary>
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
            double[] Y = { 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1 };
            string[] featureNames = { "bar", "fri/sat", "hungry", "rain", "estimate" };

            ClassificationTree tree = new ClassificationTree(5, 1, 10, 5);

            tree.train(X, Y, featureNames);
            Console.WriteLine(tree.PrintTree());

            Console.WriteLine("press any key...");
            Console.ReadKey();
        }
    }
}
