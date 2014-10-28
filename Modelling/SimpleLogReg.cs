using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;

namespace Modelling
{
    /// <summary>
    /// A custom logistic-regression-style algorithms. It allows fitting the logistic function to maximize criteria other
    /// than log-likelihood. This means the model is not a "true" logistic regression anymore, but introduces some
    /// nice flexibility.
    /// A nelder-mead algorithm tunes the B coefficients to minimize the user-supplied objective function. If no objective
    /// function is supplied, the algorithm maximizes likelihood(B)
    /// WARNING: the nelder-mead *may* be unsuitable for some low-dimensionality (i.e. one predictor) situations.
    /// </summary>
    [Serializable()]
    public class LogReg : LearnerInterface
    {
        public double[] B;

        bool includeConstantTerm = true;
        public bool IncludeConstantTerm
        {
            get { return includeConstantTerm; }
            set { includeConstantTerm = value; }
        }

        bool useApproxLogisticFn = true;
        public bool UseApproxLogisticFn
        {
            get { return useApproxLogisticFn; }
            set { useApproxLogisticFn = value; }
        }

        predictionEvaluation objectiveFn;

        /// <summary>
        /// Construct a new LogReg object
        /// </summary>
        public LogReg()
        {
            objectiveFn = likelihoodObjective;
        }

        /// <summary>
        /// Construct a new LogReg object
        /// </summary>
        /// <param name="ObjectiveFn">delegate to the objective function to be used</param>
        public LogReg(predictionEvaluation ObjectiveFn)
        {
            objectiveFn = ObjectiveFn;
        }

        /// <summary>
        /// Tune the B coefficients to minimize a user-supplied objective function
        /// </summary>
        /// <param name="trainingX">Training instances</param>
        /// <param name="trainingY">Training responses</param>
        /// <param name="obFn">Objective function to be minimized</param>
        public void train(double[][] trainingX, double[] trainingY)
        {
            // get the starting B - previous values if this model was previously trained, new double[] otherwise
            int numCoeffs = includeConstantTerm ? trainingX[0].Length + 1 : trainingX[0].Length;
            if ((B == null) || (B.Length == 0))
                B = new double[numCoeffs];
            else if (trainingX[0].Length != B.Length)
            {
                double[] newB = new double[numCoeffs];
                Array.Copy(B, newB, Math.Min(B.Length, newB.Length));
                B = newB;
            }
            
            //var solution = NelderMeadSolver.Solve(b => obFn(trainingX, trainingY, b), B);//, bLower, bUpper); 
            var solution = NelderMeadSolver.Solve(b => objectiveFn(trainingY, predict(trainingX, b)), B);//, bLower, bUpper); 
            for (int i=0; i<B.Length; i++)
            {
                B[i] = solution.GetValue(i);
            }
            
        }


        /// <summary>
        /// Predict responses
        /// </summary>
        /// <param name="X">Test instances</param>
        /// <param name="b">B coefficients</param>
        /// <returns>array of predictions for the instances in X</returns>
        private double[] predict(double[][] X, double[] b)
        {
            int testPts = X.GetLength(0);
            double[] predictions = new double[testPts];

            for (int i = 0; i < testPts; i++)
            {
                predictions[i] = logisticFn(b, X[i], useApproxLogisticFn);
            }

            return predictions;
        }

        /// <summary>
        /// Predict responses using this object's trained B coefficients
        /// </summary>
        /// <param name="X">Test instances</param>
        /// <returns>array of predictions for the instances in X</returns>
        public double[] predict(double[][] X)
        {
            return predict(X, B);
        }

        /// <summary>
        /// Get output of logistic function
        /// </summary>
        /// <param name="B">B coefficients</param>
        /// <param name="X">Input values</param>
        /// <param name="useApproximate">Set to true to use the approximate (faster) logistic function: x / (1+|x|)</param>
        /// <returns>logistic function value at X</returns>
        private double logisticFn(double[] B, double[] X, bool useApproximate)
        {
            double t = 0;
            
            for (int i = 0; i < X.Length; i++)
            {
                t += (B[i] * X[i]);
            }

            // const term is last element of B
            if (includeConstantTerm)
                t += B[X.Length];

            if (useApproximate)
                return 0.5 * t / (1 + Math.Abs(t)) + 0.5;
            else
                return 1 / (1 + Math.Exp(-t));
        }

        /// <summary>
        /// Default objective function for training LogReg
        /// </summary>
        /// <param name="trainingY">Training responses</param>
        /// <param name="predictions">Predicted responses</param>
        /// <returns>negative likelihood value</returns>
        private double likelihoodObjective(double[] trainingY, double[] predictions)
        {
            double ob = 0;
            for (int i = 0; i < trainingY.Length; i++)
            {
                ob -= predictions[i] > 0.5 ? (trainingY[i] * (predictions[i])) : ((1 - trainingY[i]) * (1 - predictions[i]));
            }
            return ob / trainingY.Length;
        }


        public LearnerInterface Copy()
        {
            LogReg newLogReg = new LogReg();
            newLogReg.objectiveFn = this.objectiveFn;
            if ((B != null) && (B.Length > 0))
            {
                newLogReg.B = new double[B.Length];
                Array.Copy(B, newLogReg.B, B.Length);
            }
            newLogReg.useApproxLogisticFn = useApproxLogisticFn ? true : false;
            newLogReg.includeConstantTerm = includeConstantTerm ? true : false;
            return newLogReg;
        }

    }
}
