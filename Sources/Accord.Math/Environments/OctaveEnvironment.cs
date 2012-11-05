// Accord Math Library
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

namespace Accord.Math.Environments
{
    using Accord.Math;
    using Accord.Math.Decompositions;
    using System.CodeDom.Compiler;

    /// <summary>
    ///   Programming environment for Octave.
    /// </summary>
    /// 
    [GeneratedCode("","")]
    public abstract class OctaveEnvironment
    {
        /// <summary>
        ///   Whether to use octave indexing or not.
        /// </summary>
        /// 
        protected static bool UseOctaveDimensionIndexing
        {
            get { return dimensionOffset == 1; }
            set { dimensionOffset = (value) ? 1 : 0; }
        }

        private static int dimensionOffset;


        /// <summary>Pi.</summary>
        protected static double pi = System.Math.PI;

        /// <summary>Machine epsilon.</summary>
        protected static double eps = Constants.DoubleEpsilon;

        // octave language commands
        /// <summary>Creates an identity matrix.</summary>
        protected static double[,] eye(int size) { return Matrix.Identity(size); }
        /// <summary>Inverts a matrix.</summary>
        protected static double[,] inv(double[,] matrix) { return Matrix.Inverse(matrix); }
        /// <summary>Inverts a matrix.</summary>
        protected static double[,] pinv(double[,] matrix) { return Matrix.PseudoInverse(matrix); }
        /// <summary>Creates a unit matrix.</summary>
        protected static double[,] ones(int size) { return Matrix.Create(size, 1.0); }
        /// <summary>Creates a zero matrix.</summary>
        protected static double[,] zeros(int size) { return Matrix.Create(size, 0.0); }
        /// <summary>Random vector.</summary>
        protected static double[,] rand(int n, int m) { return Matrix.Random(n, m, 0, 1); }
        /// <summary>Size of a matrix.</summary>
        protected static double[] size(double[,] m) { return new double[] { m.GetLength(0), m.GetLength(1) }; }
        /// <summary>Rank of a matrix.</summary>
        protected static int rank(double[,] m) { return new SingularValueDecomposition(m).Rank; }


        /// <summary>Matrix sum vector.</summary>
        protected static double[] sum(double[,] matrix) { return Matrix.Sum(matrix); }
        /// <summary>Sum of vector elements.</summary>
        protected static double sum(double[] vector) { return Matrix.Sum(vector); }
        /// <summary>Product of vector elements.</summary>
        protected static double prod(double[] vector) { return Matrix.Product(vector); }
        /// <summary>Matrix sum vector.</summary>
        protected static double[] sum(double[,] matrix, int dimension) { return Matrix.Sum(matrix, dimension - dimensionOffset); }


        /// <summary>Rounding.</summary>
        protected static double round(double f) { return System.Math.Round(f); }
        /// <summary>Ceiling.</summary>
        protected static double ceil(double f) { return System.Math.Ceiling(f); }
        /// <summary>Flooring.</summary>
        protected static double floor(double f) { return System.Math.Floor(f); }

        /// <summary>Rounding.</summary>
        protected static double[] round(double[] f) { return Matrix.Round(f, 0); }
        /// <summary>Ceiling.</summary>
        protected static double[] ceil(double[] f) { return Matrix.Ceiling(f); }
        /// <summary>Flooring.</summary>
        protected static double[] floor(double[] f) { return Matrix.Floor(f); }

        /// <summary>Rounding.</summary>
        protected static double[,] round(double[,] f) { return Matrix.Round(f, 0); }
        /// <summary>Ceiling.</summary>
        protected static double[,] ceil(double[,] f) { return Matrix.Ceiling(f); }
        /// <summary>Flooring.</summary>
        protected static double[,] floor(double[,] f) { return Matrix.Floor(f); }


        /// <summary>Sin.</summary>
        protected static double sin(double d) { return System.Math.Sin(d); }
        /// <summary>Cos.</summary>
        protected static double cos(double d) { return System.Math.Cos(d); }
        /// <summary>Expential value.</summary>
        protected static double exp(double d) { return System.Math.Exp(d); }
        /// <summary>Absolute value.</summary>
        protected static double abs(double d) { return System.Math.Abs(d); }
        /// <summary>Logarithm.</summary>
        protected static double log(double d) { return System.Math.Log(d); }

        /// <summary>Sin.</summary>
        protected static double[] sin(double[] d) { return Matrix.Apply(d, x => System.Math.Sin(x)); }
        /// <summary>Cos.</summary>
        protected static double[] cos(double[] d) { return Matrix.Apply(d, x => System.Math.Cos(x)); }
        /// <summary>Expential value.</summary>
        protected static double[] exp(double[] d) { return Matrix.Apply(d, x => System.Math.Exp(x)); }
        /// <summary>Absolute value.</summary>
        protected static double[] abs(double[] d) { return Matrix.Apply(d, x => System.Math.Abs(x)); }
        /// <summary>Logarithm.</summary>
        protected static double[] log(double[] d) { return Matrix.Apply(d, x => System.Math.Log(x)); }

