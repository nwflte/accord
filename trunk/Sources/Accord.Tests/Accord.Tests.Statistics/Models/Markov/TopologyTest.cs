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

using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Math;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
namespace Accord.Tests.Statistics
{


    /// <summary>
    ///This is a test class for HiddenMarkovModelTest and is intended
    ///to contain all HiddenMarkovModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TopologyTest
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
        ///  A test for Uniform 
        /// </summary>
        [TestMethod()]
        public void UniformTest()
        {
            // Create a new Ergodic hidden Markov model with three
            //   fully-connected states and four sequence symbols.
            var model = new HiddenMarkovModel(new Ergodic(3), 4);

            Assert.AreEqual(model.States, 3);
            Assert.IsTrue(model.Transitions.IsEqual(new double[,] 
            {
                { 0.33, 0.33, 0.33 },
                { 0.33, 0.33, 0.33 },
                { 0.33, 0.33, 0.33 },
            }, 0.01));
        }

        /// <summary>
        ///  A test for Uniform 
        /// </summary>
        [TestMethod()]
        public void ForwardTest()
        {
            // Create a new Forward-only hidden Markov model with
            // three forward-only states and four sequence symbols.
            var model = new HiddenMarkovModel(new Forward(3), 4);

            Assert.AreEqual(model.States, 3);
            Assert.IsTrue(model.Transitions.IsEqual(new double[,] 
            {
                { 0.33, 0.33, 0.33 },
                { 0.00, 0.50, 0.50 },
                { 0.00, 0.00, 1.00 },
            }, 0.01));
        }

        /// <summary>
        ///  A test for Uniform 
        /// </summary>
        [TestMethod()]
        public void ForwardTest2()
        {
            var topology = new Forward(3, 2);

            Assert.AreEqual(topology.States, 3);
            Assert.AreEqual(topology.Deepness, 2);

            double[,] actual;
            double[] pi;
            int states = topology.Create(out actual, out pi);
            var expected = new double[,] 
            {
                { 0.50, 0.50, 0.00 },
                { 0.00, 0.50, 0.50 },
                { 0.00, 0.00, 1.00 },
            };
            
            Assert.IsTrue(actual.IsEqual(expected, 0.01));
            Assert.AreEqual(states, 3);
        }
    }

}