// Accord Statistics Library
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

namespace Accord.Statistics.Testing
{
    using System;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Binomial test.
    /// </summary>
    /// 
    [Serializable]
    public class BinomialTest : HypothesisTest<BinomialDistribution>
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public OneSampleHypothesis Hypothesis { get; protected set; }

        /// <summary>
        ///   Tests the probability of two outcomes in a series of experiments.
        /// </summary>
        /// 
        /// <param name="trials">The experimental trials.</param>
        /// <param name="hypothesizedProbability">The hypothesized occurence probability.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public BinomialTest(bool[] trials, double hypothesizedProbability = 0.5,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {

            if (trials == null) throw new ArgumentNullException("trials");

            if (hypothesizedProbability < 0 || hypothesizedProbability > 1.0)
                throw new ArgumentOutOfRangeException("hypothesizedProbability");

            int successes = 0;

            for (int i = 0; i < trials.Length; i++)
                if (trials[i]) successes++;

            Compute(successes, trials.Length, hypothesizedProbability, alternate);
        }

        /// <summary>
        ///   Tests the probability of two outcomes in a series of experiments.
        /// </summary>
        /// 
        /// <param name="successes">The number of successes in the trials.</param>
        /// <param name="trials">The total number of experimental trials.</param>
        /// <param name="hypothesizedProbability">The hypothesized occurence probability.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public BinomialTest(int successes, int trials, double hypothesizedProbability = 0.5,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            if (successes > trials)
                throw new ArgumentOutOfRangeException("successes");

            if (hypothesizedProbability < 0 || hypothesizedProbability > 1.0)
                throw new ArgumentOutOfRangeException("hypothesizedProbability");

            Compute(successes, trials, hypothesizedProbability, alternate);
        }


        /// <summary>
        ///   Creates a Binomial test.
        /// </summary>
        /// 
        protected BinomialTest()
        {
        }


        /// <summary>
        ///   Computes the Binomial test.
        /// </summary>
        /// 
        protected void Compute(double statistic, int m, double p, OneSampleHypothesis alternate)
        {
            this.Statistic = statistic;
            this.StatisticDistribution = new BinomialDistribution(m, p);
            this.Hypothesis = alternate;
            this.Tail = (DistributionTail)alternate;
            this.PValue = StatisticToPValue(Statistic);

            this.OnSizeChanged();
        }

        /// <summary>
        ///   Converts a given test statistic to a p-value.
        /// </summary>
        /// 
        /// <param name="x">The value of the test statistic.</param>
        /// 
        /// <returns>The p-value for the given statistic.</returns>
        /// 
        public override double StatisticToPValue(double x)
        {
            double p;
            switch (Tail)
            {
                case DistributionTail.TwoTail:
                    p = 2.0 * StatisticDistribution.DistributionFunction((int)x);
                    break;

                case DistributionTail.OneUpper:
                    p = StatisticDistribution.ComplementaryDistributionFunction((int)x, inclusive: true);
                    break;

                case DistributionTail.OneLower:
                    p = StatisticDistribution.DistributionFunction((int)x);
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return p;
        }

        /// <summary>
        ///   Converts a given p-value to a test statistic.
        /// </summary>
        /// 
        /// <param name="p">The p-value.</param>
        /// 
        /// <returns>The test statistic which would generate the given p-value.</returns>
        /// 
        public override double PValueToStatistic(double p)
        {
            double b;
            switch (Tail)
            {
                case DistributionTail.OneLower:
                    b = StatisticDistribution.InverseDistributionFunction(p);
                    break;
                case DistributionTail.OneUpper:
                    b = StatisticDistribution.InverseDistributionFunction(1.0 - p);
                    break;
                case DistributionTail.TwoTail:
                    b = StatisticDistribution.InverseDistributionFunction(1.0 - p / 2.0);
                    break;
                default: throw new InvalidOperationException();
            }

            return b;
        }

    }
}
