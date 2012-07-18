

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Accord.Math;
    using Accord.Statistics;
    
    
    [TestClass()]
    public class KModesTest
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
        public void KModesConstructorTest()
        {
            Accord.Math.Tools.SetupGenerator(0);


            // Declare some observations
            int[][] observations = 
            {
                new int[] { 0, 0   }, // a
                new int[] { 0, 1   }, // a
                new int[] { 1, 1   }, // a
 
                new int[] { 5, 3   }, // b
                new int[] { 6, 8   }, // b
                new int[] { 6, 7   }, // b
                new int[] { 5, 8   }, // b

                new int[] { 12, 14 }, // c
                new int[] { 13, 14 }, // c
            };

            int[][] orig = observations.MemberwiseClone();

            // Create a new K-Means algorithm with 3 clusters 
            KModes kmodes = new KModes(3);

            // Compute the algorithm, retrieving an integer array
            //  containing the labels for each of the observations
            int[] labels = kmodes.Compute(observations);

            // As a result, the first three observations should belong to the
            //  same cluster (thus having the same label). The same should
            //  happen to the next four observations and to the last two.

            Assert.AreEqual(labels[0], labels[1]);
            Assert.AreEqual(labels[0], labels[2]);

            Assert.AreEqual(labels[3], labels[4]);
            Assert.AreEqual(labels[3], labels[5]);
            Assert.AreEqual(labels[3], labels[6]);

            Assert.AreEqual(labels[7], labels[8]);

            Assert.AreNotEqual(labels[0], labels[3]);
            Assert.AreNotEqual(labels[0], labels[7]);
            Assert.AreNotEqual(labels[3], labels[7]);


            int[] labels2 = kmodes.Nearest(observations);
            Assert.IsTrue(labels.IsEqual(labels2));

            // the data must not have changed!
            Assert.IsTrue(orig.IsEqual(observations));
        }

        
       
    }
}
