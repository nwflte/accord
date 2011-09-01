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

using Accord.Statistics.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Statistics.Kernels;
using Accord.Math;
using System.Diagnostics;

namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for KernelPrincipalComponentAnalysisTest and is intended
    ///to contain all KernelPrincipalComponentAnalysisTest Unit Tests
    ///</summary>
    [TestClass()]
    public class KernelPrincipalComponentAnalysisTest
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
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void TransformTest()
        {
            // Lindsay's tutorial data
            double[,] data = new double[,]
            {
                {2.5,   2.4},
                {0.5,	0.7},
                {2.2,	2.9},
                {1.9,	2.2},
                {3.1,	3.0},
                {2.3,	2.7},
                {2.0,	1.6},
                {1.0,	1.1},
                {1.5,	1.6},
                {1.1,	0.9}
            };

            // Using a linear kernel should be equivalent to standard PCA
            IKernel kernel = new Linear();

            // Create analysis
            KernelPrincipalComponentAnalysis target = new KernelPrincipalComponentAnalysis(data, kernel);

            // Compute
            target.Compute();

            double[,] actual = target.Transform(data, 2);

            // first inversed.. ?
            double[,] expected = new double[,]
            {
                { -0.827970186,  0.175115307 },
                {  1.77758033,  -0.142857227 },
                { -0.992197494, -0.384374989 },
                { -0.274210416, -0.130417207 },
                { -1.67580142,   0.209498461 },
                { -0.912949103, -0.175282444 },
                {  0.099109437,  0.349824698 },
                {  1.14457216,  -0.046417258 },
                {  0.438046137, -0.017764629 },
                {  1.22382056,   0.162675287 },
            };

            // Verify both are equal with 0.001 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.0001));

            // Assert the result equals the transformation of the input
            double[,] result = target.Result;
            double[,] projection = target.Transform(data);
            Assert.IsTrue(Matrix.IsEqual(result, projection, 0.000001));
        }

        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void TransformTest2()
        {
            // Lindsay's tutorial data
            double[,] data = new double[,]
            {
                { 2.5,  2.4},
                { 0.5,  0.7 },
                { 2.2,  2.9 },
                { 1.9,  2.2 },
                { 3.1,  3.0 },
                { 2.3,  2.7 },
                { 2.0,  1.6 },
                { 1.0,  1.1 },
                { 1.5,  1.6 },
                { 1.1,  0.9 }
            };

            // Using a linear kernel should be equivalent to standard PCA
            IKernel kernel = new Linear();

            // Create analysis
            KernelPrincipalComponentAnalysis target = new KernelPrincipalComponentAnalysis(data, kernel);

            // Set the minimum variance threshold to 0.001
            target.Threshold = 0.001;

            // Compute
            target.Compute();

            double[,] actual = target.Transform(data, 1);

            // first inversed.. ?
            double[,] expected = new double[,]
            {
                { -0.827970186 },
                {  1.77758033  },
                { -0.992197494 },
                { -0.274210416 },
                { -1.67580142  },
                { -0.912949103 },
                {  0.099109437 },
                {  1.14457216  },
                {  0.438046137 },
                {  1.22382056  },
            };

            // Verify both are equal with 0.001 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.0001));

            // Assert the result equals the transformation of the input
            double[,] result = target.Result;
            double[,] projection = target.Transform(data);
            Assert.IsTrue(Matrix.IsEqual(result, projection, 0.000001));
        }

        /// <summary>
        ///A test for Transform
        ///</summary>
        [TestMethod()]
        public void TransformTest3()
        {
            // Lindsay's tutorial data
            double[,] data = new double[,]
            {
                {2.5,   2.4},
                {0.5,	0.7},
                {2.2,	2.9},
                {1.9,	2.2},
                {3.1,	3.0},
                {2.3,	2.7},
                {2.0,	1.6},
                {1.0,	1.1},
                {1.5,	1.6},
                {1.1,	0.9}
            };

            // Using a linear kernel should be equivalent to standard PCA
            IKernel kernel = new Linear();

            // Create analysis
            KernelPrincipalComponentAnalysis target = new KernelPrincipalComponentAnalysis(data, kernel);

            // Compute
            target.Compute();

            bool thrown = false;
            try
            {
                double[,] actual = target.Transform(data, 11);
            }
            catch
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        /// <summary>
        ///A test for Revert
        ///</summary>
        [TestMethod()]
        public void RevertTest()
        {
            // Lindsay's tutorial data
            double[,] data = new double[,]
            {
                { 2.5,  2.4 },
                { 0.5,  0.7 },
                { 2.2,  2.9 },
                { 1.9,  2.2 },
                { 3.1,  3.0 },
                { 2.3,  2.7 },
                { 2.0,  1.6 },
                { 1.0,  1.1 },
                { 1.5,  1.6 },
                { 1.1,  0.9 }
            };

            // Using a linear kernel should be equivalent to standard PCA
            IKernel kernel = new Linear();

            // Create analysis
            KernelPrincipalComponentAnalysis target = new KernelPrincipalComponentAnalysis(data, kernel);

            // Compute
            target.Compute();

            // Compute image
            double[,] image = target.Transform(data, 2);

            // Compute preimage
            double[,] preimage = target.Revert(image);

            // Check if preimage equals the original data
            Assert.IsTrue(Matrix.IsEqual(data, preimage, 0.0001));
        }
            
    }
}
