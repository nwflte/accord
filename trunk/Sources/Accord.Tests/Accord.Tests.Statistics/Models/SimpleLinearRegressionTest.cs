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

using Accord.Statistics.Models.Regression.Linear;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for SimpleLinearRegressionTest and is intended
    ///to contain all SimpleLinearRegressionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SimpleLinearRegressionTest
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
        ///A test for Regress
        ///</summary>
        [TestMethod()]
        public void RegressTest()
        {
            // Declare some example data
            double[] inputs =  { 80, 60, 10, 20, 30 };
            double[] outputs = { 20, 40, 30, 50, 60 };

            // Create a new Simple Linear Regression
            SimpleLinearRegression linreg = new SimpleLinearRegression();

            // Perform the regression
            linreg.Regress(inputs, outputs);

            // Compute the output for a given input
            double y = linreg.Compute(85);

            Assert.AreEqual(28.0882352941192, y);

            // Expected slope and intercept
            double eSlope = -0.264706;
            double eIntercept = 50.588235;

            // Actual slope and intercept
            double aSlope = linreg.Slope;
            double aIntercept = linreg.Intercept;

            Assert.AreEqual(eSlope, aSlope,0.0001);
            Assert.AreEqual(eIntercept, aIntercept,0.0001);
        }
    }
}
