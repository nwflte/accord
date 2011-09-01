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
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for PolynomialTest and is intended
    ///to contain all PolynomialTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PolynomialTest
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
        ///A test for Distance
        ///</summary>
        [TestMethod()]
        public void DistanceTest()
        {
            Polynomial target = new Polynomial(1);

            double[] x = new double[] { 1, 1 };
            double[] y = new double[] { 1, 1 };

            double expected = 0;
            double actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual);


            x = new double[] { 0.5, 2.0 };
            y = new double[] { 1.3, -0.2 };

            expected = 5.48;
            actual = target.Distance(x, y);

            Assert.AreEqual(expected, actual);


            target = new Polynomial(3);

            x = new double[] { 9.4, 22.1 };
            y = new double[] { -6.21, 4 };

            expected = 571.2821;
            actual = target.Distance(x, y);
            Assert.AreEqual(expected, actual, 0.0001);
        }

        /// <summary>
        ///A test for Function
        ///</summary>
        [TestMethod()]
        public void FunctionTest()
        {
            Polynomial target = new Polynomial(1, 0);

            double[] x = new double[] { 1, 1 };
            double[] y = new double[] { 1, 1 };

            double expected = 2;
            double actual = target.Function(x, y);
            Assert.AreEqual(expected, actual);


            x = new double[] { 0.5, 2.0 };
            y = new double[] { 1.3, -0.2 };

            expected = 0.25;
            actual = target.Function(x, y);

            Assert.AreEqual(expected, actual);


            target = new Polynomial(3, 0);

            x = new double[] { 9.4, 22.1 };
            y = new double[] { -6.21, 4 };

            expected = 27070.26085757601;
            actual = target.Function(x, y);
            Assert.AreEqual(expected, actual, 0.0001);
        }
    }
}
