using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelling;

namespace CalibrationCurveTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int numInstances = 100000;
            
            // generate a set of hypothetical, perfectly-calibrated predictions
            double[] trueOut = new double[numInstances];
            double[] predictions = new double[numInstances];
            Random rnd = new Random();

            for (int i=0; i<numInstances; i++)
            {
                predictions[i] = rnd.NextDouble();
                trueOut[i] = rnd.NextDouble() <= predictions[i] ? 1 : 0;
            }

            // create the CalibrationCurve 
            CalibrationCurve curve = new CalibrationCurve(trueOut, predictions);
            double meanError = curve.meanError();
        }
    }
}
