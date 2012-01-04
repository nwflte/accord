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

namespace Accord.Statistics.Models.Fields.Functions
{
    using System;
    using System.Collections.Generic;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Fields.Features;

    /// <summary>
    ///   Potential function modeling Hidden Markov Models.
    /// </summary>
    /// 
    [Serializable]
    public sealed class HiddenMarkovClassifierFunction : BasePotentialFunction<int>, IPotentialFunction<int>
    {


        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="states">The number of states.</param>
        /// <param name="symbols">The number of symbols.</param>
        /// <param name="outputClasses">The number of output classes.</param>
        /// 
        public HiddenMarkovClassifierFunction(int states, int symbols, int outputClasses)
        {
            this.Symbols = symbols;
            this.Outputs = outputClasses;

            var l = new List<double>();
            var f = new List<IFeature<int>>();
            this.Factors = new FactorPotential<int>[Outputs];

            int index = 0;

            // Create features for initial class probabilities
            for (int c = 0; c < outputClasses; c++)
            {
                int parameters = 0;
                l.Add(Math.Log(1.0 / outputClasses));
                f.Add(new OutputFeature<int>(this, c, c));
                parameters++;

                // Create features for initial state probabilities
                for (int i = 0; i < states; i++)
                {
                    if (i == 0) l.Add(Math.Log(1.0));
                    else l.Add(Math.Log(0.0));
                    f.Add(new InitialFeature<int>(this, c, i));
                    parameters++;
                }

                // Create features for state transition probabilities
                for (int i = 0; i < states; i++)
                {
                    for (int j = 0; j < states; j++)
                    {
                        l.Add(Math.Log(1.0 / states));
                        f.Add(new TransitionFeature<int>(this, c, i, j));
                        parameters++;
                    }
                }

                // Create features for symbol emission probabilities
                for (int i = 0; i < states; i++)
                {
                    for (int j = 0; j < symbols; j++)
                    {
                        l.Add(Math.Log(1.0 / symbols));
                        f.Add(new EmissionFeature(this, c, i, j));
                        parameters++;
                    }
                }

                Factors[c] = new FactorPotential<int>(this, index, states, parameters);
                index += parameters;
            }

            this.Features = f.ToArray();
            this.Weights = l.ToArray();
        }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="classifier">The classifier model.</param>
        /// 
        public HiddenMarkovClassifierFunction(HiddenMarkovClassifier classifier)
        {
            this.Symbols = classifier.Symbols;
            this.Outputs = classifier.Classes;

            var l = new List<double>();
            var f = new List<IFeature<int>>();
            this.Factors = new FactorPotential<int>[Outputs];

            int index = 0;

            // Create features for initial class probabilities
            for (int c = 0; c < classifier.Classes; c++)
            {
                int parameters = 0;
                var model = classifier[c];

                // l.Add(Math.Log(classifier.Priors[c]));
                // f.Add(new OutputFeature<int>(this, c, c));
                // parameters++;

                // Create features for initial state probabilities
                for (int i = 0; i < model.States; i++)
                {
                    l.Add(model.Probabilities[i]);
                    f.Add(new InitialFeature<int>(this, c, i));
                    parameters++;
                }

                // Create features for state transition probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int j = 0; j < model.States; j++)
                    {
                        l.Add(model.Transitions[i, j]);
                        f.Add(new TransitionFeature<int>(this, c, i, j));
                        parameters++;
                    }
                }

                // Create features for symbol emission probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int j = 0; j < model.Symbols; j++)
                    {
                        l.Add(model.LogEmissions[i, j]);
                        f.Add(new EmissionFeature(this, c, i, j));
                        parameters++;
                    }
                }

                Factors[c] = new FactorPotential<int>(this, index, model.States, parameters);
                index += parameters;
            }

            this.Weights = l.ToArray();
            this.Features = f.ToArray();
        }



    }
}
