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
    using Accord.Math;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Histograms of Oriented Gradients [experimental].
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class is currently very experimental. Use with care.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Navneet Dalal and Bill Triggs, "Histograms of Oriented Gradients for Human Detection",
    ///       CVPR 2005. Available at: <a href="http://lear.inrialpes.fr/people/triggs/pubs/Dalal-cvpr05.pdf">
    ///       http://lear.inrialpes.fr/people/triggs/pubs/Dalal-cvpr05.pdf </a> </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public class HistogramsOfOrientedGradients
    {

        int numberOfBins = 9;
        int cellSize = 6;  // size of the cell, in number of pixels
        int blockSize = 3; // size of the block, in number of cells

        double epsilon = 1e-10;
        double binWidth;


        /// <summary>
        ///   Gets the size of a cell, in pixels.
        /// </summary>
        /// 
        public int CellSize { get { return cellSize; } }

        /// <summary>
        ///   Gets the size of a block, in pixels.
        /// </summary>
        /// 
        public int BlockSize { get { return blockSize; } }

        /// <summary>
        ///   Gets the number of histogram bins.
        /// </summary>
        /// 
        public int NumberOfBins { get { return numberOfBins; } }


        /// <summary>
        ///   Initializes a new instance of the <see cref="HistogramsOfOrientedGradients"/> class.
        /// </summary>
        /// 
        /// <param name="numberOfBins">The number of histogram bins.</param>
        /// <param name="blockSize">The size of a block, measured in cells.</param>
        /// <param name="cellSize">The size of a cell, measured in pixels.</param>
        /// 
        public HistogramsOfOrientedGradients(int numberOfBins = 9, int blockSize = 3, int cellSize = 6)
        {
            this.numberOfBins = numberOfBins;
            this.cellSize = cellSize;
            this.blockSize = blockSize;
            this.binWidth = (2.0 * System.Math.PI) / numberOfBins; // 0 to 360
        }


        /// <summary>
        ///   Process image looking for corners.
        /// </summary>
        /// 
        /// <param name="image">Source image data to process.</param>
        /// 
        /// <returns>Returns list of found corners (X-Y coordinates).</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">
        ///   The source image has incorrect pixel format.
        /// </exception>
        /// 
        public unsafe List<double[]> ProcessImage(UnmanagedImage image)
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


            // get source image size
            int width = grayImage.Width;
            int height = grayImage.Height;
            int srcStride = grayImage.Stride;
            int srcOffset = srcStride - width;


            // 1. Calculate partial differences
            float[,] direction = new float[height, width];
            float[,] magnitude = new float[height, width];

            fixed (float* ptrDir = direction, ptrMag = magnitude)
            {
                byte* src = (byte*)grayImage.ImageData.ToPointer() + srcStride + 1;

                // Skip first row and first column
                float* dir = ptrDir + width + 1;
                float* mag = ptrMag + width + 1;

                // for each line
                for (int y = 1; y < height - 1; y++)
                {
                    // for each pixel
                    for (int x = 1; x < width - 1; x++, src++, dir++, mag++)
                    {
                        // Convolution with horizontal differentiation kernel mask
                        float h = ((src[-srcStride + 1] + src[+1] + src[srcStride + 1]) -
                                  (src[-srcStride - 1] + src[-1] + src[srcStride - 1])) * 0.166666667f;

                        // Convolution vertical differentiation kernel mask
                        float v = ((src[+srcStride - 1] + src[+srcStride] + src[+srcStride + 1]) -
                                   (src[-srcStride - 1] + src[-srcStride] + src[-srcStride + 1])) * 0.166666667f;

                        // Store angles and magnitudes directly
                        *dir = (float)Math.Atan2(v, h);
                        *mag = (float)Math.Sqrt(h * h + v * v);
                    }

                    // Skip last column
                    dir++; mag++; src += srcOffset + 1;
                }
            }

            // Free some resources which wont be needed anymore
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                grayImage.Dispose();



            // 3. Compute cell histograms
            int cellCountX = (int)Math.Floor(width / (double)cellSize);
            int cellCountY = (int)Math.Floor(height / (double)cellSize);
            double[,][] histograms = new double[cellCountX, cellCountY][];

            // For each cell
            for (int i = 0; i < cellCountX; i++)
            {
                for (int j = 0; j < cellCountY; j++)
                {
                    // Compute the histogram
                    double[] histogram = new double[numberOfBins];

                    int startCellX = i * cellSize;
                    int startCellY = j * cellSize;

                    // for each pixel in the cell
                    for (int x = 0; x < cellSize; x++)
                    {
                        for (int y = 0; y < cellSize; y++)
                        {
                            double ang = direction[startCellX + x, startCellY + y];
                            double mag = magnitude[startCellX + x, startCellY + y];

                            // Get its angular bin
                            int bin = (int)System.Math.Floor((ang + Math.PI) * binWidth);

                            histogram[bin] += mag;
                        }
                    }

                    histograms[i, j] = histogram;
                }
            }

            // 3. Group the cells into larger, normalized blocks
            int blocksCountX = (int)Math.Floor(cellCountX / (double)blockSize);
            int blocksCountY = (int)Math.Floor(cellCountY / (double)blockSize);

            List<double[]> blocks = new List<double[]>();

            for (int i = 0; i < blocksCountX; i++)
            {
                for (int j = 0; j < blocksCountY; j++)
                {
                    double[] v = new double[blockSize * blockSize * numberOfBins];

                    int startBlockX = i * blockSize;
                    int startBlockY = j * blockSize;
                    int c = 0;

                    // for each cell in the block
                    for (int x = 0; x < blockSize; x++)
                    {
                        for (int y = 0; y < blockSize; y++)
                        {
                            double[] histogram = 
                                histograms[startBlockX + x, startBlockY + y];

                            // Copy all histograms to the block vector
                            for (int k = 0; k < histogram.Length; k++)
                                v[c++] = histogram[k];
                        }
                    }

                    double[] block = v.Divide(v.Euclidean() + epsilon);

                    blocks.Add(block);
                }
            }

            return blocks;
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
        public List<double[]> ProcessImage(BitmapData imageData)
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
        public List<double[]> ProcessImage(Bitmap image)
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

            List<double[]> blocks;

            try
            {
                // process the image
                blocks = ProcessImage(new UnmanagedImage(imageData));
            }
            finally
            {
                // unlock image
                image.UnlockBits(imageData);
            }

            return blocks;
        }
    }
}
