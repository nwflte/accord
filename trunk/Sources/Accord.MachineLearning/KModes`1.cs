// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//
// Copyright © Antonino Porcino, 2010
// iz8bly at yahoo.it
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

namespace Accord.MachineLearning
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Accord.Math;

    /// <summary>
    ///   K-Modes algorithm.
    /// </summary>
    /// 
    [Serializable]
    public class KModes<TData>
    {

        internal double[] proportions;
        internal TData[][] centroids;

        private KModesClusterCollection<TData> clusters;
        private Func<TData[], TData[], double> distance;


        /// <summary>
        ///   Gets the clusters found by K-modes.
        /// </summary>
        /// 
        public KModesClusterCollection<TData> Clusters
        {
            get { return clusters; }
        }

        /// <summary>
        ///   Gets the number of clusters.
        /// </summary>
        /// 
        public int K
        {
            get { return clusters.Count; }
        }

        /// <summary>
        ///   Gets the dimensionality of the data space.
        /// </summary>
        /// 
        public int Dimension
        {
            get { return centroids[0].Length; }
        }

        /// <summary>
        ///   Gets or sets the distance function used
        ///   as a distance metric between data points.
        /// </summary>
        /// 
        public Func<TData[], TData[], double> Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        /// <summary>
        ///   Initializes a new instance of KMeans algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>       
        /// <param name="distance">The distance function to use. Default is to
        /// use the <see cref="Accord.Math.Distance.SquareEuclidean(double[], double[])"/> distance.</param>
        /// 
        public KModes(int k, Func<TData[], TData[], double> distance)
        {
            if (k <= 0) throw new ArgumentOutOfRangeException("k");
            if (distance == null) throw new ArgumentNullException("distance");

            this.distance = distance;

            // To store centroids of the clusters
            this.proportions = new double[k];
            this.centroids = new TData[k][];

            // Create the object-oriented structure to hold
            //  information about the k-means' clusters.
            var clusterList = new List<KModesCluster<TData>>(k);
            for (int i = 0; i < k; i++)
                clusterList.Add(new KModesCluster<TData>(this, i));
            this.clusters = new KModesClusterCollection<TData>(this, clusterList);
        }


        /// <summary>
        ///   Randomizes the clusters inside a dataset.
        /// </summary>
        /// 
        /// <param name="data">The data to randomize the algorithm.</param>
        /// 
        public void Randomize(TData[][] data)
        {
            if (data == null) throw new ArgumentNullException("data");

            // pick K unique random indexes in the range 0..n-1
            int[] idx = Accord.Statistics.Tools.RandomSample(data.Length, K);

            // assign centroids from data set
            this.centroids = data.Submatrix(idx).MemberwiseClone();
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>     
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-5.</param>
        /// 
        public int[] Compute(TData[][] data, double threshold = 1e-5)
        {
            // Initial argument checking
            if (data == null)
                throw new ArgumentNullException("data");

            if (threshold < 0)
                throw new ArgumentException("Threshold should be a positive number.", "threshold");


            int k = this.K;
            int rows = data.Length;
            int cols = data[0].Length;


            // Perform a random initialization of the clusters
            // if the algorithm has not been initialized before.
            if (this.centroids[0] == null)
            {
                Randomize(data);
            }


            // Initial variables
            int[] labels = new int[rows];
            List<TData[]>[] clusters;
            TData[][] newCentroids;


            do // Main loop
            {

                // Reset the centroids and the
                //  cluster member counters'
                newCentroids = new TData[k][];
                clusters = new List<TData[]>[k];
                for (int i = 0; i < k; i++)
                    clusters[i] = new List<TData[]>();


                // First we will accumulate the data points
                // into their nearest clusters, storing this
                // information into the newClusters variable.

                // For each point in the data set,
                for (int i = 0; i < data.Length; i++)
                {
                    // Get the point
                    TData[] point = data[i];

                    // Compute the nearest cluster centroid
                    int c = labels[i] = Nearest(data[i]);

                    // Accumulate in the corresponding centroid
                    clusters[c].Add(point);
                }

                // Next we will compute each cluster's new centroid
                //  value by computing the mode in each cluster.

                for (int i = 0; i < k; i++)
                    newCentroids[i] = Accord.Statistics.Tools.Mode<TData[]>(clusters[i].ToArray());


                // The algorithm stops when there is no further change in the
                //  centroids (relative difference is less than the threshold).
                if (converged(centroids, newCentroids, threshold)) break;


                // go to next generation
                centroids = newCentroids;

            }
            while (true);


            // Compute cluster information (optional)
            for (int i = 0; i < k; i++)
            {
                // Compute the proportion of samples in the cluster
                proportions[i] = (double)clusters[i].Count / data.Length;
            }


            // Return the classification result
            return labels;
        }

        /// <summary>
        ///   Divides the input data into K clusters. 
        /// </summary>  
        /// 
        /// <param name="data">The data where to compute the algorithm.</param>
        /// <param name="threshold">The relative convergence threshold
        /// for the algorithm. Default is 1e-5.</param>
        /// 
        /// <param name="error">
        ///   The average distance metric from the
        ///   data points to the clusters' centroids.
        /// </param>
        /// 
        public int[] Compute(TData[][] data, out double error, double threshold = 1e-5)
        {
            // Initial argument checking
            if (data == null) throw new ArgumentNullException("data");

            // Classify the input data
            int[] labels = Compute(data, threshold);

            // Compute the average error
            error = Error(data, labels);

            // Return the classification result
            return labels;
        }

        /// <summary>
        ///   Returns the closest cluster to an input vector.
        /// </summary>
        /// 
        /// <param name="point">The input vector.</param>
        /// <returns>
        ///   The index of the nearest cluster
        ///   to the given data point. </returns>
        ///   
        public int Nearest(TData[] point)
        {
            int min_cluster = 0;
            double min_distance = distance(point, centroids[0]);

            for (int i = 1; i < centroids.Length; i++)
            {
                double dist = distance(point, centroids[i]);
                if (dist < min_distance)
                {
                    min_distance = dist;
                    min_cluster = i;
                }
            }

            return min_cluster;
        }

        /// <summary>
        ///   Returns the closest clusters to an input vector array.
        /// </summary>
        /// 
        /// <param name="points">The input vector array.</param>
        /// 
        /// <returns>
        ///   An array containing the index of the nearest cluster
        ///   to the corresponding point in the input array.</returns>
        ///   
        public int[] Nearest(TData[][] points)
        {
            return points.Apply(p => Nearest(p));
        }

        /// <summary>
        ///   Calculates the average square distance from the data points
        ///   to the clusters' centroids.
        /// </summary>
        /// 
        /// <remarks>
        ///   The average distance from centroids can be used as a measure
        ///   of the "goodness" of the clusterization. The more the data
        ///   are aggregated around the centroids, the less the average
        ///   distance.
        /// </remarks>
        /// 
        /// <returns>
        ///   The average square distance from the data points to the
        ///   clusters' centroids.
        /// </returns>
        /// 
        public double Error(TData[][] data)
        {
            return Error(data, Nearest(data));
        }

        /// <summary>
        ///   Calculates the average square distance from the data points
        ///   to the clusters' centroids.
        /// </summary>
        /// 
        /// <remarks>
        ///   The average distance from centroids can be used as a measure
        ///   of the "goodness" of the clusterization. The more the data
        ///   are aggregated around the centroids, the less the average
        ///   distance.
        /// </remarks>
        /// 
        /// <returns>
        ///   The average square distance from the data points to the
        ///   clusters' centroids.
        /// </returns>
        /// 
        public double Error(TData[][] data, int[] labels)
        {
            double error = 0.0;

            for (int i = 0; i < data.Length; i++)
                error += distance(data[i], centroids[labels[i]]);

            return error / (double)data.Length;
        }

        /// <summary>
        ///   Determines if the algorithm has converged by comparing the
        ///   centroids between two consecutive iterations.
        /// </summary>
        /// 
        /// <param name="centroids">The previous centroids.</param>
        /// <param name="newCentroids">The new centroids.</param>
        /// <param name="threshold">A convergence threshold.</param>
        /// 
        /// <returns>Returns <see langword="true"/> if all centroids had a percentage change
        ///    less than <see param="threshold"/>. Returns <see langword="false"/> otherwise.</returns>
        ///    
        private bool converged(TData[][] centroids, TData[][] newCentroids, double threshold)
        {
            for (int i = 0; i < centroids.Length; i++)
            {
                TData[] centroid = centroids[i];
                TData[] newCentroid = newCentroids[i];

                    if (System.Math.Abs(distance(centroid, newCentroid)) >= threshold)
                        return false;
            }
            return true;
        }

    }

    /// <summary>
    ///   K-modes' Cluster
    /// </summary>
    /// 
    [Serializable]
    public class KModesCluster<TData>
    {
        private KModes<TData> owner;
        private int index;

        /// <summary>
        ///   Gets the label for this cluster.
        /// </summary>
        /// 
        public int Index
        {
            get { return this.index; }
        }

        /// <summary>
        ///   Gets the cluster's centroid.
        /// </summary>
        /// 
        public TData[] Mode
        {
            get { return owner.centroids[index]; }
        }

        /// <summary>
        ///   Gets the proportion of samples in the cluster.
        /// </summary>
        /// 
        public double Proportion
        {
            get { return owner.proportions[index]; }
        }

        internal KModesCluster(KModes<TData> owner, int index)
        {
            this.owner = owner;
            this.index = index;
        }
    }

    /// <summary>
    ///   K-modes Cluster Collection.
    /// </summary>
    /// 
    [Serializable]
    public class KModesClusterCollection<TData> : ReadOnlyCollection<KModesCluster<TData>>
    {

        private KModes<TData> owner;


        /// <summary>
        ///   Gets or sets the clusters' centroids.
        /// </summary>
        /// 
        /// <value>The clusters' centroids.</value>
        /// 
        public TData[][] Centroids
        {
            get { return owner.centroids; }
            set
            {
                if (value == owner.centroids)
                    return;

                if (value == null)
                    throw new ArgumentNullException("value");

                int k = owner.K;

                if (value.Length != k)
                    throw new ArgumentException("The number of centroids should be equal to K.", "value");

                // Make a deep copy of the
                // input centroids vector.
                for (int i = 0; i < k; i++)
                    owner.centroids[i] = (TData[])value[i].Clone();

                // Reset derived information
                owner.proportions = new double[k];
            }
        }

        /// <summary>
        ///   Gets the proportion of samples in each cluster.
        /// </summary>
        /// 
        public double[] Proportions
        {
            get { return owner.proportions; }
        }

        internal KModesClusterCollection(KModes<TData> owner, IList<KModesCluster<TData>> list)
            : base(list)
        {
            this.owner = owner;
        }
    }

    /// <summary>
    ///   K-Modes algorithm.
    /// </summary>
    /// 
    [Serializable]
    public class KModes : KModes<int>
    {

        /// <summary>
        ///   Initializes a new instance of K-Modes algorithm
        /// </summary>
        /// 
        /// <param name="k">The number of clusters to divide input data.</param>    
        /// 
        public KModes(int k) : base(k, Accord.Math.Distance.Manhattan) { }
    }
}
