// Accord Unit Tests
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
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

using Accord.Statistics.Distributions.Univariate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Accord.Statistics.Distributions.Fitting;

namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for EmpiricalDistributionTest and is intended
    ///to contain all EmpiricalDistributionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EmpiricalDistributionTest
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


        /// <summary>
        ///A test for EmpiricalDistribution Constructor
        ///</summary>
        [TestMethod()]
        public void EmpiricalDistributionConstructorTest1()
        {
            double[] samples = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            double smoothing = 0.5;

            EmpiricalDistribution target = new EmpiricalDistribution(samples, smoothing);
            Assert.AreEqual(samples, target.Samples);
            Assert.AreEqual(smoothing, target.Smoothing);

            Assert.AreEqual(3, target.Mean);
            Assert.AreEqual(1.1375929179890421, target.StandardDeviation);
            Assert.AreEqual(target.Variance, target.Variance);
        }

        /// <summary>
        ///A test for EmpiricalDistribution Constructor
        ///</summary>
        [TestMethod()]
        public void EmpiricalDistributionConstructorTest2()
        {
            double[] samples = { 5, 5, 1, 4, 1, 2, 2, 3, 3, 3, 4, 3, 3, 3, 4, 3, 2, 3 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples);
            Assert.AreEqual(samples, target.Samples);
            Assert.AreEqual(1.9144923416414432, target.Smoothing);
        }

        [TestMethod()]
        public void CloneTest()
        {
            double[] samples = { 4, 2 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples);

            EmpiricalDistribution clone = (EmpiricalDistribution)target.Clone();

            Assert.AreNotSame(target, clone);
            Assert.AreEqual(target.Entropy, clone.Entropy);
            Assert.AreEqual(target.Mean, clone.Mean);
            Assert.AreNotSame(target.Samples, clone.Samples);
            Assert.AreEqual(target.StandardDeviation, clone.StandardDeviation);
            Assert.AreEqual(target.Variance, clone.Variance);

            for (int i = 0; i < clone.Samples.Length; i++)
                Assert.AreEqual(target.Samples[i], clone.Samples[i]);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            double[] samples = { 1, 5, 2, 5, 1, 7, 1, 9 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples);

            Assert.AreEqual(0.000, target.DistributionFunction(0));
            Assert.AreEqual(0.375, target.DistributionFunction(1));
            Assert.AreEqual(0.500, target.DistributionFunction(2));
            Assert.AreEqual(0.750, target.DistributionFunction(5));
            Assert.AreEqual(0.875, target.DistributionFunction(7));
            Assert.AreEqual(1.000, target.DistributionFunction(9));
        }

        [TestMethod()]
        public void FitTest()
        {
            EmpiricalDistribution target = new EmpiricalDistribution(new double[] { 0 });

            double[] observations = { 1, 5, 2, 5, 1, 7, 1, 9, 4, 2 };
            double[] weights = null;
            IFittingOptions options = null;

            target.Fit(observations, weights, options);
            Assert.AreNotSame(observations, target.Samples);

            for (int i = 0; i < observations.Length; i++)
                Assert.AreEqual(observations[i], target.Samples[i]);
        }

        ///</summary>
        [TestMethod()]
        public void ProbabilityDensityFunctionTest()
        {
            double[] samples = { 1, 5, 2, 5, 1, 7, 1, 9, 4, 2 };
            EmpiricalDistribution target = new EmpiricalDistribution(samples, 1);

            Assert.AreEqual(1.0, target.Smoothing);

            double actual;

            actual = target.ProbabilityDensityFunction(1);
            Assert.AreEqual(0.16854678051819402, actual);

            actual = target.ProbabilityDensityFunction(2);
            Assert.AreEqual(0.15866528844260089, actual);

            actual = target.ProbabilityDensityFunction(3);
            Assert.AreEqual(0.0996000842425018, actual);

            actual = target.ProbabilityDensityFunction(4);
            Assert.AreEqual(0.1008594542833362, actual);

            actual = target.ProbabilityDensityFunction(6);
            Assert.AreEqual(0.078460710909263, actual);

            actual = target.ProbabilityDensityFunction(8);
            Assert.AreEqual(0.049293898826709738, actual);
        }


    }
}
