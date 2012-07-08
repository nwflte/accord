

namespace Accord.Tests.Statistics
{

    using Accord.Statistics.Testing.Power;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Statistics.Testing;
    using Accord.Statistics;
    using Accord.Math;

    [TestClass()]
    public class ZTestPowerAnalysisTest
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
        public void ZTestPowerAnalysisConstructorTest1()
        {
            ZTestPowerAnalysis target;
            double actual, expected;

            target = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsDifferentFromHypothesis)
            {
                Effect = 0.2,
                Samples = 60,
                Size = 0.10,
            };

            target.ComputePower();

            expected = 0.4618951;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-5);


            target = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsSmallerThanHypothesis)
            {
                Effect = 0.2,
                Samples = 60,
                Size = 0.10,
            };

            target.ComputePower();

            expected = 0.00232198;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-5);


            target = new ZTestPowerAnalysis(OneSampleHypothesis.ValueIsGreaterThanHypothesis)
            {
                Effect = 0.2,
                Samples = 60,
                Size = 0.10,
            };

            target.ComputePower();

            expected = 0.6055124;
            actual = target.Power;
            Assert.AreEqual(expected, actual, 1e-5);
        }
    }
}
