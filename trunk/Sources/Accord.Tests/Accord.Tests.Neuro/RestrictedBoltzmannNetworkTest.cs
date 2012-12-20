using Accord.Neuro.Networks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AForge.Neuro;
using Accord.Neuro.Learning;

namespace Accord.Tests.Neuro
{


    [TestClass()]
    public class RestrictedBoltzmannNetworkTest
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
        public void CreateActivationNetworkTest()
        {
            double[][] inputs =
            {
                new double[] { 1,1,1,0,0,0 },
                new double[] { 1,0,1,0,0,0 },
                new double[] { 1,1,1,0,0,0 },
                new double[] { 0,0,1,1,1,0 },
                new double[] { 0,0,1,1,0,0 },
                new double[] { 0,0,1,1,1,0 }
            };

            double[][] outputs =
            {
                new double[] { 0 },
                new double[] { 0 },
                new double[] { 0 },
                new double[] { 1 },
                new double[] { 1 },
                new double[] { 1 },
            };

            RestrictedBoltzmannMachine network = createNetwork(inputs);

            ActivationNetwork ann = network.ToActivationNetwork(new SigmoidFunction(1), outputs: 1);

            ParallelResilientBackpropagationLearning teacher = new ParallelResilientBackpropagationLearning(ann);

            for (int i = 0; i < 100; i++)
            {
                teacher.RunEpoch(inputs, outputs);
            }

            double[] actual = new double[outputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                actual[i] = ann.Compute(inputs[i])[0];

            Assert.AreEqual(0, actual[0], 1e-10);
            Assert.AreEqual(0, actual[1], 1e-10);
            Assert.AreEqual(0, actual[2], 1e-10);
            Assert.AreEqual(1, actual[3], 1e-10);
            Assert.AreEqual(1, actual[4], 1e-10);
            Assert.AreEqual(1, actual[5], 1e-10);
        }

        private static RestrictedBoltzmannMachine createNetwork(double[][] inputs)
        {
            RestrictedBoltzmannMachine network = new RestrictedBoltzmannMachine(6, 2);

            network.Hidden.Neurons[0].Weights[0] = 0.00461421;
            network.Hidden.Neurons[0].Weights[1] = 0.04337112;
            network.Hidden.Neurons[0].Weights[2] = -0.10839599;
            network.Hidden.Neurons[0].Weights[3] = -0.06234004;
            network.Hidden.Neurons[0].Weights[4] = -0.03017057;
            network.Hidden.Neurons[0].Weights[5] = 0.09520391;
            network.Hidden.Neurons[0].Threshold = 0;

            network.Hidden.Neurons[1].Weights[0] = 0.08263872;
            network.Hidden.Neurons[1].Weights[1] = -0.118437;
            network.Hidden.Neurons[1].Weights[2] = -0.21710971;
            network.Hidden.Neurons[1].Weights[3] = 0.02332903;
            network.Hidden.Neurons[1].Weights[4] = 0.00953116;
            network.Hidden.Neurons[1].Weights[5] = 0.09870652;
            network.Hidden.Neurons[1].Threshold = 0;

            network.Visible.Neurons[0].Threshold = 0;
            network.Visible.Neurons[1].Threshold = 0;
            network.Visible.Neurons[2].Threshold = 0;
            network.Visible.Neurons[3].Threshold = 0;
            network.Visible.Neurons[4].Threshold = 0;
            network.Visible.Neurons[5].Threshold = 0;

            network.Visible.CopyReversedWeightsFrom(network.Hidden);


            ContrastiveDivergenceLearning target = new ContrastiveDivergenceLearning(network);

            int iterations = 5000;
            double[] errors = new double[iterations];
            for (int i = 0; i < iterations; i++)
                errors[i] = target.RunEpoch(inputs);

            return network;
        }
    }
}
