using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling
{
    public delegate double predictionEvaluation(double[] trueY, double[] predictions);

    public class CrossValidator
    {
        bool sequentialFoldAssignments = false;
        public bool SequentialFoldAssignments
        {
            get { return sequentialFoldAssignments; }
            set { sequentialFoldAssignments = value; repartition(); }
        }

        int k;
        Random rnd;
        double[][] fullX;
        double[] fullY;
        List<double[]>[] XtrainingSets;
        List<double>[] YtrainingSets;
        List<double[]>[] XtestSets;
        List<double>[] YtestSets;
        List<int>[] instancesAssignedToFolds;


        public CrossValidator(double[][] X, double[] Y, int folds, int rndSeed)
        {
            rnd = new Random(rndSeed);

            folds = Math.Min(folds, X.GetLength(0));

            fullX = X;
            fullY = Y;
            k = folds;

            repartition();
        }

        public double[] getCvPredictions(LearnerInterface learner)
        {
            return getCvPredictions(learner, false, false);
        }

        public double[] getCvPredictions(LearnerInterface learner, bool display, bool useParallel)
        {
            double[] predictions = new double[fullX.GetLength(0)];

            if (!useParallel)
            {
                for (int fold = 0; fold < k; fold++)
                {
                    if (display)
                        Console.WriteLine("fold " + fold);
                    learner.train(XtrainingSets[fold].ToArray(), YtrainingSets[fold].ToArray());
                    double[] preds = learner.predict(XtestSets[fold].ToArray());
                    for (int j = 0; j < preds.Length; j++)
                    {
                        predictions[instancesAssignedToFolds[fold][j]] = preds[j];
                    }
                    if (display)
                        Console.WriteLine("end of fold" + fold);
                }
            }
            else
            {
                Parallel.For(0, k, fold =>
                    {
                        if (display)
                            Console.WriteLine("fold " + fold);
                        LearnerInterface localLearner = (LearnerInterface)learner.Copy();
                        localLearner.train(XtrainingSets[fold].ToArray(), YtrainingSets[fold].ToArray());
                        double[] preds = localLearner.predict(XtestSets[fold].ToArray());
                        for (int j = 0; j < preds.Length; j++)
                        {
                            predictions[instancesAssignedToFolds[fold][j]] = preds[j];
                        }
                        if (display)
                            Console.WriteLine("end of fold" + fold);
                    });
            }

            return predictions;
        }

        public double crossValidate(LearnerInterface learner, predictionEvaluation eval, bool display, bool useParallel, int reps)
        {
            double[] obs = new double[reps];

            for (int rep = 0; rep < reps; rep++)
            {
                double[] predictions = getCvPredictions(learner, display, useParallel);
                obs[rep] = eval(fullY, predictions);
                repartition();
            }

            return obs.Average();
        }

        private void repartition()
        {
            XtrainingSets = new List<double[]>[k];
            YtrainingSets = new List<double>[k];
            XtestSets = new List<double[]>[k];
            YtestSets = new List<double>[k];
            instancesAssignedToFolds = new List<int>[k];

            // initialize training and test set lists
            for (int i = 0; i < k; i++)
            {
                XtrainingSets[i] = new List<double[]>();
                XtestSets[i] = new List<double[]>();
                YtrainingSets[i] = new List<double>();
                YtestSets[i] = new List<double>();

                instancesAssignedToFolds[i] = new List<int>();
            }


            int N = fullX.GetLength(0);

            // generate fold assignments (with equal numbers in each fold)
            List<int> assignmentValues = new List<int>();
            for (int i = 0; i < N; i++)
            {
                assignmentValues.Add(i % k);
            }
            if (sequentialFoldAssignments)
                assignmentValues.Sort();

            int[] assignments = new int[N];

            for (int i = 0; i < N; i++)
            {
                int randIndex = sequentialFoldAssignments ? 0 : rnd.Next(assignmentValues.Count);
                assignments[i] = assignmentValues[randIndex];
                assignmentValues.RemoveAt(randIndex);

                instancesAssignedToFolds[assignments[i]].Add(i);
            }

            // create all training & test partitions
            for (int i = 0; i < N; i++)
            {
                // check each fold individually to see where this instance belongs (training or test)
                for (int f = 0; f < k; f++)
                {
                    if (assignments[i] == f) // if this instance is assigned to this fold
                    {
                        YtestSets[f].Add(fullY[i]);
                        XtestSets[f].Add(fullX[i]);
                    }
                    else
                    {
                        YtrainingSets[f].Add(fullY[i]);
                        XtrainingSets[f].Add(fullX[i]);
                    }
                }
            }
        }
    }
}
