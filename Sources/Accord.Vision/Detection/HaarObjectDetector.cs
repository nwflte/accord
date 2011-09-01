// Accord Vision Library
// The Accord.NET Framework (LGPL) 
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
// cesarsouza at gmail.com
//
// Copyright © Masakazu Ohtsuka, 2008
//   This work is partially based on the original Project Marilena code,
//   distributed under a 2-clause BSD License. Details are listed below.
//
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
//   * Redistribution's of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//   * Redistribution's in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//
// This software is provided by the copyright holders and contributors "as is" and
// any express or implied warranties, including, but not limited to, the implied
// warranties of merchantability and fitness for a particular purpose are disclaimed.
// In no event shall the Intel Corporation or contributors be liable for any direct,
// indirect, incidental, special, exemplary, or consequential damages
// (including, but not limited to, procurement of substitute goods or services;
// loss of use, data, or profits; or business interruption) however caused
// and on any theory of liability, whether in contract, strict liability,
// or tort (including negligence or otherwise) arising in any way out of
// the use of this software, even if advised of the possibility of such damage.
//

namespace Accord.Vision.Detection
{
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.Imaging;
    using AForge.Imaging;

    /// <summary>
    ///   Object detector options for the search procedure.
    /// </summary>
    /// 
    public enum ObjectDetectorSearchMode
    {
        /// <summary>
        ///   Entire image will be scanned.
        /// </summary>
        /// 
        Default = 0,

        /// <summary>
        ///   Only a single object will be retrieved.
        /// </summary>
        /// 
        Single = 1,

        /// <summary>
        ///   If a object has already been detected inside an area,
        ///   it will not be scanned twice for inner/overlapping objects.
        /// </summary>
        /// 
        NoOverlap = 2,

        // TODO: Add mode for maximum/fixed number of faces
    }

    /// <summary>
    ///   Object detector options for window scaling.
    /// </summary>
    /// 
    public enum ObjectDetectorScalingMode
    {
        /// <summary>
        ///   Will start with a big search window and
        ///   gradually scale into smaller ones.
        /// </summary>
        /// 
        GreaterToSmaller,

        /// <summary>
        ///   Will start with small search windows and
        ///   gradually scale into greater ones.
        /// </summary>
        /// 
        SmallerToGreater,
    }

    /// <summary>
    ///   Viola-Jones Object Detector based on Haar-like features.
    /// </summary>
    /// <remarks>
    /// 
    /// <para>
    ///   The Viola-Jones object detection framework is the first object detection framework
    ///   to provide competitive object detection rates in real-time proposed in 2001 by Paul
    ///   Viola and Michael Jones. Although it can be trained to detect a variety of object
    ///   classes, it was motivated primarily by the problem of face detection.</para>
    ///   
    /// <para>
    ///   The implementation of this code has used Viola and Jones' original publication, the
    ///   OpenCV Library and the Marilena Project as reference. OpenCV is released under a BSD
    ///   license, it is free for both academic and commercial use. Please be aware that some
    ///   particular versions of the Haar object detection framework are patented by Viola and
    ///   Jones and may be subject to restrictions for use in commercial applications. The code
    ///   has been implemented with full support for tilted Haar features from the ground up.</para>
    ///   
    ///  <para>
    ///     References:
    ///     <list type="bullet">
    ///       <item><description>
    ///         <a href="http://www.cs.utexas.edu/~grauman/courses/spring2007/395T/papers/viola_cvpr2001.pdf">
    ///         Viola, P. and Jones, M. (2001). Rapid Object Detection using a Boosted Cascade
    ///         of Simple Features.</a></description></item>
    ///       <item><description>
    ///         <a href="http://en.wikipedia.org/wiki/Viola-Jones_object_detection_framework">
    ///         http://en.wikipedia.org/wiki/Viola-Jones_object_detection_framework</a>
    ///       </description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// 
    public class HaarObjectDetector : IObjectDetector
    {

        private List<Rectangle> detectedObjects;
        private HaarClassifier classifier;

        private ObjectDetectorSearchMode searchMode = ObjectDetectorSearchMode.NoOverlap;
        private ObjectDetectorScalingMode scalingMode = ObjectDetectorScalingMode.GreaterToSmaller;

        // TODO: Support ROI
        //  private Rectangle searchWindow;

        private Size minSize = new Size(15, 15);
        private Size maxSize = new Size(500, 500);
        private float factor = 1.2f;
        private int channel = RGB.R;

