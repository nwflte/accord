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

    /// <summary>
    ///   Noncentral t-distribution.
    /// </summary>
    /// 
    [Serializable]
    public class NoncentralTDistribution : UnivariateContinuousDistribution
    {

        /// <summary>
        ///   Gets the degrees of freedom for the distribution.
        /// </summary>
        /// 
        public double DegreesOfFreedom { get; private set; }

        /// <summary>
        ///   Gets the noncentrality parameter for the distribution.
        /// </summary>
        /// 
        public double Noncentrality { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="TDistribution"/> class.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom.</param>
        /// <param name="noncentrality">The noncentrality parameter.</param>
        /// 
        public NoncentralTDistribution(double degreesOfFreedom, double noncentrality)
        {
            if (degreesOfFreedom <= 0)
                throw new ArgumentOutOfRangeException("degreesOfFreedom");

            this.DegreesOfFreedom = degreesOfFreedom;
            this.Noncentrality = noncentrality;
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        public override double Mean
        {
            get
            {
                // http://en.wikipedia.org/wiki/Noncentral_t-distribution#Moments_of_the_Noncentral_t-distribution

                double v = DegreesOfFreedom;
                double u = Noncentrality;
                if (v > 1)
                    return u * Math.Sqrt(v / 2.0) * (Gamma.Function((v - 1) / 2)) / Gamma.Function(v / 2);
                return Double.NaN;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        public override double Variance
        {
            get
            {
                // http://en.wikipedia.org/wiki/Noncentral_t-distribution#Moments_of_the_Noncentral_t-distribution

                double v = DegreesOfFreedom;
                double u = Noncentrality;

                if (v > 2)
                {
                    double a = (v * (1 + u*u)) / (v - 2);
                    double b = (u * u * v) / 2;
                    double c = Gamma.Function((v - 1) / 2) / Gamma.Function(v / 2);
                    return a - b * c * c;
                }

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
            return distributionFunctionLowerTail(x, DegreesOfFreedom, Noncentrality);
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
            double u = Noncentrality;
            double v = DegreesOfFreedom;
            Func<double, double, double, double> F = distributionFunctionLowerTail;

            if (x != 0)
            {
                double A = F(x * Math.Sqrt(1 + 2 / v), v + 2, u);
                double B = F(x, v, u);
                double C = v / x;
                return C * (A - B);
            }
            else
            {
                double A = Gamma.Function((v + 1) / 2);
                double B = Math.Sqrt(Math.PI * v) * Gamma.Function(v / 2);
                double C = Math.Exp(-(u * u) / 2);
                return (A / B) * C;
            }
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
            // TODO: Implement the actual log probability
            return Math.Log(ProbabilityDensityFunction(x));
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
            throw new NotImplementedException();
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
            return new NoncentralTDistribution(DegreesOfFreedom, Noncentrality);
        }

        /// <summary>
        ///   Computes the cumulative probability at <c>t</c> of the
        ///   non-central T-distribution with DF degrees of freedom 
        ///   and non-centrality parameter.
        /// </summary>
        /// 
        /// <remarks>
        ///   This function is based on the original work done by
        ///   Russell Lenth and John Burkardt, shared under the
        ///   LGPL license.
        /// </remarks>
        /// 
        private static double distributionFunctionLowerTail(double t, double df, double delta)
        {
            double a;
            double albeta;
            double alnrpi = 0.57236494292470008707;
            double b;
            double del;
            double en;
            double errbd;
            double errmax = 1.0E-10;
            double geven;
            double godd;
            int itrmax = 100;
            double lambda;
            bool negdel;
            double p;
            double q;
            double r2pi = 0.79788456080286535588;
            double rxb;
            double s;
            double tt;
            double value; ;
            double x;
            double xeven;
            double xodd;

            value = 0.0;

            if (df <= 0.0)
            {
                throw new ArgumentOutOfRangeException("df", "Degrees of freedom must be positive.");
            }


            if (t < 0.0)
            {
                tt = -t;
                del = -delta;
                negdel = true;
            }
            else
            {
                tt = t;
                del = delta;
                negdel = false;
            }

            // Initialize twin series.
            en = 1.0;
            x = t * t / (t * t + df);

            if (x <= 0.0)
            {
                // upper tail of normal cumulative function
                value = value + Normal.HighAccuracyComplemented(del);

                if (negdel)
                {
                    value = 1.0 - value;
                }
                return value;
            }

            lambda = del * del;
            p = 0.5 * Math.Exp(-0.5 * lambda);
            q = r2pi * p * del;
            s = 0.5 - p;
            a = 0.5;
            b = 0.5 * df;
            rxb = Math.Pow(1.0 - x, b);
            albeta = alnrpi + Gamma.Log(b) - Gamma.Log(a + b);
            xodd = Beta.Incomplete(a, b, x);
            godd = 2.0 * rxb * Math.Exp(a * Math.Log(x) - albeta);
            xeven = 1.0 - rxb;
            geven = b * x * rxb;
            value = p * xodd + q * xeven;

            // Repeat until convergence.
            for (; ; )
            {
                a = a + 1.0;
                xodd = xodd - godd;
                xeven = xeven - geven;
                godd = godd * x * (a + b - 1.0) / a;
                geven = geven * x * (a + b - 0.5) / (a + 0.5);
                p = p * lambda / (2.0 * en);
                q = q * lambda / (2.0 * en + 1.0);
                s = s - p;
                en = en + 1.0;
                value = value + p * xodd + q * xeven;
                errbd = 2.0 * s * (xodd - godd);

                if (errbd <= errmax)
                {
                    break;
                }

                if (itrmax < en)
                {
                    throw new ConvergenceException("Maximum number of iterations reached.");
                }
            }

            // upper tail of normal cumulative function
            value = value + Normal.HighAccuracyComplemented(del);

            if (negdel)
            {
                value = 1.0 - value;
            }

            return value;
        }


    }
}
