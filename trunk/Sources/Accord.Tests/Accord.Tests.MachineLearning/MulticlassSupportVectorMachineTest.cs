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

using Accord.MachineLearning.VectorMachines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accord.Statistics.Kernels;

namespace Accord.Tests.MachineLearning
{


    /// <summary>
    ///This is a test class for MulticlassSupportVectorMachineTest and is intended
    ///to contain all MulticlassSupportVectorMachineTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MulticlassSupportVectorMachineTest
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
        ///A test for MulticlassSupportVectorMachine Constructor
        ///</summary>
        [TestMethod()]
        public void MulticlassSupportVectorMachineConstructorTest()
        {
            int inputs = 1;
            IKernel kernel = new Linear();
            int classes = 4;
            var target = new MulticlassSupportVectorMachine(inputs, kernel, classes);

            Assert.AreEqual(3, target.Machines.Length);
            Assert.AreEqual(classes * (classes - 1) / 2, target.Machines[0].Length + target.Machines[1].Length + target.Machines[2].Length);

            for (int i = 0; i < classes; i++)
            {
                for (int j = 0; j < classes; j++)
                {
                    if (i == j) continue;

                    var machine = target[i, j];
                    Assert.IsNotNull(machine);
                }
            }
        }

        [TestMethod()]
        public void MulticlassSupportVectorMachineConstructorTest2()
        {
            int inputs = 1;
            int classes = 3;

            IKernel kernel = new Linear();

            var target = new MulticlassSupportVectorMachine(inputs, kernel, classes);

            target[0, 1].Kernel = new Gaussian(0.1);
            target[0, 2].Kernel = new Linear();
            target[1, 2].Kernel = new Polynomial(2);

            Assert.AreEqual(target[0, 0], target[0, 0]);
            Assert.AreEqual(target[1, 1], target[1, 1]);
            Assert.AreEqual(target[2, 2], target[2, 2]);
            Assert.AreEqual(target[0, 1], target[1, 0]);
            Assert.AreEqual(target[0, 2], target[0, 2]);
            Assert.AreEqual(target[1, 2], target[1, 2]);

            Assert.AreNotEqual(target[0, 1], target[0, 2]);
            Assert.AreNotEqual(target[1, 2], target[0, 2]);
            Assert.AreNotEqual(target[1, 2], target[0, 1]);
        }
    }
}
