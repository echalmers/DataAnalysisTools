using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Modelling
{
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
        
        double?[][] x;
        public double?[][] Xnullable
        {
            get { return x; }
        }
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

        public DataSet()
        { }

        public DataSet(string filename)
        {
            fromCsv(filename);
        }

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

        //private void readNumericData(string filename)
        //{
        //    List<double[]> Xlist = new List<double[]>();
        //    List<double> Ylist = new List<double>();

        //    StreamReader rdr = new StreamReader(filename);

        //    // read first line
        //    string line = rdr.ReadLine();
        //    char[] delimiter = {','};
        //    string[] fields = line.Split(delimiter);
            
        //    // get # of features and feature names from the first line
        //    featureNames = new string[fields.Length-1];

        //    for (int i = 0; i<fields.Length-1; i++)
        //    {
        //        featureNames[i] = fields[i];
        //    }
        //    outputName = fields[fields.Length - 1];

        //    // read remainder of lines and get numeric data
        //    while ((line=rdr.ReadLine())!=null)
        //    {
        //        bool incompleteInstance = false;
        //        double[] thisInstance = new double[featureNames.Length];
        //        fields = line.Split(delimiter);
        //        for (int i=0; i<featureNames.Length; i++)
        //        {
        //            //****** skip instance if it has missing values
        //            if (fields[i] == "")
        //            {
        //                incompleteInstance = true;
        //                break;
        //            }
        //            //
        //            if (double.TryParse(fields[i], out thisInstance[i]) == false)
        //                throw new Exception("error converting to numeric data");
        //        }

        //        //****** skip instance if it has missing values
        //        if (incompleteInstance)
        //            continue;
        //        //
        //        Xlist.Add(thisInstance);

        //        double thisOutcome;
        //        if (double.TryParse(fields[fields.Length-1],out thisOutcome) == false)
        //            throw new Exception("error converting to numeric data");
        //        Ylist.Add(thisOutcome);
        //    }

        //    x = Xlist.ToArray();
        //    y = Ylist.ToArray();
        //}
    }
}
