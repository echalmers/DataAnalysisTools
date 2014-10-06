using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionaryOptimization;
using FuzzyInference;
using Modelling;

namespace GeneticFuzzyModelling
{
    public interface FuzzyModelFitnessFn : IFitnessFunction<FuzzyRuleBaseModel>
    {
        void setData(double[][] X, double[] Y);
    }

    /// <summary>
    /// A model using a trained fuzzy rule base to make predictions
    /// </summary>
    public class FuzzyRuleBaseModel : LearnerInterface
    {
        int maxGaIterations = 10;

        int numPopulations = 64;
        public int NumPopulations
        {
            get { return numPopulations; }
            set { numPopulations = value; }
        }

        int populationSize = 100;
        public int PopulationSize
        {
            get { return populationSize; }
            set { populationSize = value; }
        }


        FuzzyRuleBase ruleBase = new FuzzyRuleBase();
        public FuzzyRuleBase RuleBase
        {
            get { return ruleBase; }
            set { ruleBase = value; }
        }

        Dictionary<Variable, TriVariablePartition> underlyingPartitions = new Dictionary<Variable, TriVariablePartition>();
        public Dictionary<Variable, TriVariablePartition> UnderlyingPartitions
        {
            get { return underlyingPartitions; }
            set { underlyingPartitions = value; }
        }

        TriVariablePartition outputPartition = new TriVariablePartition(0, 1, 3);
        public TriVariablePartition OutputPartition
        {
            get { return outputPartition; }
            set { outputPartition = value; }
        }

        Variable[] variables;
        Variable outputVariable;
        int numSets;
        FuzzyModelFitnessFn fitnessFn;

        public FuzzyRuleBaseModel()
        { }

        /// <summary>
        /// Construct a new fuzzy rule base model
        /// </summary>
        /// <param name="Variables">Array of variable opjects allowed in the rule base</param>
        /// <param name="OutputVariable">Object representing the output variable</param>
        /// <param name="NumSets">Number of fuzzy sets to use when partitioning variables</param>
        /// <param name="fit">Fitness function object used in evolutionary optimization of this fuzzy rule base</param>
        public FuzzyRuleBaseModel(Variable[] Variables, Variable OutputVariable, int NumSets, FuzzyModelFitnessFn fit)
        {
            variables = Variables;
            outputVariable = OutputVariable;
            numSets = NumSets;
            fitnessFn = fit;
        }

        /// <summary>
        /// Train the fuzzy rule base
        /// </summary>
        /// <param name="trainingX">Training inputs</param>
        /// <param name="trainingY">Training outputs</param>
        public void train(double[][] trainingX, double[] trainingY)
        {
            Creator_FuzzyRuleBase_Triangle creator = new Creator_FuzzyRuleBase_Triangle(variables, outputVariable, numSets);
            Mutator_FuzzyRuleBase_Triangle mutator = new Mutator_FuzzyRuleBase_Triangle();
            Crossover_FuzzyRuleBase_Triangle cross = new Crossover_FuzzyRuleBase_Triangle();
            RouletteSelector select = new RouletteSelector();
            fitnessFn.setData(trainingX, trainingY);

            MultiPopulationGA<FuzzyRuleBaseModel> ga = new MultiPopulationGA<FuzzyRuleBaseModel>(fitnessFn, numPopulations, populationSize);
            ga.addCreationOperator(creator);
            ga.addCrossoverOperator(cross);
            ga.addMutationOperator(mutator);
            ga.addSelectionOperator(select);
            ga.initializeGAs(true);

            ga.iterate(5);
            ga.migrate(0.15);
            double bestFitness = fitnessFn.CalculateFitness(ga.OptimizedIndividual);

            for (int i = 0; i < maxGaIterations-1; i++)
            {
                ga.iterate(5);
                ga.migrate(0.15);
                double temp = fitnessFn.CalculateFitness(ga.OptimizedIndividual);
                if (temp == bestFitness)
                {
                    break;
                }
                bestFitness = temp;
            }

            ruleBase = ga.OptimizedIndividual.ruleBase;
            underlyingPartitions = ga.OptimizedIndividual.underlyingPartitions;
            outputPartition = ga.OptimizedIndividual.outputPartition;
        }

        /// <summary>
        /// Predict the output for given set of inputs
        /// </summary>
        /// <param name="X">The array of input vectors for which predictions are to be generated</param>
        /// <returns>The array of predictions</returns>
        public double[] predict(double[][] X)
        {
            double[] predictions = new double[X.GetLength(0)];

            for (int i = 0; i < X.GetLength(0); i++)
            {
                predictions[i] = ruleBase.getOutput(X[i]);
            }
            //Parallel.For(0, X.GetLength(0), i =>
            //    {
            //        FuzzyRuleBase localRuleBaseCopy = new FuzzyRuleBase(ruleBase);
            //        predictions[i] = localRuleBaseCopy.getOutput(X[i]);
            //    });
                
            return predictions;
        }

        public override string ToString()
        {
            string rules = ruleBase.ToString();
            
            // add partitions
            string partitions = "Variable Partitions:" + Environment.NewLine;
            foreach (Variable v in underlyingPartitions.Keys)
            {
                partitions += v.Name + ": ";
                foreach (FuzzySet set in underlyingPartitions[v])
                {
                    partitions += Math.Round(set.RepresentativePoint(),2) + ", ";
                }
                partitions += Environment.NewLine;
            }
            partitions += "Output: ";
            foreach (FuzzySet set in outputPartition)
            {
                double[][] pts = set.getPointsForPlot();
                partitions += Math.Round(set.RepresentativePoint(), 2) + ", ";
            }
            return rules + Environment.NewLine + partitions;
        }

        
        /// <summary>
        /// Sometimes after altering the underlying partition, we must update the rules to reflect the new sets
        /// </summary>
        /// <param name="updatedVariable">The variable which has been altered</param>
        public void RemapInputSets(Variable updatedVariable)
        {
            Dictionary<string, FuzzySet> newSets = new Dictionary<string, FuzzySet>();
            foreach (FuzzySet s in underlyingPartitions[updatedVariable])
            {
                newSets.Add(s.Name, s);
            }

            for (int rule = 0; rule < ruleBase.Count; rule++ )
            {
                if (!ruleBase[rule].Antecedents.ContainsKey(updatedVariable))
                {
                    continue;
                }
                ruleBase[rule].Antecedents[updatedVariable] = newSets[ruleBase[rule].Antecedents[updatedVariable].Name];
            }
        }

        /// <summary>
        /// sometimes after altering the underlying partition, we must update the rules to reflect the new sets
        /// </summary>
        public void RemapOutputSets()
        {
            Dictionary<string, FuzzySet> newSets = new Dictionary<string, FuzzySet>();
            foreach (FuzzySet s in outputPartition)
            {
                newSets.Add(s.Name, s);
            }

            for (int rule = 0; rule < ruleBase.Count; rule++)
            {
                ruleBase[rule].Consequent = newSets[ruleBase[rule].Consequent.Name];
            }
        }

        public LearnerInterface Copy()
        {
            FuzzyRuleBaseModel cloned = new FuzzyRuleBaseModel();
            cloned.ruleBase = new FuzzyRuleBase(ruleBase);
            cloned.underlyingPartitions = new Dictionary<Variable, TriVariablePartition>(underlyingPartitions);
            cloned.outputPartition = new TriVariablePartition(outputPartition);

            return cloned;
        }

    }


}
