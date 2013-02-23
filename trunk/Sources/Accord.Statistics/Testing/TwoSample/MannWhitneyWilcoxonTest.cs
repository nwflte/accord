// Accord Statistics Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Mann-Whitney-Wilcoxon test for unpaired samples.
    /// </summary>
    /// 
    [Serializable]
    public class MannWhitneyWilcoxonTest : HypothesisTest<MannWhitneyDistribution>
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public TwoSampleHypothesis Hypothesis { get; protected set; }
        
        /// <summary>
        ///   Gets the number of samples in the first sample.
        /// </summary>
        /// 
        public int Samples1 { get; protected set; }

        /// <summary>
        ///   Gets the number of samples in the second sample.
        /// </summary>
        /// 
        public int Samples2 { get; protected set; }

        /// <summary>
        ///   Gets the rank statistics for the first sample.
        /// </summary>
        /// 
        public double[] Rank1 { get; protected set; }

        /// <summary>
        ///   Gets the rank statistics for the second sample.
        /// </summary>
        /// 
        public double[] Rank2 { get; protected set; }

        /// <summary>
        ///   Gets the sum of ranks for the first sample. Often known as Ta.
        /// </summary>
        /// 
        public double RankSum1 { get; protected set; }

        /// <summary>
        ///   Gets the sum of ranks for the second sample. Often known as Tb.
        /// </summary>
        /// 
        public double RankSum2 { get; protected set; }

        /// <summary>
        ///   Gets the difference between the expected value for
        ///   the observed value of <see cref="RankSum1"/> and its
        ///   expected value under the null hypothesis. Often known as Ua.
        /// </summary>
        /// 
        public double Statistic1 { get; protected set; }

        /// <summary>
        ///   Gets the difference between the expected value for
        ///   the observed value of <see cref="RankSum2"/> and its
        ///   expected value under the null hypothesis. Often known as Ub.
        /// </summary>
        /// 
        public double Statistic2 { get; protected set; }

        /// <summary>
        ///   Tests whether two samples comes from the 
        ///   same distribution without assuming normality.
        /// </summary>
        /// 
        /// <param name="sample1">The first sample.</param>
        /// <param name="sample2">The second sample.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        ///
        public MannWhitneyWilcoxonTest(double[] sample1, double[] sample2, 
            TwoSampleHypothesis alternate = TwoSampleHypothesis.ValuesAreDifferent)
        {
            int n1 = sample1.Length;
            int n2 = sample2.Length;
            int n = n1 + n2;

            // Concatenate both samples and rank them
            double[] samples = sample1.Concatenate(sample2);
            double[] rank = Accord.Statistics.Tools.Rank(samples);

            // Split the rankings back and sum
            Rank1 = rank.Submatrix(0, n1 - 1);
            Rank2 = rank.Submatrix(n1, n - 1);

            double t1 = RankSum1 = Rank1.Sum();
            double t2 = RankSum2 = Rank2.Sum();

            // Estimated values for t under the null
            double t1max = n1 * n2 + (n1 * (n1 + 1)) / 2.0;
            double t2max = n1 * n2 + (n2 * (n2 + 1)) / 2.0;

            // Diff in observed t and estimated t
            double u1 = Statistic1 = t1max - t1;
            double u2 = Statistic2 = t2max - t2;

            double hypothesizedValue = (n1 * n2) / 2.0;

            Compute(u1, rank, n1, n2, alternate);
        }

        /// <summary>
        ///   Computes the Mann-Whitney-Wilcoxon test.
        /// </summary>
        /// 
        protected void Compute(double statistic, double[] ranks, int n1, int n2, TwoSampleHypothesis alternate)
        {
            this.Statistic = statistic;
            this.StatisticDistribution = new MannWhitneyDistribution(ranks, n1, n2);
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
                    p = 2.0 * StatisticDistribution.ComplementaryDistributionFunction(x);
                    break;

                case DistributionTail.OneUpper:
                    p = StatisticDistribution.DistributionFunction(x);
                    break;

                case DistributionTail.OneLower:
                    p = StatisticDistribution.ComplementaryDistributionFunction(x);
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
