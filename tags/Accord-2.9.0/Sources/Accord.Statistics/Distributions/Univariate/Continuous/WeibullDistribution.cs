// Accord Statistics Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Weibull distribution.
    /// </summary>
    /// 
    [Serializable]
    public class WeibullDistribution : UnivariateContinuousDistribution,
        ISampleableDistribution<double>
    {

        // Distribution parameters
        private double shape;
        private double scale;
        private double location;


        /// <summary>
        ///   Initializes a new instance of the <see cref="WeibullDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter lambda.</param>
        /// <param name="shape">The shape parameter k.</param>
        /// 
        public WeibullDistribution(double shape, double scale)
        {
            this.shape = shape;
            this.scale = scale;
            this.location = 0;
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return scale * Gamma.Function(1 + 1 / shape); }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return scale * scale * Gamma.Function(1 + 2 / shape) - Mean * Mean; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's median value.
        /// </value>
        /// 
        public override double Median
        {
            get { return Math.Pow(Math.Log(2), 1 / shape); }
        }

        /// <summary>
        ///   Gets the mode for this distribution.
        /// </summary>
        /// 
        /// <value>
        ///   The distribution's mode value.
        /// </value>
        /// 
        public override double Mode
        {
            get { return shape > 1 ? Math.Pow(1 - 1 / shape, 1 / shape) : 0; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { return Constants.EulerGamma * (1 - 1 / shape) + Math.Log(scale / shape) + 1; }
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
            if (x > 0)
                return 1.0 - Math.Exp(-Math.Pow((x - location) / scale, shape));
            if (x == 0)
                return Double.PositiveInfinity;
            else return 0;
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
            if (x > 0)
                return (shape / scale) * Math.Pow((x - location) / scale, shape - 1) * Math.Exp(-Math.Pow((x - location) / scale, shape));
            else return 0;
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
            if (x >= 0)
                return Math.Log(shape / scale) + (shape - 1) * Math.Log((x - location) / scale) - Math.Pow((x - location) / scale, shape);
            else return Double.NegativeInfinity;
        }

        /// <summary>
        ///   Gets the hazard function, also known as the failure rate or
        ///   the conditional failure density function for this distribution
        ///   evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The conditional failure density function <c>h(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.
        /// </returns>
        /// 
        public override double HazardFunction(double x)
        {
            return Math.Pow(shape * (x - location), shape - 1);
        }

        /// <summary>
        ///   Gets the cumulative hazard function for this
        ///   distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The cumulative hazard function <c>H(x)</c>
        ///   evaluated at <c>x</c> in the current distribution.
        /// </returns>
        /// 
        public override double CumulativeHazardFunction(double x)
        {
            return Math.Pow((x - location), shape);
        }

        /// <summary>
        ///   Gets the complementary cumulative distribution function
        ///   (ccdf) for this distribution evaluated at point <c>x</c>.
        ///   This function is also known as the Survival function.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.</param>
        /// 
        public override double ComplementaryDistributionFunction(double x)
        {
            return Math.Exp(-Math.Pow((x - location), shape));
        }

        /// <summary>
        ///   Gets the inverse of the <see cref="ComplementaryDistributionFunction"/>. 
        ///   The inverse complementary distribution function is also known as the 
        ///   inverse survival Function.
        /// </summary>
        /// 
        public double InverseComplementaryDistributionFunction(double p)
        {
            return Math.Pow(-Math.Log(p), 1 / shape);
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
        /// <remarks>
        ///   Although both double[] and double[][] arrays are supported,
        ///   providing a double[] for a multivariate distribution or a
        ///   double[][] for a univariate distribution may have a negative
        ///   impact in performance.
        /// </remarks>
        /// 
        public override void Fit(double[] observations, double[] weights, IFittingOptions options)
        {
            throw new NotImplementedException();
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
            return new WeibullDistribution(scale, shape);
        }


        #region ISampleableDistribution<double> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <returns>A random vector of observations drawn from this distribution.</returns>
        /// 
        public double[] Generate(int samples)
        {
            return Random(shape, scale, samples);
        }

        /// <summary>
        ///   Generates a random observation from the current distribution.
        /// </summary>
        /// 
        /// <returns>A random observations drawn from this distribution.</returns>
        /// 
        public double Generate()
        {
            return Random(shape, scale);
        }

        /// <summary>
        ///   Generates a random vector of observations from the 
        ///   Weibull distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter lambda.</param>
        /// <param name="shape">The shape parameter k.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Weibull distribution.</returns>
        /// 
        public static double[] Random(double shape, double scale, int samples)
        {
            double[] r = new double[samples];
            for (int i = 0; i < r.Length; i++)
            {
                double u = Accord.Math.Tools.Random.NextDouble();
                r[i] = scale * Math.Pow(-Math.Log(u), 1 / shape);
            }

            return r;
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Weibull distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter lambda.</param>
        /// <param name="shape">The shape parameter k.</param>
        /// 
        /// <returns>A random double value sampled from the specified Weibull distribution.</returns>
        /// 
        public static double Random(double shape, double scale)
        {
            double u = Accord.Math.Tools.Random.NextDouble();
            return scale * Math.Pow(-Math.Log(u), 1 / shape);
        }
        #endregion

    }
}
