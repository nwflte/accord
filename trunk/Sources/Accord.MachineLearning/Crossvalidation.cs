// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
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

namespace Accord.MachineLearning
{
    using System;

    /// <summary>
    ///   k-Fold Cross-Validation.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Cross-validation is a technique for estimating the performance of a predictive
    ///   model. It can be used to measure how the results of a statistical analysis will
    ///   generalize to an independent data set. It is mainly used in settings where the
    ///   goal is prediction, and one wants to estimate how accurately a predictive model
    ///   will perform in practice.</para>
    /// <para>
    ///   One round of cross-validation involves partitioning a sample of data into
    ///   complementary subsets, performing the analysis on one subset (called the
    ///   training set), and validating the analysis on the other subset (called the
    ///   validation set or testing set). To reduce variability, multiple rounds of 
    ///   cross-validation are performed using different partitions, and the validation 
    ///   results are averaged over the rounds.</para> 
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Cross-validation_(statistics)">
    ///       Wikipedia, The Free Encyclopedia. Cross-validation (statistics). Available on:
    ///       http://en.wikipedia.org/wiki/Cross-validation_(statistics) </a></description></item>
    ///   </list></para> 
    /// </remarks>
    /// 
    /// <example>
    ///   <code>
    ///   //Example binary data
    ///   double[][] data =
    ///   {
    ///        new double[] { -1, -1 }, new double[] {  1, -1 },
    ///        new double[] { -1,  1 }, new double[] {  1,  1 },
    ///        new double[] { -1, -1 }, new double[] {  1, -1 },
    ///        new double[] { -1,  1 }, new double[] {  1,  1 },
    ///        new double[] { -1, -1 }, new double[] {  1, -1 },
    ///        new double[] { -1,  1 }, new double[] {  1,  1 },
    ///        new double[] { -1, -1 }, new double[] {  1, -1 },
    ///        new double[] { -1,  1 }, new double[] {  1,  1 },
    ///    };
    ///
    ///    int[] xor = // result of xor
    ///    {
    ///        -1,  1,
    ///         1, -1,
    ///        -1,  1,
    ///         1, -1,
    ///        -1,  1,
    ///         1, -1,
    ///        -1,  1,
    ///         1, -1,
    ///    };
    ///
    ///
    ///    // Create a new Cross-validation algorithm passing the data set size and the number of folds
    ///    var crossvalidation = new Crossvalidation&lt;KernelSupportVectorMachine>(data.Length, 3);
    ///
    ///    // Define a fitting function using Support Vector Machines
    ///    crossvalidation.Fitting = delegate(int k, int[] trainingSet, int[] validationSet)
    ///    {
    ///        // The trainingSet and validationSet arguments specifies the
    ///        // indices of the original data set to be used as training and
    ///        // validation sets, respectively.
    ///        double[][] trainingInputs = data.Submatrix(trainingSet);
    ///        int[] trainingOutputs = xor.Submatrix(trainingSet);
    ///
    ///        double[][] validationInputs = data.Submatrix(validationSet);
    ///        int[] validationOutputs = xor.Submatrix(validationSet);
    ///
    ///        // Create a Kernel Support Vector Machine to operate on this set
    ///        var svm = new KernelSupportVectorMachine(new Polynomial(2), 2);
    ///
    ///        // Create a training algorithm and learn this set
    ///        var smo = new SequentialMinimalOptimization(svm, trainingInputs, trainingOutputs);
    ///
    ///        double trainingError = smo.Run();
    ///        double validationError = smo.ComputeError(validationInputs, validationOutputs);
    ///
    ///        // Return a new information structure containing the model and the errors achieved.
    ///        return new CrossvalidationInfo&lt;KernelSupportVectorMachine>(svm, trainingError, validationError);
    ///    };
    ///
    /// 
    ///    // Compute the cross-validation
    ///    crossvalidation.Compute();
    ///
    ///    // Get the average training and validation errors
    ///    double errorTraining   = crossvalidation.TrainingError;
    ///    double errorValidation = crossvalidation.ValidationError;
    ///   </code>
    /// </example>
    /// 
    [Serializable]
    public class CrossValidation : CrossValidation<object>
    {
        /// <summary>
        ///   Creates a new k-fold cross-validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The complete dataset for training and testing.</param>
        /// 
        public CrossValidation(int size) : base(size) { }

        /// <summary>
        ///   Creates a new k-fold cross-validation algorithm.
        /// </summary>
        /// 
        /// <param name="size">The complete dataset for training and testing.</param>
        /// <param name="folds">The number of folds, usually denoted as <c>k</c> (default is 10).</param>
        /// 
        public CrossValidation(int size, int folds) : base(size, folds) { }

        /// <summary>
        ///   Creates a new k-fold cross-validation algorithm.
        /// </summary>
        /// 
        /// <param name="indices">An already created set of fold indices for each sample in a dataset.</param>
        /// <param name="folds">The total number of folds referenced in the <paramref name="indices"/> param.</param>
        /// 
        public CrossValidation(int[] indices, int folds) : base(indices, folds) { }

        /// <summary>
        ///   Create cross-validation folds by generating
        ///   a vector of random fold indices.
        /// </summary>
        /// 
        /// <param name="size">The number of points in the data set.</param>
        /// <param name="folds">The number of folds in the cross-validation.</param>
        /// 
        /// <returns>A vector of indices defining the a fold for each point in the data set.</returns>
        /// 
        public static int[] Splittings(int size, int folds)
        {
            // Create the index vector
            int[] idx = new int[size];

            float n = (float)folds / size;
            for (int i = 0; i < size; i++)
                idx[i] = (int)System.Math.Ceiling((i + 0.9) * n) - 1;

            // Shuffle the indices vector
            Statistics.Tools.Shuffle(idx);

            return idx;
        }

    }
}
