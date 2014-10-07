using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelling;
using FuzzyInference;

namespace GeneticFuzzyModelling
{
    public class VariableExtractor
    {
        public Variable[] extractVariables(Modelling.DataSet data)
        {
            Variable[] vars = new Variable[data.FeatureNames.Length];
            for (int i=0; i<vars.Length; i++)
            {
                vars[i] = new Variable(i, data.FeatureNames[i], data.featureMax(i), data.featureMin(i));
            }

            return vars;
        }

        public Variable extractOutputVariable(Modelling.DataSet data)
        {
            return new Variable(0, data.OutputName, data.YMax(), data.YMin());
        }
    }
}
