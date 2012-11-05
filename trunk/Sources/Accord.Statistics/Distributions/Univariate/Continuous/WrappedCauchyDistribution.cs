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
    using Accord.Statistics.Distributions.Fitting;
    using AForge.Math;

    /// <summary>
    ///   Wrapped Cauchy Distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   In probability theory and directional statistics, a wrapped Cauchy distribution
    ///   is a wrapped probability distribution that results from the "wrapping" of the 
    ///   Cauchy distribution around the unit circle. The Cauchy distribution is sometimes
    ///   known as a Lorentzian distribution, and the wrapped Cauchy distribution may 
    ///   sometimes be referred to as a wrapped Lorentzian distribution.</para>
    ///   
    ///  <para>
    ///   The wrapped Cauchy distribution is often found in the field of spectroscopy where
    ///   it is used to analyze diffraction patterns (e.g. see Fabry–Pérot interferometer)</para>.
    /// </remarks>
    /// 
    /// http://en.wikipedia.org/wiki/Directional_statistics
    /// 
    public class WrappedCauchyDistribution : UnivariateContinuousDistribution, 
        IFittableDistribution<double, CauchyOptions>
    {

        private double mu;
        private double gamma;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WrappedCauchyDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="mu">The mean resultant parameter μ.</param>
        /// <param name="gamma">The gamma parameter γ.</param>
        /// 
        public WrappedCauchyDistribution(double mu, double gamma)
        {
            if (gamma <= 0)
                throw new ArgumentOutOfRangeException("gamma", "Gamma must be positive.");

            this.mu = mu;
            this.gamma = gamma;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mean value.
        /// </value>
        /// 
        public override double Mean
        {
            get { return mu; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's variance.
        /// </value>
        /// 
        public override double Variance
        {
            get { return 1 - Math.Exp(-gamma); }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's entropy.
        /// </value>
        /// 
        public override double Entropy
        {
            get { return Math.Log(2 * Math.PI * (1 - Math.Exp(-2 * gamma))); }
        }

        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        public override double DistributionFunction(double x)
        {
            throw new NotSupportedException();
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
        public override double ProbabilityDensityFunction(double x)
        {
            return (1 / (2 * Math.PI)) * Math.Sinh(gamma) / (Math.Cosh(gamma) - Math.Cos(x - mu));
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
        public override double LogProbabilityDensityFunction(double x)
        {
            return Math.Log(ProbabilityDensityFunction(x));
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
            return new WrappedCauchyDistribution(mu, gamma);
        }

        /// <summary>
        ///   Fits the underlying distribution to a given set of observations.
        /// </summary>
        /// 
        /// <param name="observations">The array of observations to fit the model against. The array
        ///   elements can be either of type double (for univariate data) or
        ///   type double[] (for multivariate data).</param>
        /// <param name="weights">The weight vector containing the weight for each of the samples.</param>
        /// <param name="options">Optional arguments which may be used during fitting, such
        ///   as regularization constants and additional parameters.</param>
        ///   
        public void  Fit(double[] observations, double[] weights = null, CauchyOptions options = null)
        {
            double sin = 0, cos = 0;
            for (int i = 0; i < observations.Length; i++)
            {
                sin += Math.Sin(observations[i]);
                cos += Math.Cos(observations[i]);
            }

            mu = new Complex(sin, cos).Phase;

            double N = observations.Length;
            double R2 = (cos / N) * (cos / N) + (sin / N) * (sin / N);
            double R2e = N / (N - 1) * (R2 - 1 / N);

            gamma = Math.Log(1 / R2e) / 2;
        }

    }
}
