using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelling;
using GeneticFuzzyModelling;
using FuzzyInference;
using System.Diagnostics;
using System.IO;

namespace GeneticFuzzyModelTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // load test data
            Modelling.DataSet data = new DataSet("test.csv"); 
            data.removeMissing();
            
            // generate list of variables from the dataset
            Variable[] variables = new Variable[data.FeatureNames.Length];
            for (int i=0; i<data.FeatureNames.Length; i++)
            {
                variables[i] = new Variable(i,data.FeatureNames[i], data.featureMax(i), data.featureMin(i));
            }
            Variable outputVariable = new Variable(data.X[0].Length-1, data.YMax(), data.YMin());

            // instantiate the fuzzy rule base model
            modelFitnessEvaluator fitnessFn = new modelFitnessEvaluator(data);
            FuzzyRuleBaseModel model = new FuzzyRuleBaseModel(variables, outputVariable, 3, fitnessFn);
            

            // train the model for minimum error of predictions
            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Training model...");
            sw.Start();
            model.train(data.X, data.Y);
            sw.Stop();

            // display the results
            Console.WriteLine("Training time is " + sw.Elapsed.TotalSeconds + "s");
            Console.WriteLine("Mean square error (on training data): " + (-fitnessFn.CalculateFitness(model)).ToString());
            Console.WriteLine(Environment.NewLine + "Model:");
            Console.Write(model.ToString());

            Console.ReadKey();
        }



        public class modelFitnessEvaluator : FuzzyModelFitnessFn
        {
            Modelling.DataSet evaluationData;

            public void setData(double[][] dummy1, double[] dummy2)
            { }

            public modelFitnessEvaluator(Modelling.DataSet EvaluationData)
            {
                evaluationData = EvaluationData;
            }

            public double CalculateFitness(FuzzyRuleBaseModel model)
            {
                // calculate the mean absolute error of the model's predictions
                double[] predictions = model.predict(evaluationData.X);
                double mse = 0;
                for (int i=0; i<predictions.Length; i++)
                {
                    //mse += Math.Abs(predictions[i] - evaluationData.Y[i]);
                    mse += (predictions[i] - evaluationData.Y[i]) * (predictions[i] - evaluationData.Y[i]);
                }

                // the model maximizes fitness, so return the negative error
                return -mse / predictions.Length;
            }

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }
    }
}
