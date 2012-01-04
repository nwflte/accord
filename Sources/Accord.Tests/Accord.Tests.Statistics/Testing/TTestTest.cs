// Accord Unit Tests
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
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

using Accord.Statistics.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Accord.Statistics.Distributions.Univariate;

namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for TTestTest and is intended
    ///to contain all TTestTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TTestTest
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
        ///A test for TTest Constructor
        ///</summary>
        [TestMethod()]
        public void TTestConstructorTest()
        {

            // mean = 0.5, var = 1
            double[] sample = 
            { 
                -0.849886940156521,	3.53492346633185,  1.22540422494611, 0.436945126810344, 1.21474290382610,
                 0.295033941700225, 0.375855651783688, 1.98969760778547, 1.90903448980048,	1.91719241342961
            };

            double hypothesizedMean = 0;
            TTestHypotesis hypothesis = TTestHypotesis.MeanIsDifferentThanHypothesis;
            TTest target = new TTest(sample, hypothesizedMean, hypothesis);

            Assert.AreEqual(3.1254485381338246, target.Statistic);
            Assert.AreEqual(Hypothesis.TwoTail, target.Hypothesis);
            Assert.AreEqual(0.012210924322697769, target.PValue);


            hypothesis = TTestHypotesis.MeanIsGreaterThanHypothesis;
            target = new TTest(sample, hypothesizedMean, hypothesis);

            Assert.AreEqual(3.1254485381338246, target.Statistic);
            Assert.AreEqual(Hypothesis.OneUpper, target.Hypothesis); // right tail
            Assert.AreEqual(0.0061054621613488846, target.PValue);


            hypothesis = TTestHypotesis.MeanIsSmallerThanHypothesis;
            target = new TTest(sample, hypothesizedMean, hypothesis);

            Assert.AreEqual(3.1254485381338246, target.Statistic);
            Assert.AreEqual(Hypothesis.OneLower, target.Hypothesis); // left tail
            Assert.AreEqual(0.99389453783865112, target.PValue);
        }
    }
}
