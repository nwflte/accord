

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math;
    using Accord.Statistics;


    [TestClass()]
    public class BootstrapTest
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
        public void BootstrapConstructorTest()
        {
            // Example from Masters, 1995

            // Assume a small dataset
            double[] data = { 3, 5, 2, 1, 7 };

            int size = data.Length;
            int resamplings = 3;
            int subsampleSize = data.Length;


            Bootstrap target = new Bootstrap(size, resamplings, subsampleSize);

            Assert.AreEqual(3, target.B);
            Assert.AreEqual(3, target.Subsamples.Length);

            for (int i = 0; i < target.Subsamples.Length; i++)
                Assert.AreEqual(5, target.Subsamples[i].Length);
        }

        [TestMethod()]
        public void BootstrapConstructorTest2()
        {
            // Example from Masters, 1995

            // Assume a small dataset
            double[] data = { 3, 5, 2, 1, 7 };
            // indices      { 0, 1, 2, 3, 4 }

            int[][] resamplings = 
            {
                new [] { 4, 0, 2, 0, 3 }, // indices of { 7, 3, 2, 3, 1 }
                new [] { 1, 3, 3, 0, 4 }, // indices of { 5, 1, 1, 3, 7 }
                new [] { 2, 2, 4, 3, 0 }, // indices of { 2, 2, 7, 1, 3 }
            };

            Bootstrap target = new Bootstrap(data.Length, resamplings);

            target.Fitting = (int[] trainingSamples, int[] validationSamples) =>
                {
                    double[] subsample = data.Submatrix(trainingSamples);
                    double mean = subsample.Mean();

                    return new BootstrapValues(mean, 0);
                };

            var result = target.Compute();

            double actualMean = result.Training.Mean;
            double actualVar = result.Training.Variance;

            Assert.AreEqual(3.2, actualMean, 1e-10);
            Assert.AreEqual(0.04, actualVar, 1e-10);
        }
    }
}
