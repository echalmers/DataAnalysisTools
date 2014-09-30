using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;

namespace Modelling
{

    //public class SimpleLogReg : LearnerInterface
    //{
    //    #region attributes
    //    int maxEpochs = 1000;
        
    //    double[] B;
    //    public double[] Beta
    //    {
    //        get { return B; }
    //    }

    //    bool includeConstTerm = true;
    //    public bool IncludeConstTerm
    //    {
    //        get { return includeConstTerm; }
    //    }

    //    double stepSize = 100;
    //    int numStepRefinements = 7;
    //    double convergenceTol = 0.001;

    //    bool useApproxLogisticFn = true;
    //    public bool UseApproxLogisticFn
    //    {
    //        get { return useApproxLogisticFn; }
    //        set { useApproxLogisticFn = value; }
    //    }
    //    #endregion

    //    public SimpleLogReg()
    //    { }
    //    public SimpleLogReg(bool IncludeConstantTerm)
    //    {
    //        includeConstTerm = IncludeConstantTerm;
    //    }
    //    public SimpleLogReg(bool IncludeConstantTerm, bool UseApproxLogisticFn)
    //    {
    //        includeConstTerm = IncludeConstantTerm;
    //        useApproxLogisticFn = UseApproxLogisticFn;
    //    }


    //    public void train(double[][] trainingX, double[] trainingY)
    //    {
    //        // prepwork
    //        int N = trainingX.GetLength(0);
    //        int D = trainingX[0].Length;

    //        double[][] X; double[] Y = trainingY;
            

    //        // insert a leading column of ones on the X matrix (if includeConstTerm == true)
    //        if (includeConstTerm)
    //        {
    //            X = new double[N][];
    //            B = new double[D + 1];

    //            for (int i = 0; i < N; i++)
    //            {
    //                X[i] = new double[D + 1];
    //                X[i][0] = 1;
    //                for (int j = 0; j < D; j++)
    //                {
    //                    X[i][j + 1] = trainingX[i][j];
    //                }
    //            }
    //        }
    //        else
    //        {
    //            X = trainingX;
    //            B = new double[D];
    //        }

    //        // run learning algorithm
    //        double P_y_1, P_y_yi, g, maxBdifference;
    //        double[] Bnew;
    //        double currentStepSize = stepSize;

    //        for (int stepSizeTrials = 0; stepSizeTrials < numStepRefinements; stepSizeTrials++)
    //        {
    //            double[] dB = new double[D+1];
    //            double[] dB_old = new double[D+1];
    //            bool signChange = false;

    //            for (int epoch = 0; epoch < maxEpochs; epoch++)
    //            {
    //                Bnew = (double[])B.Clone();
    //                for (int i = 0; i < N; i++)
    //                {
    //                    P_y_1 = logisticFn(B, X[i], useApproxLogisticFn);
    //                    P_y_yi = P_y_1 * Y[i] + (1 - Y[i]) * (1 - P_y_1);

    //                    for (int j = 0; j < B.Length; j++)
    //                    {
    //                        g = (Y[i] - 0.5) * 2 * X[i][j] * (1 - P_y_yi);
    //                        Bnew[j] += currentStepSize * g;
    //                    }
    //                }

    //                // check for convergence/oscillation
    //                maxBdifference = convergenceTol-1;
    //                for (int j = 0; j < B.Length; j++)
    //                {
    //                    dB[j] = Math.Round(Bnew[j] - B[j],3);
    //                    maxBdifference = Math.Max(Math.Abs(dB[j]), maxBdifference);
    //                    signChange = signChange || (Math.Sign(dB[j]) * Math.Sign(dB_old[j]) == -1);
    //                }
    //                if ((maxBdifference < convergenceTol) || signChange)
    //                {
    //                    B = (double[])Bnew.Clone();
    //                    currentStepSize /= 10;
    //                    break;
    //                }

    //                // update B
    //                B = (double[])Bnew.Clone();
    //                dB_old = (double[])dB.Clone();
    //            }
    //        }
    //    }

    //    public double[] predict(double[][] X)
    //    {
    //        int testPts = X.GetLength(0);
    //        double[] predictions = new double[testPts];

    //        for (int i=0; i<testPts; i++)
    //        {
    //            // decide whether the model includes a constant term
    //            if (includeConstTerm)
    //            {
    //                // append the leading one
    //                double[] Xappend = new double[X[i].Length + 1];
    //                Xappend[0] = 1;
    //                for (int j = 0; j < X[i].Length; j++)
    //                {
    //                    Xappend[j + 1] = X[i][j];
    //                }
    //                // predict
    //                predictions[i] = Math.Round(logisticFn(B, Xappend, useApproxLogisticFn), 3);
    //            }
    //            else
    //            {
    //                predictions[i] = Math.Round(logisticFn(B, X[i], useApproxLogisticFn), 3);
    //            }
    //        }

    //        return predictions;
    //    }

