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

namespace Accord.MachineLearning.Bayes
{
    using System;
    using Accord.Math;


    /// <summary>
    ///   Naïve Bayes Classifier
    /// </summary>
    /// 
    /// <seealso cref="NaiveBayes{T}"/>
    /// 
    [Serializable]
    public class NaiveBayes
    {

        private double[,][] probabilities;
        private double[] priors;
        private int[] symbols;
        private int classCount;


        #region Constructors
        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="symbols">The number of symbols for each input variable.</param>
        /// 
        public NaiveBayes(int classes, params int[] symbols)
        {
            if (classes <= 0) throw new ArgumentOutOfRangeException("classes");
            if (symbols == null) throw new ArgumentNullException("symbols");

            initialize(classes, symbols, null);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="classPriors">The prior probabilities for each output class.</param>
        /// <param name="symbols">The number of symbols for each input variable.</param>
        /// 
        public NaiveBayes(int classes, double[] classPriors, params int[] symbols)
        {
            if (classes <= 0) throw new ArgumentOutOfRangeException("classes");
            if (classPriors == null) throw new ArgumentNullException("classPriors");
            if (symbols == null) throw new ArgumentNullException("symbols");

            if (classPriors.Length != classes) throw new DimensionMismatchException("classPriors");

            initialize(classPriors.Length, symbols, classPriors);
        }

        private void initialize(int classes, int[] symbols, double[] priors)
        {
            this.classCount = classes;
            this.symbols = symbols;

            if (priors == null)
            {
                priors = new double[classes];
                for (int i = 0; i < priors.Length; i++)
                    priors[i] = 1.0 / priors.Length;
            }

            this.priors = priors;
            this.probabilities = new double[classes, symbols.Length][];

            for (int i = 0; i < classes; i++)
                for (int j = 0; j < symbols.Length; j++)
                    this.probabilities[i, j] = new double[symbols[j]];
        }
        #endregion


        /// <summary>
        ///   Gets the number of possible output classes.
        /// </summary>
        /// 
        public int ClassCount
        {
            get { return classCount; }
        }

        /// <summary>
        ///   Gets the number of inputs in the model.
        /// </summary>
        /// 
        public int InputCount
        {
            get { return symbols.Length; }
        }

        /// <summary>
        ///   Gets the number of symbols for each input in the model.
        /// </summary>
        /// 
        public int[] SymbolCount
        {
            get { return symbols; }
        }

        /// <summary>
        ///   Gets the tables of log-probabilities for the frequence of
        ///   occurance of each symbol for each class and input.
        /// </summary>
        /// 
        /// <value>A double[,] array in with each row corresponds to a 
        /// class, each column corresponds to an input variable. Each
        /// element of this double[,] array is a frequency table containing
        /// the frequence of each symbol for the corresponding variable as
        /// a double[] array.</value>
        /// 
        public double[,][] Distributions
        {
            get { return probabilities; }
        }

        /// <summary>
        ///   Gets the prior beliefs for each class.
        /// </summary>
        /// 
        public double[] Priors
        {
            get { return priors; }
        }

        /// <summary>
        ///   Initializes the frequency tables of a Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="inputs">The input data.</param>
        /// <param name="outputs">The corresponding output labels for the input data.</param>
        /// <param name="empirical">True to estimate class priors from the data, false otherwise.</param>
        /// <param name="regularization">
        ///   The amount of regularization to be used in the m-estimator. 
        ///   Default is 1e-5.</param>
        /// 
        public double Estimate(int[][] inputs, int[] outputs,
            bool empirical = true, double regularization = 1e-5)
        {
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (outputs == null) throw new ArgumentNullException("outputs");
            if (inputs.Length == 0) throw new ArgumentException("The array has zero length.", "inputs");
            if (outputs.Length != inputs.Length) throw new DimensionMismatchException("outputs");

            // For each class
            for (int i = 0; i < classCount; i++)
            {
                // Estimate conditional distributions

                // Get variables values in class i
                var idx = outputs.Find(y => y == i);
                var values = inputs.Submatrix(idx);

                if (empirical)
                    priors[i] = (double)idx.Length / inputs.Length;



                // For each variable (col)
                for (int j = 0; j < symbols.Length; j++)
                {
                    // Count value occurances and store
                    // frequencies to form probabilities
                    double[] frequencies = new double[symbols[j]];

                    // For each input row (instance)
                    // belonging to the current class
                    for (int k = 0; k < values.Length; k++)
                    {
                        int symbol = values[k][j];
                        frequencies[symbol]++;
                    }

                    // Transform into probabilities
                    for (int k = 0; k < frequencies.Length; k++)
                    {
                        // Use a M-estimator using the previously
                        // available probabilities as priors.
                        double prior = probabilities[i, j][k];
                        double num = frequencies[k] + regularization * prior;
                        double den = values.Length + regularization;

                        probabilities[i, j][k] = num / den;
                    }
                }
            }

            // Compute learning error
            int miss = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (Compute(inputs[i]) != outputs[i])
                    miss++;
            }

            return (double)miss / inputs.Length;
        }

        /// <summary>
        ///   Computes the most likely class for a given instance.
        /// </summary>
        /// 
        /// <param name="input">The input instance.</param>
        /// <returns>The most likely class for the instance.</returns>
        /// 
        public int Compute(int[] input)
        {
            double logLikelihood;
            double[] responses;

            return Compute(input, out logLikelihood, out responses);
        }

        /// <summary>
        ///   Computes the most likely class for a given instance.
        /// </summary>
        /// 
        /// <param name="input">The input instance.</param>
        /// <param name="logLikelihood">The log-likelihood for the instance.</param>
        /// <param name="responses">The response probabilities for each class.</param>
        /// <returns>The most likely class for the instance.</returns>
        /// 
        public int Compute(int[] input, out double logLikelihood, out double[] responses)
        {
            // Select the class argument which
            //   maximizes the log-likelihood:

            responses = new double[ClassCount];

            for (int i = 0; i < responses.Length; i++)
                responses[i] = this.logLikelihood(i, input);

            // Get the class with maximum log-likelihood
            int argmax; logLikelihood = responses.Max(out argmax);

            double evidence = 0;
            for (int i = 0; i < responses.Length; i++)
                evidence += responses[i] = Math.Exp(responses[i]);

            // Transform back into probabilities
            responses.Divide(evidence, inPlace: true);

            return argmax;
        }

        private double logLikelihood(int c, int[] input)
        {
            double p = Math.Log(priors[c]);

            // For each variable
            for (int i = 0; i < input.Length; i++)
            {
                int symbol = input[i];
                p += Math.Log(probabilities[c, i][symbol]);
            }

            return p;
        }
    }
}
