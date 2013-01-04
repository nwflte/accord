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
    using Accord.Statistics.Moving;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics;
    using Accord.Statistics.Distributions.Univariate;

    [TestClass()]
    public class RayleighDistributionTest
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
        public void ConstructorTest()
        {
            RayleighDistribution n = new RayleighDistribution(0.807602);
            Assert.AreEqual(1.0121790039242726, n.Mean);
            Assert.AreEqual(0.27993564482286737, n.Variance);
        }

        [TestMethod()]
        public void ProbabilityDistributionTest()
        {
            RayleighDistribution n = new RayleighDistribution(0.807602);

            double[] expected = { 0, 0.712311, 0.142855, 0.00463779, 0.0000288872 };
            double[] actual = new double[expected.Length];

            for (int i = 0; i < actual.Length; i++)
                actual[i] = n.ProbabilityDensityFunction(i);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-5);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }

        [TestMethod()]
        public void CumulativeDistributionTest()
        {
            RayleighDistribution n = new RayleighDistribution(0.807602);

            double[] expected = { 0, 0.535415, 0.953414, 0.998992, 0.999995 };
            double[] actual = new double[expected.Length];

            for (int i = 0; i < actual.Length; i++)
                actual[i] = n.DistributionFunction(i);

            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 1e-6);
                Assert.IsFalse(double.IsNaN(actual[i]));
            }
        }


        [TestMethod()]
        public void GenerateTest()
        {
            RayleighDistribution target = new RayleighDistribution(2.5);

            double[] samples = target.Generate(1000000);

            var actual = RayleighDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(2, actual.Mean, 0.01);
            Assert.AreEqual(5, actual.Variance, 0.01);
        }

        [TestMethod()]
        public void GenerateTest2()
        {
            RayleighDistribution target = new RayleighDistribution(4.2);

            double[] samples = new double[1000000];
            for (int i = 0; i < samples.Length; i++)
                samples[i] = target.Generate();

            var actual = RayleighDistribution.Estimate(samples);
            actual.Fit(samples);

            Assert.AreEqual(4, actual.Mean, 0.01);
            Assert.AreEqual(2, actual.Variance, 0.01);
        }
    }
}