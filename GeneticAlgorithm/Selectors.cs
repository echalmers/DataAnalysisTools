using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionaryOptimization
{
    public class RouletteSelector : ISelectionOperator
    {
        public int[] Select(double[] fitness, int number, Random rnd)
        {
            // calculate min fitness so we can shift all fitnesses to positive starting at 0
            double minFitness = fitness.Min();

            // calculate cumulative fitnesses and totalFitness
            double[] cumFitnesses = new double[fitness.Length];
            cumFitnesses[0] = fitness[0] - minFitness;
            for (int i = 1; i < fitness.Length; i++)
            {
                cumFitnesses[i] = (cumFitnesses[i - 1] + fitness[i] - minFitness);
            }
            double sumFitness = cumFitnesses[fitness.Length-1];

            // roulette selection
            int[] selected = new int[number];

            for (int i = 0; i < number; i++)
            {
                double wheelPosition = rnd.NextDouble() * sumFitness;
                // select the individual at the designated position on the wheel
                int index = cumFitnesses.ToList().BinarySearch(wheelPosition);
                if (index <0)
                    index = ~index;
                selected[i] = index;
            }

            return selected;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