        private Rectangle[] lastObjects;
        private int steadyThreshold = 2;



        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>.
        /// </param>
        /// 
        public HaarObjectDetector(HaarCascade cascade)
            : this(cascade, 15)
        { }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>.
        /// </param>
        /// <param name="minSize">Minimum window size to consider when searching
        /// objects. Default value is <c>15</c>.</param>
        /// 
        public HaarObjectDetector(HaarCascade cascade, int minSize)
            : this(cascade, minSize, ObjectDetectorSearchMode.NoOverlap)
        { }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>.
        /// </param>
        /// <param name="minSize">Minimum window size to consider when searching
        /// objects. Default value is <c>15</c>.</param>
        /// <param name="searchMode">The <see cref="ObjectDetectorSearchMode"/> to use
        /// during search. Please see documentation of <see cref="ObjectDetectorSearchMode"/>
        /// for details. Default value is <see cref="ObjectDetectorSearchMode.NoOverlap"/></param>
        /// 
        public HaarObjectDetector(HaarCascade cascade, int minSize, ObjectDetectorSearchMode searchMode)
            : this(cascade, minSize, searchMode, 1.2f)
        { }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>.
        /// </param>
        /// <param name="minSize">Minimum window size to consider when searching
        /// objects. Default value is <c>15</c>.</param>
        /// <param name="searchMode">The <see cref="ObjectDetectorSearchMode"/> to use
        /// during search. Please see documentation of <see cref="ObjectDetectorSearchMode"/>
        /// for details. Default value is <see cref="ObjectDetectorSearchMode.NoOverlap"/></param>
        /// <param name="scaleFactor">The re-scaling factor to use when re-scaling the window during search.</param>
        /// 
        public HaarObjectDetector(HaarCascade cascade, int minSize, ObjectDetectorSearchMode searchMode, float scaleFactor)
            : this(cascade, minSize, searchMode, scaleFactor, ObjectDetectorScalingMode.SmallerToGreater)
        { }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="Cascades.FaceHaarCascade"/>.
        /// </param>
        /// <param name="minSize">Minimum window size to consider when searching
        /// objects. Default value is <c>15</c>.</param>
        /// <param name="searchMode">The <see cref="ObjectDetectorSearchMode"/> to use
        /// during search. Please see documentation of <see cref="ObjectDetectorSearchMode"/>
        /// for details. Default is <see cref="ObjectDetectorSearchMode.NoOverlap"/>.</param>
        /// <param name="scaleFactor">The scaling factor to rescale the window
        /// during search. Default value is <c>1.2f</c>.</param>
        /// <param name="scalingMode">The <see cref="ObjectDetectorScalingMode"/> to use
        /// when re-scaling the search window during search. Default is <see cref="ObjectDetectorScalingMode.SmallerToGreater"/>.</param>
        /// 
        public HaarObjectDetector(HaarCascade cascade, int minSize, ObjectDetectorSearchMode searchMode, float scaleFactor,
            ObjectDetectorScalingMode scalingMode)
        {
            this.classifier = new HaarClassifier(cascade);
            this.minSize = new Size(minSize, minSize);
            this.searchMode = searchMode;
            this.ScalingMode = scalingMode;
            this.factor = scaleFactor;
            this.detectedObjects = new List<Rectangle>();
        }


        /// <summary>
        ///   Minimum window size to consider when searching objects.
        /// </summary>
        /// 
        public Size MinSize
        {
            get { return minSize; }
            set { minSize = value; }
        }

        /// <summary>
        ///   Maximum window size to consider when searching objects.
        /// </summary>
        /// 
        public Size MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        /// <summary>
        ///   Gets or sets the color channel to use when processing color images. 
        /// </summary>
        /// 
        public int Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        /// <summary>
        ///   Gets or sets the scaling factor to rescale the window during search.
        /// </summary>
        /// 
        public float ScalingFactor
        {
            get { return factor; }
            set { factor = value; }
        }

        /// <summary>
        ///   Gets or sets the desired searching method.
        /// </summary>
        /// 
        public ObjectDetectorSearchMode SearchMode
        {
            get { return searchMode; }
            set { searchMode = value; }
        }

        /// <summary>
        ///   Gets or sets the desired scaling method.
        /// </summary>
        /// 
        public ObjectDetectorScalingMode ScalingMode
        {
            get { return scalingMode; }
            set { scalingMode = value; }
        }

        /// <summary>
        ///   Gets the detected objects bounding boxes.
        /// </summary>
        /// 
        public Rectangle[] DetectedObjects
        {
            get { return detectedObjects.ToArray(); }
        }

