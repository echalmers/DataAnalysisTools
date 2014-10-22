using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionaryOptimization
{
    public interface ICrossoverOperator<T> : ICloneable
    {
        T Crossover(T parent1, T parent2, Random rnd);
    }

    public interface IMutationOperator<T> : ICloneable
    {
        T Mutate(T parent, Random rnd);
    }

    public interface ISelectionOperator : ICloneable
    {
        int[] Select(double[] fitnesses, int number, Random rnd);
    }

    public interface ICreationOperator<T> : ICloneable
    {
        T CreateRandomIndividual(Random rnd);
    }

    public interface IFitnessFunction<T> : ICloneable
    {
        double[] CalculateFitness(T[] individual);
    }
}
