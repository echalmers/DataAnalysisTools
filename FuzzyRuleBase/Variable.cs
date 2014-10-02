using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyInference
{
    /// <summary>
    /// Struct representing a variable in a dataset
    /// </summary>
    public struct Variable
    {
        double max;
        public double Max
        {
            get { return max; }
        }

        double min;
        public double Min
        {
            get { return min; }
        }

        int columnNumber;
        public int ColumnNumber
        {
            get { return columnNumber; }
        }

        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Constructor for a Variable object
        /// </summary>
        /// <param name="ColumnNumber">The zero-based index of the column for this variable in the dataset</param>
        /// <param name="Name">The variable's name</param>
        /// <param name="Maximum">The variable's maximum value</param>
        /// <param name="Minimum">The variable's minimum value</param>
        public Variable(int ColumnNumber, string Name, double Maximum, double Minimum)
        {
            columnNumber = ColumnNumber;
            name = Name;
            max = Maximum; min = Minimum;
        }

        /// <summary>
        /// Constructor for a Variable object
        /// </summary>
        /// <param name="ColumnNumber">The zero-based index of the column for this variable in the dataset</param>
        /// <param name="Maximum">The variable's maximum value</param>
        /// <param name="Minimum">The variable's minimum value</param>
        public Variable(int ColumnNumber, double Maximum, double Minimum)
        {
            columnNumber = ColumnNumber;
            name = "Variable" + columnNumber.ToString();
            max = Maximum; min = Minimum;
        }
    }
}
