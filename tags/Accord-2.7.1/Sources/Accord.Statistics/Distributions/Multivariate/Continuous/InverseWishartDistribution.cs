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

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Math.Decompositions;

    /// <summary>
    ///   Inverse Wishart Distribution.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   The inverse Wishart distribution, also called the inverted Wishart distribution,
    ///   is a probability distribution defined on real-valued positive-definite matrices.
    ///   In Bayesian statistics it is used as the conjugate prior for the covariance matrix
    ///   of a multivariate normal distribution.</para>
    ///
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Inverse-Wishart_distribution">
    ///       Wikipedia, The Free Encyclopedia. Inverse Wishart distribution. 
    ///       Available from: http://en.wikipedia.org/wiki/Inverse-Wishart_distribution </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public class InverseWishartDistribution : MultivariateContinuousDistribution
    {
        int p;
        double v;
        double[,] inverseScaleMatrix;

        double constant;
        double power;
        CholeskyDecomposition chol;

        double[,] mean;
        double[] variance;
        double[,] covariance;

        /// <summary>
        ///   Creates a new Inverse Wishart distribution.
        /// </summary>
        /// 
        /// <param name="degreesOfFreedom">The degrees of freedom v.</param>
        /// <param name="inverseScale">The inverse scale matrix Ψ.</param>
        /// 
        public InverseWishartDistribution(double degreesOfFreedom, double[,] inverseScale)
            : base(inverseScale.Length)
        {

            if (inverseScale.GetLength(0) != inverseScale.GetLength(1))
                throw new DimensionMismatchException("inverseScale", "Matrix must be square.");

            this.inverseScaleMatrix = inverseScale;
            this.p = inverseScale.GetLength(0);
            this.v = degreesOfFreedom;

            if (degreesOfFreedom <= p - 1)
                throw new ArgumentOutOfRangeException("degreesOfFreedom","Degrees of freedom must be greater "
                + "than or equal to the number of rows in the inverse scale matrix.");

            this.chol = new CholeskyDecomposition(inverseScale);

            if (!chol.PositiveDefinite)
                throw new NonPositiveDefiniteMatrixException("scale");

            double a = Math.Pow(chol.Determinant, degreesOfFreedom / 2.0);
            double b = Math.Pow(2, (degreesOfFreedom * p) / 2.0);
            double c = Gamma.Function(degreesOfFreedom / 2.0, p);

            this.constant = a / (b * c);
            this.power = -(degreesOfFreedom + p + 1) / 2.0;
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
                    mean = inverseScaleMatrix.Divide(v - p - 1);
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
                    for (int i = 0; i < variance.Length; i++)
                        variance[i] = (2 * inverseScaleMatrix[i, i]) /
                            ((v - p - 1) * (v - p - 1) * (v - p - 3));
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
                        double pii = inverseScaleMatrix[i, i];

                        for (int j = 0; j < p; j++)
                        {
                            double pij = inverseScaleMatrix[i, j];
                            double pjj = inverseScaleMatrix[j, j];

                            double num = (v - p + 1) * (pij * pij) + (v - p - 1) * (pii * pjj);
                            double den = (v - p) * (v - p - 1) * (v - p - 1) * (v - p - 3);

                            covariance[i, j] = num / den;
                        }
                    }
                }

                return covariance;
            }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// 
        public override double DistributionFunction(params double[] x)
        {
            throw new NotSupportedException();
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
            CholeskyDecomposition chol = new CholeskyDecomposition(x);
            double det = chol.Determinant;
            double[,] invX = chol.Inverse();

            double z = -0.5 * Matrix.Trace(inverseScaleMatrix, invX);
            return constant * Math.Pow(det, power) * Math.Exp(z);
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
        public override double LogProbabilityDensityFunction(params double[] x)
        {
            double[,] X = x.Reshape(p, p);
            return LogProbabilityDensityFunction(X);
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
        public double LogProbabilityDensityFunction(double[,] x)
        {
            CholeskyDecomposition chol = new CholeskyDecomposition(x);
            double det = chol.Determinant;
            double[,] invX = chol.Inverse();

            double z = -0.5 * Matrix.Trace(inverseScaleMatrix, invX);
            return Math.Log(constant) + power * Math.Log(det) + z;
        }
        
        /// <summary>
        ///   Not supported.
        /// </summary>
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
            return new InverseWishartDistribution(v, inverseScaleMatrix);
        }
    }
}
