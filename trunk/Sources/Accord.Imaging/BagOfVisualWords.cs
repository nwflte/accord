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
    using Accord.MachineLearning;

    /// <summary>
    ///   Bag of Visual Words
    /// </summary>
    /// 
    /// <remarks>
    ///   The bag-of-words (BoW) model can be used to extract finite
    ///   length features from otherwise varying length representations.
    ///   This class uses the <see cref="SpeededUpRobustFeaturesDetector">
    ///   SURF features detector</see> to determine a coded representation
    ///   for a given image.
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    ///   int numberOfWords = 32;
    ///   
    ///   // Create bag-of-words (BoW) with the given number of words
    ///   BagOfVisualWords bow = new BagOfVisualWords(numberOfWords);
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
    public class BagOfVisualWords : BagOfVisualWords<SpeededUpRobustFeaturePoint>
    {
        /// <summary>
        ///   Gets the <see cref="SpeededUpRobustFeaturesDetector">SURF</see>
        ///   feature point detector used to identify visual features in images.
        /// </summary>
        /// 
        public new SpeededUpRobustFeaturesDetector Detector
        {
            get { return base.Detector as SpeededUpRobustFeaturesDetector; }
        }

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/> using a
        ///   <see cref="SpeededUpRobustFeaturesDetector">surf</see>
        ///   feature detector to identify features.
        /// </summary>
        /// 
        /// <param name="numberOfWords">The number of codewords.</param>
        /// 
        public BagOfVisualWords(int numberOfWords)
            : base(new SpeededUpRobustFeaturesDetector(), numberOfWords) { }

        /// <summary>
        ///   Constructs a new <see cref="BagOfVisualWords"/> using a
        ///   <see cref="SpeededUpRobustFeaturesDetector">surf</see>
        ///   feature detector to identify features.
        /// </summary>
        /// 
        /// <param name="algorithm">The clustering algorithm to use.</param>
        /// 
        public BagOfVisualWords(IClusteringAlgorithm<double[]> algorithm)
            : base(new SpeededUpRobustFeaturesDetector(), algorithm) { }
    }

}
