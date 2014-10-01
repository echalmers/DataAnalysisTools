using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyInference
{
    /// <summary>
    /// Class representing a fuzzy rule
    /// </summary>
    [Serializable()]
    public class FuzzyRule
    {
        Dictionary<Variable, FuzzySet> antecedents = new Dictionary<Variable, FuzzySet>();
        public Dictionary<Variable, FuzzySet> Antecedents
        {
            get { return antecedents; }
        }

        FuzzySet consequent;
        public FuzzySet Consequent
        {
            get { return consequent; }
            set { consequent = value; }
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="ruleToCopy">The fuzzy set to (shallow) copy</param>
        public FuzzyRule(FuzzyRule ruleToCopy)
        {
            antecedents = new Dictionary<Variable, FuzzySet>(ruleToCopy.antecedents);
            consequent = ruleToCopy.consequent;
        }

        /// <summary>
        /// Constructor for a fuzzy rule object.
        /// The resulting rule will have the form: If variables[0] is fuzzySets[0] and .... then output is consequentFuzzySet
        /// </summary>
        /// <param name="variables">Array of variables used in this fuzzy rule</param>
        /// <param name="fuzzySets">Array of fuzzy sets corresponding the variables</param>
        /// <param name="consequentFuzzySet">The fuzzy set representing the consequent portion of the rule</param>
        public FuzzyRule(Variable[] variables, FuzzySet[] fuzzySets, FuzzySet consequentFuzzySet)
        {
            for(int i=0; i<variables.Length; i++)
            {
                antecedents.Add(variables[i], fuzzySets[i]);
            }
            consequent = consequentFuzzySet;
        }

        /// <summary>
        /// Add an antecedent to the rule
        /// </summary>
        /// <param name="InputVar">Variable</param>
        /// <param name="IsFuzzySet">Corresponding fuzzy set</param>
        public void addAntecedent(Variable InputVar, FuzzySet IsFuzzySet)
        {
            antecedents.Add(InputVar, IsFuzzySet);
        }

        public override string ToString()
        {
            string ruleForDisplay = "If ";
            foreach(Variable v in antecedents.Keys)
            {
                ruleForDisplay += v.Name + " is " + antecedents[v].Name + "(" + Math.Round(antecedents[v].RepresentativePoint(),1) + "), ";
            }
            ruleForDisplay += "then output is " + consequent.Name + "(" + Math.Round(consequent.RepresentativePoint(),1) + ")";

            return ruleForDisplay;
        }

    }


}
