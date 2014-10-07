using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Modelling
{
    /// <summary>
    ///  class useful for reading and manipulating datasets
    /// </summary>
    public class DataSet
    {
        bool missingValuesRemoved = false;

        string[] featureNames;
        public string[] FeatureNames
        {
            get { return featureNames; }
        }
        string[] textFieldNames;
        public string[] TextFieldNames
        {
            get { return textFieldNames; }
        }
        string outputName;
        public string OutputName
        {
            get { return outputName; }
            set { outputName = value; }
        }
        
        double?[][] x;
        /// <summary>
        /// All instance data including null (missing) values
        /// </summary>
        public double?[][] Xnullable
        {
            get { return x; }
        }

        /// <summary>
        /// All instance data excluding missing values
        /// Throws ArgumentNullException if missing values have not been handled using an appropriate method
        /// </summary>
        public double[][] X
        {
            get
            {
                if (missingValuesRemoved)
                    return convertFromNullable(x);
                else
                    throw new ArgumentNullException("DataSet: handle missing values before accessing double[][] X");
            }
        }
        
        double[] y;
        public double[] Y
        {
            get { return y; }
        }

        string[][] text;
        public string[][] Text
        {
            get { return text; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataSet()
        { }

        /// <summary>
        /// Construct DataSet object from data stored in a csv file
        /// </summary>
        /// <param name="filename">csv file to be read</param>
        public DataSet(string filename)
        {
            fromCsv(filename);
        }

        /// <summary>
        /// Populate this DataSet object using data stored in a csv file
        /// </summary>
        /// <param name="filename">csv file to be read</param>
        public void fromCsv(string filename)
        {
            missingValuesRemoved = false;

            List<double?[]> Xlist = new List<double?[]>();
            List<double> Ylist = new List<double>();
            List<string[]> textList = new List<string[]>();

            StreamReader rdr = new StreamReader(filename);
            char[] delimiter = { ',' };

            // read header line
            string line = rdr.ReadLine();
            string[] fields = line.Split(delimiter);

            // read lines until we determine if each field is numeric or text
            // (if there are missing values it may take more than one read)
            bool?[] isNumericField = new bool?[fields.Length - 1];
            double temp;
            while (Array.FindIndex(isNumericField, (i) => i == null) != -1) // loop while there are still fields that haven't been identified as numeric or not
            {
                line = rdr.ReadLine();
                fields = line.Split(delimiter);
                for (int f = 0; f < fields.Length-1; f++)
                {
                    if ((isNumericField[f] != null) || (fields[f] == "") || (fields[f] == null))
                        continue;

                    if (double.TryParse(fields[f], out temp))
                        isNumericField[f] = true;
                    else
                        isNumericField[f] = false;
                }
            }
            int numNumericFields = Array.FindAll(isNumericField, (i) => i == true).Length;
            int numTextFields = Array.FindAll(isNumericField, (i) => i == false).Length;


            // now we know if each field is numeric or text. Start reading again, and loading data into appropriate lists
            rdr.Close();
            rdr = new StreamReader(filename);

            // get feature names from first line
            List<string> featureNamesList = new List<string>();
            List<string> textFieldNamesList = new List<string>();
            line = rdr.ReadLine();
            fields = line.Split(delimiter);
            for (int i=0; i<fields.Length-1; i++)
            {
                if (isNumericField[i] == true)
                    featureNamesList.Add(fields[i]);
                else
                    textFieldNamesList.Add(fields[i]);
            }
            featureNames = featureNamesList.ToArray();
            textFieldNames = textFieldNamesList.ToArray();
            outputName = fields[fields.Length-1];

            // process remainder of lines
            while ((line=rdr.ReadLine())!=null)
            {
                List<double?> thisInstance = new List<double?>();
                List<string> thisText = new List<string>();
                double thisY;

                fields = line.Split(delimiter);
                for (int i=0; i<fields.Length-1; i++)
                {
                    // if this is a numeric field
                    if (isNumericField[i]==true)
                    {
                        // try to convert the field to a number
                        try
                        {
                            thisInstance.Add(Convert.ToDouble(fields[i]));
                        }
                        catch // otherwise add a null value
                        {
                            thisInstance.Add(null);
                        }
                    }

                    // if this is a text field
                    else
                    {
                        thisText.Add(fields[i]);
                    }
                }
                
                // convert the output to a number. If this fails, skip this instance
                if (!Double.TryParse(fields[fields.Length - 1], out thisY))
                    continue;

                // add the data to the dataset
                Xlist.Add(thisInstance.ToArray());
                textList.Add(thisText.ToArray());
                Ylist.Add(thisY);
            }

            x = Xlist.ToArray();
            y = Ylist.ToArray();
            text = textList.ToArray();
        }

        /// <summary>
        /// Remove all instances with missing values
        /// </summary>
        public void removeMissing()
        {
            List<double?[]> xList = x.ToList();
            List<double> yList = y.ToList();
            List<string[]> textList = text.ToList();

            for (int i = xList.Count-1; i >= 0; i--)
            {
                // if this instance contains a missing value, remove it
                if (Array.FindIndex(xList[i],(element)=>element==null) != -1)
                {
                    xList.RemoveAt(i);
                    yList.RemoveAt(i);
                    textList.RemoveAt(i);
                }
            }

            x = xList.ToArray();
            y = yList.ToArray();
            text = textList.ToArray();

            missingValuesRemoved = true;
        }

        /// <summary>
        /// Return a subset of this DataSet
        /// </summary>
        /// <param name="startIndex">Zero-based index of the first instance in the range to return</param>
        /// <param name="endIndex">Exclusive index of the last instance in the range</param>
        /// <returns>A new DataSet object containing the requested range of instances</returns>
        public DataSet dataSubset(int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;

            DataSet newSet = new DataSet();
            newSet.missingValuesRemoved = missingValuesRemoved ? true : false;

            newSet.textFieldNames = new string[textFieldNames.Length];
            Array.Copy(textFieldNames, newSet.textFieldNames, textFieldNames.Length);

            newSet.featureNames = new string[featureNames.Length];
            Array.Copy(featureNames, newSet.featureNames, featureNames.Length);

            newSet.outputName = String.Copy(outputName);
                      
            newSet.text = new string[length][];
            newSet.x = new double?[length][];
            newSet.y = new double[length];

            for (int i=0; i<length; i++)
            {
                newSet.text[i] = text[i + startIndex];
                newSet.x[i] = x[i + startIndex];
                newSet.y[i] = y[i + startIndex];
            }

            return newSet;
        }

        /// <summary>
        /// Calculate the maximum value of a feature
        /// </summary>
        /// <param name="featureIndex">Index of the feature in the dataset</param>
        /// <returns>The feature's max value</returns>
        public double featureMax(int featureIndex)
        {
            double? max = null;
            foreach(double[] instance in X)
            {
                if ((instance[featureIndex] > max) || (max == null))
                    max = instance[featureIndex];
            }
            return (double)max;
        }

        /// <summary>
        /// Calculate the minimum value of a feature
        /// </summary>
        /// <param name="featureIndex">Index of the feature in the dataset</param>
        /// <returns>The feature's minimum value</returns>
        public double featureMin(int featureIndex)
        {
            double? min = null;
            foreach (double[] instance in X)
            {
                if ((instance[featureIndex] < min) || (min == null))
                    min = instance[featureIndex];
            }
            return (double)min;
        }

        /// <summary>
        /// Calculate the maximum value of the output 
        /// </summary>
        /// <returns>The max value</returns>
        public double YMax()
        {
            double? max = null;
            foreach (double output in Y)
            {
                if ((output > max) || (max == null))
                    max = output;
            }
            return (double)max;
        }

        /// <summary>
        /// Calculate the minimum value of the output
        /// </summary>
        /// <returns>The min value</returns>
        public double YMin()
        {
            double? min = null;
            foreach (double output in Y)
            {
                if ((output < min) || (min == null))
                    min = output;
            }
            return (double)min;
        }


        private double[][] convertFromNullable(double?[][] nullable)
        {
            double[][] nonNullable = new double[nullable.Length][];

            for (int i=0; i<nullable.Length; i++)
            {
                nonNullable[i] = new double[nullable[i].Length];
                
                for (int j=0; j<nonNullable[i].Length; j++)
                {
                    nonNullable[i][j] = (double)nullable[i][j];
                }
            }

            return nonNullable;
        }

        
    }
}
