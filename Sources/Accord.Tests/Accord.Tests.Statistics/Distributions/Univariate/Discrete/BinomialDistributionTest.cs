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
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class BinomialDistributionTest
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
        public void ProbabilityMassFunctionTest()
        {
            double[] expected =
            {
                0.0, 0.0260838446329553, 0.104335628936830,  0.198238170750635,
                0.237886375826922,  0.202203904741285,  0.129410809619744,
                0.0647055601029058, 0.0258822861585249, 0.00841176318971187,
                0.00224314223412042 
            };

            int trials = 20;
            double probability = 0.166667;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            for (int i = 0; i < expected.Length; i++)
            {
                double actual = target.ProbabilityMassFunction(i - 1);
                Assert.AreEqual(expected[i], actual, 1e-10);
                Assert.IsFalse(Double.IsNaN(actual));

                double logActual = target.LogProbabilityMassFunction(i - 1);
                Assert.AreEqual(Math.Log(expected[i]), logActual, 1e-10);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void FitTest()
        {
            int trials = 5;
            BinomialDistribution target = new BinomialDistribution(trials);

            double[] observations = { 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0 };
            double[] weights = null;
            IFittingOptions options = null;

            target.Fit(observations, weights, options);

            Assert.AreEqual(4.0 / trials, target.ProbabilityOfSuccess);
        }

        [TestMethod()]
        public void BinomialDistributionConstructorTest()
        {
            int trials = 5;
            double probability = 0.52;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            Assert.AreEqual(trials, target.NumberOfTrials);
            Assert.AreEqual(probability, target.ProbabilityOfSuccess);
            Assert.AreEqual(trials * probability, target.Mean);
            Assert.AreEqual(trials * probability * (1 - probability), target.Variance);
        }

        [TestMethod()]
        public void CloneTest()
        {
            int trials = 3;
            double probability = 0.52;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            BinomialDistribution actual = (BinomialDistribution)target.Clone();

            Assert.AreNotSame(target, actual);
            Assert.AreNotEqual(target, actual);

            Assert.AreEqual(target.NumberOfTrials, actual.NumberOfTrials);
            Assert.AreEqual(target.ProbabilityOfSuccess, actual.ProbabilityOfSuccess);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            // http://www.stat.yale.edu/Courses/1997-98/101/binom.htm
            // Verified in http://stattrek.com/online-calculator/binomial.aspx

            double[] pmf =             { 0.0260838446329553, 0.10433562893683, 0.198238170750635, 0.237886375826923, 0.202203904741285 };
            double[] cdfLess = { 0.0000000000000000, 0.0260838446329553, 0.130419473569785, 0.32865764432042, 0.566544020147343 };
            double[] cdfLessEqual =    { 0.0260838446329553, 0.130419473569785, 0.32865764432042, 0.566544020147343, 0.768747924888628 };
            double[] cdfGreater =      { 0.973916155367045, 0.869580526430215, 0.67134235567958, 0.433455979852657, 0.231252075111372 };
            double[] cdfGreaterEqual = { 1.000000000000000, 0.973916155367045, 0.869580526430215, 0.67134235567958, 0.433455979852657 };

            int trials = 20;
            double probability = 0.166667;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            for (int i = 0; i < pmf.Length; i++)
            {
                {   // P(X = i)
                    double actual = target.ProbabilityMassFunction(i);
                    Assert.AreEqual(pmf[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X <= i)
                    double actual = target.DistributionFunction(i);
                    Assert.AreEqual(cdfLessEqual[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X < i)
                    double actual = target.DistributionFunction(i, inclusive: false);
                    Assert.AreEqual(cdfLess[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X > i)
                    double actual = target.ComplementaryDistributionFunction(i);
                    Assert.AreEqual(cdfGreater[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X >= i)
                    double actual = target.ComplementaryDistributionFunction(i, inclusive: true);
                    Assert.AreEqual(cdfGreaterEqual[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }
            }
        }

        [TestMethod()]
        public void DistributionFunctionTest2()
        {
            // http://www.stat.yale.edu/Courses/1997-98/101/binom.htm
            // Verified in http://stattrek.com/online-calculator/binomial.aspx

            double[] expected = { 0.0260838, 0.130419, 0.3287, 0.5665, 0.7687, 0.8982, 0.9629, 0.9887, 0.9972, 0.9994 };

            int trials = 20;
            double probability = 0.166667;
            BinomialDistribution target = new BinomialDistribution(trials, probability);

            for (int i = 0; i < expected.Length; i++)
            {
                double actual = target.DistributionFunction(i);
                Assert.AreEqual(expected[i], actual, 1e-4);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }

        [TestMethod()]
        public void DistributionFunctionTest3()
        {
            // http://www.danielsoper.com/statcalc3/calc.aspx?id=70

            double probability = 0.2;
            int successes = 2;
            int trials = 10;

            BinomialDistribution target = new BinomialDistribution(trials, probability);

            double actual = target.DistributionFunction(successes);
            double expected = 0.67779953;

            Assert.AreEqual(expected, actual, 1e-5);

        }

    }
}
