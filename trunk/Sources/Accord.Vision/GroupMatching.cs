// Accord Vision Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//
// This code has been submitted as an user contribution by darko.juric2
// GCode Issue #12 https://code.google.com/p/accord/issues/detail?id=12
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

namespace Accord.Vision.Detection
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    ///   Group matching algorithm for detection region averging.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be seen as a post-processing filter. Its goal is to
    ///   group near or contained regions together in order to produce more
    ///   robust and smooth estimates of the detected regions.
    /// </remarks>
    /// 
    public class GroupMatching
    {

        private double threshold = 0.2;
        private int classCount;

        private int minNeighbors;

        private int[] labels;
        private int[] equals;

        private List<Rectangle> filter;

        /// <summary>
        ///   Creates a new <see cref="GroupMatching"/> object.
        /// </summary>
        /// 
        /// <param name="minimumNeighbors">
        ///   The minimum number of neighbors needed to keep a detection. If a rectangle
        ///   has less than this minimum number, it will be discarded as a false positive.</param>
        /// <param name="threshold">
        ///   The minimum distance threshold to consider two rectangles as neighbors.
        ///   Default is 0.2.</param>
        /// 
        public GroupMatching(int minimumNeighbors = 2, double threshold = 0.2)
        {
            this.minNeighbors = minimumNeighbors;
            this.threshold = threshold / 2.0;
            this.filter = new List<Rectangle>();
        }

        /// <summary>
        ///   Gets or sets the minimum number of neighbors necessary to keep a detection.
        ///   If a rectangle has less neighbors than this number, it will be discarded as
        ///   a false positive.
        /// </summary>
        /// 
        public int MinimumNeighbors
        {
            get { return minNeighbors; }
            set
            {
                if (minNeighbors < 0)
                    throw new ArgumentOutOfRangeException("value", "Value must be equal to or higher than zero.");
                minNeighbors = value;
            }
        }

        /// <summary>
        ///   Groups possibly near rectangles into a smaller
        ///   set of distinct and averaged rectangles.
        /// </summary>
        /// 
        /// <param name="rectangles">The rectangles to group.</param>
        /// 
        public Rectangle[] Group(Rectangle[] rectangles)
        {
            // Start by classifying rectangles according to distance
            classify(rectangles); // assign label for near rectangles

            int[] neighborCount;

            // Average the rectangles contained in each labelled group
            Rectangle[] output = average(rectangles, out neighborCount);

            // Check supression
            if (minNeighbors > 0)
            {
                filter.Clear();

                // Discard weak rectangles which don't have enough neighbors
                for (int i = 0; i < output.Length; i++)
                    if (neighborCount[i] >= minNeighbors) filter.Add(output[i]);

                return filter.ToArray();
            }

            return output;
        }

        /// <summary>
        ///   Averages rectangles which belongs to the
        ///   same class (have the same class label)
        /// </summary>
        /// 
        private Rectangle[] average(Rectangle[] rectangles, out int[] neighborCounts)
        {
            neighborCounts = new int[classCount];

            Rectangle[] centroids = new Rectangle[classCount];
            for (int i = 0; i < rectangles.Length; i++)
            {
                int j = labels[i];

                centroids[j].X += rectangles[i].X;
                centroids[j].Y += rectangles[i].Y;
                centroids[j].Width += rectangles[i].Width;
                centroids[j].Height += rectangles[i].Height;

                neighborCounts[j]++;
            }

            for (int i = 0; i < centroids.Length; i++)
            {
                centroids[i] = new Rectangle
                (
                    x: (int)Math.Ceiling((float)centroids[i].X / neighborCounts[i]),
                    y: (int)Math.Ceiling((float)centroids[i].Y / neighborCounts[i]),
                    width: (int)Math.Ceiling((float)centroids[i].Width / neighborCounts[i]),
                    height: (int)Math.Ceiling((float)centroids[i].Height / neighborCounts[i])
                );
            }

            return centroids;
        }

        /// <summary>
        ///   Detects rectangles which are near and 
        ///   assigns similar class labels accordingly.
        /// </summary>
        /// 
        private void classify(Rectangle[] rectangles)
        {
            equals = new int[rectangles.Length];
            for (int i = 0; i < equals.Length; i++)
                equals[i] = -1;

            labels = new int[rectangles.Length];
            for (int i = 0; i < labels.Length; i++)
                labels[i] = i;

            classCount = 0;

            // If two rectangles are near, or contained in
            // each other, merge then in a single rectangle
            for (int i = 0; i < rectangles.Length - 1; i++)
            {
                for (int j = i + 1; j < rectangles.Length; j++)
                {
                    if (near(rectangles[i], rectangles[j]))
                        merge(labels[i], labels[j]);
                }
            }

            // Count the number of classes and centroids
            int[] centroids = new int[rectangles.Length];
            for (int i = 0; i < centroids.Length; i++)
                if (equals[i] == -1) centroids[i] = classCount++;

            // Classify all rectangles with their labels
            for (int i = 0; i < rectangles.Length; i++)
            {
                int root = labels[i];
                while (equals[root] != -1)
                    root = equals[root];

                labels[i] = centroids[root];
            }
        }

        /// <summary>
        ///   Merges two labels.
        /// </summary>
        /// 
        private void merge(int label1, int label2)
        {
            int root1 = label1;
            int root2 = label2;

            // Get the roots associated with the two labels
            while (equals[root1] != -1) root1 = equals[root1];
            while (equals[root2] != -1) root2 = equals[root2];

            if (root1 == root2) // labels are already connected
                return;

            int minRoot, maxRoot;
            int labelWithMinRoot, labelWithMaxRoot;

            if (root1 > root2)
            {
                maxRoot = root1;
                minRoot = root2;

                labelWithMaxRoot = label1;
                labelWithMinRoot = label2;
            }
            else
            {
                maxRoot = root2;
                minRoot = root1;

                labelWithMaxRoot = label2;
                labelWithMinRoot = label1;
            }

            equals[maxRoot] = minRoot;

            for (int root = maxRoot + 1; root <= labelWithMaxRoot; root++)
            {
                if (equals[root] == maxRoot)
                    equals[root] = minRoot;
            }
        }

        /// <summary>
        ///   Checks if two rectangles are near.
        /// </summary>
        /// 
        private bool near(Rectangle r1, Rectangle r2)
        {
            if (r1.Contains(r2) || r2.Contains(r1))
                return true;

            int minHeight = Math.Min(r1.Height, r2.Height);
            int minWidth = Math.Min(r1.Width, r2.Width);
            double delta = threshold * (minHeight + minWidth);

            return Math.Abs(r1.X - r2.X) <= delta
                && Math.Abs(r1.Y - r2.Y) <= delta
                && Math.Abs(r1.Right - r2.Right) <= delta
                && Math.Abs(r1.Bottom - r2.Bottom) <= delta;
        }
    }
}
