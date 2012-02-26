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

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Analysis;
    using Accord.Statistics.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class TwoMatrixKappaTestTest
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
        public void KappaTestConstructorTest2()
        {
            int[,] matrix1 =
            {
                { 317,  23,  0,  0 },
                {  61, 120,  0,  0 },
                {   2,   4, 60,  0 },
                {  35,  29,  0,  8 },
            };

            int[,] matrix2 =
            {
                { 377,  79,  0,  0 },
                {   2,  72,  0,  0 },
                {  33,   5, 60,  0 },
                {   3,  20,  0,  8 },
            };

            GeneralConfusionMatrix a = new GeneralConfusionMatrix(matrix1);
            GeneralConfusionMatrix b = new GeneralConfusionMatrix(matrix2);


            TwoMatrixKappaTest target = new TwoMatrixKappaTest(a, b);

            Assert.AreEqual(0.605, a.Kappa, 1e-3);
            Assert.IsFalse(double.IsNaN(a.Kappa));

            Assert.AreEqual(0.00073735, a.Variance, 1e-5);
            Assert.IsFalse(double.IsNaN(a.Variance));

            Assert.AreEqual(0.586, b.Kappa, 1e-3);
            Assert.IsFalse(double.IsNaN(b.Kappa));

            Assert.AreEqual(0.00087457, b.Variance, 1e-5);
            Assert.IsFalse(double.IsNaN(b.Variance));


            Assert.AreEqual(0.475, target.Statistic, 1e-3);
            Assert.IsFalse(double.IsNaN(target.Statistic));

            Assert.IsFalse(target.Significant);
        }



    }
}
