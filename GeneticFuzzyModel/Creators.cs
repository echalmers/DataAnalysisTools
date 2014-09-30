using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionaryOptimization;
using FuzzyInference;

namespace GeneticFuzzyModelling
{
    public class Creator_FuzzyRuleBase_Triangle : ICreationOperator<FuzzyRuleBaseModel>
    {
        int numSets = 3;
        int minRules = 1;
        int maxRules = 7;
        int minRuleLength = 1;
        int maxRuleLength = 5;
        Variable[] variables;
        Variable outputVariable;

        #region constructors
        public Creator_FuzzyRuleBase_Triangle(Variable[] Variables, Variable OutputVariable, int NumSets)
        {
            numSets = NumSets;
            variables = Variables;
            outputVariable = OutputVariable;
            maxRuleLength = Math.Min(maxRuleLength, Variables.Length);
        }
        public Creator_FuzzyRuleBase_Triangle(Variable[] Variables, Variable OutputVariable, int MinRules, int MaxRules, int MinRuleLength, int MaxRuleLength, int NumSets)        
        {
            numSets = NumSets;
            variables = Variables;
            outputVariable = OutputVariable;
            minRules = MinRules;
            maxRules = MaxRules;
            minRuleLength = MinRuleLength;
            maxRuleLength = Math.Min(MaxRuleLength, Variables.Length);
        }
        #endregion

        public FuzzyRuleBaseModel CreateRandomIndividual(Random rnd)
        {
            FuzzyRuleBaseModel rb = new FuzzyRuleBaseModel();

            // create random variable partitions
            Dictionary<Variable, TriVariablePartition> underlyingPartitions = new Dictionary<Variable,TriVariablePartition>();
            foreach (Variable v in variables)
            {
                double[] thisCorePoints = new double[numSets];
                thisCorePoints[0] = v.Min; thisCorePoints[1] = v.Max;
                for (int i=2; i<numSets; i++)
                {
                    thisCorePoints[i] = rnd.NextDouble() * (v.Max - v.Min) + v.Min;
                }
                TriVariablePartition thisPartition = new TriVariablePartition(thisCorePoints);
                underlyingPartitions.Add(v,thisPartition);
            }
            // and a random partition for the output
            double[] outCorePoints = new double[numSets];
            outCorePoints[0] = outputVariable.Min; outCorePoints[1] = outputVariable.Max;
            for (int i = 2; i < numSets; i++)
            {
                outCorePoints[i] = rnd.NextDouble() * (outputVariable.Max - outputVariable.Min) + outputVariable.Min;
            }
            TriVariablePartition outPartition = new TriVariablePartition(outCorePoints);

            // create a set of random rules
            int numRules = rnd.Next(minRules, maxRules+1);
            for (int r = 0; r<numRules; r++)
            {
                int numProps = rnd.Next(minRuleLength,maxRuleLength+1);
                Variable[] thisVariables = new Variable[numProps];
                FuzzySet[] thisSets = new FuzzySet[numProps];

                List<Variable> variableChoices = variables.ToList();

                for (int p=0; p<numProps; p++)
                {
                    thisVariables[p] = variableChoices[rnd.Next(variableChoices.Count)];
                    variableChoices.Remove(thisVariables[p]);
                    thisSets[p] = underlyingPartitions[thisVariables[p]][rnd.Next(underlyingPartitions[thisVariables[p]].Count)];
                }

                FuzzyRule thisRule = new FuzzyRule(thisVariables, thisSets,outPartition[rnd.Next(outPartition.Count)]);
                rb.RuleBase.Add(thisRule);
            }

            rb.UnderlyingPartitions = underlyingPartitions;
            rb.OutputPartition = outPartition;

            return rb;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
