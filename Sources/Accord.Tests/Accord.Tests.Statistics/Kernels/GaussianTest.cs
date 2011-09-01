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

using Accord.Statistics.Kernels;
using Accord.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for GaussianTest and is intended
    ///to contain all GaussianTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GaussianTest
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

        [TestMethod]
        public void GaussianFunctionTest()
        {
            IKernel gaussian = new Gaussian(1);

            double[] x = { 1, 1 };
            double[] y = { 1, 1 };

            double actual = gaussian.Function(x, y);

            double expected = 1;

            Assert.AreEqual(expected, actual);


            gaussian = new Gaussian(11.5);

            x = new double[] { 0.2, 5 };
            y = new double[] { 3, 0.7 };

            actual = gaussian.Function(x, y);
            expected = 0.9052480234;

            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod]
        public void GaussianDistanceTest()
        {
            IDistance gaussian = new Gaussian(1);

            double[] x = { 1, 1 };
            double[] y = { 1, 1 };

            double actual = gaussian.Distance(x, y);
            double expected = 0;

            Assert.AreEqual(expected, actual);


            gaussian = new Gaussian(11.5);

            x = new double[] { 0.2, 0.5 };
            y = new double[] { 0.3, -0.7 };

            actual = gaussian.Distance(x, y);
            expected = 341.46531595796711;

            Assert.AreEqual(expected, actual, 1e-10);
        }


        /// <summary>
        ///A test for Function
        ///</summary>
        [TestMethod()]
        public void FunctionTest()
        {
            double sigma = 0.1;
            Gaussian target = new Gaussian(sigma);
            double[] x = { 2.0, 3.1, 4.0 };
            double[] y = { 2.0, 3.1, 4.0 };
            double expected = 1;
            double actual;

            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);

            actual = target.Function(x, x);
            Assert.AreEqual(expected, actual);

            actual = target.Function(y, y);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GammaSigmaTest()
        {
            Gaussian gaussian = new Gaussian(1);
            double expected, actual, gamma, sigma;

            expected = 0.01;
            gaussian.Sigma = expected;
            gamma = gaussian.Gamma;

            gaussian.Gamma = gamma;
            actual = gaussian.Sigma;

            Assert.AreEqual(expected, actual);


            expected = 0.01;
            gaussian.Gamma = expected;
            sigma = gaussian.Sigma;

            gaussian.Sigma = sigma;
            actual = gaussian.Gamma;

            Assert.AreEqual(expected, actual,1e-12);
        }
    }
}
