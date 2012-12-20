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

namespace Accord.Tests.Imaging
{
    using Accord.Imaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.MachineLearning;
    using System.Collections.Generic;
    using System.Drawing;
    using Accord.Math;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;

    [TestClass()]
    public class BagOfVisualWordsTest
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


        // Load some test images
        Bitmap[] images =
        {
            Properties.Resources.flower01,
            Properties.Resources.flower02,
            Properties.Resources.flower03,
            Properties.Resources.flower04,
            Properties.Resources.flower05,
            Properties.Resources.flower06,
        };

        [TestMethod()]
        public void BagOfVisualWordsConstructorTest()
        {
            BagOfVisualWords bow = new BagOfVisualWords(10);

            var points = bow.Compute(images, 1e-3);

            Assert.AreEqual(10, bow.NumberOfWords);
            Assert.AreEqual(6, points.Length);

            Assert.AreEqual(405, points[0].Count);
            Assert.AreEqual(724, points[1].Count);
            Assert.AreEqual(550, points[2].Count);
            Assert.AreEqual(457, points[3].Count);
            Assert.AreEqual(725, points[4].Count);
            Assert.AreEqual(1258, points[5].Count);

            Assert.AreEqual(388.04379807392485, points[0][0].X);
            Assert.AreEqual(105.99344045132918, points[0][0].Y);

            Assert.AreEqual(335.64199612902496, points[3][7].X);
            Assert.AreEqual(152.20031176259624, points[2][3].Y);

            Assert.AreEqual(561.95888019580082, points[2][52].X);
            Assert.AreEqual(153.66589796202763, points[1][11].Y);

            Assert.AreEqual(639.25228448622192, points[0][42].X);
            Assert.AreEqual(370.24258266581012, points[4][125].Y);
        }

        [TestMethod()]
        public void GetFeatureVectorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);
            Bitmap image = images[0];

            // The Bag-of-Visual-Words model converts arbitrary-size images 
            // into fixed-length feature vectors. In this example, we will
            // be setting the codebook size to 10. This means all generated
            // feature vectors will have the same length of 10.

            // Create a new Bag-of-Visual-Words (BoW) model
            BagOfVisualWords bow = new BagOfVisualWords(10);

            // Compute the model using
            // a set of training images
            bow.Compute(images);

            // After this point, we will be able to translate
            // images into double[] feature vectors using
            double[] feature = bow.GetFeatureVector(image);

            Assert.AreEqual(10, feature.Length);


            double[][] expected = 
            {
                new double[] { 48, 24, 103, 54, 37, 26, 42, 22, 34, 15 },
                new double[] { 28, 64, 96, 121, 25, 110, 68, 105, 17, 90 },
                new double[] { 74, 48, 142, 37, 22, 28, 50, 49, 56, 44 },
            };

            double[][] actual = new double[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = bow.GetFeatureVector(images[i]);
            
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod()]
        public void SerializeTest()
        {
            Accord.Math.Tools.SetupGenerator(0);

            BagOfVisualWords bow = new BagOfVisualWords(10);

            bow.Compute(images);

            double[][] expected = new double[images.Length][];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = bow.GetFeatureVector(images[i]);

            MemoryStream stream = new MemoryStream();
            BinaryFormatter fmt = new BinaryFormatter();
            fmt.Serialize(stream, bow);
            stream.Seek(0, SeekOrigin.Begin);
            bow = (BagOfVisualWords)fmt.Deserialize(stream);

            double[][] actual = new double[expected.Length][];
            for (int i = 0; i < actual.Length; i++)
                actual[i] = bow.GetFeatureVector(images[i]);


            Assert.IsTrue(expected.IsEqual(actual));
        }

    }
}
