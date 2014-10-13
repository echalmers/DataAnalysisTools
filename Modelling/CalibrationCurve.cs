using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelling
{
    /// <summary>
    /// Class useful for finding a predictor's calibration curve, given a set of it's predictions and the corresponding true outcomes.
    /// </summary>
    [Serializable()]
    public class CalibrationCurve
    {
        double resolution = 0.05;
        double smoothingRadius = 0.05;

        Dictionary<double, double> curve;
        public Dictionary<double, double> Curve
        {
            get { return curve; }
        }

        Dictionary<double, double> resultingOutcomes;
        Dictionary<double, double> contributingInstances;

        /// <summary>
        /// Construct a new CalibrationCurve object
        /// </summary>
        /// <param name="trueOut">An array of true outcomes (values in the range [0, 1], e.g. representing failure/success)</param>
        /// <param name="predictions">The array of corresponding outcome predictions</param>
        public CalibrationCurve(double[] trueOut, double[] predictions)
        {
            calculateCurve(trueOut, predictions);
        }

        /// <summary>
        /// Construct a new CalibrationCurve object
        /// </summary>
        /// <param name="trueOut">An array of true outcomes (values in the range [0, 1], e.g. representing failure/success)</param>
        /// <param name="predictions">The array of corresponding outcome predictions</param>
        /// <param name="Resolution">The resolution of the curve</param>
        /// <param name="SmoothingRadius">Radius used in triangular smoothing</param>
        public CalibrationCurve(double[] trueOut, double[] predictions, double Resolution, double SmoothingRadius)
        {
            smoothingRadius = SmoothingRadius;
            resolution = Resolution;
            calculateCurve(trueOut, predictions);
        }

        private void calculateCurve(double[] trueOut, double[] predictions)
        {
            curve = new Dictionary<double, double>();

            contributingInstances = new Dictionary<double, double>();
            resultingOutcomes = new Dictionary<double, double>();
            
            // calculate true success probability for each predicted probability with specified resolution
            for (double p=0; p<=1.0001; p+=resolution)
            {
                contributingInstances.Add(p, 0);
                resultingOutcomes.Add(p, 0);

                TriangleFunction tri = new TriangleFunction(p - smoothingRadius, p, p + smoothingRadius);

                for (int i=0; i<trueOut.Length; i++)
                {
                    double thisInstanceWeight = tri.getValue(predictions[i]);
                    contributingInstances[p] += thisInstanceWeight;
                    resultingOutcomes[p] += (thisInstanceWeight * trueOut[i]);
                }

                curve.Add(p, resultingOutcomes[p] / contributingInstances[p]);
            }
        }

        /// <summary>
        /// Returns the weighted mean deviation of the calibration curve from ideal (diagonal),
        /// the average is weighted using the number of instances at each point
        /// </summary>
        /// <returns>The (weighted) mean deviation</returns>
        public double meanError()
        {
            double totalWeight = 0;
            double weightedErrors = 0;

            foreach(double p in contributingInstances.Keys)
            {
                weightedErrors += Math.Abs(curve[p] - p) * contributingInstances[p];
                totalWeight += contributingInstances[p];
            }

            return weightedErrors / totalWeight;
        }
    }



    class TriangleFunction
    {
        double _a, _b, _c;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="a">The 'left-most' point of the triangle</param>
        /// <param name="b">The point where the triangle peaks</param>
        /// <param name="c">The 'right-most' point of the triangle</param>
        public TriangleFunction(double a, double b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        /// <summary>
        /// Get the height of the triangle at a given x-value
        /// </summary>
        /// <param name="xValue">The input value</param>
        /// <returns>the height of the triangle at a given x-value</returns>
        public double getValue(double xValue)
        {
            double line1;
            if (_a == _b)
                line1 = xValue >= _b ? 1 : 0;
            else
                line1 = (xValue - _a) / (_b - _a);

            double line2; 
            if (_b==_c)
                line2 = xValue <= _b ? 1 : 0;
            else
                line2 = (xValue - _b) / -(_c - _b) + 1;
            return Math.Max(Math.Min(line1, line2), 0);
        }
    }
}
