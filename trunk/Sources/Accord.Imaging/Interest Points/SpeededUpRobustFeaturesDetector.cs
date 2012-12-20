// Accord Imaging Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © Christopher Evans, 2009-2011
// http://www.chrisevansdev.com/
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

namespace Accord.Imaging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Accord.Math;
    using AForge;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   SURF Feature descriptor types.
    /// </summary>
    /// 
    public enum SpeededUpRobustFeatureDescriptorType
    {
        /// <summary>
        ///   Do not compute descriptors.
        /// </summary>
        /// 
        None,

        /// <summary>
        ///   Compute standard descriptors.
        /// </summary>
        /// 
        Standard,

        /// <summary>
        ///   Compute extended descriptors.
        /// </summary>
        /// 
        Extended,
    }

    /// <summary>
    ///   Speeded-up Robust Features (SURF) detector.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Based on original implementation in the OpenSURF computer vision library
    ///   by Christopher Evans (http://www.chrisevansdev.com). Used under the LGPL
    ///   with permission of the original author.</para>
    ///   
    /// <para>
    ///   Be aware that the SURF algorithm is a patented algorithm by Anael Orlinski.
    ///   If you plan to use it in a commercial application, you may have to acquire
    ///   a license from the patent holder.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       E. Christopher. Notes on the OpenSURF Library. Available in: 
    ///       http://sites.google.com/site/chrisevansdev/files/opensurf.pdf</description></item>
    ///     <item><description>
    ///       P. D. Kovesi. MATLAB and Octave Functions for Computer Vision and Image Processing.
    ///       School of Computer Science and Software Engineering, The University of Western Australia.
    ///       Available in: http://www.csse.uwa.edu.au/~pk/Research/MatlabFns/Spatial/harris.m</description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    ///
    /// <seealso cref="SpeededUpRobustFeaturePoint"/>
    /// <seealso cref="SpeededUpRobustFeaturesDescriptor"/>
    ///
    [Serializable]
    public class SpeededUpRobustFeaturesDetector : ICornersDetector
    {
        private int octaves = 5;
        private int initial = 2;

        private double threshold = 0.0002;

        [NonSerialized]
        private ResponseLayerCollection responses;

        [NonSerialized]
        private IntegralImage integral;


        // Default description options
        [NonSerialized]
        private SpeededUpRobustFeaturesDescriptor descriptor;
        private SpeededUpRobustFeatureDescriptorType featureType = SpeededUpRobustFeatureDescriptorType.Standard;
        private bool computeOrientation = true;


        #region Constructors
        /// <summary>
        ///   Initializes a new instance of the <see cref="SpeededUpRobustFeaturesDetector"/> class.
        /// </summary>
        public SpeededUpRobustFeaturesDetector()
            : this(0.0002f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeededUpRobustFeaturesDetector"/> class.
        /// </summary>
        /// <param name="threshold">
        ///   The non-maximum suppression threshold. Default is 0.0002f.</param>
        public SpeededUpRobustFeaturesDetector(float threshold)
            : this(threshold, 5, 2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeededUpRobustFeaturesDetector"/> class.
        /// </summary>
        /// <param name="threshold">
        ///   The non-maximum suppression threshold. Default is 0.0002f.</param>
        /// <param name="octaves">
        ///   The number of octaves to use when building the <see cref="ResponseLayerCollection">
        ///   response filter</see>. Each octave corresponds to a series of maps covering a
        ///   doubling of scale in the image. Default is 5.</param>
        /// <param name="initial">
        ///   The initial step to use when building the <see cref="ResponseLayerCollection">
        ///   response filter</see>. Default is 2. </param>
        public SpeededUpRobustFeaturesDetector(float threshold, int octaves, int initial)
        {
            this.threshold = threshold;
            this.octaves = octaves;
            this.initial = initial;
        }
        #endregion



        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether all feature points
        ///   should have their orientation computed after being detected.
        ///   Default is true.
        /// </summary>
        /// 
        /// <remarks>Computing orientation requires additional processing; 
        /// set this property to false to compute the orientation of only
        /// selected points by using the <see cref="GetDescriptor()">
        /// current feature descriptor</see> for the last set of detected points.
        /// </remarks>
        /// 
        /// <value><c>true</c> if to compute orientation; otherwise, <c>false</c>.</value>
        /// 
        public bool ComputeOrientation
        {
            get { return computeOrientation; }
            set { computeOrientation = value; }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether all feature points
        ///   should have their descriptors computed after being detected.
        ///   Default is to compute standard descriptors.
        /// </summary>
        /// 
        /// <remarks>Computing descriptors requires additional processing; 
        /// set this property to false to compute the descriptors of only
        /// selected points by using the <see cref="GetDescriptor()">
        /// current feature descriptor</see> for the last set of detected points.
        /// </remarks>
        /// 
        /// <value><c>true</c> if to compute orientation; otherwise, <c>false</c>.</value>
        /// 
        public SpeededUpRobustFeatureDescriptorType ComputeDescriptors
        {
            get { return featureType; }
            set { featureType = value; }
        }

        /// <summary>
        ///   Gets or sets the non-maximum suppression
        ///   threshold. Default is 0.0002.
        /// </summary>
        /// 
        /// <value>The non-maximum suppression threshold.</value>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        ///   Gets or sets the number of octaves to use when building
        ///   the <see cref="ResponseLayerCollection">response filter</see>.
        ///   Each octave corresponds to a series of maps covering a
        ///   doubling of scale in the image. Default is 5.
        /// </summary>
        /// 
        public int Octaves
        {
            get { return octaves; }
            set
            {
                if (octaves != value)
                {
                    octaves = value;
                    responses = null;
                }
            }
        }

        /// <summary>
        ///   Gets or sets the initial step to use when building
        ///   the <see cref="ResponseLayerCollection">response filter</see>.
        ///   Default is 2.
        /// </summary>
        /// 
        public int Step
        {
            get { return initial; }
            set
            {
                if (initial != value)
                {
                    initial = value;
                    responses = null;
                }
            }
        }
        #endregion

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<SpeededUpRobustFeaturePoint> ProcessImage(UnmanagedImage image)
        {
            // check image format
            if (
                (image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppArgb)
                )
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
            }

            // make sure we have grayscale image
            UnmanagedImage grayImage = null;

            if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                grayImage = image;
            }
            else
            {
                // create temporary grayscale image
                grayImage = Grayscale.CommonAlgorithms.BT709.Apply(image);
            }


            // 1. Compute the integral for the given image
            integral = IntegralImage.FromBitmap(grayImage);



            // 2. Create and compute interest point response map
            if (responses == null)
            {
                // re-create only if really needed
                responses = new ResponseLayerCollection(image.Width, image.Height, octaves, initial);
            }
            else
            {
                responses.Update(image.Width, image.Height, initial);
            }

            // Compute the response map
            responses.Compute(integral);


            // 3. Suppress non-maximum points
            List<SpeededUpRobustFeaturePoint> featureList =
                new List<SpeededUpRobustFeaturePoint>();

            // for each image pyramid in the response map
            foreach (ResponseLayer[] layers in responses)
            {
                // Grab the three layers forming the pyramid
                ResponseLayer bot = layers[0]; // bottom layer
                ResponseLayer mid = layers[1]; // middle layer
                ResponseLayer top = layers[2]; // top layer

                int border = (top.Size + 1) / (2 * top.Step);

                int tstep = top.Step;
                int mstep = mid.Size - bot.Size;

                int mscale = mid.Width / top.Width;
                int bscale = bot.Width / top.Width;

                int r = 1;

                // for each row
                for (int y = border + 1; y < top.Height - border; y++)
                {
                    // for each pixel
                    for (int x = border + 1; x < top.Width - border; x++)
                    {
                        double currentValue = mid.Responses[y * mscale, x * mscale];

                        // for each windows' row
                        for (int i = -r; (currentValue >= threshold) && (i <= r); i++)
                        {
                            // for each windows' pixel
                            for (int j = -r; j <= r; j++)
                            {
                                int yi = y + i;
                                int xj = x + j;

                                // for each response layer
                                if (top.Responses[yi, xj] >= currentValue ||
                                    bot.Responses[yi * bscale, xj * bscale] >= currentValue || ((i != 0 || j != 0) &&
                                    mid.Responses[yi * mscale, xj * mscale] >= currentValue))
                                {
                                    currentValue = 0;
                                    break;
                                }
                            }
                        }

                        // check if this point is really interesting
                        if (currentValue >= threshold)
                        {
                            // interpolate to sub-pixel precision
                            double[] offset = interpolate(y, x, top, mid, bot);

                            if (System.Math.Abs(offset[0]) < 0.5 &&
                                System.Math.Abs(offset[1]) < 0.5 &&
                                System.Math.Abs(offset[2]) < 0.5)
                            {
                                featureList.Add(new SpeededUpRobustFeaturePoint(
                                    (x + offset[0]) * tstep,
                                    (y + offset[1]) * tstep,
                                    0.133333333 * (mid.Size + offset[2] * mstep),
                                    mid.Laplacian[y * mscale, x * mscale]));
                            }
                        }

                    }
                }
            }

            descriptor = null;

            if (featureType != SpeededUpRobustFeatureDescriptorType.None)
            {
                descriptor = new SpeededUpRobustFeaturesDescriptor(integral);
                descriptor.Extended = featureType == SpeededUpRobustFeatureDescriptorType.Extended;
                descriptor.Invariant = computeOrientation;
                descriptor.Compute(featureList);
            }
            else if (computeOrientation)
            {
                descriptor = new SpeededUpRobustFeaturesDescriptor(integral);
                foreach (var p in featureList) p.Orientation = descriptor.GetOrientation(p);
            }

            return featureList;
        }

        /// <summary>
        ///   Gets the <see cref="SpeededUpRobustFeaturesDescriptor">
        ///   feature descriptor</see> for the last processed image.
        /// </summary>
        /// 
        public SpeededUpRobustFeaturesDescriptor GetDescriptor()
        {
            if (descriptor == null)
            {
                descriptor = new SpeededUpRobustFeaturesDescriptor(integral);
                descriptor.Extended = featureType == SpeededUpRobustFeatureDescriptorType.Extended;
                descriptor.Invariant = computeOrientation;
            }

            return descriptor;
        }



        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<SpeededUpRobustFeaturePoint> ProcessImage(BitmapData imageData)
        {
            return ProcessImage(new UnmanagedImage(imageData));
        }

        /// <summary>
        ///   Process image looking for interest points.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found interest points.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public List<SpeededUpRobustFeaturePoint> ProcessImage(Bitmap image)
        {
            // check image format
            if (
                (image.PixelFormat != PixelFormat.Format8bppIndexed) &&
                (image.PixelFormat != PixelFormat.Format24bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppRgb) &&
                (image.PixelFormat != PixelFormat.Format32bppArgb)
                )
            {
                throw new UnsupportedImageFormatException("Unsupported pixel format of the source");
            }

            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            List<SpeededUpRobustFeaturePoint> corners;

            try
            {
                // process the image
                corners = ProcessImage(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                image.UnlockBits(imageData);
            }

            return corners;
        }


        private static double[] interpolate(int y, int x, ResponseLayer top, ResponseLayer mid, ResponseLayer bot)
        {
            int bs = bot.Width / top.Width;
            int ms = mid.Width / top.Width;
            int xp1 = x + 1, yp1 = y + 1;
            int xm1 = x - 1, ym1 = y - 1;

            // Compute first order scale-space derivatives
            double dx = (mid.Responses[y * ms, xp1 * ms] - mid.Responses[y * ms, xm1 * ms]) / 2f;
            double dy = (mid.Responses[yp1 * ms, x * ms] - mid.Responses[ym1 * ms, x * ms]) / 2f;
            double ds = (top.Responses[y, x] - bot.Responses[y * bs, x * bs]) / 2f;

            double[] d = 
            { 
                -dx,
                -dy,
                -ds
            };

            // Compute Hessian
            double v = mid.Responses[y * ms, x * ms] * 2.0;
            double dxx = (mid.Responses[y * ms, xp1 * ms] + mid.Responses[y * ms, xm1 * ms] - v);
            double dyy = (mid.Responses[yp1 * ms, x * ms] + mid.Responses[ym1 * ms, x * ms] - v);
            double dxs = (top.Responses[y, xp1] - top.Responses[y, x - 1] - bot.Responses[y * bs, xp1 * bs] + bot.Responses[y * bs, xm1 * bs]) / 4f;
            double dys = (top.Responses[yp1, x] - top.Responses[y - 1, x] - bot.Responses[yp1 * bs, x * bs] + bot.Responses[ym1 * bs, x * bs]) / 4f;
            double dss = (top.Responses[y, x] + bot.Responses[y * ms, x * ms] - v);
            double dxy = (mid.Responses[yp1 * ms, xp1 * ms] - mid.Responses[yp1 * ms, xm1 * ms]
                - mid.Responses[ym1 * ms, xp1 * ms] + mid.Responses[ym1 * ms, xm1 * ms]) / 4f;

            double[,] H =
            {
                { dxx, dxy, dxs },
                { dxy, dyy, dys },
                { dxs, dys, dss },
            };

            // Compute interpolation offsets
            return H.Inverse(true).Multiply(d);
        }


        /// <summary>
        ///   Response filter.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        ///   In SURF, the scale-space is divided into a number of octaves,
        ///   where an octave refers to a series of <see cref="ResponseLayer"/>
        ///   response maps covering a doubling of scale.</para>
        /// <para>
        ///   In the traditional approach to constructing a scale-space,
        ///   the image size is varied and the Gaussian filter is repeatedly
        ///   applied to smooth subsequent layers. The SURF approach leaves
        ///   the original image unchanged and varies only the filter size.</para>
        /// </remarks>
        /// 
        public class ResponseLayerCollection : IEnumerable<ResponseLayer[]>
        {
            private int width;
            private int height;
            private int step;
            private int octaves;

            private static readonly int[,] map = 
            {
                { 0,  1,  2,  3 },
                { 1,  3,  4,  5 },
                { 3,  5,  6,  7 },
                { 5,  7,  8,  9 },
                { 7,  9, 10, 11 }
            };

            private ResponseLayer[] responses;


            /// <summary>
            /// Gets the image width used by the filter.
            /// </summary>
            /// 
            public int Width { get { return width; } }

            /// <summary>
            /// Gets the image height used by the filter.
            /// </summary>
            /// 
            public int Height { get { return height; } }

            /// <summary>
            ///   Creates the initial map of responses according to
            ///   the specified number of octaves and initial step.
            /// </summary>
            /// 
            public ResponseLayerCollection(int width, int height, int octaves, int initial)
            {
                this.width = width;
                this.height = height;
                this.step = initial;
                this.octaves = octaves;

                this.initialize();
            }

            /// <summary>
            ///   Updates the response filter definitions
            ///   without recreating objects.
            /// </summary>
            /// 
            public void Update(int width, int height, int initial)
            {
                this.width = width;
                this.height = height;
                this.step = initial;

                this.update();
            }

            /// <summary>
            ///   Computes the filter using the specified <see cref="IntegralImage">
            ///   Integral Image.</see>
            /// </summary>
            /// 
            /// <param name="integral">The integral image.</param>
            /// 
            public void Compute(IntegralImage integral)
            {
                for (int i = 0; i < responses.Length; ++i)
                    responses[i].Compute(integral);
            }

            private void initialize()
            {
                List<ResponseLayer> list = new List<ResponseLayer>();

                // Get image attributes
                int w = width / step;
                int h = height / step;
                int s = step;

                if (octaves >= 1)
                {
                    list.Add(new ResponseLayer(w, h, s, 9));
                    list.Add(new ResponseLayer(w, h, s, 15));
                    list.Add(new ResponseLayer(w, h, s, 21));
                    list.Add(new ResponseLayer(w, h, s, 27));
                }

                if (octaves >= 2)
                {
                    list.Add(new ResponseLayer(w / 2, h / 2, s * 2, 39));
                    list.Add(new ResponseLayer(w / 2, h / 2, s * 2, 51));
                }

                if (octaves >= 3)
                {
                    list.Add(new ResponseLayer(w / 4, h / 4, s * 4, 75));
                    list.Add(new ResponseLayer(w / 4, h / 4, s * 4, 99));
                }

                if (octaves >= 4)
                {
                    list.Add(new ResponseLayer(w / 8, h / 8, s * 8, 147));
                    list.Add(new ResponseLayer(w / 8, h / 8, s * 8, 195));
                }

                if (octaves >= 5)
                {
                    list.Add(new ResponseLayer(w / 16, h / 16, s * 16, 291));
                    list.Add(new ResponseLayer(w / 16, h / 16, s * 16, 387));
                }

                this.responses = list.ToArray();
            }

            private void update()
            {
                // Get image attributes
                int w = width / step;
                int h = height / step;
                int s = step;

                int i = 0;
                if (octaves >= 1)
                {
                    responses[i++].Update(w, h, s, 9);
                    responses[i++].Update(w, h, s, 15);
                    responses[i++].Update(w, h, s, 21);
                    responses[i++].Update(w, h, s, 27);
                }

                if (octaves >= 2)
                {
                    responses[i++].Update(w / 2, h / 2, s * 2, 39);
                    responses[i++].Update(w / 2, h / 2, s * 2, 51);
                }

                if (octaves >= 3)
                {
                    responses[i++].Update(w / 4, h / 4, s * 4, 75);
                    responses[i++].Update(w / 4, h / 4, s * 4, 99);
                }

                if (octaves >= 4)
                {
                    responses[i++].Update(w / 8, h / 8, s * 8, 147);
                    responses[i++].Update(w / 8, h / 8, s * 8, 195);
                }

                if (octaves >= 5)
                {
                    responses[i++].Update(w / 16, h / 16, s * 16, 291);
                    responses[i++].Update(w / 16, h / 16, s * 16, 387);
                }
            }


            /// <summary>
            ///   Returns an enumerator that iterates through the collection.
            /// </summary>
            /// 
            /// <returns>
            ///   A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            /// 
            public IEnumerator<ResponseLayer[]> GetEnumerator()
            {
                ResponseLayer[] pyramid = new ResponseLayer[3];

                for (int i = 0; i < octaves; i++)
                {
                    // for each set of response layers
                    for (int j = 0; j <= 1; j++)
                    {
                        // Grab the three layers forming the pyramid
                        pyramid[0] = responses[map[i, j + 0]];
                        pyramid[1] = responses[map[i, j + 1]];
                        pyramid[2] = responses[map[i, j + 2]];
                        yield return pyramid;
                    }
                }
            }

            /// <summary>
            ///   Returns an enumerator that iterates through this collection.
            /// </summary>
            /// <returns>
            ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            /// 
            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)this.GetEnumerator();
            }

        }

        /// <summary>
        ///   Response Layer.
        /// </summary>
        /// 
        [Serializable]
        public class ResponseLayer
        {
            /// <summary>
            ///   Gets the width of the filter.
            /// </summary>
            /// 
            public int Width { get; private set; }

            /// <summary>
            ///   Gets the height of the filter.
            /// </summary>
            /// 
            public int Height { get; private set; }

            /// <summary>
            ///   Gets the filter step.
            /// </summary>
            /// 
            public int Step { get; private set; }

            /// <summary>
            ///   Gets the filter size.
            /// </summary>
            /// 
            public int Size { get; private set; }

            /// <summary>
            ///   Gets the responses computed from the filter.
            /// </summary>
            /// 
            public float[,] Responses { get; private set; }

            /// <summary>
            ///   Gets the Laplacian computed from the filter.
            /// </summary>
            /// 
            public int[,] Laplacian { get; private set; }


            /// <summary>
            ///   Initializes a new instance of the <see cref="ResponseLayer"/> class.
            /// </summary>
            /// 
            public ResponseLayer(int width, int height, int step, int filter)
            {
                this.Width = width;
                this.Height = height;
                this.Step = step;
                this.Size = filter;

                this.Responses = new float[height, width];
                this.Laplacian = new int[height, width];
            }

            /// <summary>
            ///   Updates the response layer definitions
            ///   without recreating objects.
            /// </summary>
            /// 
            public void Update(int width, int height, int step, int filter)
            {
                if (height > Height || width > Width)
                {
                    this.Responses = new float[height, width];
                    this.Laplacian = new int[height, width];
                }

                this.Width = width;
                this.Height = height;
                this.Step = step;
                this.Size = filter;
            }

            /// <summary>
            ///   Computes the filter for the specified integral image.
            /// </summary>
            /// 
            /// <param name="image">The integral image.</param>
            /// 
            public void Compute(IntegralImage image)
            {
                int b = (Size - 1) / 2 + 1;
                int c = Size / 3;
                int w = Size;
                float inv = 1f / (w * w);
                float Dxx, Dyy, Dxy;

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        // Get the image coordinates
                        int i = y * Step;
                        int j = x * Step;

                        // Compute response components
                        Dxx = ((int)image.GetRectangleSum(j - b, i - c + 1, j - b + w - 1, i - c + 2 * c - 1)
                             - (int)image.GetRectangleSum(j - c / 2, i - c + 1, j - c / 2 + c - 1, i - c + 2 * c - 1) * 3);

                        Dyy = ((int)image.GetRectangleSum(j - c + 1, i - b, j - c + 2 * c - 1, i - b + w - 1)
                             - (int)image.GetRectangleSum(j - c + 1, i - c / 2, j - c + 2 * c - 1, i - c / 2 + c - 1) * 3);

                        Dxy = ((int)image.GetRectangleSum(j + 1, i - c, j + c, i - 1)
                             + (int)image.GetRectangleSum(j - c, i + 1, j - 1, i + c)
                             - (int)image.GetRectangleSum(j - c, i - c, j - 1, i - 1)
                             - (int)image.GetRectangleSum(j + 1, i + 1, j + c, i + c));

                        // Normalize the filter responses with respect to their size
                        Dxx *= inv / 255f;
                        Dyy *= inv / 255f;
                        Dxy *= inv / 255f;

                        // Get the determinant of hessian response & laplacian sign
                        Responses[y, x] = (Dxx * Dyy) - (0.9f * 0.9f * Dxy * Dxy);
                        Laplacian[y, x] = (Dxx + Dyy) >= 0 ? 1 : 0;
                    }
                }
            }

        }


        #region ICornersDetector Members

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// <param name="image">Unmanaged source image to process.</param>
        /// <returns>
        /// Returns list of found corners (X-Y coordinates).
        /// </returns>
        List<IntPoint> ICornersDetector.ProcessImage(UnmanagedImage image)
        {
            return ProcessImage(image).ConvertAll(p => new IntPoint((int)p.X, (int)p.Y));
        }

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// <param name="imageData">Source image data to process.</param>
        /// <returns>
        /// Returns list of found corners (X-Y coordinates).
        /// </returns>
        List<IntPoint> ICornersDetector.ProcessImage(BitmapData imageData)
        {
            return ProcessImage(imageData).ConvertAll(p => new IntPoint((int)p.X, (int)p.Y));
        }

        /// <summary>
        /// Process image looking for corners.
        /// </summary>
        /// <param name="image">Source image to process.</param>
        /// <returns>
        /// Returns list of found corners (X-Y coordinates).
        /// </returns>
        List<IntPoint> ICornersDetector.ProcessImage(Bitmap image)
        {
            return ProcessImage(image).ConvertAll(p => new IntPoint((int)p.X, (int)p.Y));
        }

        #endregion

    }
}
