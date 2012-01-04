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
    using Accord.Statistics.Models.Fields.Features;
    using Accord.Statistics.Models.Markov;

    /// <summary>
    ///   Potential function modeling Hidden Markov Models.
    /// </summary>
    /// 
    [Serializable]
    public sealed class HiddenMarkovModelFunction : BasePotentialFunction<int>, IPotentialFunction<int>
    {


        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="states">The number of states.</param>
        /// <param name="symbols">The number of symbols.</param>
        /// 
        public HiddenMarkovModelFunction(int states, int symbols)
        {
            this.Symbols = symbols;

            var l = new List<double>();
            var f = new List<IFeature<int>>();


            // Create features for initial state probabilities
            for (int i = 0; i < states; i++)
            {
                l.Add(0);
                f.Add(new InitialFeature<int>(this, 0, i));
            }

            // Create features for state transition probabilities
            for (int i = 0; i < states; i++)
            {
                for (int j = 0; j < states; j++)
                {
                    l.Add(0);
                    f.Add(new TransitionFeature<int>(this, 0, i, j));
                }
            }

            // Create features for symbol emission probabilities
            for (int i = 0; i < states; i++)
            {
                for (int j = 0; j < symbols; j++)
                {
                    l.Add(0);
                    f.Add(new EmissionFeature(this, 0, i, j));
                }
            }

            this.Factors = new[] { new FactorPotential<int>(this, 0, states, f.Count) };

            this.Features = f.ToArray();
            this.Weights = l.ToArray();
        }

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="model">The hidden Markov model.</param>
        /// 
        public HiddenMarkovModelFunction(HiddenMarkovModel model)
        {
            int States = model.States;
            this.Symbols = model.Symbols;

            var l = new List<double>();
            var f = new List<IFeature<int>>();

            // Create features for initial state probabilities
            for (int i = 0; i < States; i++)
            {
                l.Add(model.Probabilities[i]);
                f.Add(new InitialFeature<int>(this, 0, i));
            }

            // Create features for state transition probabilities
            for (int i = 0; i < States; i++)
            {
                for (int j = 0; j < States; j++)
                {
                    l.Add(model.Transitions[i, j]);
                    f.Add(new TransitionFeature<int>(this, 0, i, j));
                }
            }

            // Create features for symbol emission probabilities
            for (int i = 0; i < States; i++)
            {
                for (int j = 0; j < Symbols; j++)
                {
                    l.Add(model.LogEmissions[i, j]);
                    f.Add(new EmissionFeature(this, 0, i, j));
                }
            }

            
            this.Factors = new[] { new FactorPotential<int>(this, 0, States, f.Count ) };
            this.Weights = l.ToArray();
            this.Features = f.ToArray();
        }



    }
}
