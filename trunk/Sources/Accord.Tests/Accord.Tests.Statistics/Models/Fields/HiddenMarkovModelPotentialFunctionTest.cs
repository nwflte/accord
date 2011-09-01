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

using Accord.Statistics.Models.Fields.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Fields.Features;

namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for HiddenMarkovModelPotentialFunctionTest and is intended
    ///to contain all HiddenMarkovModelPotentialFunctionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HiddenMarkovModelPotentialFunctionTest
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

        private static HiddenMarkovModel createHMM()
        {
            double[] initial = { 1.0, 0.0 };

            double[,] transitions = 
            {
                { 0.33, 0.66 },
                { 0.00, 1.00 },

            };

            double[,] emissions =
            {
                { 0.25, 0.25, 0.50 },
                { 0.05, 0.05, 0.90 }
            };

            HiddenMarkovModel model = new HiddenMarkovModel(transitions, emissions, initial);
            return model;
        }


        /// <summary>
        ///A test for HiddenMarkovModelPotentialFunction Constructor
        ///</summary>
        [TestMethod()]
        public void HiddenMarkovModelPotentialFunctionConstructorTest()
        {
            HiddenMarkovModel model = createHMM();

            HiddenMarkovModelPotentialFunction target = new HiddenMarkovModelPotentialFunction(model);

            IFeature[] features = target.Features;
            double[] weights = target.Weights;

            Assert.AreEqual(features.Length, 12);
            Assert.AreEqual(weights.Length, 12);

            int k = 0;
            for (int i = 0; i < model.States; i++)
                Assert.AreEqual(System.Math.Log(model.Probabilities[i]), weights[k++]);

            for (int i = 0; i < model.States; i++)
                for (int j = 0; j < model.States; j++)
                    Assert.AreEqual(System.Math.Log(model.Transitions[i, j]), weights[k++]);

            for (int i = 0; i < model.States; i++)
                for (int j = 0; j < model.Symbols; j++)
                    Assert.AreEqual(System.Math.Log(model.Emissions[i, j]), weights[k++]);

        }


        /// <summary>
        ///A test for Compute
        ///</summary>
        [TestMethod()]
        public void ComputeTest()
        {
            HiddenMarkovModel model = createHMM();

            HiddenMarkovModelPotentialFunction target = new HiddenMarkovModelPotentialFunction(model);

            double actual;
            double expected;

            int[] x = { 0, 1 };

            for (int t = 0; t < x.Length; t++)
            {
                for (int i = 0; i < model.States; i++)
                {
                    // Check initial state transitions
                    expected = model.Probabilities[i] * model.Emissions[i, x[t]];
                    actual = target.Compute(-1, i, x, t);
                    Assert.AreEqual(expected, actual, 1e-6);

                    // Check normal state transitions
                    for (int j = 0; j < model.States; j++)
                    {
                        expected = model.Transitions[i, j] * model.Emissions[j, x[t]];
                        actual = target.Compute(i, j, x, t);
                        Assert.AreEqual(expected, actual, 1e-6);
                    }
                }
            }

        }
    }
}
