using Accord.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Accord.Tests.Math
{

    [TestClass()]
    public class BetaTest
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
        public void IncompleteTest()
        {
            double a = 5;
            double b = 4;
            double x = 0.5;

            double actual = Beta.Incomplete(a, b, x);
            double expected = 0.36328125;

            Assert.AreEqual(expected, actual, 1e-6);
        }

    }
}
