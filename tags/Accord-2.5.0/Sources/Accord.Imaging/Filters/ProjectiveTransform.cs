// Accord Imaging Library
// Accord.NET framework
// http://www.crsouza.com
//
// Copyright © César Souza, 2009-2010
// cesarsouza at gmail.com
//

namespace Accord.Imaging.Filters
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using System;
    using Matrix = Accord.Math.Matrix;

    /// <summary>
    ///   Projective transform filter.
    /// </summary>
    /// 
    public class ProjectiveTransform : AForge.Imaging.Filters.BaseTransformationFilter
    {
        private MatrixH homography;
        private Bitmap overlayImage;
        private Point offset;
        private Point center;
        private Size imageSize;
        private Color fillColor = Color.FromArgb(0, Color.Black);
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();
        private bool keepSize = true;


        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Gets or sets the Homography matrix used to map a image passed to
        ///   the filter to the overlay image specified at filter creation.
        /// </summary>
        public MatrixH Homography
        {
            get { return homography; }
            set { homography = value; }
        }

        /// <summary>
        ///   Gets or sets the filling color used to fill blank spaces.
        /// </summary>
        /// <remarks>
        ///   The filling color will only be visible after the image is converted
        ///   to 24bpp. The alpha channel will be used internally by the filter.
        /// </remarks>
        public Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }


        /// <summary>
        ///   Constructs a new Blend filter.
        /// </summary>
        /// <param name="homography">The homography matrix mapping a second image to the overlay image.</param>
        public ProjectiveTransform(MatrixH homography)
        {
            this.homography = homography;
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        ///   Computes the new image size.
        /// </summary>
        protected override Size CalculateNewImageSize(UnmanagedImage sourceData)
        {
            if (keepSize) 
                return new Size(sourceData.Width, sourceData.Height);


            // Calculate source size
            float w = sourceData.Width;
            float h = sourceData.Height;

            // Get the four corners of the image
            PointF[] corners =
            {
                new PointF(0, 0),
                new PointF(w, 0),
                new PointF(0, h),
                new PointF(w, h),
            };

            // Project those points
            corners = homography.Inverse().TransformPoints(corners);

            // Recalculate image size
            float[] px = { corners[0].X, corners[1].X, corners[2].X, corners[3].X };
            float[] py = { corners[0].Y, corners[1].Y, corners[2].Y, corners[3].Y };

            float maxX = Matrix.Max(px);
            float minX = Matrix.Min(px);
            float newWidth = Math.Max(maxX, w) - Math.Min(0, minX);

            float maxY = Accord.Math.Matrix.Max(py);
            float minY = Accord.Math.Matrix.Min(py);
            float newHeight = Math.Max(maxY, h) - Math.Min(0, minY);


            // Store overlay image size
            this.imageSize = new Size((int)Math.Round(maxX - minX), (int)Math.Round(maxY - minY));

            // Calculate and store image offset
            int offsetX = 0, offsetY = 0;
            if (minX < 0) offsetX = (int)Math.Round(minX);
            if (minY < 0) offsetY = (int)Math.Round(minY);

            this.offset = new Point(offsetX, offsetY);

            // Return the final image size
            return new Size((int)Math.Ceiling(newWidth), (int)Math.Ceiling(newHeight));
        }


        /// <summary>
        ///   Process the image filter.
        /// </summary>
        protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {

            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            // get destination image size
            int newWidth = destinationData.Width;
            int newHeight = destinationData.Height;

            int srcPixelSize = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
            int srcStride = sourceData.Stride;

            int dstOffset = destinationData.Stride - newWidth * 4; // destination always 32bpp argb


            // fill values
            byte fillR = fillColor.R;
            byte fillG = fillColor.G;
            byte fillB = fillColor.B;
            byte fillA = fillColor.A;

            // Retrieve homography matrix as float array
            float[,] H = (float[,])homography;


            // do the job
            unsafe
            {
                byte* src = (byte*)sourceData.ImageData.ToPointer();
                byte* dst = (byte*)destinationData.ImageData.ToPointer();

                // Fill the image with filling color
                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++, dst += 4)
                    {
                        dst[0] = fillB;
                        dst[1] = fillG;
                        dst[2] = fillR;
                        dst[3] = fillA;
                    }
                    dst += dstOffset;
                }



                // destination pixel's coordinate relative to image center
                double cx, cy;

                // destination pixel's homogenous coordinate
                double hx, hy, hw;

                // source pixel's coordinates
                int ox, oy;

                src = (byte*)sourceData.ImageData.ToPointer();
                dst = (byte*)destinationData.ImageData.ToPointer();

                // Project and blend the second image
                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++, dst += 4)
                    {
                        cx = x + offset.X;
                        cy = y + offset.Y;

                        // projection using homogenous coordinates
                        hw = H[2, 0] * cx + H[2, 1] * cy + H[2, 2];
                        hx = (H[0, 0] * cx + H[0, 1] * cy + H[0, 2]) / hw;
                        hy = (H[1, 0] * cx + H[1, 1] * cy + H[1, 2]) / hw;

                        // coordinate of the nearest point
                        ox = (int)(hx);
                        oy = (int)(hy);

                        // validate source pixel's coordinates
                        if ((ox >= 0) && (oy >= 0) && (ox < width) && (oy < height))
                        {
                            int c = oy * srcStride + ox * srcPixelSize;

                            // copy the source into the destination image
                            dst[0] = (byte)src[c];

                            if (srcPixelSize >= 3)
                            {
                                // 24/32bpp
                                dst[1] = (byte)src[c + 1];
                                dst[2] = (byte)src[c + 2];
                                dst[3] = (srcPixelSize == 4)
                                    ? (byte)src[c + 3] // 32bpp
                                    : (byte)255;       // 24bpp
                            }
                            else
                            {
                                // 8bpp
                                dst[1] = (byte)src[c];
                                dst[2] = (byte)src[c];
                                dst[3] = (byte)255;
                            }
                        }
                    }
                    dst += dstOffset;
                }
            }

        }

    }
}
