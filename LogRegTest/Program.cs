﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelling;

namespace LogRegTest
{
    /// <summary>
    /// Simple test program showing the use of the LogReg and CrossValidator classes.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // generate some test data: two classes drawn from multivariate
            // normal distributions with identity covariances, one with mean
            // x1 = x2 ... = xd = -2, the other with mean x1 = x2...=xd = 1.
            int d = 5;
            int n = 100;
            NormalRandom rnd = new NormalRandom();
            double[][] X = new double[2*n][];
            double[] Y = new double[2*n];
            for (int i=0; i<n; i++)
            {
                X[i] = new double[d];
                for (int j = 0; j < d; j++)
                {
                    X[i][j] = rnd.next(-2, 2);
                }
                Y[i] = 1;
            }
            for (int i=n; i<2*n; i++)
            {
                X[i] = new double[d];
                for (int j = 0; j < d; j++)
                {
                    X[i][j] = rnd.next(1, 2);
                }
                Y[i] = 0;
            }

            // create an instance of LogReg
            LogReg lr = new LogReg();
            
            // Use 10-fold cross validation to get the LogReg's predictions for the data
            CrossValidator cv = new CrossValidator(X, Y, 10, 0);
            double[] predictions = cv.getCvPredictions(lr, false, true);

            // find the classification performance of the predictions
            // (assume a decision threshold of 0.5 on the LogReg output scores)
            ClassificationStats stats = new ClassificationStats(Y, predictions);

            Console.WriteLine("Cross-validated prediction accuracy: " + stats.Accuracy*100 + "%");
                       
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Class for generating normally distributed random numbers
    /// </summary>
    public class NormalRandom
    {
        Random rnd = new Random();

        /// <summary>
        /// Generate a random number from a normal distribution
        /// Uses the Box-Muller transform
        /// </summary>
        /// <param name="mean">mean of the underlying distribution</param>
        /// <param name="standDev">standard deviation of the underlying distribution</param>
        /// <returns>normally distributed random number</returns>
        public double next(double mean, double standDev)
        {
            double u1 = rnd.NextDouble();
            double u2 = rnd.NextDouble();
            double randNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random ~N(0,1)
            return mean + standDev * randNormal; //random normal(mean,stdDev^2)
        }
    }
}
