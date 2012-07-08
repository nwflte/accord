using Accord.Statistics.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AForge;
using Accord.Statistics.Models.Regression.Linear;

namespace Accord.Tests.Statistics
{

    [TestClass()]
    public class MultipleLinearRegressionAnalysisTest
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
        public void ComputeTest()
        {
            // Example 5.1 from 
            // http://www.weibull.com/DOEWeb/estimating_regression_models_using_least_squares.htm

            double[][] inputs = 
            {
                new double[] { 41.9, 29.1 }, // 1 
                new double[] { 43.4, 29.3 }, // 2
                new double[] { 43.9, 29.5 }, // 3
                new double[] { 44.5, 29.7 }, // 4
                new double[] { 47.3, 29.9 }, // 5
                new double[] { 47.5, 30.3 }, // 6
                new double[] { 47.9, 30.5 }, // 7
                new double[] { 50.2, 30.7 }, // 8
                new double[] { 52.8, 30.8 }, // 9
                new double[] { 53.2, 30.9 }, // 10
                new double[] { 56.7, 31.5 }, // 11
                new double[] { 57.0, 31.7 }, // 12
                new double[] { 63.5, 31.9 }, // 13
                new double[] { 65.3, 32.0 }, // 14
                new double[] { 71.1, 32.1 }, // 15
                new double[] { 77.0, 32.5 }, // 16
                new double[] { 77.8, 32.9 }, // 17
            };

            double[] outputs = 
            {
                251.3,
                251.3,
                248.3,
                267.5,
                273.0,
                276.5,
                270.3,
                274.9,
                285.0,
                290.0,
                297.0,
                302.5,
                304.5,
                309.3,
                321.7,
                330.7,
                349.0,
            };

            var target = new MultipleLinearRegressionAnalysis(inputs, outputs, intercept: true);

            target.Compute();

            Assert.AreEqual(0.968022, target.RSquared, 1e-5);
            Assert.AreEqual(0.963454, target.RSquareAdjusted, 1e-5);

            Assert.AreEqual(2, target.Table[0].DegreesOfFreedom);
            Assert.AreEqual(14, target.Table[1].DegreesOfFreedom);
            Assert.AreEqual(16, target.Table[2].DegreesOfFreedom);

            Assert.AreEqual(12816.345909673832, target.Table[0].SumOfSquares);
            Assert.AreEqual(423.37409032616614, target.Table[1].SumOfSquares);
            Assert.AreEqual(13239.719999999998, target.Table[2].SumOfSquares);

            Assert.AreEqual(6408.1729548369158, target.Table[0].MeanSquares);
            Assert.AreEqual(30.241006451869008, target.Table[1].MeanSquares);

            Assert.AreEqual(211.90342871147618, target.Table[0].Statistic);
            Assert.AreEqual(0.000000000034191538489380946, target.Table[0].Significance.PValue, 1e-16);
            Assert.IsFalse(Double.IsNaN(target.Table[0].Significance.PValue));

            Assert.AreEqual(1.2387232694931045, target.Coefficients[0].Value);
            Assert.AreEqual(12.082353323342893, target.Coefficients[1].Value);
            Assert.AreEqual(-153.51169396147372, target.Coefficients[2].Value);

            Assert.IsFalse(target.Coefficients[0].IsIntercept);
            Assert.IsFalse(target.Coefficients[1].IsIntercept);
            Assert.IsTrue(target.Coefficients[2].IsIntercept);

            Assert.AreEqual(0.394590262021004, target.Coefficients[0].StandardError);
            Assert.AreEqual(3.9322914100115307, target.Coefficients[1].StandardError);

            Assert.AreEqual(3.1392646720388844, target.Coefficients[0].TTest.Statistic);
            Assert.AreEqual(3.0725986615797285, target.Coefficients[1].TTest.Statistic);

            DoubleRange range;

            range = target.Coefficients[0].TTest.GetConfidenceInterval(0.9);
            Assert.AreEqual(0.54372744151743968, range.Min, 1e-10);
            Assert.AreEqual(1.9337190974687695, range.Max, 1e-10);

            range = target.Coefficients[1].TTest.GetConfidenceInterval(0.9);
            Assert.AreEqual(5.1563686060690417, range.Min, 1e-10);
            Assert.AreEqual(19.008338040616746, range.Max, 1e-10);


            MultipleLinearRegression mlr = new MultipleLinearRegression(2, true);
            mlr.Regress(inputs, outputs);
            double r2 = mlr.CoefficientOfDetermination(inputs, outputs, false);
            double r2a = mlr.CoefficientOfDetermination(inputs, outputs, true);

            Assert.AreEqual(r2, target.RSquared);
            Assert.AreEqual(r2a, target.RSquareAdjusted);
        }


    }
}
