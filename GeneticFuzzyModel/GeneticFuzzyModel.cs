﻿using System;
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

    public class FuzzyRuleBaseModel : LearnerInterface
    {
        int maxGaIterations = 10;

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

        TriVariablePartition outputPartition;
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

        public FuzzyRuleBaseModel(Variable[] Variables, Variable OutputVariable, int NumSets, FuzzyModelFitnessFn fit)
        {
            variables = Variables;
            outputVariable = OutputVariable;
            numSets = NumSets;
            fitnessFn = fit;
        }

        public void train(double[][] trainingX, double[] trainingY)
        {
            Creator_FuzzyRuleBase_Triangle creator = new Creator_FuzzyRuleBase_Triangle(variables, outputVariable, numSets);
            Mutator_FuzzyRuleBase_Triangle mutator = new Mutator_FuzzyRuleBase_Triangle();
            Crossover_FuzzyRuleBase_Triangle cross = new Crossover_FuzzyRuleBase_Triangle();
            RouletteSelector select = new RouletteSelector();
            fitnessFn.setData(trainingX, trainingY);

            MultiPopulationGA<FuzzyRuleBaseModel> ga = new MultiPopulationGA<FuzzyRuleBaseModel>(fitnessFn, 8, 100);
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
            string partitions = "";
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

        // sometimes after altering the underlying partition, we must update the rules to reflect the new sets
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

        public object Clone()
        {
            FuzzyRuleBaseModel cloned = new FuzzyRuleBaseModel();
            cloned.ruleBase = new FuzzyRuleBase(ruleBase);
            cloned.underlyingPartitions = new Dictionary<Variable, TriVariablePartition>(underlyingPartitions);
            cloned.outputPartition = new TriVariablePartition(outputPartition);

            return cloned;
        }

    }


}
