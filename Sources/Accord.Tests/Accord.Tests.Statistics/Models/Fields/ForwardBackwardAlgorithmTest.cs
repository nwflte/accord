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

using Accord.Statistics.Models.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Statistics.Models.Fields.Functions;
using Accord.Statistics.Models.Markov;
using Accord.Math;

namespace Accord.Tests.Statistics
{
    
    
    /// <summary>
    ///This is a test class for ForwardBackwardAlgorithmTest and is intended
    ///to contain all ForwardBackwardAlgorithmTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ForwardBackwardAlgorithmTest
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
        ///A test for Backward
        ///</summary>
        [TestMethod()]
        public void BackwardTest()
        {
            HiddenMarkovModel hmm = createHMM();
            IPotentialFunction function = new HiddenMarkovModelPotentialFunction(hmm);

            int[] observations = { 0, 1, 1, 0, 1 };

            double[,] expected = Accord.Statistics.Models.Markov.
                ForwardBackwardAlgorithm.Backward(hmm, observations);

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Backward(function, observations);


            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }

        /// <summary>
        ///A test for Forward
        ///</summary>
        [TestMethod()]
        public void ForwardTest()
        {
            HiddenMarkovModel hmm = createHMM();
            IPotentialFunction function = new HiddenMarkovModelPotentialFunction(hmm);

            int[] observations = { 0, 1, 1, 0, 1 };

            double[,] expected = Accord.Statistics.Models.Markov.
                ForwardBackwardAlgorithm.Forward(hmm, observations);

            double[,] actual = Accord.Statistics.Models.Fields.
                ForwardBackwardAlgorithm.Forward(function, observations);


            Assert.IsTrue(expected.IsEqual(actual, 1e-6));
        }
    }
}
