
namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math;

    [TestClass()]
    public class EmpiricalHazardDistributionTest
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
        public void DistributionFunctionTest()
        {

            double[] values = 
            {
                1.0000000000000000, 0.8724284533876597, 0.9698946958777951,
                1.0000000000000000, 0.9840887140861863, 1.0000000000000000,
                1.0000000000000000, 1.0000000000000000, 1.0000000000000000,
                0.9979137773216293, 1.0000000000000000
            };

            double[] times =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };

            EmpiricalHazardDistribution target = EmpiricalHazardDistribution
                .FromSurvivalValues(times, values);


            // Data from: http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html
            double[] expectedBaselineSurvivalFunction = 
            {
                1.0000, 0.9979, 0.9979, 0.9979,
                0.9979, 0.9979, 0.9820, 
                0.9820, 0.9525, 0.8310, 0.8310,
            };


            double[] hazardFunction = new double[expectedBaselineSurvivalFunction.Length];
            double[] survivalFunction = new double[expectedBaselineSurvivalFunction.Length];

            for (int i = 0; i < 11; i++)
                hazardFunction[i] = target.CumulativeHazardFunction(i + 1);

            for (int i = 0; i < 11; i++)
                survivalFunction[i] = target.ComplementaryDistributionFunction(i + 1);


            for (int i = 0; i < expectedBaselineSurvivalFunction.Length; i++)
            {
                Assert.AreEqual(expectedBaselineSurvivalFunction[i], survivalFunction[i], 0.01);

                // Ho = -log(So)
                Assert.AreEqual(hazardFunction[i], -Math.Log(survivalFunction[i]), 0.01);

                // So = exp(-Ho)
                Assert.AreEqual(survivalFunction[i], Math.Exp(-hazardFunction[i]), 0.01);
            }
        }


        [TestMethod()]
        public void DistributionFunctionTest2()
        {

            double[] values = 
            {
               0.0000000000000000, 0.0351683340828711, 0.0267358118285064,
               0.0000000000000000, 0.0103643094219679, 0.0000000000000000,
               0.0000000000000000, 0.0000000000000000, 0.0000000000000000,
               0.000762266794052363, 0.000000000000000
            };

            double[] times =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };


            EmpiricalHazardDistribution target =
                new EmpiricalHazardDistribution(times, values);

            double[] expected = 
            {
			    1.000000000000000,	
			    0.999238023657475,	
			    0.999238023657475,	
			    0.999238023657475,	
			    0.999238023657475,	
			    0.999238023657475,	
			    0.98893509519066469,	
			    0.98893509519066469,
			    0.96284543081744489,
			    0.92957227114936058,	
			    0.92957227114936058,	
            };


            double[] hazardFunction = new double[expected.Length];
            double[] survivalFunction = new double[expected.Length];
            double[] complementaryDistribution = new double[expected.Length];

            for (int i = 0; i < 11; i++)
                hazardFunction[i] = target.CumulativeHazardFunction(i + 1);

            for (int i = 0; i < 11; i++)
                survivalFunction[i] = target.ComplementaryDistributionFunction(i + 1);


            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], survivalFunction[i], 1e-5);

                // Ho = -log(So)
                Assert.AreEqual(hazardFunction[i], -Math.Log(survivalFunction[i]), 1e-5);

                // So = exp(-Ho)
                Assert.AreEqual(survivalFunction[i], Math.Exp(-hazardFunction[i]), 1e-5);
            }
        }

    }
}
