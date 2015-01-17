using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelling.DecisionTree
{
    public class ClassProbabilities
    {
        Dictionary<double, int> classInstances;
        Dictionary<double, double> probabilities { get; set; }
        public int nTotal { get; set; }

        public ClassProbabilities()
        {
            classInstances = new Dictionary<double, int>();
            nTotal = 0;
        }

        public ClassProbabilities(ClassProbabilities toCopy)
        {
            classInstances = new Dictionary<double, int>(toCopy.classInstances);
            nTotal = toCopy.nTotal;
        }

        public ClassProbabilities(double[] Y)
        {
            // tally class instances
            classInstances = new Dictionary<double, int>();
            foreach (double y in Y)
            {
                nTotal++;
                if (classInstances.ContainsKey(y))
                {
                    classInstances[y]++;
                }
                else
                {
                    classInstances.Add(y, 1);
                }
            }
        }

        public double ProbabilityOf(double classValue)
        {
            if (classInstances.ContainsKey(classValue))
            {
                return (double)classInstances[classValue] / nTotal;
            }
            else
                return 0;
        }

        public double mostLikelyClass()
        {
            double likelyClass = double.NaN;
            foreach (double classValue in classInstances.Keys)
            {
                if (double.IsNaN(likelyClass) || classInstances[classValue] > classInstances[likelyClass])
                {
                    likelyClass = classValue;
                }
            }
            return likelyClass;
        }

        public int nErrors()
        {
            return nTotal - classInstances[mostLikelyClass()];
        }

        public ClassProbabilities Combine(ClassProbabilities secondProbability)
        {
            ClassProbabilities newCP = new ClassProbabilities(this);

            newCP.nTotal = this.nTotal + secondProbability.nTotal;

            foreach(double classVal in secondProbability.classInstances.Keys)
            {
                if (newCP.classInstances.ContainsKey(classVal))
                    newCP.classInstances[classVal] += secondProbability.classInstances[classVal];
                else
                    newCP.classInstances.Add(classVal, secondProbability.classInstances[classVal]);
            }

            return newCP;
        }

        public override string ToString()
        {
            string display = "";
            foreach (double c in classInstances.Keys)
            {
                display += "class " + c + ": " + classInstances[c] + "/" + nTotal + " (" + Math.Round(ProbabilityOf(c),2) + "), ";
            }
            return display;
        }


    }
}
