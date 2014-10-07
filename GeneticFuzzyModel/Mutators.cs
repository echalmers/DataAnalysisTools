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
    /// Mutator for fuzzy rule bases which use triangular membership functions
    /// </summary>
    public class Mutator_FuzzyRuleBase_Triangle : IMutationOperator<FuzzyRuleBaseModel>
    {
        int minRuleLength = 1;
        int maxRuleLength = 4;
        int maxRules = 7;

        /// <summary>
        /// Perform mutation
        /// </summary>
        /// <param name="parent">The parent fuzzy fule base</param>
        /// <param name="rnd">The random number generator to be used</param>
        /// <returns>A mutated version of the parent rule base</returns>
        public FuzzyRuleBaseModel Mutate(FuzzyRuleBaseModel parent, Random rnd)
        {
            // possible mutations:
            // 0) delete rule
            // 1) add rule
            // 2) add antecedent proposition
            // 3) delete antecedent proposition
            // 4) change an antecedent set
            // 5) change a consequent set
            // 6) mutate an underlying partition
            // 7) mutate the output partition

            FuzzyRuleBaseModel child = (FuzzyRuleBaseModel)parent.Copy();


            switch(rnd.Next(8))
            {
                case 0: // delete a rule
                    if (child.RuleBase.Count <= 1)
                    {
                        // if there is only one rule, fall through to case 1 instead
                        goto case 1;
                    }
                    else
                    {
                        // randomly select a rule to delete
                        child.RuleBase.RemoveAt(rnd.Next(child.RuleBase.Count));
                    }
                    break;

                case 1: // add a rule
                    if (child.RuleBase.Count >= maxRules)
                    {
                        // if there are already the max number of rules, go to case 0 instead
                        goto case 0;
                    }

                    int numProps = Math.Min(child.UnderlyingPartitions.Count, rnd.Next(minRuleLength,maxRuleLength+1));
                    Variable[] thisVariables = new Variable[numProps];
                    FuzzySet[] thisSets = new FuzzySet[numProps];

                    List<Variable> variableChoices = child.UnderlyingPartitions.Keys.ToList();

                    for (int p=0; p<numProps; p++)
                    {
                        thisVariables[p] = variableChoices[rnd.Next(variableChoices.Count)];
                        variableChoices.Remove(thisVariables[p]);
                        thisSets[p] = child.UnderlyingPartitions[thisVariables[p]][rnd.Next(child.UnderlyingPartitions[thisVariables[p]].Count)];
                    }

                    FuzzyRule thisRule = new FuzzyRule(thisVariables, thisSets,child.OutputPartition[rnd.Next(child.OutputPartition.Count)]);
                    child.RuleBase.Add(thisRule);
                    break;

                case 2: // add antecedent proposition
                    int ruleNum = rnd.Next(child.RuleBase.Count);

                    // find out what variables are available to add
                    List<Variable> addVariableChoices = child.UnderlyingPartitions.Keys.ToList();

                    if (child.RuleBase[ruleNum].Antecedents.Count >= addVariableChoices.Count)
                    {
                        // if no more propositions can be added to this rule, go to case 3
                        goto case 3;
                    }
                    foreach(Variable v in child.RuleBase[ruleNum].Antecedents.Keys)
                    {
                        addVariableChoices.Remove(v);
                    }

                    // add one
                    Variable addedVar = addVariableChoices[rnd.Next(addVariableChoices.Count)];
                    FuzzySet addedSet = child.UnderlyingPartitions[addedVar][rnd.Next(child.UnderlyingPartitions[addedVar].Count)];
                    child.RuleBase[ruleNum].addAntecedent(addedVar, addedSet);
                    break;

                case 3: // delete antecedent proposition
                    ruleNum = rnd.Next(child.RuleBase.Count);

                    if (child.RuleBase[ruleNum].Antecedents.Count <= 1)
                    {
                        // if this rule has only one antecedent, go to case 4 instead
                        goto case 4;
                    }
                    Variable[] varChoices = child.RuleBase[ruleNum].Antecedents.Keys.ToArray();
                    Variable varToRemove = varChoices[rnd.Next(varChoices.Length)];
                    child.RuleBase[ruleNum].Antecedents.Remove(varToRemove);
                    break;

                case 4: // change an antecedent set
                    // select the rule and proposition
                    ruleNum = rnd.Next(child.RuleBase.Count);
                    List<Variable> propositionOptions = Enumerable.ToList(child.RuleBase[ruleNum].Antecedents.Keys);
                    Variable propVar = propositionOptions[rnd.Next(propositionOptions.Count)];

                    // get the fuzzy set options (remove the one currently in use)
                    List<FuzzySet> setOptions = new List<FuzzySet>(child.UnderlyingPartitions[propVar]);
                    bool removed = setOptions.Remove(child.RuleBase[ruleNum].Antecedents[propVar]);

                    // change the set in the selected antecedent to one of the available options
                    child.RuleBase[ruleNum].Antecedents[propVar] = setOptions[rnd.Next(setOptions.Count)];
                    break;

                case 5: // change a consequent set
                    // select the rule
                    ruleNum = rnd.Next(child.RuleBase.Count);
                    
                    // get the fuzzy set options (remove the one currently in use)
                    setOptions = new List<FuzzySet>(child.OutputPartition);
                    removed = setOptions.Remove(child.RuleBase[ruleNum].Consequent);

                    // randomly change the consequent set 
                    child.RuleBase[ruleNum].Consequent = setOptions[rnd.Next(setOptions.Count)];
                    break;

                case 6: // mutate an underlying partition
                    Variable partVar = Enumerable.ToArray(child.UnderlyingPartitions.Keys)[rnd.Next(child.UnderlyingPartitions.Count)];
                    int numSets = child.UnderlyingPartitions[partVar].Count;
                    if (numSets <= 2)
                        goto case 4;

                    double[] corePts = new double[numSets];
                    
                    for (int i=0; i<numSets; i++)
                    {
                        corePts[i] = child.UnderlyingPartitions[partVar][i].RepresentativePoint();
                    }
                    int setToChange = rnd.Next(numSets - 2) + 1;
                    double newPt = rnd.NextDouble() * (corePts[setToChange+1] - corePts[setToChange-1]) + corePts[setToChange-1];
                    corePts[setToChange] = newPt;
                    child.UnderlyingPartitions[partVar] = new TriVariablePartition(corePts);

                    child.RemapInputSets(partVar);
                    break;

                case 7: // mutate the output partition
                    numSets = child.OutputPartition.Count;
                    if (numSets <= 2)
                        goto case 5;

                    corePts = new double[numSets];
                    
                    for (int i=0; i<numSets; i++)
                    {
                        corePts[i] = child.OutputPartition[i].RepresentativePoint();
                    }
                    setToChange = rnd.Next(numSets - 2) + 1;
                    newPt = rnd.NextDouble() * (corePts[setToChange+1] - corePts[setToChange-1]) + corePts[setToChange-1];
                    corePts[setToChange] = newPt;
                    child.OutputPartition = new TriVariablePartition(corePts);

                    child.RemapOutputSets();
                    break;
            }

           
            return child;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
