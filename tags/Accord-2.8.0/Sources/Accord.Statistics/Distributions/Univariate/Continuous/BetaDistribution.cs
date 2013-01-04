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
    ///   Beta Distribution (of the first kind).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The beta distribution is a family of continuous probability distributions
    ///   defined on the interval (0, 1) parameterized by two positive shape parameters,
    ///   typically denoted by α and β. The beta distribution can be suited to the 
    ///   statistical modelling of proportions in applications where values of proportions
    ///   equal to 0 or 1 do not occur. One theoretical case where the beta distribution 
    ///   arises is as the distribution of the ratio formed by one random variable having
    ///   a Gamma distribution divided by the sum of it and another independent random 
    ///   variable also having a Gamma distribution with the same scale parameter (but 
    ///   possibly different shape parameter).</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Beta_distribution">
    ///       Wikipedia, The Free Encyclopedia. Beta distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Beta_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    ///
    [Serializable]
    public class BetaDistribution : UnivariateContinuousDistribution
    {
        double a;
        double b;


        double constant;

        /// <summary>
        ///   Creates a new Beta distribution.
        /// </summary>
        /// 
        /// <param name="alpha">The shape parameter α (alpha).</param>
        /// <param name="beta">The shape parameter β (beta).</param>
        /// 
        public BetaDistribution(double alpha, double beta)
        {
            if (alpha <= 0) throw new ArgumentOutOfRangeException("alpha");
            if (beta <= 0) throw new ArgumentOutOfRangeException("beta");

            this.a = alpha;
            this.b = beta;

            constant = 1.0 / Beta.Function(a, b);
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return a / (a + b); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return (a * b) / ((a + b) * (a + b) * (a + b + 1)); }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
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
            return Beta.Incomplete(a, b, x);
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
            if (x <= 0 || x >= 1) return 0;
            return  constant* Math.Pow(x, a - 1) * Math.Pow(1 - x, b - 1);
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
            if (x <= 0 || x >= 1) return Double.NegativeInfinity;
            return Math.Log(constant) + (a - 1) * Math.Log(x) + (b - 1) * Math.Log(1 - x);
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
            return new BetaDistribution(a, b);
        }
    }
}
