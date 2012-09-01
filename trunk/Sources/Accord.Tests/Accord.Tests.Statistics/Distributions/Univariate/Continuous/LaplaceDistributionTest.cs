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
    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass()]
    public class LaplaceDistributionTest
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
        public void LaplaceDistributionConstructorTest()
        {
            {
                LaplaceDistribution target = new LaplaceDistribution(0, 0.2);

                double[] expected = { 2.5, 1.5163266, 0.919699, 0.557825, 0.338338 };

                for (int i = 0; i < expected.Length; i++)
                {

                    double x = i / 10.0;
                    double actual = target.ProbabilityDensityFunction(x);
                    Assert.AreEqual(expected[i], actual, 1e-6);
                    Assert.IsFalse(double.IsNaN(actual));
                }
            }

            {
                LaplaceDistribution target = new LaplaceDistribution(-2, 5.79);

                double[] expected = { 0.0666469, 0.0655057, 0.06438406, 0.0632816234, 0.062198060, 0.061133051 };

                for (int i = 0; i < expected.Length; i++)
                {

                    double x = (i - 5) / 10.0;
                    double actual = target.ProbabilityDensityFunction(x);
                    Assert.AreEqual(expected[i], actual, 1e-8);
                    Assert.IsFalse(double.IsNaN(actual));
                }
            }
        }

    }
}
