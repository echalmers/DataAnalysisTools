using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyInference
{
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

        public Variable(int ColumnNumber, string Name, double Maximum, double Minimum)
        {
            columnNumber = ColumnNumber;
            name = Name;
            max = Maximum; min = Minimum;
        }

        public Variable(int ColumnNumber, double Maximum, double Minimum)
        {
            columnNumber = ColumnNumber;
            name = "Variable" + columnNumber.ToString();
            max = Maximum; min = Minimum;
        }
    }

    public class FuzzySet
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        IMembershipFunction membershipFn;

        public double CenterOfMass
        {
            get { return membershipFn.CenterOfMass(); }
        }

        public double RepresentativePoint()
        {
            return membershipFn.RepresentativePoint(1); 
        }

        public double RepresentativePoint(double alpha)
        {
            return membershipFn.RepresentativePoint(alpha);
        }

        public FuzzySet(string Name, IMembershipFunction MembershipFn)
        {
            name = Name;
            membershipFn = MembershipFn;
        }

        public double getMembershipValue(double x)
        {
            return membershipFn.getMembership(x);
        }

        public double[][] getPointsForPlot()
        {
            return membershipFn.getPointsForPlot();
        }
    }

    public interface IMembershipFunction
    {
        double getMembership(double xValue);

        double CenterOfMass();

        double RepresentativePoint(double alpha);

        double[][] getPointsForPlot();
    }

    public class triMembershipFunction : IMembershipFunction
    {
        double _a, _b, _c, centerOfMass;

        public triMembershipFunction(double a, double b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
            centerOfMass = (a+b+c)/3;
        }

        public double getMembership(double xValue)
        {
            double line1;
            if (_a == _b)
                line1 = xValue >= _b ? 1 : 0;
            else
                line1 = (xValue - _a) / (_b - _a);

            double line2; 
            if (_b==_c)
                line2 = xValue <= _b ? 1 : 0;
            else
                line2 = (xValue - _b) / -(_c - _b) + 1;
            return Math.Max(Math.Min(line1, line2), 0);
        }

        public double CenterOfMass()
        {
            return centerOfMass;
        }

        public double RepresentativePoint(double alpha)
        {
            // find average of points where y=alpha
            double x1 = (alpha * (_b - _a)) + _a;
            double x2 = _b - ((alpha - 1) * (_c - _b));

            return (x1+x2)/2;
        }

        public double[][] getPointsForPlot()
        {
            double[][] points = new double[][] 
            {
                new double[] {_a, 0},
                new double[] {_b, 1},
                new double[] {_c, 0}
            };
            return points;
        }
         
    }
}
