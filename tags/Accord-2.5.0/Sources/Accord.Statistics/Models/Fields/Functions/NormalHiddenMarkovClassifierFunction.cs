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
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Potential function modeling <see cref="HiddenMarkovClassifier">Hidden Markov Classifiers</see>.
    /// </summary>
    /// 
    [Serializable]
    public sealed class NormalHiddenMarkovClassifierFunction
        : BasePotentialFunction<double>, IPotentialFunction<double>
    {

        /// <summary>
        ///   Constructs a new potential function modeling Hidden Markov Models.
        /// </summary>
        /// 
        /// <param name="classifier">A hidden Markov sequence classifier.</param>
        /// 
        public NormalHiddenMarkovClassifierFunction(HiddenMarkovClassifier<NormalDistribution> classifier)
        {
            this.Outputs = classifier.Classes;

            var l = new List<double>();
            var f = new List<IFeature<double>>();
            this.Factors = new FactorPotential<double>[Outputs];

            int index = 0;

            // Create features for initial class probabilities
            for (int c = 0; c < classifier.Classes; c++)
            {
                int parameters = 0;
                var model = classifier[c];

                //l.Add(Math.Log(classifier.Priors[c]));
                //f.Add(new OutputFeature<double>(this, c, c));
                //parameters++;

                // Create features for initial state probabilities
                for (int i = 0; i < model.States; i++)
                {
                    l.Add(model.Probabilities[i]);
                    f.Add(new InitialFeature<double>(this, c, i));
                    parameters++;
                }

                // Create features for state transition probabilities
                for (int i = 0; i < model.States; i++)
                {
                    for (int j = 0; j < model.States; j++)
                    {
                        l.Add(model.Transitions[i, j]);
                        f.Add(new TransitionFeature<double>(this, c, i, j));
                        parameters++;
                    }
                }

                // Create features emission probabilities
                for (int i = 0; i < model.States; i++)
                {
                    double mean = model.Emissions[i].Mean;
                    double var = model.Emissions[i].Variance;

                    // Occupancy
                    l.Add(-0.5 * (Math.Log(2.0 * Math.PI * var) + (mean * mean) / var));
                    f.Add(new OccupancyFeature<double>(this, c, i));
                    parameters++;

                    // 1st Moment (x)
                    l.Add(mean / var);
                    f.Add(new FirstMomentFeature(this, c, i));
                    parameters++;

                    // 2nd Moment (x²)
                    l.Add(-1.0 / (2.0 * var));
                    f.Add(new SecondMomentFeature(this, c, i));
                    parameters++;
                }

                Factors[c] = new FactorPotential<double>(this, index, model.States, parameters);
                index += parameters;
            }

            this.Weights = l.ToArray();
            this.Features = f.ToArray();
        }

    }
}
