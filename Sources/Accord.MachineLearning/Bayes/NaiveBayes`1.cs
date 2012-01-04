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
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;


    /// <summary>
    ///   Naïve Bayes Classifier
    /// </summary>
    /// 
    /// <seealso cref="NaiveBayes"/>
    /// 
    [Serializable]
    public class NaiveBayes<TDistribution> where TDistribution : IUnivariateDistribution
    {

        private TDistribution[,] probabilities;
        private double[] priors;
        private int classCount;
        private int inputCount;

        #region Constructors
        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="prior">
        ///   A value probability prior to be used in the estimation
        ///   of each class-variable relationship. This value will be
        ///   replicated for each entry in the <see cref="Distributions"/>
        ///   property.</param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution prior)
        {
            if (classes <= 0) throw new ArgumentOutOfRangeException("classes");
            if (inputs <= 0) throw new ArgumentOutOfRangeException("inputs");
            if (prior == null) throw new ArgumentNullException("prior");

            TDistribution[,] priors = new TDistribution[classes, inputs];
            for (int i = 0; i < classes; i++)
                for (int j = 0; j < inputs; j++)
                    priors[i, j] = (TDistribution)prior.Clone();

            initialize(priors, null);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="prior">
        ///   A value probability prior to be used in the estimation
        ///   of each class-variable relationship. This value will be
        ///   replicated for each entry in the <see cref="Distributions"/>
        ///   property.</param>
        /// <param name="classPriors">The prior probabilities for each output class.</param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution prior, double[] classPriors)
        {
            if (classes <= 0) throw new ArgumentOutOfRangeException("classes");
            if (inputs <= 0) throw new ArgumentOutOfRangeException("inputs");
            if (classPriors == null) throw new ArgumentNullException("classPriors");
            if (classPriors.Length != classes) throw new DimensionMismatchException("classPriors");

            TDistribution[,] priors = new TDistribution[classPriors.Length, inputs];
            for (int i = 0; i < classPriors.Length; i++)
                for (int j = 0; j < inputs; j++)
                    priors[i, j] = (TDistribution)prior.Clone();

            initialize(priors, classPriors);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="inputPriors">
        ///   A value probability prior for each input variable, to be used
        ///   in the estimation of each class-variable relationship. This value
        ///   will be replicated for each class in the <see cref="Distributions"/>
        ///   property.</param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution[] inputPriors)
        {
            if (classes <= 0) throw new ArgumentOutOfRangeException("classes");
            if (inputs <= 0) throw new ArgumentOutOfRangeException("inputs");
            if (inputPriors == null) throw new ArgumentNullException("inputPriors");
            if (inputPriors.Length != inputs) throw new DimensionMismatchException("inputPriors");

            TDistribution[,] priors = new TDistribution[classes, inputPriors.Length];
            for (int i = 0; i < classes; i++)
                for (int j = 0; j < inputPriors.Length; j++)
                    priors[i, j] = (TDistribution)inputPriors[i].Clone();

            initialize(priors, null);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="priors">
        ///   A value probability prior for each class and input variables, to
        ///   be used in the estimation of each class-variable relationship.</param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution[,] priors)
        {
            if (classes <= 0) throw new ArgumentOutOfRangeException("classes");
            if (inputs <= 0) throw new ArgumentOutOfRangeException("inputs");

            if (priors.GetLength(0) != classes)
                throw new DimensionMismatchException("priors");

            if (priors.GetLength(1) != inputs)
                throw new DimensionMismatchException("priors");

            initialize(priors, null);
        }

        /// <summary>
        ///   Constructs a new Naïve Bayes Classifier.
        /// </summary>
        /// 
        /// <param name="classes">The number of output classes.</param>
        /// <param name="inputs">The number of input variables.</param>
        /// <param name="priors">
        ///   A value probability prior for each class and input variables, to
        ///   be used in the estimation of each class-variable relationship.</param>
        /// <param name="classPriors">The prior probabilities for each output class.</param>
        /// 
        public NaiveBayes(int classes, int inputs, TDistribution[,] priors, double[] classPriors)
        {
            if (classes <= 0) throw new ArgumentOutOfRangeException("classes");
            if (priors == null) throw new ArgumentNullException("priors");
            if (priors.Length != classes) throw new DimensionMismatchException("priors");
            if (classPriors.Length != classes) throw new DimensionMismatchException("classPriors");

            if (priors.GetLength(0) != classes)
                throw new DimensionMismatchException("priors");

            if (priors.GetLength(1) != inputs)
                throw new DimensionMismatchException("priors");

            initialize(priors, classPriors);
        }

        private void initialize(TDistribution[,] parameterPriors, double[] classPriors)
        {
            this.classCount = parameterPriors.GetLength(0);
            this.inputCount = parameterPriors.GetLength(1);
            this.priors = classPriors;
            this.probabilities = parameterPriors;

            if (priors == null)
            {
                priors = new double[classCount];
                for (int i = 0; i < priors.Length; i++)
                    priors[i] = 1.0 / priors.Length;
            }
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
            get { return inputCount; }
        }

        /// <summary>
        ///   Gets the probability distributions for each class and input.
        /// </summary>
        /// 
        /// <value>A TDistribution[,] array in with each row corresponds to a 
        /// class, each column corresponds to an input variable. Each element
        /// of this double[,] array is a a probability distribution modelling
        /// the occurance of the input variable in the corresponding class.</value>
        /// 
        public TDistribution[,] Distributions
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
        /// <param name="options">The fitting options to be used in the density estimation.</param>
        /// 
        public double Estimate(double[][] inputs, int[] outputs,
            bool empirical = true, IFittingOptions options = null)
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
                var values = inputs.Submatrix(idx, transpose: true);

                if (empirical)
                    priors[i] = (double)idx.Length / inputs.Length;

                // For each variable (col)
                for (int j = 0; j < inputCount; j++)
                    probabilities[i, j].Fit(values[j], options);
            }

            // Compute learning error
            return Error(inputs, outputs);
        }

        /// <summary>
        ///   Computes the error when predicting the given data.
        /// </summary>
        /// 
        /// <param name="inputs">The input values.</param>
        /// <param name="outputs">The output values.</param>
        /// 
        /// <returns>The percentual error of the prediction.</returns>
        /// 
        public double Error(double[][] inputs, int[] outputs)
        {
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
        public int Compute(double[] input)
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
        public int Compute(double[] input, out double logLikelihood, out double[] responses)
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

        private double logLikelihood(int c, double[] input)
        {
            double p = Math.Log(priors[c]);

            // For each variable
            for (int i = 0; i < input.Length; i++)
                p += probabilities[c, i].LogProbabilityFunction(input[i]);

            return p;
        }
    }
}
