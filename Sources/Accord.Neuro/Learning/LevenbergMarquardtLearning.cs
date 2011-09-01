// Accord Neural Net Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
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
    using Accord.Math;
    using Accord.Math.Decompositions;
    using AForge.Neuro;

    /// <summary>
    ///   The Jacobian computation method used by the Levenberg-Marquardt.
    /// </summary>
    public enum JacobianMethod
    {
        /// <summary>
        ///   Computes the Jacobian using approximation by finite differences. This
        ///   method is slow in comparison with back-propagation and should be used
        ///   only for debugging or comparison purposes.
        /// </summary>
        /// 
        ByFiniteDifferences,

        /// <summary>
        ///   Computes the Jacobian using back-propagation for the chain rule of
        ///   calculus. This is the preferred way of computing the Jacobian.
        /// </summary>
        /// 
        ByBackpropagation,
    }

    /// <summary>
    ///   Levenberg-Marquardt Learning Algorithm with optional Bayesian Regularization.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>This class implements the Levenberg-Marquardt learning algorithm,
    /// which treats the neural network learning as a function optimization
    /// problem. The Levenberg-Marquardt is one of the fastest and accurate
    /// learning algorithms for small to medium sized networks.</para>
    /// 
    /// <para>However, in general, the standard LM algorithm does not perform as well
    /// on pattern recognition problems as it does on function approximation problems.
    /// The LM algorithm is designed for least squares problems that are approximately
    /// linear. Because the output neurons in pattern recognition problems are generally
    /// saturated, it will not be operating in the linear region.</para>
    /// 
    /// <para>The advantages of the LM algorithm decreases as the number of network
    /// parameters increases. </para>
    /// 
    /// <para>Sample usage (training network to calculate XOR function):</para>
    ///   <code>
    ///   // initialize input and output values
    ///   double[][] input =
    ///   {
    ///       new double[] {0, 0}, new double[] {0, 1},
    ///       new double[] {1, 0}, new double[] {1, 1}
    ///   };
    /// 
    ///   double[][] output = 
    ///   {
    ///       new double[] {0}, new double[] {1},
    ///       new double[] {1}, new double[] {0}
    ///   };
    ///   
    ///   // create neural network
    ///   ActivationNetwork   network = new ActivationNetwork(
    ///       SigmoidFunction( 2 ),
    ///       2, // two inputs in the network
    ///       2, // two neurons in the first layer
    ///       1 ); // one neuron in the second layer
    ///     
    ///   // create teacher
    ///   LevenbergMarquardtLearning teacher = new LevenbergMarquardtLearning( network );
    ///   
    ///   // loop
    ///   while ( !needToStop )
    ///   {
    ///       // run epoch of learning procedure
    ///       double error = teacher.RunEpoch( input, output );
    ///       
    ///       // check error value to see if we need to stop
    ///       // ...
    ///   }
    /// </code>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.cs.nyu.edu/~roweis/notes/lm.pdf">
    ///       Sam Roweis. Levenberg-Marquardt Optimization.</a></description></item>
    ///     <item><description><a href="http://www-alg.ist.hokudai.ac.jp/~jan/alpha.pdf">
    ///       Jan Poland. (2001). On the Robustness of Update Strategies for the Bayesian
    ///       Hyperparameter alpha. Available on: http://www-alg.ist.hokudai.ac.jp/~jan/alpha.pdf </a></description></item>
    ///     <item><description><a href="http://cs.olemiss.edu/~ychen/publications/conference/chen_ijcnn99.pdf">
    ///       B. Wilamowski, Y. Chen. (1999). Efficient Algorithm for Training Neural Networks 
    ///       with one Hidden Layer. Available on: http://cs.olemiss.edu/~ychen/publications/conference/chen_ijcnn99.pdf </a></description></item>
    ///     <item><description><a href="http://www.inference.phy.cam.ac.uk/mackay/Bayes_FAQ.html">
    ///       David MacKay. (2004). Bayesian methods for neural networks - FAQ. Available on:
    ///       http://www.inference.phy.cam.ac.uk/mackay/Bayes_FAQ.html </a></description></item>
    ///   </list>
    /// </para>   
    /// </remarks>
    /// 
    public class LevenbergMarquardtLearning : AForge.Neuro.Learning.ISupervisedLearning
    {

        private const double lambdaMax = 1e25;


        // network to teach
        private ActivationNetwork network;


        // Bayesian Regularization variables
        private bool useBayesianRegularization;

        // Bayesian Regularization Hyperparameters
        private double gamma;
        private double alpha;
        private double beta = 1.0;

        // Levenberg-Marquardt variables
        private double[][] jacobian;
        private double[,] hessian;

        private double[] diagonal;
        private double[] gradient;
        private double[] weigths;
        private double[] deltas;
        private double[] errors;

        private JacobianMethod method;

        // Levenberg damping factor
        private double lambda = 0.1;

        // The ammount the damping factor is adjusted
        //  when searching for the minimum error surface
        private double v = 10.0;

        // Total of weights in the network
        private int numberOfParameters;



        /// <summary>
        ///   Levenberg's damping factor, also known as lambda.
        /// </summary>
        /// 
        /// <remarks><para>The value determines speed of learning.</para>
        /// 
        /// <para>Default value equals to <b>0.1</b>.</para>
        /// </remarks>
        ///
        public double LearningRate
        {
            get { return lambda; }
            set { lambda = value; }
        }

        /// <summary>
        ///   Learning rate adjustment. 
        /// </summary>
        /// 
        /// <remarks><para>The value by which the learning rate
        /// is adjusted when searching for the minimum cost surface.</para>
        /// 
        /// <para>Default value equals to <b>10</b>.</para>
        /// </remarks>
        ///
        public double Adjustment
        {
            get { return v; }
            set { v = value; }
        }

        /// <summary>
        ///   Gets the total number of parameters
        ///   in the network being trained.
        /// </summary>
        /// 
        public int NumberOfParameters
        {
            get { return numberOfParameters; }
        }

        /// <summary>
        ///   Gets the number of effective parameters being used
        ///   by the network as determined by the bayesian regularization.
        /// </summary>
        /// <remarks>
        ///   If no regularization is being used, the value will be 0.
        /// </remarks>
        /// 
        public double EffectiveParameters
        {
            get { return gamma; }
        }

        /// <summary>
        ///   Gets or sets the importance of the squared sum of network
        ///   weights in the cost function. Used by the regularization.
        /// </summary>
        /// <remarks>
        ///   This is the first bayesian hyperparameter. The default
        ///   value is 0.
        /// </remarks>
        /// 
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        ///   Gets or sets the importance of the squared sum of network
        ///   errors in the cost function. Used by the regularization.
        /// </summary>
        /// <remarks>
        ///   This is the second bayesian hyperparameter. The default
        ///   value is 1.
        /// </remarks>
        /// 
        public double Beta
        {
            get { return beta; }
            set { beta = value; }
        }

        /// <summary>
        ///   Gets or sets whether to use Bayesian Regularization.
        /// </summary>
        /// 
        public bool UseRegularization
        {
            get { return useBayesianRegularization; }
            set { useBayesianRegularization = value; }
        }



        /// <summary>
        ///   Initializes a new instance of the <see cref="LevenbergMarquardtLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Network to teach.</param>
        /// 
        public LevenbergMarquardtLearning(ActivationNetwork network) :
            this(network, false, JacobianMethod.ByBackpropagation)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LevenbergMarquardtLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Network to teach.</param>
        /// <param name="useRegularization">True to use bayesian regularization, false otherwise.</param>
        /// 
        public LevenbergMarquardtLearning(ActivationNetwork network, bool useRegularization) :
            this(network, useRegularization, JacobianMethod.ByBackpropagation)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LevenbergMarquardtLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Network to teach.</param>
        /// <param name="useRegularization">True to use bayesian regularization, false otherwise.</param>
        /// <param name="method">The method by which the Jacobian matrix will be calculated.</param>
        /// 
        public LevenbergMarquardtLearning(ActivationNetwork network, bool useRegularization, JacobianMethod method)
        {
            if (network[network.LayersCount - 1].NeuronsCount > 1)
            {
                throw new NotSupportedException("Currently only networks with a single output are supported.");
            }


            this.network = network;
            this.numberOfParameters = getNumberOfParameters(network);
            this.useBayesianRegularization = useRegularization;
            this.weigths = new double[numberOfParameters];
            this.hessian = new double[numberOfParameters, numberOfParameters];
            this.diagonal = new double[numberOfParameters];
            this.gradient = new double[numberOfParameters];
            this.jacobian = new double[numberOfParameters][];
            this.method = method;



            // Will use backpropagation method for Jacobian computation
            if (method == JacobianMethod.ByBackpropagation)
            {
                // create weight derivatives arrays
                this.weightDerivatives = new double[network.LayersCount][][];
                this.thresholdsDerivatives = new double[network.LayersCount][];

                // initialize arrays
                for (int i = 0; i < network.LayersCount; i++)
                {
                    ActivationLayer layer = network[i];

                    this.weightDerivatives[i] = new double[layer.NeuronsCount][];
                    this.thresholdsDerivatives[i] = new double[layer.NeuronsCount];

                    for (int j = 0; j < layer.NeuronsCount; j++)
                    {
                        this.weightDerivatives[i][j] = new double[layer.InputsCount];
                    }
                }
            }
            else // Will use finite difference method for Jacobian computation
            {
                // create differential coefficient arrays
                this.differentialCoefficients = createCoefficients(3);
                this.derivativeStepSize = new double[numberOfParameters];

                // initialize arrays
                for (int i = 0; i < numberOfParameters; i++)
                {
                    this.derivativeStepSize[i] = derivativeStep;
                }
            }
        }




        /// <summary>
        ///  This method should not be called. Use <see cref="RunEpoch"/> instead.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// <param name="output">Array of output vectors.</param>
        /// 
        /// <returns>Nothing.</returns>
        /// 
        /// <remarks><para>Online learning mode is not supported by the
        /// Levenberg Marquardt. Use batch learning mode instead.</para></remarks>
        ///
        public double Run(double[] input, double[] output)
        {
            throw new InvalidOperationException("Learning can only be done in batch mode.");
        }


        /// <summary>
        ///   Runs a single learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// <param name="output">Array of output vectors.</param>
        /// 
        /// <returns>Returns summary learning error for the epoch.</returns>
        /// 
        /// <remarks><para>The method runs one learning epoch, by calling running necessary
        /// iterations of the Levenberg Marquardt to achieve an error decrease.</para></remarks>
        ///
        public double RunEpoch(double[][] input, double[][] output)
        {
            // Initial definitions and memory allocations
            int N = input.Length;

            LuDecomposition decomposition = null;
            double sumOfSquaredErrors = 0.0;
            double sumOfSquaredWeights = 0.0;
            double trace = 0.0;


            // Re-allocate errorr vector if needed
            if (errors == null || errors.Length != N)
                errors = new double[N];

            // Re-allocate the Jacobian matrix if needed
            if (jacobian[0] == null || jacobian[0].Length != N)
            {
                for (int i = 0; i < jacobian.Length; i++)
                    this.jacobian[i] = new double[N];
            }


            // Compute the Jacobian matrix
            if (method == JacobianMethod.ByBackpropagation)
                sumOfSquaredErrors = JacobianByChainRule(input, output);
            else
                sumOfSquaredErrors = JacobianByFiniteDifference(input, output);


            // Create the initial weights vector
            sumOfSquaredWeights = saveNetworkToArray();


            // Compute error gradient
            for (int i = 0; i < jacobian.Length; i++)
            {
                double[] ji = jacobian[i];

                double s = 0.0;
                for (int j = 0; j < ji.Length; j++)
                    s += ji[j] * errors[j];
                gradient[i] = s;
            }

            // Compute Quasi-Hessian Matrix approximation
            //  using the outer project Jacobian (H ~ J'J)
            for (int i = 0; i < jacobian.Length; i++)
            {
                double[] ji = jacobian[i];

                for (int j = 0; j < jacobian.Length; j++)
                {
                    double[] jj = jacobian[j];

                    double s = 0.0;
                    for (int k = 0; k < ji.Length; k++)
                        s += ji[k] * jj[k];

                    hessian[i, j] = 2.0 * beta * s;
                }
            }

            // Store the Hessian diagonal for future computations
            for (int i = 0; i < diagonal.Length; i++)
                diagonal[i] = hessian[i, i];


            // Define the objective function
            // bayesian regularization objective function
            double objective = beta * sumOfSquaredErrors + alpha * sumOfSquaredWeights;
            double current = objective + 1.0;



            // Begin of the main Levenberg-Macquardt method
            lambda /= v;

            // We'll try to find a direction with less error
            //  (or where the objective function is smaller)
            while (current >= objective && lambda < lambdaMax)
            {
                lambda *= v;

                // Update diagonal (Levenberg-Marquardt formula)
                for (int i = 0; i < diagonal.Length; i++)
                    hessian[i, i] = diagonal[i] + 2.0 * (lambda + alpha);

                // Decompose to solve the linear system
                decomposition = new LuDecomposition(hessian, false, true);

                // Check if the Hessian has become non-invertible
                if (!decomposition.Nonsingular)
                {
                    // The Hessian is singular. Continue to the next
                    // iteration until the diagonal update transforms
                    // it back to non-singular.
                    continue;
                }

                // Solve using LU (or SVD) decomposition
                // TODO: Investigate the use of Cholesky
                deltas = decomposition.Solve(gradient);

                // Update weights using the calculated deltas
                sumOfSquaredWeights = loadArrayIntoNetwork();

                // Calculate the new error
                sumOfSquaredErrors = 0.0;
                for (int i = 0; i < input.Length; i++)
                {
                    network.Compute(input[i]); // forward pass
                    sumOfSquaredErrors += calculateError(output[i]);
                }

                // Update the objective function
                current = beta * sumOfSquaredErrors + alpha * sumOfSquaredWeights;

                // If the object function is bigger than before, the method
                //  is tried again using a greater damping factor.
            }

            // If this iteration caused a error drop, then next iteration
            //  will use a smaller damping factor.
            lambda /= v;



            // If we are using bayesian regularization, we need to
            //   update the bayesian hyperparameters alpha and beta
            if (useBayesianRegularization)
            {
                // References: 
                // - http://www-alg.ist.hokudai.ac.jp/~jan/alpha.pdf
                // - http://www.inference.phy.cam.ac.uk/mackay/Bayes_FAQ.html

                // Compute the trace for the inverse hessian
                trace = Matrix.Trace(decomposition.Inverse());

                // Poland update's formula:
                gamma = numberOfParameters - (alpha * trace);
                alpha = numberOfParameters / (2.0 * sumOfSquaredWeights + trace);
                beta = System.Math.Abs((N - gamma) / (2.0 * sumOfSquaredErrors));
                //beta = (N - gama) / (2.0 * sumOfSquaredErrors);

                // Original MacKay's update formula:
                //  gama = (double)networkParameters - (alpha * trace);
                //  alpha = gama / (2.0 * sumOfSquaredWeights);
                //  beta = (gama - N) / (2.0 * sumOfSquaredErrors);
            }

            return sumOfSquaredErrors;
        }

        /// <summary>
        ///   Calculates error values for the last network layer.
        /// </summary>
        /// 
        /// <param name="expected">Desired output vector.</param>
        /// 
        /// <returns>Returns summary squared error of the last layer divided by 2.</returns>
        /// 
        private double calculateError(double[] expected)
        {
            double sumOfSquaredErrors = 0.0;

            for (int j = 0; j < expected.Length; j++)
            {
                double e = expected[j] - network.Output[j];
                sumOfSquaredErrors += e * e;
            }

            return sumOfSquaredErrors / 2.0;
        }

        /// <summary>
        ///  Update network's weights.
        /// </summary>
        /// 
        /// <returns>The sum of squared weights divided by 2.</returns>
        /// 
        private double loadArrayIntoNetwork()
        {
            double w, sumOfSquaredWeights = 0.0;
            int j = 0;

            // For each layer in the network
            for (int li = 0; li < network.LayersCount; li++)
            {
                ActivationLayer layer = network[li];

                // for each neuron in the layer
                for (int ni = 0; ni < layer.NeuronsCount; ni++, j++)
                {
                    ActivationNeuron neuron = layer[ni];

                    // for each weight in the neuron
                    for (int wi = 0; wi < neuron.InputsCount; wi++, j++)
                    {
                        w = neuron[wi] = weigths[j] + deltas[j];
                        sumOfSquaredWeights += w * w;
                    }

                    // for each threshold value (bias):
                    w = neuron.Threshold = weigths[j] + deltas[j];
                    sumOfSquaredWeights += w * w;
                }
            }

            return sumOfSquaredWeights / 2.0;
        }

        /// <summary>
        ///   Creates the initial weight vector w
        /// </summary>
        /// 
        /// <returns>The sum of squared weights divided by 2.</returns>
        /// 
        private double saveNetworkToArray()
        {
            int j = 0;
            double w, sumOfSquaredWeights = 0.0;

            // for each layer in the network
            for (int li = 0; li < network.LayersCount; li++)
            {
                ActivationLayer layer = network[li];

                // for each neuron in the layer
                for (int ni = 0; ni < network[li].NeuronsCount; ni++)
                {
                    ActivationNeuron neuron = layer[ni];

                    // for each weight in the neuron
                    for (int wi = 0; wi < neuron.InputsCount; wi++)
                    {
                        // We copy it to the starting weights vector
                        w = weigths[j++] = neuron[wi];
                        sumOfSquaredWeights += w * w;
                    }

                    // and also for the threshold value (bias):
                    w = weigths[j++] = neuron.Threshold;
                    sumOfSquaredWeights += w * w;
                }
            }
            return sumOfSquaredWeights / 2.0;
        }


        /// <summary>
        ///   Gets the number of parameters in a network.
        /// </summary>
        private static int getNumberOfParameters(ActivationNetwork network)
        {
            int w = 0;
            for (int i = 0; i < network.LayersCount; i++)
            {
                for (int j = 0; j < network[i].NeuronsCount; j++)
                {
                    // number of weights plus the bias value
                    w += network[i][j].InputsCount + 1;
                }
            }
            return w;
        }





        #region Jacobian Calculation By Chain Rule

        private double[][][] weightDerivatives;
        private double[][] thresholdsDerivatives;

        /// <summary>
        ///   Calculates the Jacobian matrix by using the chain rule.
        /// </summary>
        /// <param name="input">The input vectors.</param>
        /// <param name="output">The desired output vectors.</param>
        /// <returns>The sum of squared errors for the last error divided by 2.</returns>
        /// 
        private double JacobianByChainRule(double[][] input, double[][] output)
        {
            double e, sumOfSquaredErrors = 0.0;

            // foreach training vector
            for (int i = 0; i < input.Length; i++)
            {
                // Do a forward pass
                network.Compute(input[i]);

                // Calculate the derivatives for the j-th output            
                //  by using a backpropagation pass
                e = errors[i] = CalculateDerivatives(input[i], output[i], 0);
                sumOfSquaredErrors += e * e;
                int j = 0;


                // Create the Jacobian matrix row: for each layer in the network
                for (int li = 0; li < network.LayersCount; li++)
                {
                    ActivationLayer layer = network[li];

                    double[] layerThresoldDerivatives = thresholdsDerivatives[li];
                    double[][] layerDerivatives = weightDerivatives[li];

                    // for each neuron in the layer
                    for (int ni = 0; ni < layerDerivatives.Length; ni++)
                    {
                        ActivationNeuron neuron = layer[ni];

                        double[] neuronWeightDerivatives = layerDerivatives[ni];

                        // for each weight of the neuron
                        for (int wi = 0; wi < neuronWeightDerivatives.Length; wi++)
                        {
                            // copy derivative
                            jacobian[j++][i] = neuronWeightDerivatives[wi];
                        }

                        // also copy for the neuron threshold
                        jacobian[j++][i] = layerThresoldDerivatives[ni];
                    }
                }

            }
            return sumOfSquaredErrors / 2.0;
        }

        /// <summary>
        ///   Calculates partial derivatives for all weights of the network.
        /// </summary>
        /// 
        /// <param name="input">The input vector.</param>
        /// <param name="desiredOutput">Desired output vector.</param>
        /// <param name="outputIndex">The current output location (index) in the desired output vector.</param>
        /// 
        /// <returns>Returns summary squared error of the last layer.</returns>
        /// 
        private double CalculateDerivatives(double[] input, double[] desiredOutput, int outputIndex)
        {
            // assume, that all neurons of the network have the same activation function
            IActivationFunction function = network[0][0].ActivationFunction;

            double[] previousLayerOutput;

            // Start by the output layer first
            int outputLayerIndex = network.LayersCount - 1;
            ActivationLayer outputLayer = network[outputLayerIndex];

            // If we have only one single layer, the previous layer outputs is given by the input layer
            previousLayerOutput = (outputLayerIndex == 0) ? input : network[outputLayerIndex - 1].Output;

            // Assume single output neuron
            ActivationNeuron outputNeuron = outputLayer[outputIndex];
            double[] neuronWeightDerivatives = weightDerivatives[outputLayerIndex][outputIndex];

            double output = outputNeuron.Output;
            double e = desiredOutput[outputIndex] - output;
            double derivative = function.Derivative2(output);

            // Set derivative for each weight in the neuron
            for (int i = 0; i < previousLayerOutput.Length; i++)
                neuronWeightDerivatives[i] = derivative * previousLayerOutput[i];

            // Set derivative for the current threshold (bias) term
            thresholdsDerivatives[outputLayerIndex][outputIndex] = derivative;


            // Now, proceed to the hidden layers
            for (int layerIndex = network.LayersCount - 2; layerIndex >= 0; layerIndex--)
            {
                int nextLayerIndex = layerIndex + 1;

                ActivationLayer layer = network[layerIndex];
                ActivationLayer nextLayer = network[nextLayerIndex];

                // If we are in the first layer, the previous layer is just the input layer
                previousLayerOutput = (layerIndex == 0) ? input : network[layerIndex - 1].Output;

                // Now, we will compute the derivatives for the current layer applying the chain
                //  rule. To apply the chain-rule, we will make use of the previous derivatives
                //  computed for the inner layers (forming a calculation chain, hence the name).

                // So, for each neuron in the current layer:
                for (int neuronIndex = 0; neuronIndex < layer.NeuronsCount; neuronIndex++)
                {
                    ActivationNeuron neuron = layer[neuronIndex];

                    neuronWeightDerivatives = weightDerivatives[layerIndex][neuronIndex];

                    double[] layerDerivatives = thresholdsDerivatives[layerIndex];
                    double[] nextLayerDerivatives = thresholdsDerivatives[layerIndex + 1];

                    double sum = 0;

                    // The chain-rule can be stated as (f(w*g(x))' = f'(w*g(x)) * w*g'(x)
                    //
                    // We will start computing the second part of the product. Since the g' 
                    //  derivatives have already been computed in the previous computation,
                    //  we will be summing all previous function derivatives and weighting
                    //  them using their connection weight (sinapses).
                    //
                    // So, for each neuron in the next layer:
                    for (int j = 0; j < nextLayerDerivatives.Length; j++)
                    {
                        // retrieve the weight connecting the output of the current
                        //   neuron and the activation function of the next neuron.
                        double weight = nextLayer[j][neuronIndex];

                        // accumulate the sinapse weight * next layer derivative
                        sum += weight * nextLayerDerivatives[j];
                    }

                    // Continue forming the chain-rule statement
                    derivative = sum * function.Derivative2(neuron.Output);

                    // Set derivative for each weight in the neuron
                    for (int i = 0; i < previousLayerOutput.Length; i++)
                        neuronWeightDerivatives[i] = derivative * previousLayerOutput[i];

                    // Set derivative for the current threshold
                    layerDerivatives[neuronIndex] = derivative;

                    // The threshold derivatives also gather the derivatives for
                    // the layer, and thus can be re-used in next calculations.
                }
            }

            // return error
            return e;
        }
        #endregion


        #region Jacobian Calculation by Finite Differences

        // References:
        // - Trent F. Guidry, http://www.trentfguidry.net/

        private double[] derivativeStepSize;
        private const double derivativeStep = 1e-2;
        private double[][,] differentialCoefficients;


        /// <summary>
        ///   Calculates the Jacobian Matrix using Finite Differences
        /// </summary>
        /// <returns>Returns the sum of squared errors of the network divided by 2.</returns>
        private double JacobianByFiniteDifference(double[][] input, double[][] desiredOutput)
        {
            double[] networkOutput;
            double e, sumOfSquaredErrors = 0;
            int N = input.Length;

            // foreach training vector
            for (int i = 0; i < N; i++)
            {
                networkOutput = network.Compute(input[i]);

                // Calculate network error to build the residuals vector
                e = errors[i] = desiredOutput[i][0] - networkOutput[0];
                sumOfSquaredErrors += e * e;

                // Computation of one of the Jacobian Matrix rows by nummerical differentiation:
                // for each weight wj in the network, we have to compute its partial
                //   derivative to build the jacobian matrix.
                int jj = 0;

                // So, for each layer:
                for (int layerIndex = 0; layerIndex < network.LayersCount; layerIndex++)
                {
                    ActivationLayer layer = network[layerIndex];

                    // for each neuron:
                    for (int neuronIndex = 0; neuronIndex < layer.NeuronsCount; neuronIndex++)
                    {
                        ActivationNeuron neuron = layer[neuronIndex];

                        // for each weight:
                        for (int weight = 0; weight < neuron.InputsCount; weight++)
                        {
                            // Compute its partial derivative
                            jacobian[jj][i] = ComputeDerivative(input[i], layerIndex, neuronIndex, weight, ref derivativeStepSize[jj], networkOutput[0]);
                            jj++;
                        }
                        // and also for each threshold value (bias)
                        jacobian[jj][i] = ComputeDerivative(input[i], layerIndex, neuronIndex, -1, ref derivativeStepSize[jj], networkOutput[0]);
                        jj++;
                    }
                }
            }

            // returns the sum of squared errors / 2
            return sumOfSquaredErrors / 2.0;
        }



        /// <summary>
        ///   Creates the coefficients to be used when calculating
        ///   the approximate Jacobian by using finite differences.
        /// </summary>
        /// 
        private static double[][,] createCoefficients(int points)
        {
            double[][,] coefficients = new double[points][,];

            for (int i = 0; i < points; i++)
            {
                double[,] delts = new double[points, points];

                for (int j = 0; j < points; j++)
                {
                    double delt = (double)(j - i);
                    double hterm = 1.0;

                    for (int k = 0; k < points; k++)
                    {
                        delts[j, k] = hterm / Accord.Math.Special.Factorial(k);
                        hterm *= delt;
                    }
                }

                coefficients[i] = Matrix.Inverse(delts);
                double fac = Accord.Math.Special.Factorial(points);

                for (int j = 0; j < points; j++)
                    for (int k = 0; k < points; k++)
                        coefficients[i][j, k] = (System.Math.Round(coefficients[i][j, k] * fac, MidpointRounding.AwayFromZero)) / fac;
            }

            return coefficients;
        }

        /// <summary>
        ///   Computes the derivative of the network in respect to the
        ///   weight passed as parameter.
        /// </summary>
        private double ComputeDerivative(double[] inputs,
            int layer, int neuron, int weight,
            ref double stepSize, double networkOutput)
        {
            int numPoints = differentialCoefficients.Length;
            double ret = 0.0;
            double originalValue;

            // Saves a copy of the original value in the neuron
            if (weight >= 0) originalValue = network[layer][neuron][weight];
            else originalValue = network[layer][neuron].Threshold;

            double[] points = new double[numPoints];

            if (originalValue != 0.0)
                stepSize = derivativeStep * System.Math.Abs(originalValue);
            else stepSize = derivativeStep;

            int centerPoint = (numPoints - 1) / 2;

            for (int i = 0; i < numPoints; i++)
            {
                if (i != centerPoint)
                {
                    double newValue = originalValue + ((double)(i - centerPoint)) * stepSize;

                    if (weight >= 0) network[layer][neuron][weight] = newValue;
                    else network[layer][neuron].Threshold = newValue;

                    points[i] = network.Compute(inputs)[0];
                }
                else
                {
                    points[i] = networkOutput;
                }
            }

            ret = 0.0;
            for (int i = 0; i < differentialCoefficients.Length; i++)
            {
                ret += differentialCoefficients[centerPoint][1, i] * points[i];
            }

            ret /= System.Math.Pow(stepSize, 1);


            // Changes back the modified value
            if (weight >= 0) network[layer][neuron][weight] = originalValue;
            else network[layer][neuron].Threshold = originalValue;

            return ret;
        }
        #endregion

    }
}