    //    private double logisticFn(double[] B, double[] X, bool useApproximate)
    //    {
    //        double t = 0;
    //        for (int i = 0; i < B.Length; i++)
    //        {
    //            t += (B[i] * X[i]);
    //        }

    //        if (useApproximate)
    //            return 0.5 * t / (1 + Math.Abs(t)) + 0.5;
    //        else
    //            return 1 / (1 + Math.Exp(-t));
    //    }

    //    public object Clone()
    //    {
    //        return this.MemberwiseClone();
    //    }
    //}

    public class LogReg : LearnerInterface
    {
        public double[] B;
        bool useApproxLogisticFn = true;

        public void train(double[][] trainingX, double[] trainingY)
        {
            if ((B == null) || (B.Length == 0))
                B = new double[trainingX[0].Length];
            else if (trainingX[0].Length != B.Length)
            {
                double[] newB = new double[trainingX[0].Length];
                Array.Copy(B, newB, Math.Min(B.Length, newB.Length));
                B = newB;
            }
            
            //for (int i=0; i<1000; i++)
            //{
            //    Console.WriteLine(Run(trainingX, trainingY).ToString());
            //}

            //double[] bLower = new double[B.Length];
            //double[] bUpper = new double[B.Length];
            //for (int i=0; i<B.Length; i++)
            //{
            //    bUpper[i] = 10;
            //    bLower[i] = -10;
            //}

            var solution = NelderMeadSolver.Solve(b => objectiveFn(trainingX, trainingY, b), B);//, bLower, bUpper); 
            for (int i=0; i<B.Length; i++)
            {
                B[i] = solution.GetValue(i);
            }
            
            //Console.WriteLine(solution.GetSolutionValue(0));
        }

        private double[] predict(double[][] X, double[] b)
        {
            int testPts = X.GetLength(0);
            double[] predictions = new double[testPts];

            for (int i = 0; i < testPts; i++)
            {
                // decide whether the model includes a constant term
                if (false)//(includeConstTerm)
                {
                    // append the leading one
                    double[] Xappend = new double[X[i].Length + 1];
                    Xappend[0] = 1;
                    for (int j = 0; j < X[i].Length; j++)
                    {
                        Xappend[j + 1] = X[i][j];
                    }
                    // predict
                    predictions[i] = logisticFn(b, Xappend, useApproxLogisticFn);
                }
                else
                {
                    predictions[i] = logisticFn(b, X[i], useApproxLogisticFn);
                }
            }

            return predictions;
        }

        public double[] predict(double[][] X)
        {
            return predict(X, B);
        }

        public double Run(double[][] inputs, double[] outputs)
        {
            double rate = 0.01;

            // Initial definitions and memory allocations
            double[] coefficients = B;
            double[] previous = (double[])B.Clone();

            // Compute the complete error gradient
            double[] gradient = new double[B.Length];

                for (int i = 0; i < inputs.Length; i++)
                {
                    double actual = logisticFn(B, inputs[i], true);
                    double error = outputs[i] - actual;

                    gradient[0] += error;
                    for (int j = 0; j < inputs[i].Length; j++)
                        gradient[j] += inputs[i][j] * error;
                }

                // Update coefficients using the gradient
                for (int i = 0; i < coefficients.Length; i++)
                    coefficients[i] += rate * gradient[i];


            // Return the maximum parameter change
                double[] deltas = new double[B.Length];
            for (int i = 0; i < previous.Length; i++)
                deltas[i] = Math.Abs(coefficients[i] - previous[i]) / Math.Abs(previous[i]);

            return deltas.Max();
        }

        private double logisticFn(double[] B, double[] X, bool useApproximate)
        {
            double t = 0;
            for (int i = 0; i < B.Length; i++)
            {
                t += (B[i] * X[i]);
            }

            if (useApproximate)
                return 0.5 * t / (1 + Math.Abs(t)) + 0.5;
            else
                return 1 / (1 + Math.Exp(-t));
        }

        private double objectiveFn(double[][] trainingX, double[] trainingY, double[] b)
        {
            double[] predictions = predict(trainingX, b);
            double ob = 0;
            for (int i=0; i<trainingY.Length; i++)
            {
                ob += (trainingY[i] - predictions[i]) * (trainingY[i] - predictions[i]);
                //ob -= predictions[i] > 0.5 ? (trainingY[i] * (predictions[i])) : ((1 - trainingY[i]) * (1 - predictions[i]));
                //ob += Math.Abs(trainingY[i] - predictions[i]);
            }
            return ob / trainingY.Length;
        }

        public LearnerInterface Copy()
        {
            LogReg newLogReg = new LogReg();
            if ((B != null) && (B.Length > 0))
            {
                newLogReg.B = new double[B.Length];
                Array.Copy(B, newLogReg.B, B.Length);
            }
            newLogReg.useApproxLogisticFn = useApproxLogisticFn ? true : false;
            return newLogReg;
        }

    }
}
