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
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class TwoSampleKolmogorovSmirnovTestTest
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
        public void TwoSampleKolmogorovSmirnovTestConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            // Create a K-S test to verify if two samples have been
            // drawn from different populations. In this example, we
            // will first generate a number of samples from different
            // distributions, and then check if the K-S test can indeed
            // see the difference:

            // Generate 15 points from a Normal distribution with mean 5 and sigma 2
            double[] sample1 = new NormalDistribution(mean: 5, stdDev: 1).Generate(25);

            // Generate 15 points from an uniform distribution from 0 to 10
            double[] sample2 = new UniformContinuousDistribution(a: 0, b: 10).Generate(25);

            // Now we can create a K-S test and test the unequal hypothesis:
            var test = new TwoSampleKolmogorovSmirnovTest(sample1, sample2,
                TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal);

            bool significant = test.Significant; // outputs true

            Assert.IsTrue(test.Significant);
            Assert.AreEqual(0.44, test.Statistic, 1e-15);
            Assert.IsFalse(Double.IsNaN(test.Statistic));
            Assert.AreEqual(0.012408, test.PValue, 1e-4);
        }
    }
}
