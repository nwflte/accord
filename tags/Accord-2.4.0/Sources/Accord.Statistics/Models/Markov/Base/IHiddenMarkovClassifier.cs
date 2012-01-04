// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//

namespace Accord.Statistics.Models.Markov
{
    using System;

    /// <summary>
    ///   Common interface for sequence classifiers using <see cref="IHiddenMarkovModel">
    ///   hidden Markov models</see>.
    /// </summary>
    public interface IHiddenMarkovClassifier
    {

        /// <summary>
        ///   Computes the most likely class for a given sequence.
        /// </summary>
        /// 
        /// <param name="sequence">The sequence of observations.</param>
        /// <param name="responsibilities">The class responsibilities (or
        /// the probability of the sequence to belong to each class). When
        /// using threshold models, the sum of the probabilities will not
        /// equal one, and the amount left was the threshold probability.
        /// If a threshold model is not being used, the array should sum to
        /// one.</param>
        /// 
        /// <returns>Return the label of the given sequence, or -1 if it has
        /// been rejected.</returns>
        /// 
        int Compute(Array sequence, out double[] responsibilities);

        /// <summary>
        ///   Gets the number of classes which can be recognized by this classifier.
        /// </summary>
        /// 
        int Classes { get; }

    }
}
