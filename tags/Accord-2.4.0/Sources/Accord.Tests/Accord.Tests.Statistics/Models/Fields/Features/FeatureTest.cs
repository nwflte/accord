using Accord.Statistics.Models.Fields.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Accord.Statistics.Models.Fields.Functions;

namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for InitialFeatureTest and is intended
    ///to contain all InitialFeatureTest Unit Tests
    ///</summary>
    [TestClass()]
    public class InitialFeatureTest
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
        public void ComputeTest()
        {
            var hmm = HiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

            IPotentialFunction<int> owner = new HiddenMarkovClassifierFunction(hmm);

            int[] x = new int[] { 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0 };

            foreach (var factor in owner.Factors)
            {
                for (int y = 0; y < owner.Outputs; y++)
                {
                    double[,] fwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Forward(factor, x, y);

                    double[,] bwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Backward(factor, x, y);

                    double[,] lnfwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogForward(factor, x, y);

                    double[,] lnbwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogBackward(factor, x, y);

                    for (int i = 0; i < fwd.GetLength(0); i++)
                        for (int j = 0; j < fwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(fwd[i, j]), lnfwd[i, j], 1e-10);

                    for (int i = 0; i < bwd.GetLength(0); i++)
                        for (int j = 0; j < bwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(bwd[i, j]), lnbwd[i, j], 1e-10);


                    foreach (var feature in factor)
                    {
                        double expected = System.Math.Log(feature.Marginal(fwd, bwd, x, y));
                        double actual = feature.LogMarginal(lnfwd, lnbwd, x, y);

                        Assert.AreEqual(expected, actual, 1e-10);
                        Assert.IsFalse(Double.IsNaN(actual));
                    }

                }
            }
        }

        [TestMethod()]
        public void ComputeTest2()
        {
            var hmm = NormalHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

            IPotentialFunction<double> owner = new NormalHiddenMarkovClassifierFunction(hmm);


            double[] x = new double[] { 0, 1, 2, 1, 7, 2, 1, -2, 5, 3, 4 };

            foreach (var factor in owner.Factors)
            {
                for (int y = 0; y < owner.Outputs; y++)
                {
                    double[,] fwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Forward(factor, x, y);

                    double[,] bwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Backward(factor, x, y);

                    double[,] lnfwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogForward(factor, x, y);

                    double[,] lnbwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogBackward(factor, x, y);


                    for (int i = 0; i < fwd.GetLength(0); i++)
                        for (int j = 0; j < fwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(fwd[i, j]), lnfwd[i, j], 1e-10);

                    for (int i = 0; i < bwd.GetLength(0); i++)
                        for (int j = 0; j < bwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(bwd[i, j]), lnbwd[i, j], 1e-10);

                    foreach (var feature in factor)
                    {
                        double expected = System.Math.Log(feature.Marginal(fwd, bwd, x, y));
                        double actual = feature.LogMarginal(lnfwd, lnbwd, x, y);

                        Assert.AreEqual(expected, actual, 1e-10);
                        Assert.IsFalse(Double.IsNaN(actual));
                    }
                }
            }
        }

        [TestMethod()]
        public void ComputeTest3()
        {
            var hmm = MultivariateNormalHiddenMarkovClassifierPotentialFunctionTest.CreateModel1();

            var owner = new MultivariateNormalHiddenMarkovClassifierFunction(hmm);


            double[][] x = 
            { 
                new double[] { 0 },
                new double[] { 1 },
                new double[] { 3 },
                new double[] { 1 },
                new double[] { 2 },
                new double[] { 8 },
                new double[] { 0 },
                new double[] { 10 },
                new double[] { System.Math.PI },
            };

            foreach (var factor in owner.Factors)
            {
                for (int y = 0; y < owner.Outputs; y++)
                {
                    double[,] fwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Forward(factor, x, y);

                    double[,] bwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.Backward(factor, x, y);

                    double[,] lnfwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogForward(factor, x, y);

                    double[,] lnbwd = Accord.Statistics.Models.Fields
                        .ForwardBackwardAlgorithm.LogBackward(factor, x, y);


                    for (int i = 0; i < fwd.GetLength(0); i++)
                        for (int j = 0; j < fwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(fwd[i, j]), lnfwd[i, j], 1e-10);

                    for (int i = 0; i < bwd.GetLength(0); i++)
                        for (int j = 0; j < bwd.GetLength(1); j++)
                            Assert.AreEqual(System.Math.Log(bwd[i, j]), lnbwd[i, j], 1e-10);


                    foreach (var feature in factor)
                    {
                        double expected = System.Math.Log(feature.Marginal(fwd, bwd, x, y));
                        double actual = feature.LogMarginal(lnfwd, lnbwd, x, y);

                        Assert.AreEqual(expected, actual, 1e-10);
                        Assert.IsFalse(Double.IsNaN(actual));
                    }

                }
            }
        }

    }
}
