using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling
{
    public class ClassificationStats
    {
        #region attributes
        int tp = 0, tn = 0, fp = 0, fn = 0;

        double[] trueY, predictedY;
        public double[] PredictedY
        {
            get { return predictedY; }
            set { predictedY = value; }
        }
        public double[] TrueY
        {
            get { return trueY; }
            set { trueY = value; }
        }

        double accuracy;
        public double Accuracy
        {
            get { return accuracy; }
            set { accuracy = value; }
        }

        double brierScore = 0;
        public double BrierScore
        {
            get { return brierScore; }
            set { brierScore = value; }
        }
        #endregion

        public ClassificationStats(double[] TrueY, double[] PredictedY)
        {
            trueY = (double[])TrueY.Clone();
            predictedY = (double[])PredictedY.Clone();

            // tabulate confusion matrix and calculate brier score at the same time
            for (int i = 0; i < trueY.Length; i++)
            {
                brierScore += (trueY[i] - predictedY[i]) * (trueY[i] - predictedY[i]);

                if (trueY[i] >= 0.5 && predictedY[i] >= 0.5)
                    tp++;
                else if (trueY[i] >= 0.5 && predictedY[i] < 0.5)
                    fn++;
                else if (trueY[i] < 0.5 && predictedY[i] < 0.5)
                    tn++;
                else if (trueY[i] < 0.5 && predictedY[i] >= 0.5)
                    fp++;
            }
            brierScore /= trueY.Length;

            // calculate accuracy
            accuracy = (double)(tp + tn) / trueY.Length;

        }


    }
}
