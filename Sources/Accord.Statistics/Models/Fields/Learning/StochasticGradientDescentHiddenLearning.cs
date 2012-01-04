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

    /// <summary>
    ///   Gradient Descent learning algorithm for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations.</typeparam>
    /// 
    public class GradientDescentHiddenLearning<T> : BaseHiddenConditionalRandomFieldLearning<T>, IHiddenConditionalRandomFieldLearning<T>
    {
        private double learningRate = 0.1;
        private int iterations = 0;

        private T[][] aObservations;
        private int[] aOutput;

        /// <summary>
        ///   Gets or sets the learning rate to use as the gradient
        ///   descent step size. Default value is 1e-1.
        ///   
        /// </summary>
        public double LearningRate
        {
            get { return learningRate; }
            set { learningRate = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GradientDescentHiddenLearning&lt;T&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="model">The model to be trained.</param>
        /// 
        public GradientDescentHiddenLearning(HiddenConditionalRandomField<T> model)
            : base(model)
        {
            aObservations = new T[1][];
            aOutput = new int[1];
        }

        /// <summary>
        ///   Resets the step size.
        /// </summary>
        /// 
        public void Reset()
        {
            iterations = 0;
        }

        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observation's labels.</param>
        /// 
        /// <returns>The error in the last iteration.</returns>
        /// 
        public double RunEpoch(T[][] observations, int[] outputs)
        {
            iterations++;

            double error = 0;

            // In batch mode, we will use the average of the gradients
            // at each point as a better estimate of the true gradient.
            double[] average = new double[Model.Function.Weights.Length];

            // For each training point
            for (int i = 0; i < observations.Length; i++)
            {
                aObservations[0] = observations[i];
                aOutput[0] = outputs[i];

                // Compute the gradient
                double[] g = base.Gradient(Model.Function.Weights, aObservations, aOutput);

                // Accumulate
                for (int j = 0; j < average.Length; j++)
                    average[i] += g[i];

                error += LastError;
#if DEBUG
                for (int j = 0; j < average.Length; j++)
                    if (Double.IsNaN(average[i]))
                        throw new Exception();
#endif
            }

            // Compute the average gradient
            for (int i = 0; i < average.Length; i++)
                average[i] /= observations.Length;

            // Update the model using a dynamic step size
            for (int i = 0; i < Model.Function.Weights.Length; i++)
                Model.Function.Weights[i] -= (learningRate / iterations) * average[i];

#if DEBUG
            for (int j = 0; j < Model.Function.Weights.Length; j++)
                if (Double.IsNaN(Model.Function.Weights[j]))
                    throw new Exception();
#endif

            return error;
        }


        /// <summary>
        ///   Runs one iteration of the learning algorithm with the
        ///   specified input training observation and corresponding
        ///   output label.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="output">The observation's labels.</param>
        /// 
        /// <returns>The error in the last iteration.</returns>
        /// 
        public double Run(T[] observations, int output)
        {
            aObservations[0] = observations;
            aOutput[0] = output;

            double[] g = base.Gradient(Model.Function.Weights, aObservations, aOutput);

            for (int i = 0; i < Model.Function.Weights.Length; i++)
                Model.Function.Weights[i] -= (learningRate / iterations) * g[i];

            return LastError;
        }

    }
}
