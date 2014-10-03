using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionaryOptimization;
using System.Diagnostics;

namespace GAtest
{
    /// <summary>
    /// A simple test program for GeneticAlgorithm and MultiPopulationGA classes.
    /// The test optimizes a 10-dimensional sphere function (the correct solution is a vector of 10 zeros)
    /// The GA uses a population size of 200, while the multi-population GA uses 10 populations of 20.
    /// Thanks to parallel execution of the populations, the multi-population finds the solution faster.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            int dimensionality = 10;
            double lowerBound = -10;
            double upperBound = 10;
            double fitnessTol = -0.001;
            
            // instances of creator, crossover, mutator, selector, & fitness function
            ICreationOperator<double[]> creator = new Creator_DoubleVector(dimensionality, upperBound, lowerBound);
            ICrossoverOperator<double[]> crossover = new ScatteredCrossover_DoubleVector();
            IMutationOperator<double[]> mutator = new Mutator_DoubleVector();
            ISelectionOperator selector = new RouletteSelector();
            IFitnessFunction<double[]> fitFn = new fitnessFn();

            // instantiate a single-population GA
            GeneticAlgorithm<double[]> GA = new GeneticAlgorithm<double[]>(crossover, mutator, selector, creator, fitFn, 200, 0);

            // run the GA 5 generations at a time until the solution is within desired tolerance (or 100 iterations)
            Console.WriteLine("Running GA..." + Environment.NewLine);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            double[] solution = new double[dimensionality];
            double fitness=0;
            for (int i=0; i<100; i++)
            {
                solution = GA.iterate(5);
                fitness = fitFn.CalculateFitness(solution);
                if (fitness > fitnessTol)
                    break;
            }
            sw.Stop();

            // display GA's results
            Console.WriteLine("Finished in " + sw.Elapsed.TotalMilliseconds + "ms");
            Console.Write("Final solution: [ ");
            foreach (double d in solution)
                Console.Write(Math.Round(d, 2) + " ");
            Console.WriteLine("]");
            Console.WriteLine("Fitness value: " + fitness + Environment.NewLine);


            // run the same problem using the multi-population GA
            MultiPopulationGA<double[]> multiGA = new MultiPopulationGA<double[]>(fitFn, 10, 20);
            multiGA.addCreationOperator(creator);
            multiGA.addCrossoverOperator(crossover);
            multiGA.addMutationOperator(mutator);
            multiGA.addSelectionOperator(selector);
            multiGA.initializeGAs(true);

            Console.WriteLine("Running multi-population GA..." + Environment.NewLine);
            sw.Reset();
            sw.Start();

            // run the multi-population GA 5 generations at a time until the solution is within desired tolerance (or 100 iterations)
            // migrate individuals between populations every 5 generations
            solution = new double[dimensionality];
            fitness = 0;
            for (int i = 0; i < 100; i++)
            {
                solution = multiGA.iterate(5);
                fitness = fitFn.CalculateFitness(solution);
                if (fitness > fitnessTol)
                    break;
            }
            sw.Stop();

            // display GA's results
            Console.WriteLine("Finished in " + sw.Elapsed.TotalMilliseconds + "ms");
            Console.Write("Final solution: [ ");
            foreach (double d in solution)
                Console.Write(Math.Round(d, 2) + " ");
            Console.WriteLine("]");
            Console.WriteLine("Fitness value: " + fitness + Environment.NewLine);


            Console.ReadKey();
        }

        /// <summary>
        /// IFitnessFunction implementation of an inverted sphere function:
        /// f(X) = -1*sum_i(X[i]^2)
        /// The best (maximum) fitness is f=0, at X[0]==X[1]==...X[d]==0
        /// </summary>
        class fitnessFn : IFitnessFunction<double[]>
        {
            public double CalculateFitness(double[] individual)
            {
                double fitness = 0;
                foreach(double d in individual)
                {
                    fitness -= (d * d);
                }
                return fitness;
            }

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }
    }
}
