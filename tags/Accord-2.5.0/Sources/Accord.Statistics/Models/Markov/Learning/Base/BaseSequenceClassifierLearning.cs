// Accord Statistics Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//

namespace Accord.Statistics.Models.Markov.Learning
{
    using System;
    using Accord.Math;
    using System.Threading.Tasks;

    /// <summary>
    ///   Configuration function delegate for Sequence Classifier Learning algorithms.
    /// </summary>
    public delegate IUnsupervisedLearning ClassifierLearningAlgorithmConfiguration(int modelIndex);


    /// <summary>
    ///   Abstract base class for Sequence Classifier learning algorithms.
    /// </summary>
    /// 
    public abstract class BaseSequenceClassifierLearning<TClassifier, TModel>
        where TClassifier : BaseHiddenMarkovClassifier<TModel>
        where TModel : IHiddenMarkovModel
    {


        /// <summary>
        ///   Gets the classifier being trained by this instance.
        /// </summary>
        /// <value>The classifier being trained by this instance.</value>
        /// 
        public TClassifier Classifier { get; private set; }

        /// <summary>
        ///   Gets or sets the configuration function specifying which
        ///   training algorithm should be used for each of the models
        ///   in the hidden Markov model set.
        /// </summary>
        /// 
        public ClassifierLearningAlgorithmConfiguration Algorithm { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether a threshold model
        ///   should be created or updated after training to support rejection.
        /// </summary>
        /// <value><c>true</c> to update the threshold model after training;
        /// otherwise, <c>false</c>.</value>
        /// 
        public bool Rejection { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the class priors
        ///   should be estimated from the data, as in an empirical bayes method.
        /// </summary>
        /// 
        public bool Empirical { get; set; }


        /// <summary>
        ///   Creates a new instance of the learning algorithm for a given 
        ///   Markov sequence classifier using the specified configuration
        ///   function.
        /// </summary>
        /// 
        protected BaseSequenceClassifierLearning(TClassifier classifier,
            ClassifierLearningAlgorithmConfiguration algorithm)
        {
            this.Classifier = classifier;
            this.Algorithm = algorithm;
        }



        /// <summary>
        ///   Trains each model to recognize each of the output labels.
        /// </summary>
        /// <returns>The sum log-likelihood for all models after training.</returns>
        /// 
        protected double Run<T>(T[] inputs, int[] outputs)
        {
            int classes = Classifier.Classes;
            double[] logLikelihood = new double[classes];
            int[] classCounts = new int[classes];

            // For each model,
#if !DEBUG
            Parallel.For(0, classes, i =>
#else
            for (int i = 0; i < classes; i++)
#endif
            {
                // Select the input/output set corresponding
                //  to the model's specialization class
                int[] inx = outputs.Find(y => y == i);
                T[] observations = inputs.Submatrix(inx);

                classCounts[i] = observations.Length;

                if (observations.Length > 0)
                {
                    // Create and configure the learning algorithm
                    IUnsupervisedLearning teacher = Algorithm(i);

                    // Train the current model in the input/output subset
                    logLikelihood[i] = teacher.Run(observations as Array[]);
                }
            }
#if !DEBUG
            );
#endif

            if (Empirical)
            {
                for (int i = 0; i < classes; i++)
                    Classifier.Priors[i] = (double)classCounts[i] / inputs.Length;
            }

            if (Rejection)
                Classifier.Threshold = Threshold();

            // Returns the sum log-likelihood for all models.
            return logLikelihood.Sum();
        }

        /// <summary>
        ///   Creates a new <see cref="Threshold">threshold model</see>
        ///   for the current set of Markov models in this sequence classifier.
        /// </summary>
        /// <returns>A <see cref="Threshold">threshold Markov model</see>.</returns>
        /// 
        public abstract TModel Threshold();

    }
}
