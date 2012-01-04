// Accord Statistics Library
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

namespace Accord.Statistics.Models.Fields.Learning
{
    using System;
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Statistics.Models.Fields.Features;
    using Accord.Statistics.Models.Fields.Functions;

    /// <summary>
    ///   Base class for <see cref="IHiddenConditionalRandomFieldLearning{T}">
    ///   Hidden Conditional Random Fields learning algorithms</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations being modeled.</typeparam>
    /// 
    public abstract class BaseHiddenConditionalRandomFieldLearning<T>
    {
        private HiddenConditionalRandomField<T> model;
        private IPotentialFunction<T> function;

        private double beta = 0;

        private T[][] inputs;
        private int[] outputs;
        private double[][] logLikelihoods;

        private double[] g;
        private double[] lnZx, lnZxy;
        private double error;

        /// <summary>
        ///   Gets or sets the inputs to be used in the next
        ///   call to the Objective or Gradient functions.
        /// </summary>
        /// 
        protected T[][] Inputs
        {
            get { return inputs; }
            set
            {
                if (inputs != value)
                {
                    inputs = value;

                    g = new double[model.Function.Weights.Length];
                    lnZx = new double[model.Function.Weights.Length];
                    lnZxy = new double[model.Function.Weights.Length];
                }
            }
        }

        /// <summary>
        ///   Gets or sets the outputs to be used in the next
        ///   call to the Objective or Gradient functions.
        /// </summary>
        /// 
        protected int[] Outputs
        {
            get { return outputs; }
            set { outputs = value; }
        }

        /// <summary>
        ///   Gets the error computed in the last call
        ///   to the gradient or objective functions.
        /// </summary>
        /// 
        protected double LastError
        {
            get { return error; }
        }

        /// <summary>
        ///   Gets or sets the amount of the parameter weights
        ///   which should be included in the objective function.
        ///   Default is 0 (do not include regularization).
        /// </summary>
        /// 
        public double Regularization
        {
            get { return beta; }
            set { beta = value; }
        }

        /// <summary>
        ///   Gets the model being trained.
        /// </summary>
        /// 
        public HiddenConditionalRandomField<T> Model
        {
            get { return model; }
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="BaseHiddenConditionalRandomFieldLearning&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model to be trained.</param>
        /// 
        protected BaseHiddenConditionalRandomFieldLearning(HiddenConditionalRandomField<T> model)
        {
            this.model = model;
            this.function = model.Function;
        }

        /// <summary>
        ///   Computes the objective (cost) function for the Hidden
        ///   Conditional Random Field (negative log-likelihood).
        /// </summary>
        /// 
        /// <param name="parameters">The parameter vector lambda to use in the model.</param>
        /// <param name="inputs">The inputs to compute the cost function.</param>
        /// <param name="outputs">The respective outputs to compute the cost function.</param>
        /// <returns>The value of the objective function for the given parameters.</returns>
        /// 
        public double Objective(double[] parameters, T[][] inputs, int[] outputs)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;
            return Objective(parameters);
        }

        /// <summary>
        ///   Computes the gradient (vector of derivatives) vector for
        ///   the cost function, which may be used to guide optimization.
        /// </summary>
        /// 
        /// <param name="parameters">The parameter vector lambda to use in the model.</param>
        /// <param name="inputs">The inputs to compute the cost function.</param>
        /// <param name="outputs">The respective outputs to compute the cost function.</param>
        /// <returns>The value of the gradient vector for the given parameters.</returns>
        /// 
        public double[] Gradient(double[] parameters, T[][] inputs, int[] outputs)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;
            return Gradient(parameters);
        }

        /// <summary>
        ///   Computes the objective (cost) function for the Hidden
        ///   Conditional Random Field (negative log-likelihood) using
        ///   the input/outputs stored in this object.
        /// </summary>
        /// 
        /// <param name="parameters">The parameter vector lambda to use in the model.</param>
        /// 
        protected double Objective(double[] parameters)
        {
            model.Function.Weights = parameters;


            // Regularization
            double sumSquaredWeights = 0;
            if (beta != 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                    if (!(Double.IsInfinity(parameters[i]) || Double.IsNaN(parameters[i])))
                        sumSquaredWeights += parameters[i] * parameters[i];
                sumSquaredWeights = sumSquaredWeights * 0.5 * beta;
            }

            double logLikelihood = model.LogLikelihood(inputs, outputs, out logLikelihoods);

#if DEBUG
            if (logLikelihood == 0)
                throw new Exception();

            if (Double.IsNaN(logLikelihood))
                logLikelihood = Double.NegativeInfinity;
#endif

            // Maximize the log-likelihood and minimize
            // the a portion of sum of squared weights
            return -logLikelihood + sumSquaredWeights;
        }


