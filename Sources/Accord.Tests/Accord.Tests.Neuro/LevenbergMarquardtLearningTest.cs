// Accord Unit Tests
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

using Accord.Neuro.Learning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AForge.Neuro;
using Accord.Math;
using AForge.Neuro.Learning;
using Accord.Neuro;
using System;

namespace Accord.Tests.Neuro
{


    /// <summary>
    ///This is a test class for LevenbergMarquardtLearningTest and is intended
    ///to contain all LevenbergMarquardtLearningTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LevenbergMarquardtLearningTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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


        /// <summary>
        ///A test for RunEpoch
        ///</summary>
        [TestMethod()]
        public void RunEpochTest()
        {
            // bipolar data
            double[][] input = new double[4][] {
											new double[] {-1, -1},
											new double[] {-1,  1},
											new double[] { 1, -1},
											new double[] { 1,  1}
										};
            double[][] output = new double[4][] {
											 new double[] {-1},
											 new double[] { 1},
											 new double[] { 1},
											 new double[] {-1}
										 };


            int maxAttempt = 10;
            int attempt = 0;
            bool converged = false;
            while (attempt < maxAttempt && converged == false)
            {
                // create multi-layer neural network
                ActivationNetwork network = new ActivationNetwork(
                       new BipolarSigmoidFunction(2),
                       2, 2, 1);

                // create teacher
                LevenbergMarquardtLearning teacher = new LevenbergMarquardtLearning(network,
                    false, JacobianMethod.ByBackpropagation);

                // set learning rate and momentum
                teacher.LearningRate = 0.1;

                double error = double.MaxValue;
                double learningErrorLimit = 0.2;
                int resetIter = 1000;

                int c = 0;
                while (error >= learningErrorLimit && c < resetIter)
                {
                    error = teacher.RunEpoch(input, output);
                    c++;
                }

                if (error <= learningErrorLimit)
                    converged = true;
            }

            Assert.IsTrue(converged);
        }

