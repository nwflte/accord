// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Analysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using System;

    [TestClass()]
    public class PrincipalComponentAnalysisTest
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


        // Lindsay's tutorial data
        private static double[,] data = 
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

        public void TransformTest()
        {
            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data);

            // Compute
            target.Compute();

            // Transform
            double[,] actual = target.Transform(data);

            // first inversed.. ?
            double[,] expected = new double[,]
            {
                {  0.827970186, -0.175115307 },
                { -1.77758033,   0.142857227 },
                {  0.992197494,  0.384374989 },
                {  0.274210416,  0.130417207 },
                {  1.67580142,  -0.209498461 },
                {  0.912949103,  0.175282444 },
                { -0.099109437, -0.349824698 },
                { -1.14457216,   0.046417258 },
                { -0.438046137,  0.017764629 },
                { -1.22382056,  -0.162675287 },
            };

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 0.01));

            // Assert the scores equals the transformation of the input
            double[,] result = target.Result;
            Assert.IsTrue(Matrix.IsEqual(result, expected, 0.01));
        }

        [TestMethod()]
        public void TransformTest2()
        {
            // Lindsay's tutorial data
            double[,] datat = data.Transpose();

            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(datat);

            // Compute
            target.Compute();

            // Transform
            double[,] actual = target.Transform(datat);

            // Assert the scores equals the transformation of the input

            double[,] result = target.Result;
            Assert.IsTrue(Matrix.IsEqual(result, actual, 0.01));
        }

        [TestMethod()]
        public void TransformTest3()
        {
            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data);

            // Compute
            target.Compute();

            bool thrown = false;
            try
            {
                double[,] actual = target.Transform(data, 3);
            }
            catch { thrown = true; }

            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void Revert()
        {
            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data);

            // Compute
            target.Compute();

            // Transform
            double[,] image = target.Transform(data);

            // Reverse
            double[,] actual = target.Revert(image);

            // Verify both are equal with 0.01 tolerance value
            Assert.IsTrue(Matrix.IsEqual(actual, data, 0.01));
        }

        [TestMethod]
        public void ExceptionTest()
        {
            double[,] data = 
            {
                { 1, 2 },
                { 5, 2 },
                { 2, 2 },
                { 4, 2 },
            };


            PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(data, AnalysisMethod.Standardize);

            bool thrown = false;

            try { pca.Compute(); }
            catch (ArithmeticException ex)
            {
                ex.ToString();
                thrown = true;
            }

            // Assert that an appropriate exception has been
            //   thrown in the case of a constant variable.
            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        [DeploymentItem("Accord.Statistics.dll")]
        public void adjustTest()
        {
            PrincipalComponentAnalysis target = new PrincipalComponentAnalysis(data, AnalysisMethod.Standardize);

            double[,] expected =
            {
                {  0.87874523495823,   0.578856809114491 },
                { -1.66834240260186,  -1.42942191638476  },
                {  0.496682089324217,  1.16952702249663  },
                {  0.114618943690204,  0.342588723761638 },
                {  1.64287152622626,   1.28766106517305  },
                {  0.624036471202221,  0.933258937143772 },
                {  0.241973325568208, -0.366215532296923 },
                { -1.03157049321184,  -0.956885745679056 },
                { -0.394798583821814, -0.366215532296923 },
                { -0.904216111333831, -1.19315383103191  }
            };


            double[,] actual = target.Adjust(data, false);

            Assert.IsTrue(expected.IsEqual(actual, 0.00001));
            Assert.AreNotEqual(data, actual);

            actual = target.Adjust(data, true);
            Assert.IsTrue(expected.IsEqual(actual, 0.00001));
            Assert.AreEqual(data, actual);
        }

    }
}
