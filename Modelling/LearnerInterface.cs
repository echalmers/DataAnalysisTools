using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling
{
    /// <summary>
    ///  Interface that all learning algorithms should derive from
    /// </summary>
    public interface LearnerInterface
    {
        void train(double[][] trainingX, double[] trainingY);
        double[] predict(double[][] X);

        /// <summary>
        /// Method intended to produce a copy of a learner
        /// The leaner's thread safety may depend on whether this is a deep or a shallow copy
        /// </summary>
        /// <returns>A copy of the object</returns>
        LearnerInterface Copy();
    }
}
