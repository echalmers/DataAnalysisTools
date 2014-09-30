using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyInference
{
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

        public FuzzyRule(FuzzyRule ruleToCopy)
        {
            antecedents = new Dictionary<Variable, FuzzySet>(ruleToCopy.antecedents);
            consequent = ruleToCopy.consequent;
        }

        public FuzzyRule(Variable[] variables, FuzzySet[] fuzzySets, FuzzySet consequentFuzzySet)
        {
            for(int i=0; i<variables.Length; i++)
            {
                antecedents.Add(variables[i], fuzzySets[i]);
            }
            consequent = consequentFuzzySet;
        }

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
