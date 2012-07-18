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
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Statistics.Testing.Power;    
    
    [TestClass()]
    public class ZTestTest
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
        public void ZTestConstructorTest()
        {
            // Example from: http://en.wikipedia.org/wiki/Z-test

            /* Suppose that in a particular geographic region, the mean and standard
             * deviation of scores on a reading test are 100 points, and 12 points, 
             * respectively. Our interest is in the scores of 55 students in a particular
             * school who received a mean score of 96. We can ask whether this mean score
             * is significantly lower than the regional mean — that is, are the students
             * in this school comparable to a simple random sample of 55 students from the
             * region as a whole, or are their scores surprisingly low?
             */

            ZTest target;

            double mean = 96;
            double stdDev = 12;
            int samples = 55;
            double hypothesizedMean = 100;
            target = new ZTest(mean, stdDev, samples, hypothesizedMean,
                OneSampleHypothesis.ValueIsSmallerThanHypothesis);
            
            Assert.AreEqual(target.Statistic, -2.47, 0.01);
            Assert.AreEqual(target.PValue, 0.0068, 0.001);

            /* This is the one-sided p-value for the null hypothesis that the 55 students 
             * are comparable to a simple random sample from the population of all test-takers.
             * The two-sided p-value is approximately 0.014 (twice the one-sided p-value).
             */

            target = new ZTest(mean, stdDev, samples, hypothesizedMean, 
                OneSampleHypothesis.ValueIsDifferentFromHypothesis);

            Assert.AreEqual(target.Statistic, -2.47, 0.01);
            Assert.AreEqual(target.PValue, 0.014, 0.005);
            
        }

        [TestMethod()]
        public void ZTestConstructorTest2()
        {
            // Example from http://science.kennesaw.edu/~jdemaio/1107/hypothesis_testing.htm

            double mean = 63;
            double stdDev = 8;
            int samples = 49;

            double hypothesizedMean = 58;

            // Null Hypothesis: mean = 58
            // Alternative    : mean > 58

            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsGreaterThanHypothesis;
            ZTest target = new ZTest(mean, stdDev, samples, hypothesizedMean, hypothesis);

            Assert.AreEqual(4.38, target.Statistic, 0.01);
            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void ZTestConstructorTest3()
        {
            // Example from http://science.kennesaw.edu/~jdemaio/1107/hypothesis_testing.htm

            double mean = 53;
            double stdDev = 8;
            int samples = 49;

            double hypothesizedMean = 58;

            // Null Hypothesis: mean = 58
            // Alternative    : mean > 58

            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsGreaterThanHypothesis;
            ZTest target = new ZTest(mean, stdDev, samples, hypothesizedMean, hypothesis);

            Assert.AreEqual(-4.38, target.Statistic, 0.01);
            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
            Assert.IsFalse(target.Significant);
        }

        [TestMethod()]
        public void ZTestConstructorTest4()
        {
            // Example from http://science.kennesaw.edu/~jdemaio/1107/hypothesis_testing.htm

            double mean = 51.5;
            double stdDev = 10;
            int samples = 100;

            double hypothesizedMean = 50;

            // Null Hypothesis: mean =  58
            // Alternative    : mean != 58

            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis;
            ZTest target = new ZTest(mean, stdDev, samples, hypothesizedMean, hypothesis);

            Assert.AreEqual(1.5, target.Statistic, 0.01);
            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, target .Tail);
            Assert.IsFalse(target.Significant);
        }

        [TestMethod()]
        public void PowerTest1()
        {
            // Example from http://wise.cgu.edu/powermod/computing.asp

            double mean = 105;
            double stdDev = 10;
            int samples = 25;
            double hypothesizedMean = 100;

            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsGreaterThanHypothesis;
            ZTest target = new ZTest(mean, stdDev, samples, hypothesizedMean, hypothesis);
            var power = target.Analysis;

            Assert.AreEqual(0.804, power.Power, 1e-3);

            mean = 90;
            target = new ZTest(mean, stdDev, samples, hypothesizedMean, hypothesis);

            power = target.Analysis;
            Assert.AreEqual(0.0, power.Power, 1e-3);
        }

        [TestMethod()]
        public void SampleSizeTest1()
        {
            // Example from http://wise.cgu.edu/powermod/computing.asp

            double mean = 104;
            double stdDev = 10;
            int samples = 25;
            double hypothesizedMean = 100;

            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsGreaterThanHypothesis;
            ZTest target = new ZTest(mean, stdDev, samples, hypothesizedMean, hypothesis);

            ZTestPowerAnalysis power = (ZTestPowerAnalysis)target.Analysis;

            power.Size = 0.01;
            power.Power = 0.90;
            power.ComputeSamples();

            double sampleSize = System.Math.Ceiling(power.Samples);

            Assert.AreEqual(82, sampleSize, 1e-16);
        }

        [TestMethod()]
        public void PowerTest2()
        {
            // Example from http://www.cyclismo.org/tutorial/R/power.html

            double mean = 6.5;
            double stdDev = 2;
            int samples = 20;
            double hypothesizedMean = 5;

            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis;
            ZTest target = new ZTest(mean, stdDev, samples, hypothesizedMean, hypothesis);

            IPowerAnalysis power = target.Analysis;

            Assert.AreEqual(0.918362, power.Power, 1e-6);
        }

        [TestMethod()]
        public void StatisticToPValueTest()
        {
            double z  = 1.96;
            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                double actual = target.StatisticToPValue(z);
                double expected = 0.05;
                Assert.AreEqual(expected, actual, 1e-5);
            }

            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
                Assert.AreEqual(DistributionTail.OneLower, target.Tail);
                double actual = target.StatisticToPValue(z);
                double expected = 0.975;
                Assert.AreEqual(expected, actual, 1e-5);
            }

            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
                Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
                double actual = target.StatisticToPValue(z);
                double expected = 0.025;
                Assert.AreEqual(expected, actual, 1e-5);
            }

            z = -1.96;
            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                double actual = target.StatisticToPValue(z);
                double expected = 0.05;
                Assert.AreEqual(expected, actual, 1e-5);
            }

            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
                Assert.AreEqual(DistributionTail.OneLower, target.Tail);
                double actual = target.StatisticToPValue(z);
                double expected = 0.025;
                Assert.AreEqual(expected, actual, 1e-5);
            }

            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
                Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
                double actual = target.StatisticToPValue(z);
                double expected = 0.975;
                Assert.AreEqual(expected, actual, 1e-5);
            }
        }

        [TestMethod()]
        public void PValueToStatisticTest()
        {
            double p = 0.05;
            double z = 0;
            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = 1.96;
                Assert.AreEqual(expected, actual, 0.01);
            }

            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
                Assert.AreEqual(DistributionTail.OneLower, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = -1.6449;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
                Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = 1.6449;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            p = 0.95;
            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsDifferentFromHypothesis);
                Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = 0.0627;
                Assert.AreEqual(expected, actual, 1e-5);
            }

            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsSmallerThanHypothesis);
                Assert.AreEqual(DistributionTail.OneLower, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = 1.6449;
                Assert.AreEqual(expected, actual, 1e-4);
            }

            {
                ZTest target = new ZTest(z, OneSampleHypothesis.ValueIsGreaterThanHypothesis);
                Assert.AreEqual(DistributionTail.OneUpper, target.Tail);
                double actual = target.PValueToStatistic(p);
                double expected = -1.6449;
                Assert.AreEqual(expected, actual, 1e-4);
            }
        }

        [TestMethod()]
        public void EffectSizeTest1()
        {
            // Example from http://wise.cgu.edu/powermod/computing.asp

            double mean = 104;
            double stdDev = 10;
            int samples = 25;
            double hypothesizedMean = 100;

            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsGreaterThanHypothesis;
            ZTest target = new ZTest(mean, stdDev, samples, hypothesizedMean, hypothesis);
            ZTestPowerAnalysis power = (ZTestPowerAnalysis)target.Analysis;

            power.Size = 0.05;
            power.Power = 0.80;
            power.ComputeEffect();

            Assert.AreEqual(0.50, power.Effect, 0.005);
        }

        [TestMethod()]
        public void EffectSizeTest2()
        {
            // Example from http://wise.cgu.edu/powermod/computing.asp

            double mean = 104;
            double stdDev = 10;
            int samples = 25;
            double hypothesizedMean = 100;

            OneSampleHypothesis hypothesis = OneSampleHypothesis.ValueIsDifferentFromHypothesis;
            ZTest target = new ZTest(mean, stdDev, samples, hypothesizedMean, hypothesis);
            ZTestPowerAnalysis power = (ZTestPowerAnalysis)target.Analysis;

            power.Size = 0.05;
            power.Power = 0.80;
            power.ComputeEffect();

            Assert.AreEqual(0.56, power.Effect, 0.001);
        }

    }
}
