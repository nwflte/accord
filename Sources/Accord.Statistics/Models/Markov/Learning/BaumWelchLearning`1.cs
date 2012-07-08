// Accord Statistics Library
// The Accord.NET Framework
// http://accord.googlecode.com
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

namespace Accord.Statistics.Models.Markov.Learning
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Baum-Welch learning algorithm for arbitrary-density Hidden Markov Models.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   The Baum-Welch algorithm is a kind of Expectation-Maximization algorithm.
    ///   For continuous models, it estimates the matrix of state transition probabilities
    ///   A and the vector of initial state probabilities pi. For the state emission 
    ///   densities, it weights each observation and lets the estimation algorithms
    ///   for each of the densities to fit the distributions to the observations.</para>
    /// </remarks>
    /// 
    /// <example>
    ///   In the following example, we will create a Continuous Hidden Markov Model using
    ///   a univariate Normal distribution to model properly model continuous sequences.
    ///   <code>
    ///   // Create continuous sequences. In the sequences below, there
    ///   //  seems to be two states, one for values between 0 and 1 and
    ///   //  another for values between 5 and 7. The states seems to be
    ///   //  switched on every observation.
    ///   double[][] sequences = new double[][] 
    ///   {
    ///       new double[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 },
    ///       new double[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 },
    ///       new double[] { 0.1, 7.0, 0.1, 7.0, 0.2, 5.6 },
    ///   };
    /// 
    ///             
    ///   // Specify a initial normal distribution for the samples.
    ///   NormalDistribution density = NormalDistribution();
    /// 
    ///   // Creates a continuous hidden Markov Model with two states organized in a forward
    ///   //  topology and an underlying univariate Normal distribution as probability density.
    ///   var model = new HiddenMarkovModel&lt;NormalDistribution&gt;(new Ergodic(2), density);
    /// 
    ///   // Configure the learning algorithms to train the sequence classifier until the
    ///   // difference in the average log-likelihood changes only by as little as 0.0001
    ///   var teacher = new BaumWelchLearning&lt;NormalDistribution&gt;(model)
    ///   {
    ///       Tolerance = 0.0001,
    ///       Iterations = 0,
    ///   };
    /// 
    ///   // Fit the model
    ///   double likelihood = teacher.Run(sequences);
    /// 
    ///   // See the probability of the sequences learned
    ///   double l1 = model.Evaluate(new[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 }); // 0.87
    ///   double l2 = model.Evaluate(new[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 }); // 1.00
    /// 
    ///   // See the probability of an unrelated sequence
    ///   double l3 = model.Evaluate(new[] { 1.1, 2.2, 1.3, 3.2, 4.2, 1.0 }); // 0.00
    /// </code>
    /// </example>
    /// 
    /// <example>
    ///   In the following example, we will create a Discrete Hidden Markov Model
    ///   using a Generic Discrete Probability Distribution to reproduce the same
    ///   code example given in <seealso cref="BaumWelchLearning"/> documentation.
    ///   <code>
    ///   // Arbitrary-density Markov Models can operate using any
    ///   // probability distribution, including discrete ones. 
    ///   
    ///   // In the follwing example, we will try to create a
    ///   // Discrete Hidden Markov Model using a discrete
    ///   // distribution to detect if a given sequence starts
    ///   // with a zero and has any number of ones after that.
    ///   
    ///   double[][] sequences = new double[][] 
    ///   {
    ///       new double[] { 0,1,1,1,1,0,1,1,1,1 },
    ///       new double[] { 0,1,1,1,0,1,1,1,1,1 },
    ///       new double[] { 0,1,1,1,1,1,1,1,1,1 },
    ///       new double[] { 0,1,1,1,1,1         },
    ///       new double[] { 0,1,1,1,1,1,1       },
    ///       new double[] { 0,1,1,1,1,1,1,1,1,1 },
    ///       new double[] { 0,1,1,1,1,1,1,1,1,1 },
    ///   };
    ///   
    ///   // Create a new Hidden Markov Model with 3 states and
    ///   //  a generic discrete distribution with two symbols
    ///   var hmm = new HiddenMarkovModel.CreateGeneric(3, 2);
    ///   
    ///   // We will try to fit the model to the data until the difference in
    ///   //  the average log-likelihood changes only by as little as 0.0001
    ///   var teacher = new BaumWelchLearning&lt;UniformDiscreteDistribution&gt;(hmm)
    ///   { 
    ///       Tolerance = 0.0001,
    ///       Iterations = 0 
    ///   };
    ///   
    ///   // Begin model training
    ///   double ll = teacher.Run(sequences);
    ///   
    /// 
    ///   // Calculate the probability that the given
    ///   //  sequences originated from the model
    ///   double l1 = hmm.Evaluate(new double[] { 0, 1 });       // 0.999
    ///   double l2 = hmm.Evaluate(new double[] { 0, 1, 1, 1 }); // 0.916
    ///   
    ///   // Sequences which do not start with zero have much lesser probability.
    ///   double l3 = hmm.Evaluate(new double[] { 1, 1 });       // 0.000
    ///   double l4 = hmm.Evaluate(new double[] { 1, 0, 0, 0 }); // 0.000
    ///   
    ///   // Sequences which contains few errors have higher probabability
    ///   //  than the ones which do not start with zero. This shows some
    ///   //  of the temporal elasticity and error tolerance of the HMMs.
    ///   double l5 = hmm.Evaluate(new double[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }); // 0.034
    ///   double l6 = hmm.Evaluate(new double[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }); // 0.034
    ///   </code>
    /// </example>
    /// 
    public class BaumWelchLearning<TDistribution> : BaseBaumWelchLearning, IUnsupervisedLearning
        where TDistribution : IDistribution
    {

        private HiddenMarkovModel<TDistribution> model;

        private IFittingOptions fittingOptions;

        private double[][][] vectorObservations;
        private Array samples;
        private double[] weights;

        /// <summary>
        ///   Gets the model being trained.
        /// </summary>
        /// 
        public HiddenMarkovModel<TDistribution> Model
        {
            get { return model; }
        }

        /// <summary>
        ///   Gets or sets the distribution fitting options
        ///   to use when estimating distribution densities
        ///   during learning.
        /// </summary>
        /// <value>The distribution fitting options.</value>
        /// 
        public IFittingOptions FittingOptions
        {
            get { return fittingOptions; }
            set { fittingOptions = value; }
        }

        /// <summary>
        ///   Creates a new instance of the Baum-Welch learning algorithm.
        /// </summary>
        /// 
        public BaumWelchLearning(HiddenMarkovModel<TDistribution> model)
            : base(model)
        {
            this.model = model;
        }


        /// <summary>
        ///   Runs the Baum-Welch learning algorithm for hidden Markov models.
        /// </summary>
        /// <remarks>
        ///   Learning problem. Given some training observation sequences O = {o1, o2, ..., oK}
        ///   and general structure of HMM (numbers of hidden and visible states), determine
        ///   HMM parameters M = (A, B, pi) that best fit training data. 
        /// </remarks>
        /// <param name="observations">
        ///   The sequences of univariate or multivariate observations used to train the model.
        ///   Can be either of type double[] (for the univariate case) or double[][] for the
        ///   multivariate case.
        /// </param>
        /// <returns>
        ///   The average log-likelihood for the observations after the model has been trained.
        /// </returns>
        /// 
        public new double Run(params Array[] observations)
        {

            // Convert the generic representation to a vector of multivariate sequences
            this.vectorObservations = new double[observations.Length][][];
            for (int i = 0; i < vectorObservations.Length; i++)
                this.vectorObservations[i] = convert(observations[i], model.Dimension);


            // Sample array, used to store all observations as a sample vector
            //   will be useful when fitting the distribution models.
            if (model.Emissions[0] is IUnivariateDistribution)
            {
                this.samples = (Array)Accord.Math.Matrix.Concatenate(
                    Accord.Math.Matrix.Stack(vectorObservations));
            }
            else
            {
                this.samples = (Array)Accord.Math.Matrix.Stack(vectorObservations);
            }

            this.weights = new double[samples.Length];


            return base.Run(observations);
        }

        /// <summary>
        ///   Computes the ksi matrix of probabilities for a given observation
        ///   referenced by its index in the input training data.
        /// </summary>
        /// <param name="index">The index of the observation in the input training data.</param>
        /// <param name="lnFwd">The matrix of forward probabilities for the observation.</param>
        /// <param name="lnBwd">The matrix of backward probabilities for the observation.</param>
        /// 
        protected override void ComputeKsi(int index, double[,] lnFwd, double[,] lnBwd)
        {
            int states = model.States;
            double[,] logA = model.Transitions;
            TDistribution[] B = model.Emissions;

            double[][] sequence = vectorObservations[index];

            int T = sequence.Length;
            var logKsi = LogKsi[index];


            for (int t = 0; t < T - 1; t++)
            {
                double lnsum = Double.NegativeInfinity;
                double[] x = sequence[t + 1];

                for (int i = 0; i < states; i++)
                {
                    for (int j = 0; j < states; j++)
                    {
                        logKsi[t][i, j] = lnFwd[t, i] + lnBwd[t + 1, j] + logA[i, j] + B[j].LogProbabilityFunction(x);
                        lnsum = Special.LogSum(lnsum, logKsi[t][i, j]);
                    }
                }

                // Normalize if different from zero
                if (lnsum != Double.NegativeInfinity)
                    for (int i = 0; i < states; i++)
                        for (int j = 0; j < states; j++)
                            logKsi[t][i, j] = logKsi[t][i, j] - lnsum;
            }
        }

        /// <summary>
        ///   Updates the emission probability matrix.
        /// </summary>
        /// <remarks>
        ///   Implementations of this method should use the observations
        ///   in the training data and the Gamma probability matrix to
        ///   update the probability distributions of symbol emissions.
        /// </remarks>
        /// 
        protected override void UpdateEmissions()
        {
            var B = model.Emissions;

            // For each state i in the model
            for (int i = 0; i < B.Length; i++)
            {
                double lnsum = Double.NegativeInfinity;

                // For each observation sequence k
                for (int k = 0, w = 0; k < vectorObservations.Length; k++)
                {
                    int T = vectorObservations[k].Length;

                    // For each observation t in k
                    for (int t = 0; t < T; t++, w++)
                    {
                        weights[w] = LogGamma[k][t, i];
                        lnsum = Special.LogSum(lnsum, weights[w]);
                    }
                }

                // Normalize if different from zero
                if (lnsum != Double.NegativeInfinity)
                    for (int w = 0; w < weights.Length; w++)
                        weights[w] = weights[w] - lnsum;

                // Convert to probabilities
                for (int w = 0; w < weights.Length; w++)
                    weights[w] = Math.Exp(weights[w]);

                // Estimate the distribution for state i
                B[i].Fit(samples, weights, fittingOptions);
            }
        }

        /// <summary>
        ///   Computes the forward and backward probabilities matrices
        ///   for a given observation referenced by its index in the
        ///   input training data.
        /// </summary>
        /// <param name="index">The index of the observation in the input training data.</param>
        /// <param name="lnFwd">Returns the computed forward probabilities matrix.</param>
        /// <param name="lnBwd">Returns the computed backward probabilities matrix.</param>
        /// 
        protected override void ComputeForwardBackward(int index, double[,] lnFwd, double[,] lnBwd)
        {
            int states = model.States;
            int T = vectorObservations[index].Length;

            System.Diagnostics.Debug.Assert(lnBwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(lnBwd.GetLength(1) == states);
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(0) >= T);
            System.Diagnostics.Debug.Assert(lnFwd.GetLength(1) == states);

            ForwardBackwardAlgorithm.LogForward(model, vectorObservations[index], lnFwd);
            ForwardBackwardAlgorithm.LogBackward(model, vectorObservations[index], lnBwd);
        }


        /// <summary>
        ///   Converts a univariate or multivariate array
        ///   of observations into a two-dimensional jagged array.
        /// </summary>
        /// 
        private static double[][] convert(Array array, int dimension)
        {
            double[][] multivariate = array as double[][];
            if (multivariate != null) return multivariate;

            double[] univariate = array as double[];
            if (univariate != null) return Accord.Math.Matrix.Split(univariate, dimension);

            throw new ArgumentException("Invalid array argument type.", "array");
        }
    }
}
