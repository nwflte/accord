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
    using Accord.Math.Optimization;

    /// <summary>
    ///   Conjugate Gradient learning algorithm for <see cref="HiddenConditionalRandomField{T}">
    ///   Hidden Conditional Hidden Fields</see>.
    /// </summary>
    /// 
    public class ConjugateGradientHiddenLearning<T> : BaseHiddenConditionalRandomFieldLearning<T>, IHiddenConditionalRandomFieldLearning<T>
    {

        private ConjugateGradient cg;

        /// <summary>
        ///   Gets or sets the tolerance threshold to detect convergence
        ///   of the log-likelihood function between two iterations. The
        ///   default value is 1e-3.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return cg.Tolerance; }
            set { cg.Tolerance = value; }
        }

        /// <summary>
        ///   Constructs a new Conjugate Gradient learning algorithm.
        /// </summary>
        /// 
        public ConjugateGradientHiddenLearning(HiddenConditionalRandomField<T> model)
            : base(model)
        {
            cg = new ConjugateGradient(model.Function.Weights.Length);
            cg.Tolerance = 1e-3;
            cg.Function = base.Objective;
            cg.Gradient = base.Gradient;
        }

        /// <summary>
        ///   Runs the learning algorithm with the specified input
        ///   training observations and corresponding output labels.
        /// </summary>
        /// 
        /// <param name="observations">The training observations.</param>
        /// <param name="outputs">The observation's labels.</param>
        /// 
        public double RunEpoch(T[][] observations, int[] outputs)
        {
            this.Inputs = observations;
            this.Outputs = outputs;

            try
            {
                cg.Minimize(Model.Function.Weights);
            }
            catch (LineSearchFailedException)
            {
                // TODO: Restructure CG to avoid exceptions.
            }

            Model.Function.Weights = cg.Solution;

            return Model.LogLikelihood(observations, outputs);
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
            throw new NotSupportedException();
        }

    }
}
