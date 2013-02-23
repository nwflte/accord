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
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.MachineLearning;
    using Accord.Math;
    using AForge.Imaging;

    /// <summary>
    ///   Bag of Visual Words
    /// </summary>
    /// 
    /// <remarks>
    ///   The bag-of-words (BoW) model can be used to extract finite
    ///   length features from otherwise varying length representations.
    ///   This class can uses any <see cref="IFeatureDetector{TPoint}">feature
    ///   detector</see> to determine a coded representation for a given image.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   int numberOfWords = 32;
    ///   
    ///   // Create bag-of-words (BoW) with the given number of words
    ///   var bow = new BagOfVisualWords&lt;SpeededUpRobustFeaturePoint>(
    ///      new SpeededUpRobustFeaturesDetector(), numberOfWords);
    ///   
    ///   // Create the BoW codebook using a set of training images
    ///   bow.Compute(imageArray);
    ///   
    ///   // Create a fixed-length feature vector for a new image
    ///   double[] featureVector = bow.GetFeatureVector(image);
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class BagOfVisualWords<TPoint> : IBagOfWords<Bitmap>, IBagOfWords<UnmanagedImage>
        where TPoint : IFeaturePoint
    {

        /// <summary>
        ///   Gets the number of words in this codebook.
        /// </summary>
        /// 
        public int NumberOfWords { get; private set; }

        /// <summary>
        ///   Gets the K-Means algorithm used to create this model.
        /// </summary>
        /// 
        public IClusteringAlgorithm<double[]> Clustering { get; private set; }

        /// <summary>
        ///   Gets the <see cref="SpeededUpRobustFeaturesDetector">SURF</see>
        ///   feature point detector used to identify visual features in images.
        /// </summary>
        /// 
        public IFeatureDetector<TPoint> Detector { get; private set; }


        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        /// <param name="detector">The feature detector to use.</param>
        /// <param name="numberOfWords">The number of codewords.</param>
        /// 
        public BagOfVisualWords(IFeatureDetector<TPoint> detector, int numberOfWords)
        {
            this.NumberOfWords = numberOfWords;
            this.Clustering = new KMeans(numberOfWords);
            this.Detector = detector;
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/>.
        /// </summary>
        /// 
        /// <param name="detector">The feature detector to use.</param>
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfVisualWords(IFeatureDetector<TPoint> detector, IClusteringAlgorithm<double[]> algorithm)
        {
            this.NumberOfWords = algorithm.Clusters.Count;
            this.Clustering = algorithm;
            this.Detector = detector;
        }

        /// <summary>
        ///   Computes the Bag of Words model.
        /// </summary>
        /// 
        /// <param name="images">The set of images to initialize the model.</param>
        /// <param name="threshold">Convergence rate for the k-means algorithm. Default is 1e-5.</param>
        /// 
        /// <returns>The list of feature points detected in all images.</returns>
        /// 
        public List<TPoint>[] Compute(Bitmap[] images, double threshold = 1e-5)
        {

            var descriptors = new List<double[]>();
            var imagePoints = new List<TPoint>[images.Length];

            // For all images
            for (int i = 0; i < images.Length; i++)
            {
                Bitmap image = images[i];

                // Compute the feature points
                var points = Detector.ProcessImage(image);

                foreach (IFeaturePoint point in points)
                    descriptors.Add(point.Descriptor);

                imagePoints[i] = points;
            }

            // Compute K-Means of the descriptors
            double[][] data = descriptors.ToArray();

            Clustering.Compute(data, threshold);

            return imagePoints;
        }

        /// <summary>
        ///   Gets the codeword representation of a given image.
        /// </summary>
        /// 
        /// <param name="value">The image to be processed.</param>
        /// 
        /// <returns>A double vector with the same length as words
        /// in the code book.</returns>
        /// 
        public double[] GetFeatureVector(Bitmap value)
        {
            // lock source image
            BitmapData imageData = value.LockBits(
                new Rectangle(0, 0, value.Width, value.Height),
                ImageLockMode.ReadOnly, value.PixelFormat);

            double[] features;

            try
            {
                // process the image
                features = GetFeatureVector(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                value.UnlockBits(imageData);
            }

            return features;
        }

        /// <summary>
        ///   Gets the codeword representation of a given image.
        /// </summary>
        /// 
        /// <param name="value">The image to be processed.</param>
        /// 
        /// <returns>A double vector with the same length as words
        /// in the code book.</returns>
        /// 
        public double[] GetFeatureVector(UnmanagedImage value)
        {
            // Detect feature points in image
            List<TPoint> points = Detector.ProcessImage(value);

            return GetFeatureVector(points);
        }

        /// <summary>
        ///   Gets the codeword representation of a given image.
        /// </summary>
        /// 
        /// <param name="points">The interest points of the image.</param>
        /// 
        /// <returns>A double vector with the same length as words
        /// in the code book.</returns>
        /// 
        public double[] GetFeatureVector(List<TPoint> points)
        {
            int[] features = new int[NumberOfWords];

            // Detect all activation centroids
            Parallel.For(0, points.Count, i =>
            {
                int j = Clustering.Clusters.Nearest(points[i].Descriptor);

                // Form feature vector
                Interlocked.Increment(ref features[j]);
            });

            return features.ToDouble();
        }

    }
}
