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
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class HypergeometricDistributionTest
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
        public void HypergeometricDistributionConstructorTest()
        {
            bool thrown;

            thrown = false;
            try { new HypergeometricDistribution(10, 0, 11); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { new HypergeometricDistribution(10, 11, 9); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { new HypergeometricDistribution(0, 0, 1); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { new HypergeometricDistribution(1, 0, 0); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            try { new HypergeometricDistribution(1, -1, 1); }
            catch (ArgumentException) { thrown = true; }
            Assert.IsTrue(thrown);

            int N = 10;
            int n = 8;
            int m = 9;

            var target = new HypergeometricDistribution(N, m, n);
            Assert.AreEqual(N, target.PopulationSize);
            Assert.AreEqual(n, target.SampleSize);
            Assert.AreEqual(m, target.PopulationSuccess);

            double dN = N;
            double dn = n;
            double dm = m;

            Assert.AreEqual(dn * (dm / dN), target.Mean);
            Assert.AreEqual(dn * dm * (dN - dm) * (dN - dn) / (dN * dN * (dN - 1.0)), target.Variance);
        }

        [TestMethod()]
        public void CloneTest()
        {
            int populationSize = 12;
            int draws = 5;
            int success = 4;
            var target = new HypergeometricDistribution(populationSize, success, draws);

            var actual = (HypergeometricDistribution)target.Clone();

            Assert.AreNotSame(target, actual);
            Assert.AreNotEqual(target, actual);

            Assert.AreEqual(target.PopulationSize, actual.PopulationSize);
            Assert.AreEqual(target.SampleSize, actual.SampleSize);
            Assert.AreEqual(target.PopulationSuccess, actual.PopulationSuccess);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            int populationSize = 15;
            int draws = 7;
            int success = 8;
            var target = new HypergeometricDistribution(populationSize, success, draws);

            int k = 5;
            double expected = 0.96829836829836835;
            double actual = target.DistributionFunction(k);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [TestMethod()]
        public void LogProbabilityMassFunctionTest()
        {
            int populationSize = 15;
            int draws = 7;
            int success = 8;
            var target = new HypergeometricDistribution(populationSize, success, draws);

            int k = 5;
            double expected = Math.Log(0.182750582750583);
            double actual = target.LogProbabilityMassFunction(k);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void ProbabilityMassFunctionTest()
        {
            int N = 50;
            int n = 10;
            int m = 5;
            var target = new HypergeometricDistribution(N, m, n);

            int k = 4;
            double expected = 0.0039645830580150657;
            double actual = target.ProbabilityMassFunction(k);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ProbabilityMassFunctionTest2()
        {
            int populationSize = 15;
            int draws = 7;
            int success = 8;
            var target = new HypergeometricDistribution(populationSize, success, draws);

            int k = 5;
            double expected = 0.182750582750583;
            double actual = target.ProbabilityMassFunction(k);
            Assert.AreEqual(expected, actual, 1e-10);
            Assert.IsFalse(Double.IsNaN(actual));
        }

        [TestMethod()]
        public void DistributionFunctionTest2()
        {
            // Verified against http://stattrek.com/online-calculator/hypergeometric.aspx

            int population = 20;
            int populationSuccess = 8;
            int sample = 6;

            double[] pmf = { 0.0238390092879257, 0.163467492260062, 0.357585139318886, 0.317853457172343, 0.119195046439628, 0.0173374613003096, 0.000722394220846233 };
            double[] less = { 0.0000000000000000, 0.023839009287926, 0.187306501547988, 0.544891640866874, 0.862745098039217, 0.981940144478844, 0.999277605779154 };
            double[] lessEqual = { 0.0238390092879257, 0.187306501547988, 0.544891640866874, 0.862745098039217, 0.981940144478845, 0.999277605779154, 1 };
            double[] greater = { 0.976160990712074, 0.812693498452012, 0.455108359133126, 0.137254901960783, 0.018059855521155, 0.000722394220845968, 0 };
            double[] greaterEqual = { 1, 0.976160990712074, 0.812693498452012, 0.455108359133126, 0.137254901960783, 0.0180598555211555, 0.00072239422084619 };

            var target = new HypergeometricDistribution(population, populationSuccess, sample);

            for (int i = 0; i < pmf.Length; i++)
            {
                {   // P(X = i)
                    double actual = target.ProbabilityMassFunction(i);
                    Assert.AreEqual(pmf[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X <= i)
                    double actual = target.DistributionFunction(i);
                    Assert.AreEqual(lessEqual[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X < i)
                    double actual = target.DistributionFunction(i, inclusive: false);
                    Assert.AreEqual(less[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X > i)
                    double actual = target.ComplementaryDistributionFunction(i);
                    Assert.AreEqual(greater[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }

                {   // P(X >= i)
                    double actual = target.ComplementaryDistributionFunction(i, inclusive: true);
                    Assert.AreEqual(greaterEqual[i], actual, 1e-4);
                    Assert.IsFalse(Double.IsNaN(actual));
                }
            }

        }

    }
}
