using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modelling.DecisionTree
{
    /// <summary>
    /// Interface from which all branchingFunction objects derive.
    /// </summary>
    public interface BranchingFunction
    {
        /// <summary>
        /// Applies the branching function to determine the index of the decision branch associated with the given instance
        /// </summary>
        /// <param name="X">The instance for which a decision branch is to be calculated</param>
        /// <returns>The index of the decision branch</returns>
        int Branch(double[] X);

        /// <summary>
        /// Converts the branching function into a human-readable string
        /// </summary>
        string Print();

        /// <summary>
        /// Returns a copy of the branching function object
        /// </summary>
        BranchingFunction Copy();
    }

    /// <summary>
    /// Branching function for use with discrete valued variables
    /// </summary>
    public class DiscreteValueBranchingFunction : BranchingFunction
    {
        public List<double> distinctValues {get; set;}
        public int splitFeatureIndex {get; set;}
        public string splitFeatureName {get; set;}

        /// <summary>
        /// Copy constructor. Creates a deep copy of the supplied object.
        /// </summary>
        /// <param name="toCopy">the DiscreteValueBranchingFunction object to copy</param>
        public DiscreteValueBranchingFunction(DiscreteValueBranchingFunction toCopy)
        {
            distinctValues = new List<double>(toCopy.distinctValues);
            splitFeatureIndex = toCopy.splitFeatureIndex;
            splitFeatureName = toCopy.splitFeatureName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DistinctValues">Distinct values of the discrete variable</param>
        /// <param name="SplitFeatureIndex">The column index of the variable to be considered in the feature vector</param>
        /// <param name="SplitFeatureName">The name of the variable to be considered</param>
        public DiscreteValueBranchingFunction(List<double> DistinctValues, int SplitFeatureIndex, string SplitFeatureName)
        {
            distinctValues = DistinctValues;
            splitFeatureIndex = SplitFeatureIndex;
            splitFeatureName = SplitFeatureName;
        }

        /// <summary>
        /// Accepts a feature vector and applies the branching function to determine the appropriate decision branch
        /// </summary>
        /// <param name="X">The feature vector</param>
        /// <returns>The index of the decision branch corresponding to this input vector, as per the branching function</returns>
        public int Branch(double[] X)
        {
            int index = distinctValues.IndexOf(X[splitFeatureIndex]);
            if (index == -1)
                throw new ArgumentException("encountered a feature value not seen in training");
            else return index;
        }

        /// <summary>
        /// Converts the branching function to a human-readable string
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            return splitFeatureName + "={" + String.Join(",", distinctValues) + "}";
        }

        /// <summary>
        /// Create a copy of this branching function object.
        /// </summary>
        /// <returns></returns>
        public BranchingFunction Copy()
        {
            return new DiscreteValueBranchingFunction(this);
        }
    }

    /// <summary>
    /// Branching function for use with continuous-valued variables. Supports binary branching only.
    /// </summary>
    public class ContinuousValueBranchFunction : BranchingFunction
    {
        public int splitFeatureIndex {get; set;}
        public string splitFeatureName {get; set;}
        public double thresholdValue { get; set; }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="toCopy">the ContinuousValueBranchFunction object to copy</param>
        public ContinuousValueBranchFunction(ContinuousValueBranchFunction toCopy)
        {
            splitFeatureIndex = toCopy.splitFeatureIndex;
            splitFeatureName = toCopy.splitFeatureName;
            thresholdValue = toCopy.thresholdValue;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ThresholdValue">The threshold value used by the branching function</param>
        /// <param name="SplitFeatureIndex">The column index of the variable to be considered in the feature vector</param>
        /// <param name="SplitFeatureName">The name of the variable to be considered</param>
        public ContinuousValueBranchFunction(double ThresholdValue, int SplitFeatureIndex, string SplitFeatureName)
        {
            thresholdValue = ThresholdValue;
            splitFeatureIndex = SplitFeatureIndex;
            splitFeatureName = SplitFeatureName;
        }

        /// <summary>
        /// Accepts a feature vector and applies the branching function to determine the appropriate decision branch
        /// </summary>
        /// <param name="X">The feature vector</param>
        /// <returns>The index of the decision branch corresponding to this input vector, as per the branching function.
        ///  0 if the split feature is >= the threshold value, 1 otherwise.</returns>
        public int Branch(double[] X)
        {
            return X[splitFeatureIndex] >= thresholdValue ? 0 : 1;
        }

        /// <summary>
        /// Converts the branching function to a human-readable string
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            return splitFeatureName + " >= " + Math.Round(thresholdValue,3);
        }

        /// <summary>
        /// Create a copy of this branching function object.
        /// </summary>
        /// <returns></returns>
        public BranchingFunction Copy()
        {
            return new ContinuousValueBranchFunction(this);
        }
    }

}
