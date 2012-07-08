

namespace Accord.Tests.Math
{
    using Accord.Math.Optimization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math;


    [TestClass()]
    public class BrentSearchTest
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
        public void FindRootTest()
        {
            //  Example from http://en.wikipedia.org/wiki/Brent%27s_method

            Func<double, double> f = x => (x + 3) * Math.Pow((x - 1), 2);
            double a = -4;
            double b = 4 / 3.0;

            double expected = -3;
            double actual = BrentSearch.FindRoot(f, a, b);

            Assert.AreEqual(expected, actual, 1e-6);
            Assert.IsFalse(Double.IsNaN(actual));
        }


        [TestMethod()]
        public void MaximizeTest()
        {
            Func<double, double> f = x => -2 * x * x - 3 * x + 5;

            double expected = -3 / 4.0;
            double actual = BrentSearch.Maximize(f, -200, +200);

            Assert.AreEqual(expected, actual, 1e-10);
        }


        [TestMethod()]
        public void MinimizeTest()
        {
            Func<double, double> f = x => 2 * x * x - 3 * x + 5;

            double expected = 3 / 4.0;
            double actual = BrentSearch.Minimize(f, -200, +200);

            Assert.AreEqual(expected, actual, 1e-10);
        }
    }
}
