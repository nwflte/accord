// Accord Unit Tests
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

using Accord.MachineLearning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
namespace Accord.Tests.MachineLearning
{


    /// <summary>
    ///This is a test class for CrossvalidationTest and is intended
    ///to contain all CrossvalidationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CrossvalidationTest
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
        ///A test for Cross-validation Constructor
        ///</summary>
        [TestMethod()]
        public void CrossvalidationConstructorTest()
        {

            Accord.Math.Tools.SetupGenerator(0);


            // Example binary data
            double[][] data =
            {
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
                new double[] { -1, -1 }, new double[] {  1, -1 },
                new double[] { -1,  1 }, new double[] {  1,  1 },
            };

            int[] xor = // result of xor
            {
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
                -1,       1,
                 1,      -1,
            };


            // Create a new Cross-validation algorithm passing the data set size and the number of folds
            var crossvalidation = new Crossvalidation<KernelSupportVectorMachine>(data.Length, 3);

            // Define a fitting function using Support Vector Machines
            crossvalidation.Fitting = delegate(int[] trainingSet, int[] validationSet)
            {
                // The trainingSet and validationSet arguments specifies the
                // indices of the original data set to be used as training and
                // validation sets, respectively.
                double[][] trainingInputs = data.Submatrix(trainingSet);
                int[] trainingOutputs = xor.Submatrix(trainingSet);

                double[][] validationInputs = data.Submatrix(validationSet);
                int[] validationOutputs = xor.Submatrix(validationSet);

                // Create a Kernel Support Vector Machine to operate on this set
                var svm = new KernelSupportVectorMachine(new Polynomial(2), 2);

                // Create a training algorithm and learn this set
                var smo = new SequentialMinimalOptimization(svm, trainingInputs, trainingOutputs);

                double trainingError = smo.Run();
                double validationError = smo.ComputeError(validationInputs, validationOutputs);

                // Return a new information structure containing the model and the errors achieved.
                return new CrossvalidationInfo<KernelSupportVectorMachine>(svm, trainingError, validationError);
            };


            // Compute the cross-validation
            var result = crossvalidation.Compute();

            // Get the average training and validation errors
            double errorTraining = result.TrainingMean;
            double errorValidation = result.ValidationMean;

            Assert.AreEqual(3, crossvalidation.K);
            Assert.AreEqual(0, result.TrainingMean);
            Assert.AreEqual(0, result.ValidationMean);

            Assert.AreEqual(3, crossvalidation.Folds.Length);
            Assert.AreEqual(3, crossvalidation.Models.Length);
        }

    }
}
