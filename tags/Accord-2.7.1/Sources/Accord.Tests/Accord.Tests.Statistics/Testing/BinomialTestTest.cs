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
    using System;

    [TestClass()]
    public class BinomialTestTest
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
        public void BinomialTestConstructorTest1()
        {

            bool[] trials = { true, true, true, true, true, true, true, true, true, false };

            BinomialTest target = new BinomialTest(trials,
                hypothesizedProbability: 0.5, alternate: OneSampleHypothesis.ValueIsGreaterThanHypothesis);

            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);

            Assert.AreEqual(0.010742, target.PValue, 1e-5);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void BinomialTestConstructorTest4()
        {

            bool[] trials = { false, false, false, false, false, false, false, false, false, true };

            BinomialTest target = new BinomialTest(trials,
                hypothesizedProbability: 0.5, alternate: OneSampleHypothesis.ValueIsSmallerThanHypothesis);

            Assert.AreEqual(OneSampleHypothesis.ValueIsSmallerThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneLower, target.Tail);

            Assert.AreEqual(0.010742, target.PValue, 1e-5);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void BinomialTestConstructorTest2()
        {
            int successes = 5;
            int trials = 6;
            double probability = 1 / 4.0;

            BinomialTest target = new BinomialTest(successes, trials,
                hypothesizedProbability: probability, 
                alternate: OneSampleHypothesis.ValueIsGreaterThanHypothesis);

            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);

            Assert.AreEqual(0.004638, target.PValue, 1e-5);
            Assert.IsTrue(target.Significant);
        }

        [TestMethod()]
        public void BinomialTestConstructorTest3()
        {
            BinomialTest target = new BinomialTest(5, 18);

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, target.Tail);

            Assert.AreEqual(0.09625, target.PValue, 1e-4);
            Assert.IsFalse(target.Significant);
        }
    }
}
