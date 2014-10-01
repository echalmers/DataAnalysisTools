using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyInference
{
    /// <summary>
    /// Class representing a fuzzy set
    /// </summary>
    [Serializable()]
    public class FuzzySet
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        IMembershipFunction membershipFn;

        /// <summary>
        /// The center of mass of the membership function (assuming no alpha cut)
        /// </summary>
        public double CenterOfMass
        {
            get { return membershipFn.CenterOfMass(); }
        }

        /// <summary>
        /// A representative value for the set, assuming no alpha cut
        /// </summary>
        /// <returns></returns>
        public double RepresentativePoint()
        {
            return membershipFn.RepresentativePoint(1); 
        }

        /// <summary>
        /// A representative value for the set given an alpha cut
        /// </summary>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public double RepresentativePoint(double alpha)
        {
            return membershipFn.RepresentativePoint(alpha);
        }

        /// <summary>
        /// Constructor for a fuzzy set object
        /// </summary>
        /// <param name="Name">A linguistic-term name for the fuzzy set</param>
        /// <param name="MembershipFn">The fuzzy set's underlying membership function</param>
        public FuzzySet(string Name, IMembershipFunction MembershipFn)
        {
            name = Name;
            membershipFn = MembershipFn;
        }

        /// <summary>
        /// Returns a point's membership in this set
        /// </summary>
        /// <param name="x">The input value</param>
        /// <returns>Membership value</returns>
        public double getMembershipValue(double x)
        {
            return membershipFn.getMembership(x);
        }

        /// <summary>
        /// Returns x-y pairs useful for plotting the fuzzy set's membership function
        /// </summary>
        /// <returns>An array of double[2] x,y pairs</returns>
        public double[][] getPointsForPlot()
        {
            return membershipFn.getPointsForPlot();
        }
    }

    /// <summary>
    ///  All membership function objects must derive from this interface
    /// </summary>
    public interface IMembershipFunction
    {
        double getMembership(double xValue);

        double CenterOfMass();

        double RepresentativePoint(double alpha);

        double[][] getPointsForPlot();
    }

    /// <summary>
    /// A triangular membership function
    /// </summary>
    [Serializable()]
    public class triMembershipFunction : IMembershipFunction
    {
        double _a, _b, _c, centerOfMass;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="a">The 'left-most' point of the triangle: the minimum input value for which membership = 0</param>
        /// <param name="b">The point where the triangle peaks</param>
        /// <param name="c">The 'right-most' point of the triangle: the maximum input value for which membership = 0</param>
        public triMembershipFunction(double a, double b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
            centerOfMass = (a+b+c)/3;
        }

        /// <summary>
        /// Get a point's membership value in this fuzzy set
        /// </summary>
        /// <param name="xValue">The input value</param>
        /// <returns>The input value's membership in this fuzzy set</returns>
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

        /// <summary>
        /// Get this membership function's center of mass
        /// </summary>
        /// <returns>Center of mass</returns>
        public double CenterOfMass()
        {
            return centerOfMass;
        }

        /// <summary>
        ///  Get a representative point for this membership function given an alpha cut
        /// </summary>
        /// <param name="alpha">The alpha value</param>
        /// <returns>The representative point</returns>
        public double RepresentativePoint(double alpha)
        {
            // find average of points where y=alpha
            double x1 = (alpha * (_b - _a)) + _a;
            double x2 = _b - ((alpha - 1) * (_c - _b));

            return (x1+x2)/2;
        }

        /// <summary>
        /// Returns a set of x,y pairs useful for plotting this membership function
        /// </summary>
        /// <returns>An array of double[2] x,y pairs</returns>
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
