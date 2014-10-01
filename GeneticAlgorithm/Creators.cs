using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionaryOptimization
{
    /// <summary>
    /// 'Creator' class for individuals of type double[]
    /// </summary>
    public class Creator_DoubleVector : ICreationOperator<double[]>
    {
        int d;
        double maxValue = 1;
        double minValue = -1;
        double range = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="D">Dimensionality, or number of elements in the double[]</param>
        /// <param name="MaxValue">Maximum allowed value of a double in the array</param>
        /// <param name="MinValue">Minimum allowed value of a double in the array</param>
        public Creator_DoubleVector(int D, double MaxValue, double MinValue)
        {
            d = D;
            maxValue = MaxValue;
            minValue = MinValue;
            range = maxValue - minValue;
        }

        /// <summary>
        /// Creates a random double[] with parameters as supplied to the constructor
        /// </summary>
        /// <param name="rnd">The random number generator to be used</param>
        /// <returns>An array of random doubles</returns>
        public double[] CreateRandomIndividual(Random rnd)
        {
            double[] vector = new double[d];
            for (int i=0; i<d; i++)
            {
                vector[i] = rnd.NextDouble()*range+minValue;
            }
            return vector;
        }

        
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
