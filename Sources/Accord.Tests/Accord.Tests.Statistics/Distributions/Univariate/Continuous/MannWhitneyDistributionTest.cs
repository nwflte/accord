// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math;

    [TestClass()]
    public class MannWhitneyDistributionTest
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
        public void ProbabilityDensityFunctionTest()
        {
            double[] ranks = { 1, 2, 3, 4, 5 };

            int n1 = 2;
            int n2 = 3;
            int N = n1 + n2;

            Assert.AreEqual(N, ranks.Length);


            var target = new MannWhitneyDistribution(ranks, n1, n2);

            // Number of possible combinations is 5!/(3!2!) = 10.
            int nc = (int)Special.Binomial(5, 3);

            Assert.AreEqual(10, nc);

            double[] expected = { 0.1, 0.1, 0.2, 0.2, 0.2, 0.1, 0.1, 0.0, 0.0 };
            double sum = 0;

            for (int i = 0; i < expected.Length; i++)
            {
                // P(U=i)
                double actual = target.ProbabilityDensityFunction(i);
                Assert.AreEqual(expected[i], actual);
                sum += actual;
            }

            Assert.AreEqual(1, sum);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            double[] ranks = { 1, 2, 3, 4, 5 };

            int n1 = 2;
            int n2 = 3;
            int N = n1 + n2;

            Assert.AreEqual(N, ranks.Length);


            var target = new MannWhitneyDistribution(ranks, n1, n2);

            // Number of possible combinations is 5!/(3!2!) = 10.
            int nc = (int)Special.Binomial(5, 3);

            Assert.AreEqual(10, nc);

            double[] expected = { 0.0, 0.1, 0.1, 0.2, 0.2, 0.2, 0.1, 0.1, 0.0, 0.0 };
            expected = expected.CumulativeSum();

            for (int i = 0; i < expected.Length; i++)
            {
                // P(U<=i)
                double actual = target.DistributionFunction(i);
                Assert.AreEqual(expected[i], actual,1e-10);
            }

        }
    }
}
