using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionaryOptimization;
using FuzzyInference;

namespace GeneticFuzzyModelling
{
    /// <summary>
    /// Class for creating random fuzzy rule bases involving triangular membership functions
    /// </summary>
    public class Creator_FuzzyRuleBase_Triangle : ICreationOperator<FuzzyRuleBaseModel>
    {
        int numSets = 3;
        int minRules = 1;
        int maxRules = 3;
        int minRuleLength = 1;
        int maxRuleLength = 5;
        Variable[] variables;
        Variable outputVariable;

        #region constructors

        /// <summary>
        /// Construct a new creator object
        /// </summary>
        /// <param name="Variables">Array of variable objects allowed in the fuzzy rule base</param>
        /// <param name="OutputVariable">The object representing the output variable</param>
        /// <param name="NumSets">The number of fuzzy sets to use when partitioning variables (must be >=2)</param>
        public Creator_FuzzyRuleBase_Triangle(Variable[] Variables, Variable OutputVariable, int NumSets)
        {
            numSets = NumSets;
            variables = Variables;
            outputVariable = OutputVariable;
            maxRuleLength = Math.Min(maxRuleLength, Variables.Length);
        }

        /// <summary>
        /// Construct a new creator object
        /// </summary>
        /// <param name="Variables">Array of variable objects allowed in the fuzzy rule base</param>
        /// <param name="OutputVariable">The object representing the output variable</param>
        /// <param name="MinRules">Minimum number of rules allowed in a rule base</param>
        /// <param name="MaxRules">Maximum number of rules allowed in a rule base</param>
        /// <param name="MinRuleLength">Minimum number of antecedents in a rule</param>
        /// <param name="MaxRuleLength">Maximum number of antecedents in a rule</param>
        /// <param name="NumSets">The number of fuzzy sets to use when partitioning variables (must be >=2)</param>
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

        /// <summary>
        /// Create a random fuzzy rule base
        /// </summary>
        /// <param name="rnd">Random number generator to be used</param>
        /// <returns>A random rule base</returns>
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
