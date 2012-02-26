// Accord Machine Learning Library
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

namespace Accord.MachineLearning.VectorMachines
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using Accord.Math;
    using Accord.Statistics.Kernels;

    /// <summary>
    ///   Decision strategies for <see cref="MulticlassSupportVectorMachine">
    ///   Multi-class Support Vector Machines</see>.
    /// </summary>
    /// 
    public enum MulticlassComputeMethod
    {
        /// <summary>
        ///   Max-voting method (also known as 1vs1 decision).
        /// </summary>
        /// 
        Voting,

        /// <summary>
        ///   Elimination method (also known as DAG decision).
        /// </summary>
        /// 
        Elimination,
    }

    /// <summary>
    ///   One-against-one Multi-class Kernel Support Vector Machine Classifier.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The Support Vector Machine is by nature a binary classifier. One of the ways
    ///   to extend the original SVM algorithm to multiple classes is to build a one-
    ///   against-one scheme where multiple SVMs specialize to recognize each of the
    ///   available classes. By using a competition scheme, the original multi-class
    ///   classification problem is then reduced to <c>n*(n/2)</c> smaller binary problems.</para>
    /// <para>
    ///   Currently this class supports only Kernel machines as the underlying classifiers.
    ///   If a Linear Support Vector Machine is needed, specify a Linear kernel in the
    ///   constructor at the moment of creation. </para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       <a href="http://courses.media.mit.edu/2006fall/mas622j/Projects/aisen-project/index.html">
    ///        http://courses.media.mit.edu/2006fall/mas622j/Projects/aisen-project/index.html </a></description></item>
    ///     <item><description>
    ///       <a href="http://nlp.stanford.edu/IR-book/html/htmledition/multiclass-svms-1.html">
    ///        http://nlp.stanford.edu/IR-book/html/htmledition/multiclass-svms-1.html </a></description></item>
    ///     </list></para>
    ///     
    /// </remarks>
    ///
    /// <example>
    ///   <code>
    ///   // Sample data
    ///   //   The following is simple auto association function
    ///   //   where each input correspond to its own class. This
    ///   //   problem should be easily solved by a Linear kernel.
    ///
    ///   // Sample input data
    ///   double[][] inputs =
    ///   {
    ///       new double[] { 0 },
    ///       new double[] { 3 },
    ///       new double[] { 1 },
    ///       new double[] { 2 },
    ///   };
    ///   
    ///   // Output for each of the inputs
    ///   int[] outputs = { 0, 3, 1, 2 };
    ///   
    ///   
    ///   // Create a new Linear kernel
    ///   IKernel kernel = new Linear();
    ///   
    ///   // Create a new Multi-class Support Vector Machine with one input,
    ///   //  using the linear kernel and for four disjoint classes.
    ///   var machine = new MulticlassSupportVectorMachine(1, kernel, 4);
    ///   
    ///   // Create the Multi-class learning algorithm for the machine
    ///   var teacher = new MulticlassSupportVectorLearning(machine, inputs, outputs);
    ///   
    ///   // Configure the learning algorithm to use SMO to train the
    ///   //  underlying SVMs in each of the binary class subproblems.
    ///   teacher.Algorithm = (svm, classInputs, classOutputs, i, j) =>
    ///       new SequentialMinimalOptimization(svm, classInputs, classOutputs);
    ///   
    ///   // Run the learning algorithm
    ///   double error = teacher.Run();
    ///   </code>
    /// </example>
    ///
    /// <seealso cref="Learning.MulticlassSupportVectorLearning"/>
    ///
    [Serializable]
    public class MulticlassSupportVectorMachine : ISupportVectorMachine
    {

        // Underlying classifiers
        private KernelSupportVectorMachine[][] machines;

        private int? totalVectors;
        private int? uniqueVectors;


        /// <summary>
        ///   Constructs a new Multi-class Kernel Support Vector Machine
        /// </summary>
        /// 
        /// <param name="kernel">The chosen kernel for the machine.</param>
        /// <param name="inputs">The number of inputs for the machine.</param>
        /// <param name="classes">The number of classes in the classification problem.</param>
        /// <remarks>
        ///   If the number of inputs is zero, this means the machine
        ///   accepts a indefinite number of inputs. This is often the
        ///   case for kernel vector machines using a sequence kernel.
        /// </remarks>
        /// 
        public MulticlassSupportVectorMachine(int inputs, IKernel kernel, int classes)
        {
            if (classes <= 1)
                throw new ArgumentException("The machine must have at least two classes.", "classes");

            // Create the kernel machines
            machines = new KernelSupportVectorMachine[classes - 1][];
            for (int i = 0; i < machines.Length; i++)
            {
                machines[i] = new KernelSupportVectorMachine[i + 1];

                for (int j = 0; j <= i; j++)
                    machines[i][j] = new KernelSupportVectorMachine(kernel, inputs);
            }
        }

        /// <summary>
        ///   Constructs a new Multi-class Kernel Support Vector Machine
        /// </summary>
        /// 
        /// <param name="machines">
        ///   The machines to be used in each of the pairwise class subproblems.
        /// </param>
        /// 
        public MulticlassSupportVectorMachine(KernelSupportVectorMachine[][] machines)
        {
            if (machines == null) throw new ArgumentNullException("machines");

            this.machines = machines;
        }

        /// <summary>
        ///   Gets the classifier for <paramref name="class1"/> against <paramref name="class2"/>.
        /// </summary>
        /// 
        /// <remarks>
        ///   If the index of <paramref name="class1"/> is greater than <paramref name="class2"/>,
        ///   the classifier for the <paramref name="class2"/> against <paramref name="class1"/>
        ///   will be returned instead. If both indices are equal, null will be
        ///   returned instead.
        /// </remarks>
        /// 
        public KernelSupportVectorMachine this[int class1, int class2]
        {
            get
            {
                if (class1 == class2)
                    return null;
                if (class1 > class2)
                    return machines[class1 - 1][class2];
                else
                    return machines[class2 - 1][class1];
            }
        }

        /// <summary>
        ///   Gets the total number of support vectors
        ///   in the entire multi-class machine.
        /// </summary>
        /// 
        public int SupportVectorTotal
        {
            get
            {
                if (totalVectors == null)
                {
                    int count = 0;
                    for (int i = 0; i < machines.Length; i++)
                        for (int j = 0; j < machines[i].Length; j++)
                            count += machines[i][j].SupportVectors.Length;
                    totalVectors = count;
                }

                return totalVectors.Value;
            }
        }

        /// <summary>
        ///   Gets the number of unique support 
        ///   vectors in the multi-class machine.
        /// </summary>
        /// 
        public int SupportVectorUnique
        {
            get
            {
                if (uniqueVectors == null)
                {
                    HashSet<double[]> unique = new HashSet<double[]>();
                    for (int i = 0; i < machines.Length; i++)
                        for (int j = 0; j < machines[i].Length; j++)
                            for (int k = 0; k < machines[i][j].SupportVectors.Length; k++)
                                unique.Add(machines[i][j].SupportVectors[k]);
                    uniqueVectors = unique.Count;
                }

                return uniqueVectors.Value;
            }
        }

        /// <summary>
        ///   Gets the number of classes.
        /// </summary>
        /// 
        public int Classes
        {
            get { return machines.Length + 1; }
        }

        /// <summary>
        ///   Gets the number of inputs of the machines.
        /// </summary>
        /// 
        public int Inputs
        {
            get { return machines[0][0].Inputs; }
        }

        /// <summary>
        ///   Gets a value indicating whether this machine produces probabilistic outputs.
        /// </summary>
        /// 
        /// <value>
        ///   <c>true</c> if this machine produces probabilistic outputs; otherwise, <c>false</c>.
        /// </value>
        /// 
        public bool IsProbabilistic
        {
            get { return machines[0][0].IsProbabilistic; }
        }

        /// <summary>
        ///   Gets the subproblems classifiers.
        /// </summary>
        /// 
        public KernelSupportVectorMachine[][] Machines
        {
            get { return machines; }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        ///
        public int Compute(double[] inputs)
        {
            double output; // Compute using elimination method as default.
            return Compute(inputs, MulticlassComputeMethod.Elimination, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        /// 
        public int Compute(double[] inputs, out double output)
        {
            // Compute using elimination method as default.
            return Compute(inputs, MulticlassComputeMethod.Elimination, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="responses">The model response for each class.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        public int Compute(double[] inputs, out double[] responses)
        {
            double output; // Compute using elimination method as default.
            return Compute(inputs, MulticlassComputeMethod.Elimination, out responses, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// <param name="responses">The model response for each class.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        public int Compute(double[] inputs, MulticlassComputeMethod method, out double[] responses, out double output)
        {
            if (method == MulticlassComputeMethod.Voting)
            {
                int[] votes;
                int result = computeVoting(inputs, out votes, out output);

                responses = new double[votes.Length];
                for (int i = 0; i < responses.Length; i++)
                    responses[i] = votes[i] * (2.0 / (Classes * (Classes - 1)));

                return result;
            }
            else
            {
                return computeElimination(inputs, out responses, out output);
            }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// <param name="responses">The model response for each class.</param>
        /// 
        /// <returns>The class decision for the given input.</returns>
        /// 
        public int Compute(double[] inputs, MulticlassComputeMethod method, out double[] responses)
        {
            double output;
            return Compute(inputs, method, out responses, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The class decision for the given input.</returns>
        ///
        public int Compute(double[] inputs, MulticlassComputeMethod method, out double output)
        {
            if (method == MulticlassComputeMethod.Voting)
            {
                int[] votes;
                return computeVoting(inputs, out votes, out output);
            }
            else
            {
                double[] responses;
                return computeElimination(inputs, out responses, out output);
            }
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="method">The <see cref="MulticlassComputeMethod">
        ///   multi-class classification method</see> to use.</param>
        /// 
        /// <returns>The class decision for the given input.</returns>
        ///
        public int Compute(double[] inputs, MulticlassComputeMethod method)
        {
            double output;
            return Compute(inputs, method, out output);
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="votes">A vector containing the number of votes for each class.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        private int computeVoting(double[] inputs, out int[] votes, out double output)
        {
            // out variables cannot be passed into delegates,
            // so will be creating a copy for the vote array.
            int[] voting = new int[this.Classes];
            var cache = new ConcurrentDictionary<double[], double>(Environment.ProcessorCount, (int)(SupportVectorUnique * 1.5));

            // For each class
            Parallel.For(0, Classes, i =>
            {
                // For each other class
                for (int j = 0; j < i; j++)
                {
                    // Retrieve and compute the two-class problem
                    KernelSupportVectorMachine machine = this[i, j];

                    // Decide between class 0 and class 1
                    double answer = compute(cache, machine, inputs);

                    // Determine the winner class
                    int y = (answer < 0) ? i : j;

                    // Increment votes for the winner
                    Interlocked.Increment(ref voting[y]);
                }
            });

            // Voting finished.
            votes = voting;

            // Select class which maximum number of votes
            int imax; output = Matrix.Max(votes, out imax);

            // Determine output probability using no. of votes
            output = output * (2.0 / (Classes * (Classes - 1)));

            return imax; // Return the winner as the output.
        }

        /// <summary>
        ///   Computes the given input to produce the corresponding output.
        /// </summary>
        /// 
        /// <remarks>
        ///   This method computes the decision for a one-against-one multiclass
        ///   support vector machine using the Directed Acyclic Graph method by
        ///   Platt, Cristianini and Shawe-Taylor. Details are given on the 
        ///   original paper "Large Margin DAGs for Multiclass Classification", 2000.
        /// </remarks>
        /// 
        /// <param name="inputs">An input vector.</param>
        /// <param name="responses">The model response for each class.</param>
        /// <param name="output">The output of the machine. If this is a 
        ///   <see cref="IsProbabilistic">probabilistic</see> machine, the
        ///   output is the probability of the positive class. If this is
        ///   a standard machine, the output is the distance to the decision
        ///   hyperplane in feature space.</param>
        /// 
        /// <returns>The decision label for the given input.</returns>
        /// 
        private int computeElimination(double[] inputs, out double[] responses, out double output)
        {
            // Acyclic Directed Graph decision
            var cache = new Dictionary<double[], double>((int)(SupportVectorUnique * 1.5));

            responses = new double[Classes];
            bool[] losers = new bool[Classes];
            bool probabilistic = IsProbabilistic;

            if (probabilistic)
                for (int i = 0; i < responses.Length; i++)
                    responses[i] = 1.0;

            // Start on first machine
            int currentWinner = 0;
            int adversary = -1;
            double y = 0;


            for (int t = 0; t < Classes - 1; t++)
            {
                // Get a class which hasn't lost yet
                for (int i = 0; i < losers.Length; i++)
                    if (!losers[i] && i != currentWinner) { adversary = i; break; }

                // Get the machine for this problem
                KernelSupportVectorMachine machine = this[currentWinner, adversary];

                // Compute the two-class decision problem
                double answer = compute(cache, machine, inputs, out y);

                // Check who won and update
                int lastWinner = currentWinner;
                if ((currentWinner > adversary && answer < 0) ||
                    (currentWinner < adversary && answer > 0))
                {
                    // The adversary has lost
                    losers[adversary] = true;

                    if (probabilistic)
                    {
                        // Decrease adversary likelihood
                        responses[adversary] *= 1.0 - y;

                        // Increase for all other classes
                        for (int i = 0; i < responses.Length; i++)
                            if (i != adversary) responses[i] *= y;
                    }
                    else
                    {
                        responses[adversary] = -y;
                    }
                }
                else
                {
                    // The adversary has won
                    losers[lastWinner] = true;
                    currentWinner = adversary;

                    if (probabilistic)
                    {
                        // Decrease loser likelihood
                        responses[lastWinner] *= y;

                        // Increase for all other classes
                        for (int i = 0; i < responses.Length; i++)
                            if (i != lastWinner) responses[i] *= 1.0 - y;
                    }
                    else
                    {
                        responses[lastWinner] = y;
                    }
                }
            }


            if (!probabilistic)
                responses[currentWinner] = y;

#if DEBUG
            else
            {
                int imax; responses.Max(out imax);
                if (imax != currentWinner)
                    throw new Exception();
            }
#endif

            // Return output for winner class
            output = responses[currentWinner];

            return currentWinner;
        }


        /// <summary>
        ///   Compute SVM output with support vector sharing.
        /// </summary>
        /// 
        private static double compute(Dictionary<double[], double> cache,
            KernelSupportVectorMachine machine, double[] input, out double output)
        {
            output = machine.Threshold;

            for (int i = 0; i < machine.SupportVectors.Length; i++)
            {
                double value;
                if (!cache.TryGetValue(machine.SupportVectors[i], out value))
                {
                    value = machine.Kernel.Function(machine.SupportVectors[i], input);
                    cache.Add(machine.SupportVectors[i], value);
                }

                output += machine.Weights[i] * value;
            }

            if (machine.IsProbabilistic)
            {
                output = machine.Link.Inverse(output);
                return output >= 0.5 ? 1 : -1;
            }

            return output >= 0 ? 1 : -1;
        }

        /// <summary>
        ///   Compute SVM output with concurrent support vector sharing.
        /// </summary>
        /// 
        private static double compute(ConcurrentDictionary<double[], double> cache,
           KernelSupportVectorMachine machine, double[] input)
        {
            double output = machine.Threshold;

            for (int i = 0; i < machine.SupportVectors.Length; i++)
            {
                double value;

                lock (machine.SupportVectors[i])
                {
                    if (!cache.TryGetValue(machine.SupportVectors[i], out value))
                    {
                        value = machine.Kernel.Function(machine.SupportVectors[i], input);
                        cache.TryAdd(machine.SupportVectors[i], value);
                    }
                }

                output += machine.Weights[i] * value;
            }

            if (machine.IsProbabilistic)
            {
                output = machine.Link.Inverse(output);
                return output >= 0.5 ? 1 : -1;
            }

            return output >= 0 ? 1 : -1;
        }


        /// <summary>
        ///   Saves the machine to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the machine is to be serialized.</param>
        /// 
        public void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the machine to a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file to which the machine is to be serialized.</param>
        /// 
        public void Save(string path)
        {
            Save(new FileStream(path, FileMode.Create));
        }

        /// <summary>
        ///   Loads a machine from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static MulticlassSupportVectorMachine Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (MulticlassSupportVectorMachine)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a machine from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the machine is to be deserialized.</param>
        /// 
        /// <returns>The deserialized machine.</returns>
        /// 
        public static MulticlassSupportVectorMachine Load(string path)
        {
            return Load(new FileStream(path, FileMode.Open));
        }
    }
}