        /// <summary>
        ///   Gets the internal Cascade Classifier used by this detector.
        /// </summary>
        public HaarClassifier Classifier
        {
            get { return classifier; }
        }

        /// <summary>
        ///   Gets how many frames the object has
        ///   been detected in a steady position.
        /// </summary>
        /// <value>
        ///   The number of frames the detected object
        ///   has been in a steady position.</value>
        ///   
        public int Steady { get; private set; }


        /// <summary>
        ///   Performs object detection on the given frame.
        /// </summary>
        /// 
        public Rectangle[] ProcessFrame(Bitmap frame)
        {
            return ProcessFrame(UnmanagedImage.FromManagedImage(frame));
        }

        /// <summary>
        ///   Performs object detection on the given frame.
        /// </summary>
        /// 
        public Rectangle[] ProcessFrame(UnmanagedImage image)
        {
            // Creates an integral image representation of the frame
            IntegralImage2 integralImage = IntegralImage2.FromBitmap(
                image, channel, classifier.Cascade.HasTiltedFeatures);

            // Creates a new list of detected objects.
            this.detectedObjects.Clear();

            int width = integralImage.Width;
            int height = integralImage.Height;

            int baseWidth = classifier.Cascade.Width;
            int baseHeight = classifier.Cascade.Height;

            Rectangle window = Rectangle.Empty;

            float fstart, fstop, fstep; bool inv;

            // Set initial parameters according to scaling mode
            if (scalingMode == ObjectDetectorScalingMode.SmallerToGreater)
            {
                fstart = 1f;
                fstop = System.Math.Min(width / (float)baseWidth, height / (float)baseHeight);
                fstep = factor;
                inv = false;
            }
            else
            {
                fstart = System.Math.Min(width / (float)baseWidth, height / (float)baseHeight);
                fstop = 1f;
                fstep = 1f / factor;
                inv = true;
            }

            for (float f = fstart; (inv && f > fstop) || (!inv && f < fstop); f *= fstep)
            {
                // Set the classifier window scale
                classifier.Scale = f;

                // Get the scaled window size
                window.Width = (int)(baseWidth * f);
                window.Height = (int)(baseHeight * f);

                // Check if the window is lesser than the minimum size
                if (window.Width < minSize.Width && window.Height < minSize.Height &&
                    window.Width > maxSize.Width && window.Height > maxSize.Height)
                {
                    // If we are in inverted operation (bigger to smaller),
                    if (inv)
                    {
                        goto EXIT; // it won't get bigger, so we should stop.
                    }
                    else
                    {
                        continue; // continue until it gets greater.
                    }
                }


                // Grab some scan loop parameters
                int stepx = window.Width >> 3;
                int stepy = window.Height >> 3;

                int endx = width - window.Width;
                int endy = height - window.Height;


                // Scan the integral image searching for positives
                for (int y = 0; y < endy; y += stepy)
                {
                    window.Y = y;

                    for (int x = 0; x < endx; x += stepx)
                    {
                        window.X = x;

                        if (searchMode == ObjectDetectorSearchMode.NoOverlap && overlaps(window))
                            continue; // We have already detected something here, moving along.

                        // Try to detect and object inside the window
                        if (classifier.Compute(integralImage, window))
                        {
                            // an object has been detected
                            detectedObjects.Add(window);

                            if (searchMode == ObjectDetectorSearchMode.Single)
                                goto EXIT; // Stop on first object found
                        }
                    }
                }
            }

        EXIT:

            Rectangle[] objects = detectedObjects.ToArray();

            checkSteadiness(objects);

            lastObjects = objects;

            // Returns the array of detected objects.
            return objects;
        }

        private void checkSteadiness(Rectangle[] rectangles)
        {
            if (lastObjects == null ||
                rectangles == null ||
                rectangles.Length == 0)
            {
                Steady = 0;
                return;
            }

            bool equals = true;
            foreach (Rectangle current in rectangles)
            {
                bool found = false;
                foreach (Rectangle last in lastObjects)
                {
                    if (current.IsEqual(last, steadyThreshold))
                    {
                        found = true;
                        continue;
                    }
                }

                if (!found)
                {
                    equals = false;
                    break;
                }
            }

            if (equals)
                Steady++;

            else
                Steady = 0;
        }

        private bool overlaps(Rectangle rect)
        {
            foreach (Rectangle r in detectedObjects)
            {
                if (rect.IntersectsWith(r))
                    return true;
            }
            return false;
        }


    }

}
