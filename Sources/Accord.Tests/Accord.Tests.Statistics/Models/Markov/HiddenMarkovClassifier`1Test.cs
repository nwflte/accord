// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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

namespace Accord.Tests.Statistics
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Distributions.Multivariate;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Math;
    using System;


    [TestClass()]
    public class GenericSequenceClassifierTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void LearnTest1()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a univariate sequence and the same sequence backwards.
            double[][] sequences = new double[][] 
            {
                new double[] { 0,1,2,3,4 }, // This is the first  sequence with label = 0
                new double[] { 4,3,2,1,0 }, // This is the second sequence with label = 1
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };

            // Creates a sequence classifier containing 2 hidden Markov Models
            //  with 2 states and an underlying Normal distribution as density.
            NormalDistribution density = new NormalDistribution();
            var classifier = new HiddenMarkovClassifier<NormalDistribution>(2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<NormalDistribution>(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<NormalDistribution>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double likelihood1, likelihood2;

            // Try to classify the first sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out likelihood1);

            // Try to classify the second sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out likelihood2);

            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);


            Assert.AreEqual(-13.271981026832929, logLikelihood, 1e-10);
            Assert.AreEqual(0.99999791320102149, likelihood1, 1e-10);
            Assert.AreEqual(0.99999791320102149, likelihood2, 1e-10);
            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(likelihood1));
            Assert.IsFalse(double.IsNaN(likelihood2));
        }

        [TestMethod()]
        public void LearnTest2()
        {
            // Creates a new Hidden Markov Model with 10 states
            var initial = new MultivariateNormalDistribution(3);
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(2, new Ergodic(10), initial);

            Assert.AreEqual(3, classifier.Models[0].Dimension);
            Assert.AreEqual(3, classifier.Models[1].Dimension);

            Assert.AreEqual(2, classifier.Models.Length);

            for (int i = 0; i < 2; i++)
            {
                var model = classifier.Models[i];
                Assert.AreEqual(3, model.Dimension);
                Assert.AreEqual(10, model.States);
                Assert.AreEqual(10, model.Emissions.Length);

                for (int j = 0; j < model.Emissions.Length; j++)
                {
                    var distribution = model.Emissions[j];
                    Assert.IsNotNull(distribution);
                    Assert.AreEqual(distribution.Dimension, 3);
                }
            }
        }

        [TestMethod()]
        public void LearnTest3()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0 },
                    new double[] { 1 },
                    new double[] { 2 },
                    new double[] { 3 },
                    new double[] { 4 },
                }, 

                new double[][]
                {
                     // This is the second sequence with label = 1
                    new double[] { 4 },
                    new double[] { 3 },
                    new double[] { 2 },
                    new double[] { 1 },
                    new double[] { 0 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };

            // Creates a sequence classifier containing 2 hidden Markov Models
            //  with 2 states and an underlying Normal distribution as density.
            MultivariateNormalDistribution density = new MultivariateNormalDistribution(1);
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<MultivariateNormalDistribution>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double likelihood1, likelihood2;

            // Try to classify the first sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out likelihood1);

            // Try to classify the second sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out likelihood2);

            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);

            Assert.AreEqual(-13.271981026832929, logLikelihood, 1e-14);
            Assert.AreEqual(0.99999791320102149, likelihood1, 1e-15);
            Assert.AreEqual(0.99999791320102149, likelihood2, 1e-15);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(likelihood1));
            Assert.IsFalse(double.IsNaN(likelihood2));
        }

        [TestMethod()]
        public void LearnTest4()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0 },
                    new double[] { 1 },
                    new double[] { 2 },
                    new double[] { 3 },
                    new double[] { 4 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 4 },
                    new double[] { 3 },
                    new double[] { 2 },
                    new double[] { 1 },
                    new double[] { 0 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };


            // Create a mixture of two 1-dimensional normal distributions (by default,
            // initialized with zero mean and unit covariance matrices).
            var density = new MultivariateMixture<MultivariateNormalDistribution>(
                new MultivariateNormalDistribution(1),
                new MultivariateNormalDistribution(1));

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<MultivariateMixture<MultivariateNormalDistribution>>(
                2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateMixture<MultivariateNormalDistribution>>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<MultivariateMixture<MultivariateNormalDistribution>>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double likelihood1, likelihood2;

            // Try to classify the 1st sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out likelihood1);

            // Try to classify the 2nd sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out likelihood2);


            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);

            Assert.AreEqual(-13.271981026832933, logLikelihood, 1e-10);
            Assert.AreEqual(0.99999791320102149, likelihood1, 1e-10);
            Assert.AreEqual(0.99999791320102149, likelihood2, 1e-10);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(likelihood1));
            Assert.IsFalse(double.IsNaN(likelihood2));
        }

        [TestMethod()]
        public void LearnTest5()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0, 1 },
                    new double[] { 1, 2 },
                    new double[] { 2, 3 },
                    new double[] { 3, 4 },
                    new double[] { 4, 5 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 4,  3 },
                    new double[] { 3,  2 },
                    new double[] { 2,  1 },
                    new double[] { 1,  0 },
                    new double[] { 0, -1 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };


            var density = new MultivariateNormalDistribution(2);

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(
                2, new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<MultivariateNormalDistribution>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,

                    FittingOptions = new NormalOptions() { Diagonal = true }
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double logLikelihood1, logLikelihood2;

            // Try to classify the 1st sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out logLikelihood1);

            // Try to classify the 2nd sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out logLikelihood2);


            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);

            Assert.AreEqual(-24.560599651649841, logLikelihood, 1e-10);
            Assert.AreEqual(0.99999999998806466, logLikelihood1, 1e-10);
            Assert.AreEqual(0.99999999998806466, logLikelihood2, 1e-10);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(logLikelihood1));
            Assert.IsFalse(double.IsNaN(logLikelihood2));
        }

        [TestMethod()]
        public void LearnTest6()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.
            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0, 1 },
                    new double[] { 1, 2 },
                    new double[] { 2, 3 },
                    new double[] { 3, 4 },
                    new double[] { 4, 5 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 4,  3 },
                    new double[] { 3,  2 },
                    new double[] { 2,  1 },
                    new double[] { 1,  0 },
                    new double[] { 0, -1 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };


            var density = new MultivariateNormalDistribution(2);

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(
                2, new Custom(new double[2, 2], new double[2]), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<MultivariateNormalDistribution>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,

                    FittingOptions = new NormalOptions() { Diagonal = true }
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double response1, response2;

            // Try to classify the 1st sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out response1);

            // Try to classify the 2nd sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out response2);

            Assert.AreEqual(double.NegativeInfinity, logLikelihood);
            Assert.AreEqual(0, response1);
            Assert.AreEqual(0, response2);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(response1));
            Assert.IsFalse(double.IsNaN(response2));
        }

        [TestMethod()]
        public void LearnTest7()
        {
            // Create a Continuous density Hidden Markov Model Sequence Classifier
            // to detect a multivariate sequence and the same sequence backwards.

            double[][][] sequences = new double[][][]
            {
                new double[][] 
                { 
                    // This is the first  sequence with label = 0
                    new double[] { 0, 1 },
                    new double[] { 1, 2 },
                    new double[] { 2, 3 },
                    new double[] { 3, 4 },
                    new double[] { 4, 5 },
                }, 

                new double[][]
                {
                        // This is the second sequence with label = 1
                    new double[] { 4,  3 },
                    new double[] { 3,  2 },
                    new double[] { 2,  1 },
                    new double[] { 1,  0 },
                    new double[] { 0, -1 },
                }
            };

            // Labels for the sequences
            int[] labels = { 0, 1 };


            var initialDensity = new MultivariateNormalDistribution(2);

            // Creates a sequence classifier containing 2 hidden Markov Models with 2 states
            // and an underlying multivariate mixture of Normal distributions as density.
            var classifier = new HiddenMarkovClassifier<MultivariateNormalDistribution>(
                classes: 2, topology: new Forward(2), initial: initialDensity);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution>(
                classifier,

                // Train each model until the log-likelihood changes less than 0.0001
                modelIndex => new BaumWelchLearning<MultivariateNormalDistribution>(
                    classifier.Models[modelIndex])
                {
                    Tolerance = 0.0001,
                    Iterations = 0,

                    FittingOptions = new NormalOptions()
                    {
                        Diagonal = true,      // only diagonal covariance matrices
                        Regularization = 1e-5 // avoid non-positive definite errors
                    }
                }
            );

            // Train the sequence classifier using the algorithm
            double logLikelihood = teacher.Run(sequences, labels);


            // Calculate the probability that the given
            //  sequences originated from the model
            double likelihood, likelihood2;

            // Try to classify the 1st sequence (output should be 0)
            int c1 = classifier.Compute(sequences[0], out likelihood);

            // Try to classify the 2nd sequence (output should be 1)
            int c2 = classifier.Compute(sequences[1], out likelihood2);


            Assert.AreEqual(0, c1);
            Assert.AreEqual(1, c2);

            Assert.AreEqual(-24.560663315259973, logLikelihood, 1e-10);
            Assert.AreEqual(0.99999999998805045, likelihood, 1e-10);
            Assert.AreEqual(0.99999999998805045, likelihood2, 1e-10);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(likelihood));
            Assert.IsFalse(double.IsNaN(likelihood2));
        }

        [TestMethod()]
        public void LearnTest8()
        {
            // Declare some testing data
            double[][] inputs = new double[][]
            {
                new double[] { 0,0,1,2 },     // Class 0
                new double[] { 0,1,1,2 },     // Class 0
                new double[] { 0,0,0,1,2 },   // Class 0
                new double[] { 0,1,2,2,2 },   // Class 0

                new double[] { 2,2,1,0 },     // Class 1
                new double[] { 2,2,2,1,0 },   // Class 1
                new double[] { 2,2,2,1,0 },   // Class 1
                new double[] { 2,2,2,2,1 },   // Class 1
            };

            int[] outputs = new int[]
            {
                0,0,0,0, // First four sequences are of class 0
                1,1,1,1, // Last four sequences are of class 1
            };


            // We are trying to predict two different classes
            int classes = 2;

            // Each sequence may have up to 3 symbols (0,1,2)
            int symbols = 3;

            // Nested models will have 3 states each
            int[] states = new int[] { 3, 3 };

            // Creates a new Hidden Markov Model Classifier with the given parameters
            var classifier = HiddenMarkovClassifier.CreateGeneric(classes, states, symbols);


            // Create a new learning algorithm to train the sequence classifier
            var teacher = new HiddenMarkovClassifierLearning<GeneralDiscreteDistribution>(classifier,

                // Train each model until the log-likelihood changes less than 0.001
                modelIndex => new BaumWelchLearning<GeneralDiscreteDistribution>(classifier.Models[modelIndex])
                {
                    Tolerance = 0.001,
                    Iterations = 0
                }
            );

            // Enable support for sequence rejection
            teacher.Rejection = true;

            // Train the sequence classifier using the algorithm
            double likelihood = teacher.Run(inputs, outputs);


            var threshold = classifier.Threshold;

            Assert.AreEqual(classifier.Models[0].Transitions[0, 0], threshold.Transitions[0, 0], 1e-10);
            Assert.AreEqual(classifier.Models[0].Transitions[1, 1], threshold.Transitions[1, 1], 1e-10);
            Assert.AreEqual(classifier.Models[0].Transitions[2, 2], threshold.Transitions[2, 2], 1e-10);

            Assert.AreEqual(classifier.Models[1].Transitions[0, 0], threshold.Transitions[3, 3], 1e-10);
            Assert.AreEqual(classifier.Models[1].Transitions[1, 1], threshold.Transitions[4, 4], 1e-10);
            Assert.AreEqual(classifier.Models[1].Transitions[2, 2], threshold.Transitions[5, 5], 1e-10);

            for (int i = 0; i < 3; i++)
                for (int j = 3; j < 6; j++)
                    Assert.AreEqual(Double.NegativeInfinity, threshold.Transitions[i, j]);

            for (int i = 3; i < 6; i++)
                for (int j = 0; j < 3; j++)
                    Assert.AreEqual(Double.NegativeInfinity, threshold.Transitions[i, j]);

            Assert.IsFalse(Matrix.HasNaN(threshold.Transitions));


            classifier.Sensitivity = 0.5;

            // Will assert the models have learned the sequences correctly.
            for (int i = 0; i < inputs.Length; i++)
            {
                int expected = outputs[i];
                int actual = classifier.Compute(inputs[i], out likelihood);
                Assert.AreEqual(expected, actual);
            }


            double[] r0 = new double[] { 1, 1, 0, 0, 2 };


            double logRejection;
            int c = classifier.Compute(r0, out logRejection);

            Assert.AreEqual(-1, c);
            Assert.AreEqual(0.99893048690086783, logRejection);
            Assert.IsFalse(double.IsNaN(logRejection));

            logRejection = threshold.Evaluate(r0);
            Assert.AreEqual(-4.7048235516322334, logRejection, 1e-10);
            Assert.IsFalse(double.IsNaN(logRejection));

            threshold.Decode(r0, out logRejection);
            Assert.AreEqual(-7.0705785431547579, logRejection, 1e-10);
            Assert.IsFalse(double.IsNaN(logRejection));

            foreach (var model in classifier.Models)
            {
                double[,] A = model.Transitions;

                for (int i = 0; i < A.GetLength(0); i++)
                {
                    double[] row = A.Exp().GetRow(i);
                    double sum = row.Sum();
                    Assert.AreEqual(1, sum, 1e-10);
                }
            }
            {
                double[,] A = classifier.Threshold.Transitions;

                for (int i = 0; i < A.GetLength(0); i++)
                {
                    double[] row = A.GetRow(i);
                    double sum = row.Exp().Sum();
                    Assert.AreEqual(1, sum, 1e-6);
                }
            }
        }

    }
}
