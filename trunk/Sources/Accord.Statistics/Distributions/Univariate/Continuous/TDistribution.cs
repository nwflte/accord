// Accord Statistics Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Math;

    /// <summary>
    ///   Student's t-distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Student's_t-distribution">
    ///       Wikipedia, The Free Encyclopedia. Student's t-distribution. Available on:
    ///       http://en.wikipedia.org/wiki/Student's_t-distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class TDistribution : UnivariateContinuousDistribution
    {

        private double constant;


        /// <summary>
        ///   Gets the degrees of freedom for the distribution.
        /// </summary>
        /// 
        public double DegreesOfFreedom { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="TDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom.</param>
        /// 
        public TDistribution(double degreesOfFreedom)
        {
            if (degreesOfFreedom < 1)
                throw new ArgumentOutOfRangeException("degreesOfFreedom");

            this.DegreesOfFreedom = degreesOfFreedom;

            double v = degreesOfFreedom;

            // TODO: Use LogGamma instead.
            this.constant = Gamma.Function((v + 1) / 2.0) / (Math.Sqrt(v * Math.PI) * Gamma.Function(v / 2.0));
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get { return (DegreesOfFreedom > 1) ? 0 : Double.NaN; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get
            {
                if (DegreesOfFreedom > 2)
                    return DegreesOfFreedom / (DegreesOfFreedom - 2);
                else if (DegreesOfFreedom > 1)
                    return Double.PositiveInfinity;
                return Double.NaN;
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction(double x)
        {
            double v = DegreesOfFreedom;
            double sqrt = Math.Sqrt(x * x + v);
            double u = (x + sqrt) / (2 * sqrt);
            return Beta.Incomplete(v / 2.0, v / 2.0, u);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>x</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(double x)
        {
            double v = DegreesOfFreedom;
            return constant * Math.Pow(1 + (x * x) / DegreesOfFreedom, -(v + 1) / 2.0);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double LogProbabilityDensityFunction(double x)
        {
            double v = DegreesOfFreedom;
            return Math.Log(constant) - ((v + 1) / 2.0) * Math.Log(1 + (x * x) / DegreesOfFreedom);
        }

        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   this distribution evaluated at probability <c>p</c>. This function
        ///   is also known as the Quantile function.
        /// </summary>
        /// 
        /// <remarks>
        ///   The Inverse Cumulative Distribution Function (ICDF) specifies, for
        ///   a given probability, the value which the random variable will be at,
        ///   or below, with that probability.
        /// </remarks>
        /// 
        public override double InverseDistributionFunction(double p)
        {
            return inverseDistributionLeftTail(DegreesOfFreedom, p);
        }

        /// <summary>
        ///  Not supported.
        /// </summary>
        /// 
        public override void Fit(double[] observations, double[] weights, Fitting.IFittingOptions options)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            return new TDistribution(DegreesOfFreedom);
        }



        /// <summary>
        ///   Gets the inverse of the cumulative distribution function (icdf) for
        ///   the left tail T-distribution evaluated at probability <c>p</c>.
        /// </summary>
        /// 
        /// <remarks>
        ///   Based on the stdtril function from the Cephes Math Library
        ///   Release 2.8, adapted with permission of Stephen L. Moshier.
        /// </remarks>
        /// 
        private static double inverseDistributionLeftTail(double df, double p)
        {
            if (p > 0.25 && p < 0.75)
            {
                if (p == 0.5)
                    return 0;

                double u = 1.0 - 2.0 * p;
                double z = Beta.IncompleteInverse(0.5, 0.5 * df, Math.Abs(u));
                double t = Math.Sqrt(df * z / (1.0 - z));

                if (p < 0.5)
                    t = -t;

                return t;
            }
            else
            {
                int rflg = -1;

                if (p >= 0.5)
                {
                    p = 1.0 - p;
                    rflg = 1;
                }

                double z = Beta.IncompleteInverse(0.5 * df, 0.5, 2.0 * p);

                if ((Double.MaxValue * z) < df)
                    return rflg * Double.MaxValue;

                double t = Math.Sqrt(df / z - df);
                return rflg * t;
            }
        }

    }
}
