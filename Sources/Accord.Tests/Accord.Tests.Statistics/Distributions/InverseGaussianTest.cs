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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Moving;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Univariate;

    [TestClass()]
    public class InverseGaussianTest
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
        public void ConstructorTest()
        {
            InverseGaussian g = new InverseGaussian(1.2, 4.2);
            Assert.AreEqual(1.2, g.Mean);
            Assert.AreEqual(4.2, g.Shape);

            Assert.AreEqual(0.41142857142857142, g.Variance);
            Assert.AreEqual(0.64142698058981851, g.StandardDeviation);
        }

        [TestMethod()]
        public void ProbabilityFunctionTest()
        {
            InverseGaussian g = new InverseGaussian(1.2, 4.2);

            double expected = 0.363257;
            double actual = g.ProbabilityDensityFunction(0.42);

            Assert.AreEqual(expected, actual, 1e-6);
        }

        [TestMethod()]
        public void CumulativeFunctionTest()
        {
            InverseGaussian g = new InverseGaussian(1.2, 4.2);

            double expected = 0.3520473;
            double actual = g.DistributionFunction(0.42);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(double.IsNaN(actual));
        }


    }
}