using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionaryOptimization
{
    public class ScatteredCrossover_DoubleVector : ICrossoverOperator<double[]> 
    {
        public double[] Crossover(double[] parent1, double[] parent2, Random rnd)
        {
            int d = parent1.Length;
            double[] child = new double[d];

            for (int i=0; i<d; i++)
            {
                if (rnd.NextDouble() > 0.5)
                {
                    child[i] = parent1[i];
                }
                else
                {
                    child[i] = parent2[i];
                }
            }

            return child;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
