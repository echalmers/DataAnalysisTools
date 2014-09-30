using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling
{
    public interface LearnerInterface
    {
        void train(double[][] trainingX, double[] trainingY);
        double[] predict(double[][] X);
        LearnerInterface Copy();
    }
}
