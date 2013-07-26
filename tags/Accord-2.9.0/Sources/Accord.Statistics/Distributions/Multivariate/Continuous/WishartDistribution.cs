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

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Wishart Distribution.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Wishart distribution is a generalization to multiple dimensions of 
    ///   the chi-squared distribution, or, in the case of non-integer degrees of
    ///   freedom, of the gamma distribution.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Wishart_distribution">
    ///       Wikipedia, The Free Encyclopedia. Wishart distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Wishart_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class WishartDistribution : MultivariateContinuousDistribution
    {

        int p;
        double n;
        double[,] scaleMatrix;

        double constant;
        double power;
        CholeskyDecomposition chol;

        double[,] mean;
        double[] variance;
        double[,] covariance;

        /// <summary>
        ///   Creates a new Wishart distribution.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom n.</param>
        /// <param name="scale">The positive-definite matrix scale matrix V.</param>
        /// 
        public WishartDistribution(double degreesOfFreedom, double[,] scale)
            : base(scale.Length)
        {
            if (scale.GetLength(0) != scale.GetLength(1))
                throw new DimensionMismatchException("scale", "Matrix must be square.");

            this.scaleMatrix = scale;
            this.n = degreesOfFreedom;
            this.p = scale.GetLength(0);

            if (degreesOfFreedom <= p - 1)
                throw new ArgumentOutOfRangeException("degreesOfFreedom","Degrees of freedom must be greater "
                + "than or equal to the number of rows in the scale matrix.");

            this.chol = new CholeskyDecomposition(scale);

            if (!chol.PositiveDefinite)
                throw new NonPositiveDefiniteMatrixException("scale");

            double a = Math.Pow(chol.Determinant, degreesOfFreedom / 2.0);
            double b = Math.Pow(2, (degreesOfFreedom * p) / 2.0);
            double c = Gamma.Function(degreesOfFreedom / 2.0, p);

            this.constant = 1.0 / (a * b * c);
            this.power = (degreesOfFreedom - p - 1) / 2.0;
        }


        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>A vector containing the mean values for the distribution.</value>
        /// 
        public double[,] MeanMatrix
        {
            get
            {
                if (mean == null)
                    mean = n.Multiply(scaleMatrix);
                return mean;
            }
        }



        /// <summary>
        ///   Gets the mean for this distribution as a flat matrix.
        /// </summary>
        /// 
        /// <value>A vector containing the mean values for the distribution.</value>
        /// 
        public override double[] Mean
        {
            get { return MeanMatrix.Reshape(0); }
        }


        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>A vector containing the variance values for the distribution.</value>
        /// 
        public override double[] Variance
        {
            get
            {
                if (variance == null)
                {
                    variance = new double[p];
                    for (int i = 0; i < p; i++)
                    {
                        double vii = scaleMatrix[i, i];
                        variance[i] = 2 * n * (vii * vii);
                    }
                }
                return variance;
            }
        }


        /// <summary>
        ///   Gets the variance-covariance matrix for this distribution.
        /// </summary>
        /// 
        /// <value>A matrix containing the covariance values for the distribution.</value>
        /// 
        public override double[,] Covariance
        {
            get
            {
                if (covariance == null)
                {
                    covariance = new double[p, p];
                    for (int i = 0; i < p; i++)
                    {
                        double vii = scaleMatrix[i, i];

                        for (int j = 0; j < p; j++)
                        {
                            double vij = scaleMatrix[i, j];
                            double vjj = scaleMatrix[j, j];

                            covariance[i, j] = n * (vij * vij + vii * vjj);
                        }
                    }
                }

                return covariance;
            }
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.
        ///   For a matrix distribution, such as the Wishart's, this
        ///   should be a positive-definite matrix or a matrix written
        ///   in flat vector form.
        /// </param>
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
        public override double ProbabilityDensityFunction(params double[] x)
        {
            double[,] X = x.Reshape(p, p);
            return ProbabilityDensityFunction(X);
        }

        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.
        ///   For a matrix distribution, such as the Wishart's, this
        ///   should be a positive-definite matrix or a matrix written
        ///   in flat vector form.
        /// </param>
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
        public double ProbabilityDensityFunction(double[,] x)
        {
            double det = x.Determinant();
            double z = 0.5 * chol.Solve(x).Trace();
            return constant * Math.Pow(det, power) * Math.Exp(z);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf)
        ///   for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.
        ///   For a matrix distribution, such as the Wishart's, this
        ///   should be a positive-definite matrix or a matrix written
        ///   in flat vector form.
        /// </param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        public override double LogProbabilityDensityFunction(params double[] x)
        {
            double[,] X = x.Reshape(p, p);
            return LogProbabilityDensityFunction(X);
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf)
        ///   for this distribution evaluated at point <c>x</c>.
        /// </summary>
        /// 
        /// <param name="x">A single point in the distribution range.
        ///   For a matrix distribution, such as the Wishart's, this
        ///   should be a positive-definite matrix or a matrix written
        ///   in flat vector form.
        /// </param>
        ///   
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        public double LogProbabilityDensityFunction(double[,] x)
        {
            double det = x.Determinant();
            double z = 0.5 * chol.Solve(x).Trace();
            return Math.Log(constant) + power * Math.Log(det) + z;
        }

        /// <summary>
        ///   Unsupported.
        /// </summary>
        /// 
        public override double DistributionFunction(params double[] x)
        {
            throw new NotSupportedException();
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
        public override void Fit(double[][] observations, double[] weights, Fitting.IFittingOptions options)
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
            return new WishartDistribution(n, scaleMatrix);
        }
    }
}
