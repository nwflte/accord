using Accord.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Accord.Tests.Math
{


    /// <summary>
    ///This is a test class for GammaTest and is intended
    ///to contain all GammaTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GammaTest
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


        [TestMethod()]
        public void FunctionTest()
        {
            double[] x = 
            {
                1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0,
                3.1, 5.7, 56.2, 53.8, 5.1, 6.5, 8.8, 114.2, 1024.6271,
               -2, -52.1252, -0.10817480950786047, -0.11961291417237133,
                -0.12078223763524518, 0, 281982742.12985912, 0.5212392
            };

            double[] expected =    
            {
                  1.000000000000000e+00, 0.9513507698668732, 9.181687423997607e-01,
                  8.974706963062772e-01, 8.872638175030753e-01, 8.862269254527581e-01,
                  8.935153492876903e-01, 9.086387328532904e-01, 9.313837709802427e-01, 
                  9.617658319073874e-01, 1.000000000000000e+00, 2.197620278392477e+00,
                  7.252763452022295e+01, 2.835938400359957e+73, 1.929366760161528e+69,
                  2.793175373836837e+01, 2.878852778150444e+02, 2.633998635450860e+04,
                  5.749274244634086e+184, Double.PositiveInfinity, Double.NaN,
                 -6.188338737526232e-68, -9.940515795403039e+00,  -9.070713053754153e+00,
                 -8.991245623853780e+00, Double.NaN, Double.PositiveInfinity, 1.701905559094028e+00

            };

            for (int i = 0; i < x.Length; i++)
            {
                double xi = x[i];
                double expectedi = expected[i];

                if (Double.IsNaN(expectedi))
                {
                    bool thrown = false;
                    try { Gamma.Function(xi); }
                    catch { thrown = true; }
                    Assert.IsTrue(thrown);
                }
                else
                {
                    double actual = Gamma.Function(xi);

                    Assert.AreEqual(expectedi, actual, System.Math.Abs(expectedi) * 1e-12);
                }
            }
        }


        [TestMethod()]
        public void LogTest()
        {
            double[] x = 
            {
                1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0,
                3.1, 5.7, 56.2, 53.8, 5.1, 6.5, 8.8, 114.2, 1024.6271,
               -2, -52.1252, -0.10817480950786047, -0.11961291417237133,
                -0.12078223763524518, 0, 281982742.12985912, 0.5212392
            };

            double[] expected =    
            {
                 0.000000000000000e+00,  -0.04987244125983974, -8.537409000331581e-02,
                -1.081748095078605e-01, -1.196129141723713e-01, -1.207822376352452e-01,
                -1.125917656967558e-01, -9.580769740706588e-02, -7.108387291437214e-02, 
                -3.898427592308334e-02,  0.000000000000000e+00,  7.873750832738625e-01,
                 4.283967655031580e+00,  1.691310846763928e+02,  1.595355632621249e+02,
                 3.329764168475224e+00,  5.662562059857142e+00,  1.017884345724507e+01,
                 4.254247307394230e+02,  6.075627024736053e+03,  Double.PositiveInfinity,
                 -1.547531196513472e+02,  2.296618910207815e+00,  2.205050877768351e+00,
                 2.196251395487650e+00,  Double.PositiveInfinity,  5.204655969482103e+09,
                 5.317485404177827e-01
            };

            for (int i = 0; i < x.Length; i++)
            {
                double xi = x[i];
                double expectedi = expected[i];

                if (Double.IsNaN(expectedi) || Double.IsInfinity(expectedi))
                {
                    bool thrown = false;
                    try { Gamma.Function(xi); }
                    catch { thrown = true; }
                    Assert.IsTrue(thrown);
                }
                else
                {
                    double actual = Gamma.Log(xi);

                    Assert.AreEqual(expectedi, actual, System.Math.Abs(expectedi) * 1e-14);
                }
            }
        }


        [TestMethod()]
        public void DigammaTest()
        {
            double[] x = { 1, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0 };

            double[] expected =    
            {
                -0.5772156649015329,
                -0.4237549404110768, 
                -0.2890398965921883,  
                -0.1691908888667997,
                -0.06138454458511615,
                 0.03648997397857652,
                 0.1260474527734763,
                 0.208547874873494,
                 0.2849914332938615,
                 0.3561841611640597,
                 0.4227843350984671,
            };

            for (int i = 0; i < x.Length; i++)
            {
                double xi = x[i];
                double expectedi = expected[i];
                double actual = Gamma.Digamma(xi);

                Assert.AreEqual(expectedi, actual, 1e-10);
            }
        }
    }
}
