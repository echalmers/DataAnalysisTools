using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionaryOptimization
{    
    /// <summary>
    /// A versatile genetic algorithm which uses generics to optimize an individual of any type
    /// </summary>
    /// <typeparam name="T">The type of the individual to be optimized</typeparam>
    public class GeneticAlgorithm<T>
    {

        #region attributes

        double eliteFraction = 0.1;
        public double EliteFraction
        {
            get { return eliteFraction; }
            set { eliteFraction = value; }
        }

        double crossoverRate = 0.6;
        public double CrossoverRate
        {
            get { return crossoverRate; }
            set 
            { 
                crossoverRate = Math.Min(value, 1);
                mutationRate = 1 - crossoverRate;
            }
        }

        double mutationRate = 0.4;
        public double MutationRate
        {
            get { return mutationRate; }
            set 
            { 
                mutationRate = Math.Min(value, 1);
                crossoverRate = 1 - mutationRate;
            }
        }

        int populationSize = 10;
        public int PopulationSize
        {
            get { return populationSize; }
        }

        T[] population;
        public T[] Population
        {
            get { return population; }
            set { population = value; }
        }

        double[] fitness;
        public double[] Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }

        int elapsedGenerations = 0;
        public int ElapsedGenerations
        {
            get { return elapsedGenerations; }
        }

        Random rnd;

        #endregion

        #region operators
        ICrossoverOperator<T> crossoverOperator;
        IMutationOperator<T> mutationOperator;
        ISelectionOperator selectionOperator;
        ICreationOperator<T> creationOperator;
        IFitnessFunction<T> fitnessFunction;
        #endregion

        /// <summary>
        /// Construct a GeneticAlgorithm object
        /// </summary>
        /// <param name="CrossoverOperator">The object which will handle crossover operations</param>
        /// <param name="MutationOperator">The object which will handle mutation operations</param>
        /// <param name="SelectionOperator">The object which will handle selection</param>
        /// <param name="CreationOperator">The object which will creation of random individuals</param>
        /// <param name="FitnessFunction">The object which will handle calculation of an individual's fitness</param>
        /// <param name="PopulationSize">The numer of individuals to maintain in the population</param>
        /// <param name="rndSeed">Random seed</param>
        public GeneticAlgorithm(ICrossoverOperator<T> CrossoverOperator, IMutationOperator<T> MutationOperator, ISelectionOperator SelectionOperator,
                                ICreationOperator<T> CreationOperator, IFitnessFunction<T> FitnessFunction, int PopulationSize, int rndSeed)
        {
            rnd = new Random(rndSeed);

            populationSize = PopulationSize;
            crossoverOperator = CrossoverOperator;
            mutationOperator = MutationOperator;
            selectionOperator = SelectionOperator;
            creationOperator = CreationOperator;
            fitnessFunction = FitnessFunction;

            // create the initial population
            population = new T[populationSize];
            fitness = new double[populationSize];
            for (int i=0; i<populationSize; i++)
            {
                population[i] = creationOperator.CreateRandomIndividual(rnd);
                fitness[i] = fitnessFunction.CalculateFitness(population[i]);
            }
        }
        
        /// <summary>
        /// Select a number of individuals from the population, using the selection operator provided
        /// </summary>
        /// <param name="number">Number of individuals to select</param>
        /// <returns>The selected individuals' indices in the population</returns>
        public int[] selectIndividuals(int number)
        {
            return selectionOperator.Select(fitness, number, rnd);
        }

        /// <summary>
        /// Run the genetic algorithm for a specified number of iterations
        /// </summary>
        /// <param name="generations">The number of iterations to perform</param>
        /// <returns>The best individual in the population after iterating</returns>
        public T iterate(int generations)
        {            
            // initialize the new population
            T[] newPopulation = new T[populationSize];
            double[] newFitness = new double[populationSize];
            int bestIndividualIndex = 0;

            // calculate how many elite individuals to use
            int eliteCount = (int)Math.Round(populationSize * eliteFraction,0);
            eliteCount = Math.Max(eliteCount, 1);

            for (int generation=0; generation<generations; generation++)
            {
                // move elite individuals (best individual always moves)
                bestIndividualIndex = fitness.ToList().IndexOf(fitness.Max());
                newPopulation[0] = population[bestIndividualIndex];
                newFitness[0] = fitness[bestIndividualIndex];

                int[] eliteIndeces = selectionOperator.Select((double[])fitness.Clone(), eliteCount-1, rnd);
                for (int i = 0; i < eliteCount-1; i++)
                {
                    newPopulation[i+1] = population[eliteIndeces[i]];
                    newFitness[i+1] = fitness[eliteIndeces[i]];
                }

                // create mutations and crossovers
                for (int i = eliteCount; i < populationSize; i++)
                {
                    if(rnd.NextDouble()<crossoverRate) // do a crossover
                    {
                        int[] parentIndeces = selectionOperator.Select(fitness, 2, rnd);
                        T child = crossoverOperator.Crossover(population[parentIndeces[0]], population[parentIndeces[1]], rnd);
                        newPopulation[i] = child;
                        newFitness[i] = fitnessFunction.CalculateFitness(child);
                    }
                    else // do a mutation
                    {
                        int[] parentIndex = selectionOperator.Select(fitness, 1, rnd);
                        T child = mutationOperator.Mutate(population[parentIndex[0]], rnd);
                        newPopulation[i] = child;
                        newFitness[i] = fitnessFunction.CalculateFitness(child);
                    }
                }

                // implement the new population
                population = newPopulation;
                fitness = newFitness;

                elapsedGenerations++;
            }

            double maxFitness = fitness.Max();
            bestIndividualIndex = fitness.ToList().IndexOf(maxFitness);
            return population[bestIndividualIndex]; ;
        }
    }

    /// <summary>
    /// A versatile multi-population genetic algorithm which uses generics to optimize an individual of any type
    /// The multiple populations run in parallel for fast operation
    /// </summary>
    /// <typeparam name="T">The type of the individual to be optimized</typeparam>
    public class MultiPopulationGA<T>
    {
        #region attributes
        T optimizedIndividual;
        public T OptimizedIndividual
        {
            get { return optimizedIndividual; }
        }

        Random rnd = new Random();
        int populationSize;
        int numPopulations;
        List<GeneticAlgorithm<T>> GAs = new List<GeneticAlgorithm<T>>();
        List<ICrossoverOperator<T>> crossoverOperators = new List<ICrossoverOperator<T>>();
        List<IMutationOperator<T>> mutationOperators = new List<IMutationOperator<T>>();
        List<ISelectionOperator> selectionOperators = new List<ISelectionOperator>();
        List<ICreationOperator<T>> creationOperators = new List<ICreationOperator<T>>();
        IFitnessFunction<T> fitnessFunction;
        #endregion

        /// <summary>
        /// Construct a new multi-population genetic algorithm object
        /// </summary>
        /// <param name="fitnessFn">The object which will handle calculation of an individual's fitness</param>
        /// <param name="NumPopulations">The number of parallel populations to maintain</param>
        /// <param name="PopulationSize">The number of individuals in each population</param>
        public MultiPopulationGA(IFitnessFunction<T> fitnessFn, int NumPopulations, int PopulationSize)
        {
            populationSize = PopulationSize;
            numPopulations = NumPopulations;
            fitnessFunction = fitnessFn;
        }

        /// <summary>
        /// Add an ICrossoverOperator object to the pool of crossover operators available to the multi-population GA
        /// </summary>
        /// <param name="crossover">The ICrossoverOperator</param>
        public void addCrossoverOperator(ICrossoverOperator<T> crossover)
        {
            crossoverOperators.Add(crossover);
        }

        /// <summary>
        /// Add an IMutationOperator object to the pool of mutation operators available to the multi-population GA
        /// </summary>
        /// <param name="mutator">The IMutationOperator</param>
        public void addMutationOperator(IMutationOperator<T> mutator)
        {
            mutationOperators.Add(mutator);
        }

        /// <summary>
        /// Add an ISelectionOperator object to the pool of selection operators available to the multi-population GA
        /// </summary>
        /// <param name="selector">The ISelectionOperator</param>
        public void addSelectionOperator(ISelectionOperator selector)
        {
            selectionOperators.Add(selector);
        }

        /// <summary>
        /// Add an ICreationOperator object to the pool of creation operators available to the multi-population GA
        /// </summary>
        /// <param name="creator">The ICreationOperator</param>
        public void addCreationOperator(ICreationOperator<T> creator)
        {
            creationOperators.Add(creator);
        }

        /// <summary>
        /// Initialize the populations. Each population will use a random combination of the creation, selection, crossover, and mutation operators supplied to the MultiPopulationGA
        /// </summary>
        /// <param name="randomizeReproductionSettings">Set to true to randomize crossover rates and elite fractions for each population</param>
        public void initializeGAs(bool randomizeReproductionSettings)
        {
            GAs.Clear();
            for (int i=0; i<numPopulations; i++)
            {
                ICrossoverOperator<T> thisCrossover = (ICrossoverOperator<T>)crossoverOperators[rnd.Next(crossoverOperators.Count)].Clone();
                IMutationOperator<T> thisMutator = (IMutationOperator<T>)mutationOperators[rnd.Next(mutationOperators.Count)].Clone();
                ISelectionOperator thisSelector = (ISelectionOperator)selectionOperators[rnd.Next(selectionOperators.Count)].Clone();
                ICreationOperator<T> thisCreator = (ICreationOperator<T>)creationOperators[rnd.Next(creationOperators.Count)].Clone();
                IFitnessFunction<T> thisFitness = (IFitnessFunction<T>)fitnessFunction.Clone();

                GeneticAlgorithm<T> thisGA = new GeneticAlgorithm<T>(thisCrossover, thisMutator, thisSelector, thisCreator, thisFitness, populationSize, i);
                if (randomizeReproductionSettings)
                {
                    thisGA.CrossoverRate = rnd.NextDouble() * 0.5 + 0.25;
                    thisGA.EliteFraction = rnd.NextDouble()*0.1 + 0.05;
                }

                GAs.Add(thisGA);
            }
        }

        /// <summary>
        /// Run the multi-population GA for a specified number of iterations
        /// </summary>
        /// <param name="generations">The number of generations to iterate</param>
        /// <returns>The best individual in all populations after iterating</returns>
        public T iterate(int generations)
        {
            if (GAs.Count < 2)
                throw new NullReferenceException("Less than 2 populations initialized");

            T[] bestIndividuals = new T[numPopulations];
            double[] bestFitnesses = new double[numPopulations];

            Parallel.For(0, numPopulations, i =>
                {
                    bestIndividuals[i] = GAs[i].iterate(generations);
                    bestFitnesses[i] = fitnessFunction.CalculateFitness(bestIndividuals[i]);
                });

            //for (int i = 0; i < numPopulations; i++)
            //{
            //    bestIndividuals[i] = GAs[i].iterate(generations);
            //    bestFitnesses[i] = fitnessFunction.CalculateFitness(bestIndividuals[i]);
            //};

            int bestIndex = bestFitnesses.ToList().IndexOf(bestFitnesses.Max());
            optimizedIndividual = bestIndividuals[bestIndex];
            return bestIndividuals[bestIndex];
        }

        /// <summary>
        /// Allow some individuals to migrate randomly between populations
        /// </summary>
        /// <param name="fraction">the fraction of each population to migrate</param>
        public void migrate(double fraction)
        {
            int numIndividuals = (int)Math.Round(fraction * populationSize);
            numIndividuals = Math.Max(numIndividuals, 1);
            int[][] selectionIndeces = new int[numPopulations][];
            List<T> selectedIndividuals = new List<T>();
            List<double> selectedFitness = new List<double>();

            // identify which individuals are moving
            for (int i=0; i<numPopulations; i++)
            {
                selectionIndeces[i] = GAs[i].selectIndividuals(numIndividuals);
                for (int j=0; j<numIndividuals; j++)
                {
                    selectedIndividuals.Add(GAs[i].Population[selectionIndeces[i][j]]);
                    selectedFitness.Add(GAs[i].Fitness[selectionIndeces[i][j]]);
                }
            }

            // randomly fill each gap in each population with one of the selected individuals
            Random rnd = new Random();
            for (int i=0; i<numPopulations; i++)
            {
                for (int j=0; j<numIndividuals; j++)
                {
                    int randIndex = rnd.Next(selectedIndividuals.Count);
                    GAs[i].Population[selectionIndeces[i][j]] = selectedIndividuals[randIndex];
                    GAs[i].Fitness[selectionIndeces[i][j]] = selectedFitness[randIndex];
                    selectedIndividuals.RemoveAt(randIndex);
                    selectedFitness.RemoveAt(randIndex);
                }
            }
        }
    }
}
