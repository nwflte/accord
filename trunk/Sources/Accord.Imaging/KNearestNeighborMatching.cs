// Accord Imaging Library
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

namespace Accord.Imaging
{
    using System;
    using System.Collections.Generic;
    using Accord.MachineLearning;
    using Accord.Math;
    using AForge;

    /// <summary>
    ///   Nearest neighbor feature point matching algorithm.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///     This class matches feature points using a <see cref="KNearestNeighbors">
    ///     k-Nearest Neighbors</see> algorithm.</para>
    /// </remarks>
    ///
    /// <seealso cref="CorrelationMatching"/>
    /// <seealso cref="RansacHomographyEstimator"/>
    /// 
    public class KNearestNeighborMatching
    {

        /// <summary>
        ///   Gets or sets the number k of nearest neighbors.
        /// </summary>
        /// 
        public int K { get; set; }

        /// <summary>
        ///   Gets or sets a minimum relevance threshold
        ///   used to find matching pairs
        /// </summary>
        /// 
        public double Threshold { get; set; }

        /// <summary>
        ///   Constructs a new Correlation Matching algorithm.
        /// </summary>
        /// 
        public KNearestNeighborMatching(int k)
        {
            this.K = k;
        }


        /// <summary>
        ///   Matches two sets of feature points.
        /// </summary>
        /// 
        public IntPoint[][] Match(IFeaturePoint[] points1, IFeaturePoint[] points2)
        {
            // Get the descriptors associated with each feature point
            double[][] features1 = new double[points1.Length][];
            for (int i = 0; i < features1.Length; i++)
                features1[i] = points1[i].Descriptor;

            double[][] features2 = new double[points2.Length][];
            for (int i = 0; i < features2.Length; i++)
                features2[i] = points2[i].Descriptor;

            // Create a k-Nearest Neighbor classifier to classify points
            // in the second image to nearest points in the first image
            KNearestNeighbors knn = new KNearestNeighbors(K, points1.Length,
                features1, Matrix.Indices(0, points1.Length));


            double[] scores = new double[points2.Length];
            int[] classes = new int[points2.Length];
            for (int i = 0; i < points2.Length; i++)
                classes[i] = knn.Compute(points2[i].Descriptor, out scores[i]);

            int[] bestMatch = new int[points1.Length];
            double[] bestScore = new double[points1.Length];
            for (int i = 0; i < bestScore.Length; i++)
                bestScore[i] = Double.PositiveInfinity;

            for (int i = 0; i < points1.Length; i++)
            {
                // Get all points matching with this point    
                for (int j = 0; j < points2.Length; j++)
                {
                    if (classes[j] == i && scores[j] > Threshold)
                    {
                        if (scores[j] < bestScore[i])
                        {
                            bestScore[i] = scores[j];
                            bestMatch[i] = j;
                        }
                    }
                }
            }

            List<IntPoint> p1 = new List<IntPoint>();
            List<IntPoint> p2 = new List<IntPoint>();

            // Get the two sets of points
            for (int i = 0; i < points1.Length; i++)
            {
                if (bestScore[i] != Double.PositiveInfinity)
                {
                    IFeaturePoint pi = points1[i];
                    IFeaturePoint pj = points2[bestMatch[i]];
                    p1.Add(new IntPoint((int)pi.X, (int)pi.Y));
                    p2.Add(new IntPoint((int)pj.X, (int)pj.Y));
                }
            }

            IntPoint[] m1 = p1.ToArray();
            IntPoint[] m2 = p2.ToArray();

            // Create matching point pairs
            return new IntPoint[][] { m1, m2 };
        }

    }
}
