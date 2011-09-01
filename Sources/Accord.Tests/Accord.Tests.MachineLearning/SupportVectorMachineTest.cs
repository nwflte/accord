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

using Accord.MachineLearning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Statistics.Kernels;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;

namespace Accord.Tests.MachineLearning
{


    /// <summary>
    ///This is a test class for SupportVectorMachineTest and is intended
    ///to contain all SupportVectorMachineTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SupportVectorMachineTest
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
        ///A test for Compute
        ///</summary>
        [TestMethod()]
        public void ComputeTest()
        {
            // Example AND problem
            double[][] inputs =
            {
                new double[] { 0, 0 }, // 0 and 0: 0 (label -1)
                new double[] { 0, 1 }, // 0 and 1: 0 (label -1)
                new double[] { 1, 0 }, // 1 and 0: 0 (label -1)
                new double[] { 1, 1 }  // 1 and 1: 1 (label +1)
            };

            // Dichotomy SVM outputs should be given as [-1;+1]
            int[] labels =
            {
                // 0,  0,  0, 1
                  -1, -1, -1, 1
            };

            // Create a Support Vector Machine for the given inputs
            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Gaussian(0.1), inputs[0].Length);

            // Instantiate a new learning algorithm for SVMs
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(machine, inputs, labels);

            // Set up the learning algorithm
            smo.Complexity = 1.0;

            // Run
            double error = smo.Run();

            Assert.AreEqual(System.Math.Sign(machine.Compute(inputs[0])), -1);
            Assert.AreEqual(System.Math.Sign(machine.Compute(inputs[1])), -1);
            Assert.AreEqual(System.Math.Sign(machine.Compute(inputs[2])), -1);
            Assert.AreEqual(System.Math.Sign(machine.Compute(inputs[3])), 1);

            Assert.AreEqual(error, 0);

            Assert.AreEqual(machine.Threshold, -0.6669921875);
            Assert.AreEqual(machine.Weights[0], -0.3330078125);
            Assert.AreEqual(machine.Weights[1], -0.333984375);
            Assert.AreEqual(machine.Weights[2], -0.3330078125);
            Assert.AreEqual(machine.Weights[3], +1.0);
        }

        /// <summary>
        ///A test for Compute
        ///</summary>
        [TestMethod()]
        public void ComputeTest2()
        {
            // XOR
            double[][] inputs =
            {
                new double[] { 0, 0},
                new double[] { 0, 1},
                new double[] { 1, 0},
                new double[] { 1, 1}
            };

            int[] labels =
            {
                -1,
                 1,
                 1,
                -1
            };

            KernelSupportVectorMachine machine = new KernelSupportVectorMachine(new Gaussian(0.1), inputs[0].Length);
            SequentialMinimalOptimization smo = new SequentialMinimalOptimization(machine, inputs, labels);

            smo.Complexity = 1;
            double error = smo.Run();

            Assert.AreEqual(machine.Compute(inputs[0]), -1);
            Assert.AreEqual(machine.Compute(inputs[1]),  1);
            Assert.AreEqual(machine.Compute(inputs[2]),  1);
            Assert.AreEqual(machine.Compute(inputs[3]), -1);

            Assert.AreEqual(error, 0);

        }


    }
}
