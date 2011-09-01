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

using Accord.Statistics.Moving;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Statistics;
namespace Accord.Tests.Statistics
{
    
    
    /// <summary>
    ///This is a test class for MovingStatisticsTest and is intended
    ///to contain all MovingStatisticsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MovingStatisticsTest
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
        ///A test for Push
        ///</summary>
        [TestMethod()]
        public void PushTest()
        {
            double[] values = { 0.24, 1.61, 2.22, 5.82 };

            int windowSize = values.Length;

            MovingNormalStatistics target = new MovingNormalStatistics(windowSize);

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double actualMean = target.Mean;
            double expectedMean = Tools.Mean(values);
            Assert.AreEqual(expectedMean, actualMean);

            double actualVariance = target.Variance;
            double expectedVariance = Tools.Variance(values);
            Assert.AreEqual(expectedVariance, actualVariance);
        }

        /// <summary>
        ///A test for Push
        ///</summary>
        [TestMethod()]
        public void PushTest2()
        {
            double[] values = { 0.24, 1.61, 2.22, 5.82 };

            int windowSize = values.Length;

            MovingNormalStatistics target = new MovingNormalStatistics(windowSize);

            target.Push(0.29);
            target.Push(1.11);

            for (int i = 0; i < values.Length; i++)
                target.Push(values[i]);

            double actualMean = target.Mean;
            double expectedMean = Tools.Mean(values);
            Assert.AreEqual(expectedMean, actualMean);

            double actualVariance = target.Variance;
            double expectedVariance = Tools.Variance(values);
            Assert.AreEqual(expectedVariance, actualVariance);
        }
    }
}
