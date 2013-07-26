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
    ///   Gamma distribution.
    /// </summary>
    /// 
    [Serializable]
    public class GammaDistribution : UnivariateContinuousDistribution,
        IFittableDistribution<double, IFittingOptions>,
        ISampleableDistribution<double>
    {

        // Distribution parameters
        private double scale;
        private double shape;

        // Derived measures
        private double constant;
        private double lnconstant;


        /// <summary>
        ///   Constructs a Gamma distribution.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter theta.</param>
        /// <param name="shape">The shape parameter k.</param>
        /// 
        public GammaDistribution(double scale, double shape)
        {
            init(scale, shape);
        }

        private void init(double scale, double shape)
        {
            this.scale = scale;
            this.shape = shape;

            this.constant = 1.0 / (Math.Pow(scale, shape) * Gamma.Function(shape));
            this.lnconstant = -(shape * Math.Log(scale) + Gamma.Log(shape));
        }

        /// <summary>
        ///   Gets the distribution's Scale
        ///   parameter theta.
        /// </summary>
        /// 
        public double Scale
        {
            get { return scale; }
        }

        /// <summary>
        ///   Gets the distribution's Shape
        ///   parameter k.
        /// </summary>
        /// 
        public double Shape
        {
            get { return shape; }
        }

        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get { return shape * scale; }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get { return shape * scale * scale; }
        }

        /// <summary>
        ///   Gets the median for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's median value.</value>
        /// 
        public override double Median
        {
            get { return double.NaN; }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { return shape + Math.Log(scale) + Gamma.Log(shape) + (1 - shape) * Gamma.Digamma(shape); }
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
            return Gamma.LowerIncomplete(shape, x / scale);
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
            return constant * Math.Pow(x, shape - 1) * Math.Exp(-x / scale);
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
            return lnconstant + (shape - 1) * Math.Log(x) - x / scale;
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
            if (options != null)
                throw new ArgumentException("This method does not accept fitting options.");

            if (weights != null)
                throw new ArgumentException("This distribution does not support weighted samples.");

            double lnsum = 0;
            for (int i = 0; i < observations.Length; i++)
                lnsum += Math.Log(observations[i]);

            double mean = observations.Mean();

            double s = Math.Log(mean) - lnsum / observations.Length;

            // initial approximation
            double newK = (3 - s + Math.Sqrt((s - 3) * (s - 3) + 24 * s)) / (12 * s);

            // Use Newton-Raphson approximation
            double oldK;

            do
            {
                oldK = newK;
                newK = oldK - (Math.Log(newK) - Gamma.Digamma(newK) - s) / ((1 / newK) - Gamma.Trigamma(newK));
            }
            while (Math.Abs(oldK - newK) / Math.Abs(oldK) < Double.Epsilon);


            double theta = mean / newK;

            init(theta, newK);
        }

        private GammaDistribution() { }

        /// <summary>
        ///   Estimates a new Gamma distribution from a given set of observations.
        /// </summary>
        /// 
        public static GammaDistribution Estimate(double[] observations)
        {
            return Estimate(observations, null);
        }

        /// <summary>
        ///   Estimates a new Gamma distribution from a given set of observations.
        /// </summary>
        /// 
        public static GammaDistribution Estimate(double[] observations, double[] weights)
        {
            var n = new GammaDistribution();
            n.Fit(observations, weights, null);
            return n;
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
            return new GammaDistribution(scale, shape);
        }


        #region ISamplableDistribution<double> Members

        /// <summary>
        ///   Generates a random vector of observations from the current distribution.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
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
        ///   Gamma distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter theta.</param>
        /// <param name="shape">The shape parameter k.</param>
        /// <param name="samples">The number of samples to generate.</param>
        ///
        /// <returns>An array of double values sampled from the specified Gamma distribution.</returns>
        /// 
        public static double[] Random(double shape, double scale, int samples)
        {
            double[] r = new double[samples];

            if (shape < 1)
            {
                double d = shape + 1.0 - 1.0 / 3.0;
                double c = 1.0 / Math.Sqrt(9 * d);

                for (int i = 0; i < r.Length; i++)
                {
                    double U = Accord.Math.Tools.Random.Next();
                    r[i] = scale * rgama(d, c) * Math.Pow(U, 1.0 / shape);
                }

            }
            else
            {
                double d = shape - 1.0 / 3.0;
                double c = 1.0 / Math.Sqrt(9 * d);

                for (int i = 0; i < r.Length; i++)
                {
                    r[i] = scale * rgama(d, c);
                }
            }

            return r;
        }

        /// <summary>
        ///   Generates a random observation from the 
        ///   Gamma distribution with the given parameters.
        /// </summary>
        /// 
        /// <param name="scale">The scale parameter theta.</param>
        /// <param name="shape">The shape parameter k.</param>
        /// 
        /// <returns>A random double value sampled from the specified Gamma distribution.</returns>
        /// 
        public static double Random(double shape, double scale)
        {
            if (shape < 1)
            {
                double d = shape + 1.0 - 1.0 / 3.0;
                double c = 1.0 / Math.Sqrt(9 * d);

                double U = Accord.Math.Tools.Random.Next();
                return scale * rgama(d, c) * Math.Pow(U, 1.0 / shape);
            }
            else
            {
                double d = shape - 1.0 / 3.0;
                double c = 1.0 / Math.Sqrt(9 * d);

                return scale * rgama(d, c);
            }
        }



        /// <summary>
        ///   Marsaglia's Simple Method
        /// </summary>
        /// 
        private static double rgama(double d, double c)
        {
            // References:
            //
            // - Marsaglia, G. A Simple Method for Generating Gamma Variables, 2000
            //

            while (true)
            {
                // 2. Generate v = (1+cx)^3 with x normal
                double x, t, v;

                do
                {
                    x = NormalDistribution.Standard.Generate();
                    t = (1.0 + c * x);
                    v = t * t * t;
                } while (v <= 0);


                // 3. Generate uniform U
                double U = Accord.Math.Tools.Random.NextDouble();

                // 4. If U < 1-0.0331*x^4 return d*v.
                double x2 = x * x;
                if (U < 1 - 0.0331 * x2 * x2)
                    return d * v;

                // 5. If log(U) < 0.5*x^2 + d*(1-v+log(v)) return d*v.
                if (Math.Log(U) < 0.5 * x2 + d * (1.0 - v + Math.Log(v)))
                    return d * v;

                // 6. Goto step 2
            }
        }

        #endregion
    }
}
