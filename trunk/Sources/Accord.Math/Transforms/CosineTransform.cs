// Accord Math Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
// cesarsouza at gmail.com
//
// Copyright © Diego Catalano, 2013
// diego.catalano at live.com
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

namespace Accord.Math
{
    using System;
    using AForge.Math;

    /// <summary>
    ///   Discrete Cosine Transformation.
    /// </summary>
    /// 
    public static class CosineTransform
    {

        /// <summary>
        ///   Forward Discrete Cosine Transform.
        /// </summary>
        /// 
        public static void DCT(double[] data)
        {
            double[] result = new double[data.Length];
            double c = Math.PI / (2.0 * data.Length);
            double scale = Math.Sqrt(2.0 / data.Length);

            for (int k = 0; k < data.Length; k++)
            {
                double sum = 0;
                for (int n = 0; n < data.Length; n++)
                    sum += data[n] * Math.Cos((2.0 * n + 1.0) * k * c);
                result[k] = scale * sum;
            }

            data[0] = result[0] / Constants.Sqrt2;
            for (int i = 1; i < data.Length; i++)
                data[i] = result[i];
        }

        /// <summary>
        ///   Inverse Discrete Cosine Transform.
        /// </summary>
        /// 
        public static void IDCT(double[] data)
        {
            double[] result = new double[data.Length];
            double c = Math.PI / (2.0 * data.Length);
            double scale = Math.Sqrt(2.0 / data.Length);

            for (int k = 0; k < data.Length; k++)
            {
                double sum = data[0] / Constants.Sqrt2;
                for (int n = 1; n < data.Length; n++)
                    sum += data[n] * Math.Cos((2 * k + 1) * n * c);

                result[k] = scale * sum;
            }

            for (int i = 0; i < data.Length; i++)
                data[i] = result[i];
        }


        /// <summary>
        ///   Forward 2D Discrete Cosine Transform.
        /// </summary>
        /// 
        public static void DCT(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            double[] row = new double[cols];
            double[] col = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < row.Length; j++)
                    row[j] = data[i, j];

                DCT(row);

                for (int j = 0; j < row.Length; j++)
                    data[i, j] = row[j];
            }

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < col.Length; i++)
                    col[i] = data[i, j];

                DCT(col);

                for (int i = 0; i < col.Length; i++)
                    data[i, j] = col[i];
            }
        }

        /// <summary>
        ///   Inverse 2D Discrete Cosine Transform.
        /// </summary>
        /// 
        public static void IDCT(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            double[] row = new double[cols];
            double[] col = new double[rows];

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < row.Length; i++)
                    col[i] = data[i, j];

                IDCT(col);

                for (int i = 0; i < col.Length; i++)
                    data[i, j] = col[i];
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < row.Length; j++)
                    row[j] = data[i, j];

                IDCT(row);

                for (int j = 0; j < row.Length; j++)
                    data[i, j] = row[j];
            }
        }
    }
}