        /// <summary>
        ///   Testing Jacobian by Finite Diferences 
        /// </summary>
        [TestMethod()]
        public void RunEpochTest2()
        {
            // bipolar data
            double[][] input = new double[4][] {
											new double[] {-1, -1},
											new double[] {-1,  1},
											new double[] { 1, -1},
											new double[] { 1,  1}
										};
            double[][] output = new double[4][] {
											 new double[] {-1},
											 new double[] { 1},
											 new double[] { 1},
											 new double[] {-1}
										 };


            int maxAttempt = 10;
            int attempt = 0;
            bool converged = false;
            while (attempt < maxAttempt && converged == false)
            {
                // create multi-layer neural network
                ActivationNetwork network = new ActivationNetwork(
                       new BipolarSigmoidFunction(2),
                       2, 2, 1);

                // create teacher
                LevenbergMarquardtLearning teacher = new LevenbergMarquardtLearning(network,
                    false, JacobianMethod.ByFiniteDifferences);

                // set learning rate and momentum
                teacher.LearningRate = 0.1;

                double error = double.MaxValue;
                double learningErrorLimit = 0.2;
                int resetIter = 1000;

                int c = 0;
                while (error >= learningErrorLimit && c < resetIter)
                {
                    error = teacher.RunEpoch(input, output);
                    c++;
                }

                if (error <= learningErrorLimit)
                    converged = true;
            }

            Assert.IsTrue(converged);
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            // Four training samples of the xor function

            // two inputs (x and y)
            double[][] input = new double[4][]
            {
					new double[] {-1, -1},
					new double[] {-1,  1},
					new double[] { 1, -1},
					new double[] { 1,  1}
			};

            // one output (z = x ^ y)
            double[][] output = new double[4][] 
            {
				    new double[] {-1},
				    new double[] { 1},
				    new double[] { 1},
				    new double[] {-1}
            };


            // create multi-layer neural network
            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), // use a bipolar sigmoid activation function
                   2, // two inputs
                   3, // three hidden neurons
                   1  // one output neuron
                   );

            // create teacher
            LevenbergMarquardtLearning teacher = new LevenbergMarquardtLearning(
                network, // the neural network
                false,   // whether or not to use bayesian regularization
                JacobianMethod.ByBackpropagation // jacobian calculation method
                );


            // set learning rate and momentum
            teacher.LearningRate = 0.1;

            // start the supervisioned learning
            for (int i = 0; i < 1000; i++)
            {
                double error = teacher.RunEpoch(input, output);
            }

            // If we reached here, the constructor test has passed.
        }



        [TestMethod()]
        public void JacobianByChainRuleTest()
        {
            // Network with one hidden layer: 2-2-1
            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
			    new double[] {-1, -1},
			    new double[] {-1,  1},
			    new double[] { 1, -1},
			    new double[] { 1,  1}
			};

            double[][] output =
            {
				new double[] {-1},
				new double[] { 1},
				new double[] { 1},
				new double[] {-1}
			};

            Neuron.RandGenerator = Accord.Math.Tools.Random;


            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 2, 1);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30;
            teacher2.LearningRate = 1e30;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            PrivateObject privateTeacher1 = new PrivateObject(teacher1);
            PrivateObject privateTeacher2 = new PrivateObject(teacher2);

            var jacobian1 = (double[][])privateTeacher1.GetField("jacobian");
            var jacobian2 = (double[][])privateTeacher2.GetField("jacobian");

            Assert.AreEqual(jacobian1[0][0], -0.47895513745387097);
            Assert.AreEqual(jacobian1[0][1], -0.05863886707282373);
            Assert.AreEqual(jacobian1[0][2], 0.057751100929897485);
            Assert.AreEqual(jacobian1[0][3], 0.0015185010717608583);

            Assert.AreEqual(jacobian1[7][0], -0.185400783651892);
            Assert.AreEqual(jacobian1[7][1], 0.025575161626462877);
            Assert.AreEqual(jacobian1[7][2], 0.070494677797224889);
            Assert.AreEqual(jacobian1[7][3], 0.037740463822781616);


            Assert.AreEqual(jacobian2[0][0], -0.4789595904719437);
            Assert.AreEqual(jacobian2[0][1], -0.058636153936941729);
            Assert.AreEqual(jacobian2[0][2], 0.057748435491340212);
            Assert.AreEqual(jacobian2[0][3], 0.0015184453425611988);

            Assert.AreEqual(jacobian2[7][0], -0.1854008206574258);
            Assert.AreEqual(jacobian2[7][1], 0.025575150379247645);
            Assert.AreEqual(jacobian2[7][2], 0.070494269423259301);
            Assert.AreEqual(jacobian2[7][3], 0.037740117733922635);


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-4);
                }
            }

        }

        [TestMethod()]
        public void JacobianByChainRuleTest2()
        {
            // Network with two hidden layers: 2-4-3-1
            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
			    new double[] {-1, -1},
			    new double[] {-1,  1},
			    new double[] { 1, -1},
			    new double[] { 1,  1}
			};

            double[][] output =
            {
				new double[] {-1},
				new double[] { 1},
				new double[] { 1},
				new double[] {-1}
			};

            Neuron.RandGenerator = Accord.Math.Tools.Random;


            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 4, 3, 1);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30;
            teacher2.LearningRate = 1e30;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            PrivateObject privateTeacher1 = new PrivateObject(teacher1);
            PrivateObject privateTeacher2 = new PrivateObject(teacher2);

            var jacobian1 = (double[][])privateTeacher1.GetField("jacobian");
            var jacobian2 = (double[][])privateTeacher2.GetField("jacobian");


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-4);
                }
            }

        }

        [TestMethod()]
        public void JacobianByChainRuleTest3()
        {
            // Network with 3 hidden layers: 2-4-3-4-1

            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
			    new double[] {-1, -1},
			    new double[] {-1,  1},
			    new double[] { 1, -1},
			    new double[] { 1,  1}
			};

            double[][] output =
            {
				new double[] {-1},
				new double[] { 1},
				new double[] { 1},
				new double[] {-1}
			};

            Neuron.RandGenerator = Accord.Math.Tools.Random;


            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 4, 3, 4, 1);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30;
            teacher2.LearningRate = 1e30;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            PrivateObject privateTeacher1 = new PrivateObject(teacher1);
            PrivateObject privateTeacher2 = new PrivateObject(teacher2);

            var jacobian1 = (double[][])privateTeacher1.GetField("jacobian");
            var jacobian2 = (double[][])privateTeacher2.GetField("jacobian");


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-4);
                }
            }

        }

        [TestMethod()]
        public void JacobianByChainRuleTest4()
        {
            // Network with no hidden layers: 3-1

            Accord.Math.Tools.SetupGenerator(0);

            double[][] input = 
            {
			    new double[] {-1, -1},
			    new double[] {-1,  1},
			    new double[] { 1, -1},
			    new double[] { 1,  1}
			};

            double[][] output =
            {
				new double[] {-1},
				new double[] { 1},
				new double[] { 1},
				new double[] {-1}
			};

            Neuron.RandGenerator = Accord.Math.Tools.Random;


            ActivationNetwork network = new ActivationNetwork(
                   new BipolarSigmoidFunction(2), 2, 1);

            var teacher1 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByFiniteDifferences);

            var teacher2 = new LevenbergMarquardtLearning(network,
                false, JacobianMethod.ByBackpropagation);

            // Set lambda to lambda max so no iterations are performed
            teacher1.LearningRate = 1e30;
            teacher2.LearningRate = 1e30;

            teacher1.RunEpoch(input, output);
            teacher2.RunEpoch(input, output);

            PrivateObject privateTeacher1 = new PrivateObject(teacher1);
            PrivateObject privateTeacher2 = new PrivateObject(teacher2);

            var jacobian1 = (double[][])privateTeacher1.GetField("jacobian");
            var jacobian2 = (double[][])privateTeacher2.GetField("jacobian");


            for (int i = 0; i < jacobian1.Length; i++)
            {
                for (int j = 0; j < jacobian1[i].Length; j++)
                {
                    double j1 = jacobian1[i][j];
                    double j2 = jacobian2[i][j];

                    Assert.AreEqual(j1, j2, 1e-5);
                }
            }
        }

    }
}