        /// <summary>Sin.</summary>
        protected static double[,] sin(double[,] d) { return Matrix.Apply(d, x => System.Math.Sin(x)); }
        /// <summary>Cos.</summary>
        protected static double[,] cos(double[,] d) { return Matrix.Apply(d, x => System.Math.Cos(x)); }
        /// <summary>Expential value.</summary>
        protected static double[,] exp(double[,] d) { return Matrix.Apply(d, x => System.Math.Exp(x)); }
        /// <summary>Absolute value.</summary>
        protected static double[,] abs(double[,] d) { return Matrix.Apply(d, x => System.Math.Abs(x)); }
        /// <summary>Logarithm.</summary>
        protected static double[,] log(double[,] d) { return Matrix.Apply(d, x => System.Math.Log(x)); }



        // decompositions
        #region svd
        /// <summary>Singular value decomposition.</summary>
        protected double[] svd(double[,] m)
        {
            var svd = new SingularValueDecomposition(m, false, false, true);
            return svd.Diagonal;
        }

        /// <summary>Singular value decomposition.</summary>
        protected static void svd(double[,] m, out double[,] U)
        {
            var svd = new SingularValueDecomposition(m, true, false, true);
            U = svd.LeftSingularVectors;
        }

        /// <summary>Singular value decomposition.</summary>
        protected static void svd(double[,] m, out double[,] U, out double[] S)
        {
            var svd = new SingularValueDecomposition(m, true, false, true);
            U = svd.LeftSingularVectors;
            S = svd.Diagonal;
        }

        /// <summary>Singular value decomposition.</summary>
        protected static void svd(double[,] m, out double[,] U, out double[] S, out double[,] V)
        {
            var svd = new SingularValueDecomposition(m, true, true, true);
            U = svd.LeftSingularVectors;
            S = svd.Diagonal;
            V = svd.RightSingularVectors;
        }
        #endregion

        #region qr
        /// <summary>QR decomposition.</summary>
        protected static void qr(double[,] m, out double[,] Q, out double[,] R)
        {
            var qr = new QrDecomposition(m);
            Q = qr.OrthogonalFactor;
            R = qr.UpperTriangularFactor;
        }

        /// <summary>QR decomposition.</summary>
        protected static void qr(double[,] m, out double[,] Q, out double[,] R, out double[] d)
        {
            var qr = new QrDecomposition(m);
            Q = qr.OrthogonalFactor;
            R = qr.UpperTriangularFactor;
            d = qr.Diagonal;
        }
        #endregion

        #region eig
        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, out double[,] V)
        {
            var eig = new EigenvalueDecomposition(a);
            V = eig.Eigenvectors;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, out double[,] V, out double[] im)
        {
            var eig = new EigenvalueDecomposition(a);
            V = eig.Eigenvectors;
            im = eig.ImaginaryEigenvalues;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, double[,] b, out double[,] V)
        {
            var eig = new GeneralizedEigenvalueDecomposition(a, b);
            V = eig.Eigenvectors;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, double[,] b, out double[,] V, out double[] im)
        {
            var eig = new GeneralizedEigenvalueDecomposition(a, b);
            V = eig.Eigenvectors;
            im = eig.ImaginaryEigenvalues;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, double[,] b, out double[,] V, out double[] alphar, out double[] beta)
        {
            var eig = new GeneralizedEigenvalueDecomposition(a, b);
            V = eig.Eigenvectors;
            beta = eig.Betas;
            alphar = eig.RealAlphas;
            return eig.RealEigenvalues;
        }

        /// <summary>Eigenvalue decomposition.</summary>
        protected static double[] eig(double[,] a, double[,] b, out double[,] V, out double[] im, out double[] alphar, out double[] alphai, out double[] beta)
        {
            var eig = new GeneralizedEigenvalueDecomposition(a, b);
            V = eig.Eigenvectors;
            im = eig.ImaginaryEigenvalues;
            beta = eig.Betas;
            alphar = eig.RealAlphas;
            alphai = eig.ImaginaryAlphas;
            return eig.RealEigenvalues;
        }
        #endregion

        #region chol
        /// <summary>Cholesky decomposition.</summary>
        protected static double[,] chol(double[,] a)
        {
            var chol = new CholeskyDecomposition(a);
            return chol.LeftTriangularFactor;
        }
        #endregion

    }
}
