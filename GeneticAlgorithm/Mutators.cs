using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionaryOptimization
{
    /// <summary>
    /// Mutator class for individuals of type double[]
    /// </summary>
    public class Mutator_DoubleVector : IMutationOperator<double[]>
    {
        double mutateFraction = 0.2;
        public double MutateFraction
        {
          get { return mutateFraction; }
          set { mutateFraction = Math.Min(1,value); }
        }

        double mutateAmount = 0.5;
        public double MutateAmount
        {
            get { return mutateAmount; }
            set { mutateAmount = Math.Min(1,value); }
        }

        /// <summary>
        /// Perform random mutation of a double[]
        /// </summary>
        /// <param name="parent">the parent individual to be mutated</param>
        /// <param name="rnd">the random number generator to be used</param>
        /// <returns>a mutated version of the parent individual</returns>
        public double[] Mutate(double[] parent, Random rnd)
        {
            double[] child = (double[])parent.Clone();

            // calculate how many elements to mutate (max)
            int numMutations = (int)Math.Round(mutateFraction * parent.Length);
            numMutations = Math.Max(numMutations, 1);

            // choose and mutate the elements
            for (int i=0; i<numMutations; i++)
            {
                int randIndex = rnd.Next(child.Length);
                double amount = (rnd.NextDouble() * mutateAmount * 2) - mutateAmount;
                child[randIndex] += (rnd.NextDouble() - 0.5);
            }

            return child;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
