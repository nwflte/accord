// Accord Neural Net Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009, 2010
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

namespace Accord.Neuro
{
    using System;
    using AForge;
    using AForge.Neuro;

    /// <summary>
    ///  Nguyen-Widrow weight initializer
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The Nguyen-Widrow initialization algorithm chooses values in
    /// order to distribute the active region of each neuron in the layer
    /// approximately evenly across the layers' input space.</para>
    /// 
    /// <para>The values contain a degree of randomness, so they are not the
    /// same each time this function is called.</para> 
    /// </remarks>
    /// 
    public class NguyenWidrow
    {
        private ActivationNetwork network;
        private Range randRange;
        private double beta;

        /// <summary>
        ///   Constructs a new Nguyen-Widrow Weight Initializer.
        /// </summary>
        /// 
        /// <param name="network">The activation network whose weights will be initialized.</param>
        /// 
        public NguyenWidrow(ActivationNetwork network)
        {
            this.network = network;

            int hiddenNodes = network[0].NeuronsCount;
            int inputNodes = network[0].InputsCount;

            randRange = new Range(-0.5f, 0.5f);
            beta = 0.7 * Math.Pow(hiddenNodes, 1.0 / inputNodes);
        }

        /// <summary>
        ///   Randomizes (initializes) the weights of
        ///   the network using Nguyen-Widrow method's.
        /// </summary>
        /// 
        public void Randomize()
        {
            Neuron.RandGenerator = Accord.Math.Tools.Random;
            Neuron.RandRange = randRange;

            for (int i = 0; i < network.LayersCount; i++)
            {
                for (int j = 0; j < network[i].NeuronsCount; j++)
                {
                    ActivationNeuron neuron = network[i][j];
                    neuron.Randomize();
                    double norm = 0.0;

                    // Calculate the Euclidean Norm for the weights
                    for (int k = 0; k < neuron.InputsCount; k++)
                        norm += neuron[k] * neuron[k];
                    norm += neuron.Threshold * neuron.Threshold;

                    norm = System.Math.Sqrt(norm);

                    // Rescale the weights using beta and the norm
                    for (int k = 0; k < neuron.InputsCount; k++)
                        neuron[k] = beta * neuron[k] / norm;
                    neuron.Threshold = beta * neuron.Threshold / norm;
                }
            }
        }

    }
}
