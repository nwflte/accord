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
using Accord.Math;
namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for LinearDiscriminantAnalysisTest and is intended
    ///to contain all LinearDiscriminantAnalysisTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LinearDiscriminantAnalysisTest
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


        [TestMethod()]
        public void ComputeTest()
        {
            double[,] inputs;
            LinearDiscriminantAnalysis lda;
            createAnalysis(out inputs, out lda);

            // Compute the analysis
            lda.Compute();

            Assert.IsTrue(Matrix.IsEqual(lda.Classes[0].Scatter,
                new double[,] { { 0.80, -0.40 }, { -0.40, 2.64 } }, 0.01));

            Assert.IsTrue(Matrix.IsEqual(lda.Classes[1].Scatter,
                new double[,] { { 1.84, -0.04 }, { -0.04, 2.64 } }, 0.01));

            Assert.IsTrue(Matrix.IsEqual(lda.ScatterBetweenClass,
                new double[,] { { 29.16, 21.60 }, { 21.60, 16.00 } }, 0.01));

            Assert.IsTrue(Matrix.IsEqual(lda.ScatterWithinClass,
                new double[,] { { 2.64, -0.44 }, { -0.44, 5.28 } }, 0.01));
        }

        [TestMethod()]
        public void ProjectionTest()
        {
            double[,] inputs;
            LinearDiscriminantAnalysis lda;
            createAnalysis(out inputs, out lda);

            // Compute the analysis
            lda.Compute();

            // Project the input data into discriminant space
            double[,] projection = lda.Transform(inputs);

            Assert.AreEqual(projection[0, 0], 4.4273255813953485);
            Assert.AreEqual(projection[0, 1], 1.9629629629629628);
            Assert.AreEqual(projection[1, 0], 3.7093023255813953);
            Assert.AreEqual(projection[1, 1], -2.5185185185185186);
            Assert.AreEqual(projection[2, 0], 3.2819767441860463);
            Assert.AreEqual(projection[2, 1], -1.5185185185185186);
            Assert.AreEqual(projection[3, 0], 5.5639534883720927);
            Assert.AreEqual(projection[3, 1], -3.7777777777777777);
            Assert.AreEqual(projection[4, 0], 5.7093023255813957);
            Assert.AreEqual(projection[4, 1], -1.0370370370370372);
            Assert.AreEqual(projection[5, 0], 13.273255813953488);
            Assert.AreEqual(projection[5, 1], -3.3333333333333339);
            Assert.AreEqual(projection[6, 0], 9.4186046511627914);
            Assert.AreEqual(projection[6, 1], -3.5555555555555554);
            Assert.AreEqual(projection[7, 0], 11.136627906976745);
            Assert.AreEqual(projection[7, 1], 1.6666666666666661);
            Assert.AreEqual(projection[8, 0], 10.991279069767442);
            Assert.AreEqual(projection[8, 1], -1.0740740740740744);
            Assert.AreEqual(projection[9, 0], 13.418604651162791);
            Assert.AreEqual(projection[9, 1], -0.59259259259259345);

            // Assert the result equals the transformation of the input
            double[,] result = lda.Result;
            Assert.IsTrue(Matrix.IsEqual(result, projection));
        }

        [TestMethod()]
        public void ComputeTest2()
        {

            double[,] inputs;
            LinearDiscriminantAnalysis lda;
            createAnalysis(out inputs, out lda);

            // Compute the analysis
            lda.Compute();

            Assert.AreEqual(2, lda.Classes.Count);
            Assert.AreEqual(3.0, lda.Classes[0].Mean[0]);
            Assert.AreEqual(3.6, lda.Classes[0].Mean[1]);
            Assert.AreEqual(5, lda.Classes[0].Indices.Length);

            Assert.AreEqual(0, lda.Classes[0].Indices[0]);
            Assert.AreEqual(1, lda.Classes[0].Indices[1]);
            Assert.AreEqual(2, lda.Classes[0].Indices[2]);
            Assert.AreEqual(3, lda.Classes[0].Indices[3]);
            Assert.AreEqual(4, lda.Classes[0].Indices[4]);

            Assert.AreEqual(5, lda.Classes[1].Indices[0]);
            Assert.AreEqual(6, lda.Classes[1].Indices[1]);
            Assert.AreEqual(7, lda.Classes[1].Indices[2]);
            Assert.AreEqual(8, lda.Classes[1].Indices[3]);
            Assert.AreEqual(9, lda.Classes[1].Indices[4]);

            Assert.AreEqual(2, lda.Discriminants.Count);
            Assert.AreEqual(15.65685019206146, lda.Discriminants[0].Eigenvalue);
            Assert.AreEqual(-0.00000000000000, lda.Discriminants[1].Eigenvalue, 1e-15);

            Assert.AreEqual(5.7, lda.Means[0]);
            Assert.AreEqual(5.6, lda.Means[1]);
        }




        private static void createAnalysis(out double[,] inputs, out LinearDiscriminantAnalysis lda)
        {
            // Create some sample input data

            // This is the same data used in the example by Gutierrez-Osuna
            // http://research.cs.tamu.edu/prism/lectures/pr/pr_l10.pdf

            inputs =
            new double[,]{
                {  4,  1 }, // Class 1
                {  2,  4 },
                {  2,  3 },
                {  3,  6 },
                {  4,  4 },

                {  9, 10 }, // Class 2
                {  6,  8 },
                {  9,  5 },
                {  8,  7 },
                { 10,  8 }
            };

            int[] output = 
            {
                1, 1, 1, 1, 1, // Class labels for the input vectors
                2, 2, 2, 2, 2
            };

            // Create a new Linear Discriminant Analysis object
            lda = new LinearDiscriminantAnalysis(inputs, output);
        }

    }
}
