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

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;    
    
    [TestClass()]
    public class KNearestNeighborTest
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
        public void KNearestNeighborConstructorTest()
        {
            double[][] inputs = 
            {
                new double[] { -5, -2, -1 },
                new double[] { -5, -5, -6 },

                new double[] {  2,  1,  1 },
                new double[] {  1,  1,  2 },
                new double[] {  1,  2,  2 },
                new double[] {  3,  1,  2 },

                new double[] { 11,  5,  4 },
                new double[] { 15,  5,  6 },
                new double[] { 10,  5,  6 },
            };

            int[] outputs =
            {
                0, 0,
                1, 1, 1, 1,
                2, 2, 2
            };

            int k = 3;
           
            KNearestNeighbor target = new KNearestNeighbor(k, inputs, outputs);

            for (int i = 0; i < inputs.Length; i++)
            {
                int actual = target.Compute(inputs[i]);
                int expected = outputs[i];

                Assert.AreEqual(expected, actual);
            }

            double[][] test = 
            {
                new double[] { -4, -3, -1 },
                new double[] { -5, -4, -4 },

                new double[] {  5,  3,  4 },
                new double[] {  3,  1,  6 },

                new double[] { 10,  5,  4 },
                new double[] { 13,  4,  5 },
            };

            int[] expectedOutputs =
            {
                0, 0,
                1, 1,
                2, 2,
            };

            for (int i = 0; i < test.Length; i++)
            {
                int actual = target.Compute(test[i]);
                int expected = expectedOutputs[i];

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
