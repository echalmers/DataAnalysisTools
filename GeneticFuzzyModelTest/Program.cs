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
    /// <summary>
    /// This is a simple test program for the GeneticFuzzyModel class.
    /// The test supposes two input variables (x1 and x2) and an output variable (y) all with range 0-1. 
    /// Each is partitioned into 3 triangular fuzzy sets ("low"/"med"/"high"). A rule base is created with 
    /// two rules:
    ///     "If x1 is low and x2 is low, y is low"
    ///     "If x1 is low and x2 is med, y is med"
    /// This rule base is used to generate y values corresponding to 100 random [x1,x2] instances, giving a
    /// training dataset of 100 instances.
    /// A GeneticFuzzyModel is instantiated and trained using this data. After the training process the 
    /// GeneticFuzzyModel's rule base should closely match the original.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // declare the variables and fuzzy sets
            int numSets = 3;
            Variable x1 = new Variable(0, "x1", 1, 0);
            Variable x2 = new Variable(1, "x2", 1, 0);
            Variable y = new Variable(2, "y", 1, 0);

            TriVariablePartition x1part = new TriVariablePartition(0, 1, numSets);
            TriVariablePartition x2part = new TriVariablePartition(0, 1, numSets);
            TriVariablePartition ypart = new TriVariablePartition(0, 1, numSets);

            // create the fuzzy rule base
            FuzzyRuleBase rb = new FuzzyRuleBase();
            rb.Add(new FuzzyRule(new Variable[] { x1, x2 }, new FuzzySet[] { x1part[0], x2part[0] }, ypart[0]));
            rb.Add(new FuzzyRule(new Variable[] { x1, x2 }, new FuzzySet[] { x1part[0], x2part[1] }, ypart[1]));

            // use the fuzzy rule base to create a training dataset
            Random rnd = new Random();
            double[][] testX = new double[100][];
            double[] testY = new double[100];
            for (int i=0; i<100; i++)
            {
                testX[i] = new double[2] { rnd.NextDouble(), rnd.NextDouble() };
                testY[i] = rb.getOutput(testX[i]);
            }

            
            // instantiate the fuzzy rule base model
            modelFitnessEvaluator fitnessFn = new modelFitnessEvaluator(testX, testY);
            FuzzyRuleBaseModel model = new FuzzyRuleBaseModel(new Variable[] { x1, x2 }, y, numSets, fitnessFn);
            

            // train the model to emulate the original rule base (train for minimum error of predictions)
            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Training model...");
            sw.Start();
            model.train(testX, testY);
            sw.Stop();

            // display the results
            Console.WriteLine("Training time is " + sw.Elapsed.TotalSeconds + "s");
            Console.WriteLine("Mean square error (on training data): " + (-fitnessFn.CalculateFitness(model)).ToString());
            Console.WriteLine(Environment.NewLine + "Rule-base used to generate data:");
            Console.Write(rb.ToString());

            Console.WriteLine(Environment.NewLine + "Model trained to emulate the rule-base:");
            Console.Write(model.ToString());

            Console.ReadKey();
        }



        public class modelFitnessEvaluator : FuzzyModelFitnessFn
        {
            double[][] xData;
            double[] yData;

            public void setData(double[][] dummy1, double[] dummy2)
            { }

            public modelFitnessEvaluator(double[][] X, double[] Y)
            {
                xData = X;
                yData = Y;
            }

            public double CalculateFitness(FuzzyRuleBaseModel model)
            {
                // calculate the mean absolute error of the model's predictions
                double[] predictions = model.predict(xData);
                double mse = 0;
                for (int i=0; i<predictions.Length; i++)
                {
                    //mse += Math.Abs(predictions[i] - evaluationData.Y[i]);
                    mse += (predictions[i] - yData[i]) * (predictions[i] - yData[i]);
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
