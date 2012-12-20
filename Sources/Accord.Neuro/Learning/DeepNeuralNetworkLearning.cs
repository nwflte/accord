// Accord Neural Net Library
// The Accord.NET Framework
// http://accord.googlecode.com
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

namespace Accord.Neuro.Learning
{
    using System;
    using System.Collections.Generic;
    using Accord.Neuro.Layers;
    using Accord.Neuro.Networks;
    using AForge.Neuro.Learning;
    using AForge.Neuro;

    /// <summary>
    ///   Delegate used to configure and create layer-specific learning algorithms.
    /// </summary>
    /// 
    /// <param name="network">The network layer being trained.</param>
    /// <param name="index">The index of the layer in the deep network.</param>
    /// 
    /// <returns>
    ///   The function should return an instance of the algorithm 
    ///   which should be used to train the network.
    /// </returns>
    /// 
    public delegate ISupervisedLearning ActivationNetworkLearningConfigurationFunction(
        ActivationNetwork network, int index);

    /// <summary>
    ///   Deep Neural Network learning algorithm.
    /// </summary>
    /// 
    public class DeepNeuralNetworkLearning : ISupervisedLearning
    {

        private DeepBeliefNetwork network;
        private ActivationNetworkLearningConfigurationFunction configure;

        private int layerIndex = 0;

        private ISupervisedLearning[] algorithms;

        /// <summary>
        ///   Gets or sets the configuration function used
        ///   to specify and create the learning algorithms
        ///   for each of the layers of the deep network.
        /// </summary>
        /// 
        public ActivationNetworkLearningConfigurationFunction Algorithm
        {
            get { return configure; }
            set
            {
                configure = value;
                createAlgorithms();
            }
        }

        private void createAlgorithms()
        {
            algorithms = new ISupervisedLearning[network.Machines.Count];
            for (int i = 0; i < network.Machines.Count; i++)
            {
                RestrictedBoltzmannMachine layer = network.Machines[i];
                algorithms[i] = configure(layer, i);
            }
        }

        /// <summary>
        ///   Gets or sets the current layer index being
        ///   trained by the deep learning algorithm.
        /// </summary>
        /// 
        public int LayerIndex
        {
            get { return layerIndex; }
            set
            {
                if (layerIndex < 0 || layerIndex >= network.Machines.Count)
                    throw new ArgumentOutOfRangeException("value");

                layerIndex = value;
            }
        }

        /// <summary>
        ///   Creates a new <see cref="DeepBeliefNetworkLearning"/> algorithm.
        /// </summary>
        /// 
        /// <param name="network">The network to be trained.</param>
        /// 
        public DeepNeuralNetworkLearning(DeepBeliefNetwork network)
        {
            this.network = network;
        }

        /// <summary>
        ///   Gets the learning data needed to train the <see cref="LayerIndex">currently
        ///   selected layer</see>. The return of this function should then be passed to
        ///   <see cref="RunEpoch(double[][], double[][])"/> to actually run a learning epoch.
        /// </summary>
        /// 
        /// <param name="input">The batch of input data.</param>
        /// 
        /// <returns>The learning data for the current layer.</returns>
        /// 
        public double[][] GetLayerInput(double[][] input)
        {
            return GetLayerInput(new[] { input })[0];
        }

        /// <summary>
        ///   Gets the learning data needed to train the <see cref="LayerIndex">currently
        ///   selected layer</see>. The return of this function should then be passed to
        ///   <see cref="RunEpoch(double[][], double[][])"/> to actually run a learning epoch.
        /// </summary>
        /// 
        /// <param name="batches">The mini-batches of input data.</param>
        /// 
        /// <returns>The learning data for the current layer.</returns>
        /// 
        public double[][][] GetLayerInput(double[][][] batches)
        {
            if (layerIndex == 0)
                return batches;

            var outputBatches = new double[batches.Length][][];

            for (int j = 0; j < batches.Length; j++)
            {
                int batchSize = batches[j].Length;

                double[][] inputs = batches[j];
                double[][] outputs = new double[batchSize][];

                for (int i = 0; i < inputs.Length; i++)
                {
                    double[] responses = network.Compute(inputs[i]);
                    outputs[i] = network.Machines[layerIndex - 1].Hidden.Output;
                }

                outputBatches[j] = outputs;
            }

            return outputBatches;
        }

        /// <summary>
        ///   Gets the <see cref="IUnsupervisedLearning">unsupervised 
        ///   learning algorithm</see> allocated for the given layer.
        /// </summary>
        /// 
        /// <param name="layerIndex">The index of the layer to get the algorithm for.</param>
        /// 
        public ISupervisedLearning GetLayerAlgorithm(int layerIndex)
        {
            return algorithms[layerIndex];
        }


        /// <summary>
        ///   Runs a single learning iteration.
        /// </summary>
        /// 
        /// <param name="input">A single input vector.</param>
        /// <param name="output">The corresponding output vector.</param>
        /// 
        /// <returns>
        ///   Returns the learning error after the iteration.
        /// </returns>
        /// 
        public double Run(double[] input, double[] output)
        {
            // Get layer learning algorithm
            var teacher = algorithms[layerIndex];

            // Learn the layer using data
            return teacher.Run(input, output);
        }

        /// <summary>
        ///   Runs a single batch epoch
        ///   of the learning algorithm.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// <param name="output">Array of corresponding output vectors.</param>
        /// 
        /// <returns>
        ///   Returns sum of learning errors.
        /// </returns>
        /// 
        public double RunEpoch(double[][] input, double[][] output)
        {
            // Get layer learning algorithm
            var teacher = algorithms[layerIndex];

            // Learn the layer using data
            return teacher.RunEpoch(input, output);
        }

        /// <summary>
        ///   Runs a single learning epoch using
        ///   multiple mini-batches to improve speed.
        /// </summary>
        /// 
        /// <param name="inputBatches">Array of input batches.</param>
        /// <param name="outputBatches">Array of corresponding output batches.</param>
        /// 
        /// <returns>
        ///   Returns sum of learning errors.
        /// </returns>
        /// 
        public double RunEpoch(double[][][] inputBatches, double[][][] outputBatches)
        {
            // Get layer learning algorithm
            var teacher = algorithms[layerIndex];

            // Learn the layer using data
            double error = 0;
            for (int i = 0; i < inputBatches.Length; i++)
                error += teacher.RunEpoch(inputBatches[i], outputBatches[i]);

            return error;
        }

        /// <summary>
        ///   Computes the reconstruction error for 
        ///   a given set of input values.
        /// </summary>
        /// 
        /// <param name="inputs">The input values.</param>
        /// <param name="outputs">The corresponding output values.</param>
        /// 
        /// <returns>The squared reconstruction error.</returns>
        /// 
        public double ComputeError(double[][] inputs, double[][] outputs)
        {
            double error = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] output = network.Compute(inputs[i]);

                for (int j = 0; j < inputs[i].Length; j++)
                {
                    double e = output[j] - outputs[i][j];
                    error += e * e;
                }
            }
            return error;
        }



    }
}
