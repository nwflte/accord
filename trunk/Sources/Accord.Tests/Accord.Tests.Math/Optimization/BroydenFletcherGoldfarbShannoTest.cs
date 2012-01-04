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

using Accord.Math.Optimization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace Accord.Tests.Math
{


    /// <summary>
    ///This is a test class for LBFGSTest and is intended
    ///to contain all LBFGSTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BroydenFletcherGoldfarbShannoTest
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
        ///A test for lbfgs
        ///</summary>
        [TestMethod()]
        public void lbfgsTest()
        {
            Func<double[], double> f = rosenbrockFunction;
            Func<double[], double[]> g = rosenbrockGradient;

            Assert.AreEqual(104, f(new[] { -1.0, 2.0 }));


            int n = 2; // number of variables
            double[] initial = { -1.2, 1 };

            BroydenFletcherGoldfarbShanno lbfgs = new BroydenFletcherGoldfarbShanno(n, f, g);

            double actual = lbfgs.Minimize(initial);
            double expected = 0;
            Assert.AreEqual(expected, actual, 1e-10);

            double[] result = lbfgs.Solution;

            Assert.AreEqual(49, lbfgs.Evaluations);
            Assert.AreEqual(40, lbfgs.Iterations);
            Assert.AreEqual(0.99999999999963229, result[0]);
            Assert.AreEqual(0.99999999999924027, result[1]);

            double y = f(result);
            double[] d = g(result);

            Assert.AreEqual(1.9432410039142452E-25, y);
            Assert.AreEqual(0.0000000000089901419642010907, d[0]);
            Assert.AreEqual(-0.0000000000048627768478581856, d[1]);
        }



        // The famous Rosenbrock test function.
        public static double rosenbrockFunction(double[] x)
        {
            double a = x[1] - x[0] * x[0];
            double b = 1 - x[0];
            return b * b + 100 * a * a;
        }

        // Gradient of the Rosenbrock test function.
        public static double[] rosenbrockGradient(double[] x)
        {
            double a = x[1] - x[0] * x[0];
            double b = 1 - x[0];

            double f0 = -2 * b - 400 * x[0] * a;
            double f1 = 200 * a;

            return new[] { f0, f1 };
        }

    }
}
