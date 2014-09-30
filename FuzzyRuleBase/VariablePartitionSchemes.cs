﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyInference
{
    public abstract class VariablePartition : List<FuzzySet>
    {

    }

    public class TriVariablePartition : VariablePartition
    {
        public TriVariablePartition(TriVariablePartition partitionToCopy)
        {
            foreach(FuzzySet s in partitionToCopy)
            {
                this.Add(s);
            }
        }

        public TriVariablePartition(double[] corePoints)
        {
            populateList(corePoints);
        }

        public TriVariablePartition(double min, double max, int numSets)
        {
            populateList(min, max, numSets);
        }

        private void populateList(double min, double max, int numSets)
        {
            double range = max - min;
            double spacing = range / (numSets - 1);
            double[] corePoints = new double[numSets];
            for(int i=0; i<numSets; i++)
            {
                corePoints[i] = min + (i * spacing);
            }
            populateList(corePoints);
        }

        private void populateList(double[] corePoints)
        {
            // clear the existing list
            base.Clear();

            int numSets = corePoints.Length;
            string[] names;
            switch (numSets)
            {
                case 2:
                    names = new string[] {"Low", "High"};
                    break;
                case 3:
                    names = new string[] {"Low","Med","High"};
                    break;
                case 4:
                    names = new string[] {"Low", "Med-Low","Med-High","High"};
                    break;
                case 5:
                    names = new string[] { "Low", "Med-Low", "Med", "Med-High", "High" };
                    break;
                case 6:
                    names = new string[] { "XLow", "Low", "Med-Low", "Med-High", "High", "XHigh" };
                    break;
                case 7:
                    names = new string[] { "XLow", "Low", "Med-Low", "Med", "Med-High", "High", "XHigh" };
                    break;
                default:
                    throw new Exception("number of fuzzy sets must be >=2 and <=7");
            }

            // convert the triangle core points into a list (for sorting)
            List<double> corePointsList = corePoints.ToList();
            corePointsList.Sort();

            // create the List of fuzzy sets' names and membership functions
            for(int i=0; i<numSets; i++)
            {
                triMembershipFunction thisFunction = new triMembershipFunction(corePointsList[Math.Max(0,i - 1)], corePointsList[i], corePointsList[Math.Min(numSets-1, i + 1)]);
                base.Add(new FuzzySet(names[i], thisFunction));
            }
        }
            
    }


    
}