        /// <summary>
        ///   Computes the gradient using the 
        ///   input/outputs stored in this object.
        /// </summary>
        /// 
        /// <param name="parameters">The parameter vector lambda to use in the model.</param>
        /// <returns>The value of the gradient vector for the given parameters.</returns>
        /// 
        protected double[] Gradient(double[] parameters)
        {
            model.Function.Weights = parameters;

            error = 0;

            // The previous call to Objective should have computed
            // the log-likelihoods for all input values. However, if
            // this hasn't been the case, compute them now:

            if (logLikelihoods == null)
                model.LogLikelihood(inputs, outputs, out logLikelihoods);

            // Compute the partition function using the previously
            // computed likelihoods. Also compute the total error

            // For each x, compute lnZ(x) and lnZ(x,y)
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] lli = logLikelihoods[i];

                // Compute the marginal likelihood
                double sum = Double.NegativeInfinity;
                for (int j = 0; j < lli.Length; j++)
                    sum = Special.LogSum(sum, lli[j]);

                lnZx[i] = sum;
                lnZxy[i] = lli[outputs[i]];

                // compute and return the negative
                // log-likelihood as error function
                error -= lnZxy[i] - lnZx[i];
            }

            // Now start computing the gradient w.r.t to the
            // feature functions. Each feature function belongs
            // to a factor potential function, so:

            // For each clique potential (factor potential function)
#if DEBUG
            for (int c = 0; c < function.Factors.Length; c++)
#else
            Parallel.For(0, function.Factors.Length, c =>
#endif
            {
                FactorPotential<T> factor = function.Factors[c];

                // Compute all forward and backward matrices to be
                //  used in the feature functions marginal computations.

                var lnFwds = new double[inputs.Length, function.Outputs][,];
                var lnBwds = new double[inputs.Length, function.Outputs][,];
                for (int i = 0; i < inputs.Length; i++)
                {
                    for (int j = 0; j < function.Outputs; j++)
                    {
                        lnFwds[i, j] = ForwardBackwardAlgorithm.LogForward(factor, inputs[i], j);
                        lnBwds[i, j] = ForwardBackwardAlgorithm.LogBackward(factor, inputs[i], j);
                    }
                }

                double[] marginals = new double[function.Outputs];

                // For each feature in the factor potential function
                int end = factor.ParameterIndex + factor.FeatureCount;
                for (int k = factor.ParameterIndex; k < end; k++)
                {
                    IFeature<T> feature = function.Features[k];
                    double weight = function.Weights[k];

                    if (Double.IsInfinity(weight))
                    {
                        g[k] = 0; continue;
                    }


                    // Compute the two marginal sums for the gradient calculation
                    // as given in eq. 1.52 of Sutton, McCallum; "An introduction to
                    // Conditional Random Fields for Relational Learning". The sums
                    // will be computed in the log domain for numerical stability.

                    double lnsum1 = Double.NegativeInfinity;
                    double lnsum2 = Double.NegativeInfinity;

                    // For each training sample (sequences)
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        T[] x = inputs[i]; // training input
                        int y = outputs[i];  // training output

                        // Compute marginals for all possible outputs
                        for (int j = 0; j < marginals.Length; j++)
                            marginals[j] = feature.LogMarginal(lnFwds[i, j], lnBwds[i, j], x, j);


                        // The first term contains a marginal probability p(w|x,y), which is
                        // exactly a marginal distribution of the clamped CRF (eq. 1.46).
                        lnsum1 = Special.LogSum(lnsum1, marginals[y] - lnZxy[i]);

                        // The second term contains a different marginal p(w,y|x) which is the
                        // same marginal probability required in as fully-observed CRF.
                        for (int j = 0; j < marginals.Length; j++)
                            lnsum2 = Special.LogSum(lnsum2, marginals[j] - lnZx[i]);

#if DEBUG
                        if (Double.IsNaN(lnsum1) || Double.IsNaN(lnsum2))
                            throw new Exception();
#endif
                    }

                    // Compute the current derivative
                    double derivative = -(Math.Exp(lnsum1) - Math.Exp(lnsum2));

#if DEBUG
                    if (Double.IsNaN(derivative))
                        throw new Exception();
#endif

                    // Include regularization derivative if required
                    if (beta != 0) derivative += weight * beta;

                    g[k] = derivative;
                }
            }
#if !DEBUG
);
#endif

            // Reset log-likelihoods so they are recomputed in the next run,
            // either by the Objective function or by the Gradient calculation.

            logLikelihoods = null;

            return g; // return the gradient.
        }

    }
}
