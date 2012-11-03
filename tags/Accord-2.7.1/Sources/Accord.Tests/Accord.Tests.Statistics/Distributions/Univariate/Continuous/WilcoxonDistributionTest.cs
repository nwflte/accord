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
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;


    [TestClass()]
    public class WilcoxonDistributionTest
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
        public void WScore()
        {
            int[] signs = { -1, -1, +1, -1, +1, +1, +1, +1, +1, +1, +1, +1 };
            double[] ranks = { 1, 2, 3, 4, 5.5, 5.5, 7, 8, 9, 10, 11, 12 };
            double[] diffs = { 0.1, 0.2, 0.3, 0.6, 1.5, 1.5, 1.8, 2.0, 2.1, 2.3, 2.6, 12.4 };

            double wm, wp, w;
            {
                double expected = 7;
                wm = WilcoxonDistribution.WNegative(signs, ranks);
                Assert.AreEqual(expected, wm);
            }

            {
                double expected = 71;
                wp = WilcoxonDistribution.WPositive(signs, ranks);
                Assert.AreEqual(expected, wp);
            }

            {
                double expected = 7;
                w = WilcoxonDistribution.WMinimum(signs, ranks);
                Assert.AreEqual(expected, w);
            }

            {
                double n = signs.Length;
                double total = wm + wp;
                double expected = (n * (n + 1)) / 2;
                Assert.AreEqual(expected, total);
            }
        }

        [TestMethod()]
        public void ProbabilityTest()
        {
            // Example from https://onlinecourses.science.psu.edu/stat414/node/319

            double[] ranks = { 1, 2, 3 };

            WilcoxonDistribution target = new WilcoxonDistribution(ranks);

            double[] expected = { 1 / 8.0, 1 / 8.0, 1 / 8.0, 2 / 8.0, 1 / 8.0, 1 / 8.0, 1 / 8.0 };

            for (int i = 0; i < expected.Length; i++)
            {
                // P(W=i)
                double actual = target.ProbabilityDensityFunction(i);
                Assert.AreEqual(expected[i], actual);
            }
        }

        [TestMethod()]
        public void CumulativeTest()
        {
            // Example from https://onlinecourses.science.psu.edu/stat414/node/319

            double[] ranks = { 1, 2, 3 };

            WilcoxonDistribution target = new WilcoxonDistribution(ranks);

            double[] probabilities = { 0.0, 1 / 8.0, 1 / 8.0, 1 / 8.0, 2 / 8.0, 1 / 8.0, 1 / 8.0, 1 / 8.0 };
            double[] expected = Accord.Math.Matrix.CumulativeSum(probabilities);

            for (int i = 0; i < expected.Length; i++)
            {
                // P(W<=i)
                double actual = target.DistributionFunction(i);
                Assert.AreEqual(expected[i], actual);
            }
        }

    }
}
