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
    /// Class for performing crossovers on fuzzy rule bases involving triangular membership functions
    /// </summary>
    public class Crossover_FuzzyRuleBase_Triangle : ICrossoverOperator<FuzzyRuleBaseModel>
    {
        /// <summary>
        /// Perform crossover
        /// </summary>
        /// <param name="parent1">The first parent rule base</param>
        /// <param name="parent2">The second parent rule base</param>
        /// <param name="rnd">The random number generator to be used</param>
        /// <returns>The child rule base</returns>
        public FuzzyRuleBaseModel Crossover(FuzzyRuleBaseModel parent1, FuzzyRuleBaseModel parent2, Random rnd)
        {
            FuzzyRuleBaseModel child = new FuzzyRuleBaseModel();

            // average the underlying fuzzy partitions
            foreach(Variable v in parent1.UnderlyingPartitions.Keys)
            {
                child.UnderlyingPartitions.Add(v, averagePartition(parent1.UnderlyingPartitions[v], parent2.UnderlyingPartitions[v]));
            }

            child.OutputPartition = averagePartition(parent1.OutputPartition, parent2.OutputPartition);

            // randomly combine rule bases
            FuzzyRuleBase[] rbs = new FuzzyRuleBase[] {new FuzzyRuleBase(parent1.RuleBase), new FuzzyRuleBase(parent2.RuleBase)};
            int numRules = (rbs[0].Count + rbs[1].Count) / 2;

            for(int i=0; i<numRules; i++)
            {
                int parentNumber = rnd.Next(2);

                if (rbs[parentNumber].Count == 0) // if this parent is out of rules
                {
                    parentNumber = 1 - parentNumber;
                }

                int ruleNumber = rnd.Next(rbs[parentNumber].Count);
                child.RuleBase.Add(rbs[parentNumber][ruleNumber]);
                rbs[parentNumber].RemoveAt(ruleNumber);
            }

            // remap all fuzzy sets in the rules to the new partitions
            foreach (Variable v in parent1.UnderlyingPartitions.Keys)
            {
                child.RemapInputSets(v);
            }
            child.RemapOutputSets();

            return child;

        }

        private TriVariablePartition averagePartition(TriVariablePartition partition1, TriVariablePartition partition2)
        {
            int numSets = partition1.Count;

            // average the two variable partitions
            double[] newCorePts = new double[numSets];
            for (int i = 0; i < numSets; i++)
            {
                newCorePts[i] = (partition1[i].RepresentativePoint() + partition2[i].RepresentativePoint()) / 2;
            }
            return new TriVariablePartition(newCorePts);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
