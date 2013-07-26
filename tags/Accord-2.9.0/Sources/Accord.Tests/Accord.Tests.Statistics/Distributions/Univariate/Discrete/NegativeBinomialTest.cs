// Accord Unit Tests
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    public class NegativeBinomialTest
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
        public void NegativeBinomialConstructorTest()
        {
            double expected, actual;

            {
                NegativeBinomial target = new NegativeBinomial(6, 0.42);
                actual = target.ProbabilityMassFunction(-1);
                expected = 0.0;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityMassFunction(0);
                expected = 0.00548903;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityMassFunction(1);
                expected = 0.0191018;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityMassFunction(2);
                expected = 0.0387767;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));

                actual = target.ProbabilityMassFunction(10);
                expected = 0.0710119;
                Assert.AreEqual(expected, actual, 1e-7);
                Assert.IsFalse(Double.IsNaN(actual));
            }
        }
    }
}
