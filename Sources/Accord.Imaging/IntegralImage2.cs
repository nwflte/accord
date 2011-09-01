// Accord Imaging Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
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
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Imaging;

    /// <summary>
    ///   Joint representation of both Integral Image and Squared Integral Image.
    /// </summary>
    /// 
    /// <remarks>
    ///   Using this representation, both structures can be created in a single pass
    ///   over the data. This is interesting for real time applications. This class
    ///   also accepts a channel parameter indicating the Integral Image should be
    ///   computed using a specified color channel. This avoids costly conversions.
    /// </remarks>
    /// 
    public class IntegralImage2
    {
        // TODO: Check the use of pointers for those matrix images with a profiler.

        private int[,] iiSum = null;  // normal  integral image
        private int[,] iiSquareSum = null; // squared integral image
        private int[,] iiTiltedSum = null; // tilted  integral image

        private int width;
        private int height;


        /// <summary>
        ///   Gets the image's width.
        /// </summary>
        /// 
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        ///   Gets the image's height.
        /// </summary>
        /// 
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        ///   Gets the Integral Image for values' sum.
        /// </summary>
        /// 
        public int[,] Image
        {
            get { return iiSum; }
        }

        /// <summary>
        ///   Gets the Integral Image for values' squared sum.
        /// </summary>
        /// 
        public int[,] Squared
        {
            get { return iiSquareSum; }
        }

        /// <summary>
        ///   Gets the Integral Image for tilted values' sum.
        /// </summary>
        /// 
        public int[,] Rotated
        {
            get { return iiTiltedSum; }
        }

        /// <summary>
        ///   Constructs a new Integral image of the given size.
        /// </summary>
        /// 
        protected IntegralImage2(int width, int height, bool computeTilted)
        {
            this.width = width;
            this.height = height;
            iiSum = new int[height + 1, width + 1];
            iiSquareSum = new int[height + 1, width + 1];

            if (computeTilted)
                iiTiltedSum = new int[height + 2, width + 2];
        }

        /// <summary>
        ///   Constructs a new Integral image from a Bitmap image.
        /// </summary>
        /// 
        public static IntegralImage2 FromBitmap(Bitmap image, int channel)
        {
            return FromBitmap(image, channel, false);
        }

        /// <summary>
        ///   Constructs a new Integral image from a Bitmap image.
        /// </summary>
        /// 
        public static IntegralImage2 FromBitmap(Bitmap image, int channel, bool computeTilted)
        {
            // check image format
            if (!(image.PixelFormat == PixelFormat.Format8bppIndexed ||
                image.PixelFormat == PixelFormat.Format24bppRgb ||
                image.PixelFormat == PixelFormat.Format32bppArgb))
            {
                throw new UnsupportedImageFormatException("Only grayscale and 24 bpp RGB images are supported.");
            }


            // lock source image
            BitmapData imageData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);

            // process the image
            IntegralImage2 im = FromBitmap(imageData, channel, computeTilted);

            // unlock image
            image.UnlockBits(imageData);

            return im;
        }

        /// <summary>
        ///   Constructs a new Integral image from a BitmapData image.
        /// </summary>
        /// 
        public static IntegralImage2 FromBitmap(BitmapData imageData, int channel)
        {
            return FromBitmap(new UnmanagedImage(imageData), channel);
        }

        /// <summary>
        ///   Constructs a new Integral image from a BitmapData image.
        /// </summary>
        /// 
        public static IntegralImage2 FromBitmap(BitmapData imageData, int channel, bool computeTilted)
        {
            return FromBitmap(new UnmanagedImage(imageData), channel, computeTilted);
        }

        /// <summary>
        ///   Constructs a new Integral image from an unmanaged image.
        /// </summary>
        /// 
        public static IntegralImage2 FromBitmap(UnmanagedImage image, int channel)
        {
            return FromBitmap(image, channel, false);
        }

        /// <summary>
        ///   Constructs a new Integral image from an unmanaged image.
        /// </summary>
        /// 
        public static IntegralImage2 FromBitmap(UnmanagedImage image, int channel, bool computeTilted/*, TODO: Rectangle roi*/)
        {

            // check image format
            if (!(image.PixelFormat == PixelFormat.Format8bppIndexed ||
                image.PixelFormat == PixelFormat.Format24bppRgb ||
                image.PixelFormat == PixelFormat.Format32bppArgb))
            {
                throw new UnsupportedImageFormatException("Only grayscale and 24 bpp RGB images are supported.");
            }

            int pixelSize = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;

            // get source image size
            int width = image.Width;
            int height = image.Height;
            int stride = image.Stride;
            int offset = stride - width * pixelSize;

            // create integral image
            IntegralImage2 im = new IntegralImage2(width, height, computeTilted);
            int[,] ii1 = im.iiSum;
            int[,] ii2 = im.iiSquareSum;
            int[,] iit = im.iiTiltedSum;


            if (image.PixelFormat == PixelFormat.Format8bppIndexed && channel != 0)
                throw new ArgumentException("Only the first channel is available for 8 bpp images.", "channel");

            // do the job
            unsafe
            {
                byte* src = (byte*)image.ImageData.ToPointer() + channel;

                // for each line
                for (int y = 1; y <= height; y++)
                {
                    // for each pixel
                    for (int x = 1; x <= width; x++, src += pixelSize)
                    {
                        int p = *src;
                        int p2 = p * p;
                        ii1[y, x] = p + ii1[y, x - 1] + ii1[y - 1, x] - ii1[y - 1, x - 1];
                        ii2[y, x] = p2 + ii2[y, x - 1] + ii2[y - 1, x] - ii2[y - 1, x - 1];
                    }
                    src += offset;
                }


                if (computeTilted)
                {
                    src = (byte*)image.ImageData.ToPointer() + channel;

                    // TODO: Optimize with matrix pointers. Will probably make the code cleaner.

                    // Left-to-right, top-to-bottom pass
                    for (int y = 1; y <= height; y++)
                    {
                        for (int x = 2; x < width + 2; x++, src += pixelSize)
                            iit[y, x] = *src + iit[y - 1, x - 1] + iit[y, x - 1] - iit[y - 1, x - 2];
                        src += offset;
                    }

                    for (int x = 2; x < width + 2; x++, src += pixelSize)
                        iit[height + 1, x] = iit[height, x - 1] + iit[height + 1, x - 1] - iit[height, x - 2];


                    // Right-to-left, bottom-to-top pass
                    for (int y = height; y >= 0; y--)
                        for (int x = width + 1; x >= 1; x--)
                            iit[y, x] += iit[y + 1, x - 1];

                    for (int y = height + 1; y >= 0; y--)
                        for (int x = width + 1; x >= 2; x--)
                            iit[y, x] -= iit[y, x - 2];

                }

            } // end unsafe


            return im;
        }

        /// <summary>
        ///   Gets the sum of the pixels in a rectangle of the Integral image.
        /// </summary>
        /// 
        public int GetSum(int x, int y, int width, int height)
        {
            return iiSum[y, x] + iiSum[y + height, x + width]
                 - iiSum[y + height, x] - iiSum[y, x + width];
        }

        /// <summary>
        ///   Gets the sum of the squared pixels in a rectangle of the Integral image.
        /// </summary>
        /// 
        public int GetSum2(int x, int y, int width, int height)
        {
            return iiSquareSum[y, x] + iiSquareSum[y + height, x + width]
                 - iiSquareSum[y + height, x] - iiSquareSum[y, x + width];
        }


        /// <summary>
        ///   Gets the sum of the pixels in a tilted rectangle of the Integral image.
        /// </summary>
        /// 
        public int GetSumT(int x, int y, int width, int height)
        {
            return iiTiltedSum[y + width, x + width + 1] + iiTiltedSum[y + height, x - height + 1]
                - iiTiltedSum[y, x + 1] - iiTiltedSum[y + width + height, x + width - height + 1];
        }
    }

}
