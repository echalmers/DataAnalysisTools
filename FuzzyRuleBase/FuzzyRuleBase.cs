using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyInference
{
   
    /// <summary>
    /// Class representing a fuzzy rule base
    /// </summary>
    public class FuzzyRuleBase : List<FuzzyRule>
    {
        //public void addRule(FuzzyRule rule)
        //{
        //    base.Add(rule);
        //}

        //public void deleteRule(FuzzyRule rule)
        //{
        //    base.Remove(rule);
        //}

        //public void deleteRule(int Index)
        //{
        //    base.RemoveAt(Index);
        //}

        public FuzzyRuleBase()
        {
        }

        /// <summary>
        ///  Copy constructor
        /// </summary>
        /// <param name="rbToCopy"></param>
        public FuzzyRuleBase(FuzzyRuleBase rbToCopy)// : base(rb)
        {
            foreach (FuzzyRule rule in rbToCopy)
            {
                FuzzyRule copiedRule = new FuzzyRule(rule);
                this.Add(copiedRule);
            }
        }

        /// <summary>
        /// Apply the rule base to an input value
        /// </summary>
        /// <param name="X">The input vector</param>
        /// <returns>The defuzzified output</returns>
        public double getOutput(double[] X)
        {
            double sumActivation = 0;
            double output = 0;

            // evaluate each rule
            foreach (FuzzyRule rule in this)
            {
                double[] thisRuleActivations = new double[rule.Antecedents.Count];
                Variable[] thisRuleVariables = rule.Antecedents.Keys.ToArray();

                // evaluate each proposition
                for (int i=0; i<rule.Antecedents.Count; i++)
                {
                    int varIndex = thisRuleVariables[i].ColumnNumber;
                    FuzzySet set = rule.Antecedents[thisRuleVariables[i]];
                    thisRuleActivations[i] = set.getMembershipValue(X[varIndex]);
                }

                // apply AND operator
                double andedActivation = thisRuleActivations.Min();

                // add the result to the weighted average
                output += andedActivation * rule.Consequent.RepresentativePoint(andedActivation);
                sumActivation += andedActivation;
            }
            output /= sumActivation + 0.0001; // add 0.0001 to avoid divide by 0

            return output;
        }

        public override string ToString()
        {
            string forDisplay = "";
            foreach(FuzzyRule rule in this)
            {
                forDisplay += rule.ToString() + Environment.NewLine;
            }

            return forDisplay;
        }
    }

    
}
