using Accord.Statistics.Distributions.Univariate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Accord.Math;

namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for MannWhitneyDistributionTest and is intended
    ///to contain all MannWhitneyDistributionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MannWhitneyDistributionTest
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
