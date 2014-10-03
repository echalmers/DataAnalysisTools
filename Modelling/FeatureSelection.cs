using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling
{
    /// <summary>
    /// A simple, forward feature selection algorithm. Candidate features are evaluated using 
    /// cross validation and a user-supplied objective function
    /// </summary>
    public class GreedyFeatureSelector
    {
        #region fields
        List<int> unselected = new List<int>();
        List<int> selected = new List<int>();
        double[][] X;
        double[] Y;
        double? bestFitness = null;

        int cvFolds = 8;
        public int CvFolds
        {
            get { return cvFolds; }
            set { cvFolds = value; }
        }

        int cvReps = 5;
        public int CvReps
        {
            get { return cvReps; }
            set { cvReps = value; }
        }
        
        int maxFeatures = 1000;
        public int MaxFeatures
        {
            get { return maxFeatures; }
            set { maxFeatures = value; }
        }

        int[] keepin;
        public int[] Keepin
        {
            get { return keepin; }
            set { keepin = value; }
        }

        int rndSeed;
        #endregion

        /// <summary>
        /// Create a feature selector object
        /// </summary>
        /// <param name="trainingX">Training instances</param>
        /// <param name="trainingY">Training outputs</param>
        /// <param name="RandSeed">Seed for random number generation</param>
        public GreedyFeatureSelector(double[][] trainingX, double[] trainingY, int RandSeed)
        {
            X = trainingX;
            Y = trainingY;
            rndSeed = RandSeed;
        }

        /// <summary>
        /// Perform feature selection
        /// </summary>
        /// <param name="learner">the learner to evaluate</param>
        /// <param name="evaluate">the objective function used in the evaluation</param>
        /// <returns>the indices of the features selected</returns>
        public int[] SelectFeatures(LearnerInterface learner, predictionEvaluation evaluate)
        {
            int D = X[0].Length;

            // initialize fitness, and selected and unselected lists
            bestFitness = null;
            unselected.Clear(); selected.Clear();
            for (int i = 0; i < D; i++)
            {
                unselected.Add(i);
            }

            // move all 'keepin' features to the selected list
            if (keepin != null)
            {
                foreach (int i in keepin)
                {
                    selected.Add(i);
                    unselected.Remove(i);
                }
            }

            // outer loop of all candidate features
            for (int i = 0; i < D; i++)
            {
                // inner loop of individual candidate features
                int bestCandidate = -1;

                foreach (int candidate in unselected)
                {
                    // copy the list of current selections as a starting point
                    List<int> candidateSet = new List<int>(selected);

                    // add the current candidate to the candidate set 
                    candidateSet.Add(candidate);

                    // reduce the dataset to the candidate set
                    double[][] Xreduced = reduceDataset(candidateSet.ToArray());

                    //foreach (int s in candidateSet.ToArray())
                    //{
                    //    Console.Write(s + ",");
                    //}

                    // get cross validated predictions using the candidate set
                    CrossValidator cv = new CrossValidator(Xreduced, Y, cvFolds, rndSeed);
                    //double[] thesePredictions = cv.getCvPredictions(learner);
                    //double thisFitness = evaluate(Y, thesePredictions);
                    double thisFitness = cv.crossValidate(learner, evaluate, false, false, cvReps);

                    //Console.Write("[" + thisFitness + "]" + Environment.NewLine);

                    // see if adding this candidate has improved anything
                    if ((bestFitness == null) || (thisFitness < bestFitness))
                    {
                        bestCandidate = candidate;
                        bestFitness = thisFitness;
                    }
                }

                // if none of the candidates improved the fitness, break
                if (bestCandidate == -1)
                    break;
                else // otherwise transfer the best candidate to the 'selected' list
                {
                    selected.Add(bestCandidate);
                    unselected.Remove(bestCandidate);
                    foreach (int feature in selected)
                    {
                        Console.Write(feature + ",");
                    }
                    Console.Write("[" + bestFitness + "]" + Environment.NewLine);

                    if (selected.Count >= maxFeatures)
                    {
                        break;
                    }
                }
            }

            // return the 'selected' list as an array
            return selected.ToArray();
        }

        /// <summary>
        /// reduce the dataset (supplied during construction) to only selected features
        /// </summary>
        /// <param name="selections">indices of selected features</param>
        /// <returns>the feature-reduced training data</returns>
        public double[][] reduceDataset(int[] selections)
        {
            double[][] xReduced = new double[X.Length][];
            for (int i = 0; i < X.Length; i++)
            {
                xReduced[i] = new double[selections.Length];

                for (int j = 0; j < selections.Length; j++)
                {
                    xReduced[i][j] = X[i][selections[j]];
                }
            }

            return xReduced;
        }



    }
}
