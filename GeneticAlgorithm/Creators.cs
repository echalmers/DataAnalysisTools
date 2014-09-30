using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionaryOptimization
{
    
    public class Creator_DoubleVector : ICreationOperator<double[]>
    {
        int d;
        double maxValue = 1;
        double minValue = -1;
        double range = 2;

        public Creator_DoubleVector(int D, double MaxValue, double MinValue)
        {
            d = D;
            maxValue = MaxValue;
            minValue = MinValue;
            range = maxValue - minValue;
        }

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
