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

using Accord.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using Accord.Math.Formats;

namespace Accord.Tests.Math
{


    /// <summary>
    ///This is a test class for MatrixTest and is intended
    ///to contain all MatrixTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MatrixFormatTest
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


        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseTest()
        {
            string str;

            double[,] expected, actual;

            expected = new double[,] 
            {
                { 1, 2 },
                { 3, 4 },
            };


            str = "[1 2; 3 4]";
            actual = Matrix.Parse(str, OctaveMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = "1 2\r\n3 4";
            actual = Matrix.Parse(str, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"1 2
                    3 4";
            actual = Matrix.Parse(str, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"double[,]
                   {
                      { 1, 2 },
                      { 3, 4 }
                   };";
            actual = Matrix.Parse(str, CSharpMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"double[][]
                   {
                      new double[] { 1, 2 },
                      new double[] { 3, 4 }
                   };";
            actual = Matrix.Parse(str, CSharpJaggedMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));
        }

        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseJaggedTest()
        {
            string str;

            double[][] expected, actual;

            expected = new double[][] 
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 },
            };


            str = "[1 2; 3 4]";
            actual = Matrix.ParseJagged(str, OctaveMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = "1 2\r\n3 4";
            actual = Matrix.ParseJagged(str, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"1 2
                    3 4";
            actual = Matrix.ParseJagged(str, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"double[,]
                   {
                      { 1, 2 },
                      { 3, 4 }
                   };";
            actual = Matrix.ParseJagged(str, CSharpMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));

            str = @"double[][]
                   {
                      new double[] { 1, 2 },
                      new double[] { 3, 4 }
                   };";
            actual = Matrix.ParseJagged(str, CSharpJaggedMatrixFormatProvider.InvariantCulture);
            Assert.IsTrue(actual.IsEqual(expected));
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            double[,] matrix = 
            {
                { 1, 2 },
                { 3, 4 },
            };

            string expected, actual;

            expected = "[1 2; 3 4]";
            actual = Matrix.ToString(matrix, OctaveMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = "1 2 \r\n3 4";
            actual = Matrix.ToString(matrix, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = "new double[][] {\r\n" +
                       "    new double[] { 1, 2 },\r\n" +
                       "    new double[] { 3, 4 } \r\n" +
                       "};";
            actual = Matrix.ToString(matrix, CSharpJaggedMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = "new double[,] {\r\n" +
                       "    { 1, 2 },\r\n" +
                       "    { 3, 4 } \r\n" +
                       "};";
            actual = Matrix.ToString(matrix, CSharpMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest2()
        {
            double[][] matrix = 
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 },
            };

            string expected, actual;

            expected = "[1 2; 3 4]";
            actual = Matrix.ToString(matrix, OctaveMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = "1 2 \r\n3 4";
            actual = Matrix.ToString(matrix, DefaultMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = "new double[][] {\r\n" +
                       "    new double[] { 1, 2 },\r\n" +
                       "    new double[] { 3, 4 } \r\n" +
                       "};";
            actual = Matrix.ToString(matrix, CSharpJaggedMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);


            expected = "new double[,] {\r\n" +
                       "    { 1, 2 },\r\n" +
                       "    { 3, 4 } \r\n" +
                       "};";
            actual = Matrix.ToString(matrix, CSharpMatrixFormatProvider.InvariantCulture);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void StringFormat()
        {
            double[,] matrix = 
            {
                { 1, 2 },
                { 3, 4 },
            };

            string expected, actual;

            expected = "[1 2; 3 4]";
            actual = String.Format(OctaveMatrixFormatProvider.InvariantCulture, "{0:Ms}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "1 2 3 4";
            actual = String.Format(DefaultMatrixFormatProvider.InvariantCulture, "{0:Ms}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "new double[][] { new double[] { 1, 2 }, new double[] { 3, 4 } };";
            actual = String.Format(CSharpJaggedMatrixFormatProvider.InvariantCulture, "{0:Ms}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "new double[,] { { 1, 2 }, { 3, 4 } };";
            actual = String.Format(CSharpMatrixFormatProvider.InvariantCulture, "{0:Ms}", matrix);
            Assert.AreEqual(expected, actual);



            expected = "[1.00 2.00; 3.00 4.00]";
            actual = String.Format(OctaveMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "1.00 2.00 3.00 4.00";
            actual = String.Format(DefaultMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "new double[][] { new double[] { 1.00, 2.00 }, new double[] { 3.00, 4.00 } };";
            actual = String.Format(CSharpJaggedMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix);
            Assert.AreEqual(expected, actual);


            expected = "new double[,] { { 1.00, 2.00 }, { 3.00, 4.00 } };";
            actual = String.Format(CSharpMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void StringFormat2()
        {
            double[][] matrix = 
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 },
            };

            string expected, actual;


            expected = "[1.00 2.00; 3.00 4.00]";
            actual = String.Format(OctaveMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix as Array);
            Assert.AreEqual(expected, actual);

            expected = "1.00 2.00 3.00 4.00";
            actual = String.Format(DefaultMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix as Array);
            Assert.AreEqual(expected, actual);

            expected = "new double[][] { new double[] { 1.00, 2.00 }, new double[] { 3.00, 4.00 } };";
            actual = String.Format(CSharpJaggedMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix as Array);
            Assert.AreEqual(expected, actual);

            expected = "new double[,] { { 1.00, 2.00 }, { 3.00, 4.00 } };";
            actual = String.Format(CSharpMatrixFormatProvider.InvariantCulture, "{0:Ms,N2}", matrix as Array);
            Assert.AreEqual(expected, actual);

        }

    }
}
