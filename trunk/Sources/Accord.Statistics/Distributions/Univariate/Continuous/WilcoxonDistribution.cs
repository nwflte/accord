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

namespace Accord.Statistics.Distributions.Univariate
{
    using System;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Math;


    /// <summary>
    ///   Wilcoxon's W statistic distribution.
    /// </summary>
    /// 
    /// <remarks>
    ///   This is the distribution for the positive side statistic, W+. Some textbooks
    ///   (and statistical packages) use alternate definitions for W, which should be
    ///   compared with the appropriate statistic tables or alternate distributions.
    /// </remarks>
    /// 
    [Serializable]
    public class WilcoxonDistribution : UnivariateContinuousDistribution
    {

        /// <summary>
        ///   Gets the number of effective samples.
        /// </summary>
        /// 
        public int Samples { get; private set; }

        /// <summary>
        ///   Gets the rank statistics for the distribution.
        /// </summary>
        /// 
        public double[] Ranks { get; private set; }

        private double[] table;

        /// <summary>
        ///   Creates a new Wilcoxon's W+ distribution.
        /// </summary>
        /// 
        /// <param name="ranks">The rank statistics for the samples.</param>
        /// 
        public WilcoxonDistribution(double[] ranks)
        {
            // Remove zero elements
            int[] idx = ranks.Find(x => x != 0);
            this.Ranks = ranks.Submatrix(idx);
            this.Samples = idx.Length;

            if (Samples < 12)
            {
                // For a small sample (< 12) the distribution is exact.

                // Generate all possible combinations in advance
                int[][] combinations = Combinatorics.TruthTable(Samples);

                // Transform binary into sign combinations
                for (int i = 0; i < combinations.Length; i++)
                    for (int j = 0; j < combinations[i].Length; j++)
                        combinations[i][j] = Math.Sign(combinations[i][j] * 2 - 1);

                // Compute all possible values for W+ considering those signs
                this.table = new double[combinations.Length];
                for (int i = 0; i < combinations.Length; i++)
                    table[i] = WPositive(combinations[i], ranks);

                Array.Sort(table);
            }
        }


        /// <summary>
        ///   Gets the cumulative distribution function (cdf) for
        ///   this distribution evaluated at point <c>k</c>.
        /// </summary>
        /// 
        /// <param name="w">A single point in the distribution range.</param>
        /// 
        /// <remarks>
        ///   The Cumulative Distribution Function (CDF) describes the cumulative
        ///   probability that a given value or any value smaller than it will occur.
        /// </remarks>
        /// 
        public override double DistributionFunction(double w)
        {
            if (Samples >= 12)
            {
                double z = ((w + 0.5) - Mean) / Math.Sqrt(Variance);

                double p = NormalDistribution.Standard.DistributionFunction(Math.Abs(z));

                return p;
            }
            else
            {
                for (int i = 0; i < table.Length; i++)
                    if (w <= table[i]) return i / (double)table.Length;

                return 1;
            }
        }

        /// <summary>
        ///   Computes the Wilcoxon's W+ statistic.
        /// </summary>
        /// 
        /// <remarks>
        ///   The W+ statistic is computed as the
        ///   sum of all positive signed ranks.
        /// </remarks>
        /// 
        public static double WPositive(int[] signs, double[] ranks)
        {
            double sum = 0;
            for (int i = 0; i < signs.Length; i++)
                if (signs[i] > 0) sum += ranks[i];

            return sum;
        }

        /// <summary>
        ///   Computes the Wilcoxon's W- statistic.
        /// </summary>
        /// 
        /// <remarks>
        ///   The W- statistic is computed as the
        ///   sum of all negative signed ranks.
        /// </remarks>
        /// 
        public static double WNegative(int[] signs, double[] ranks)
        {
            double sum = 0;
            for (int i = 0; i < signs.Length; i++)
                if (signs[i] < 0) sum += ranks[i];
            return sum;
        }

        /// <summary>
        ///   Computes the Wilcoxon's W statistic.
        /// </summary>
        /// 
        /// <remarks>
        ///   The W statistic is computed as the
        ///   minimum of the W+ and W- statistics.
        /// </remarks>
        /// 
        public static double WMinimum(int[] signs, double[] ranks)
        {
            double wp = 0, wn = 0;
            for (int i = 0; i < signs.Length; i++)
                if (signs[i] < 0) wn += ranks[i];
                else if (signs[i] > 0) wp += ranks[i];

            double min = Math.Min(wp, wn);

            return min;
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            return new WilcoxonDistribution(Ranks);
        }



        /// <summary>
        ///   Gets the mean for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's mean value.</value>
        /// 
        public override double Mean
        {
            get
            {
                int n = Samples;
                return n * (n + 1) / 4.0;
            }
        }

        /// <summary>
        ///   Gets the variance for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's variance.</value>
        /// 
        public override double Variance
        {
            get
            {
                int n = Samples;
                return (n * (n + 1) * (2 * n + 1)) / 24;
            }
        }

        /// <summary>
        ///   Gets the entropy for this distribution.
        /// </summary>
        /// 
        /// <value>The distribution's entropy.</value>
        /// 
        public override double Entropy
        {
            get { throw new NotSupportedException(); }
        }


        /// <summary>
        ///   Gets the probability density function (pdf) for
        ///   this distribution evaluated at point <c>w</c>.
        /// </summary>
        /// 
        /// <param name="w">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The probability of <c>w</c> occurring
        ///   in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double ProbabilityDensityFunction(double w)
        {
            // For all possible values for W, find how many
            // of them are equal to the requested value.

            int count = 0;
            for (int i = 0; i < table.Length; i++)
                if (table[i] == w) count++;
            return count / (double)table.Length;
        }

        /// <summary>
        ///   Gets the log-probability density function (pdf) for
        ///   this distribution evaluated at point <c>w</c>.
        /// </summary>
        /// 
        /// <param name="w">A single point in the distribution range.</param>
        /// 
        /// <returns>
        ///   The logarithm of the probability of <c>x</c>
        ///   occurring in the current distribution.
        /// </returns>
        /// 
        /// <remarks>
        ///   The Probability Density Function (PDF) describes the
        ///   probability that a given value <c>x</c> will occur.
        /// </remarks>
        /// 
        public override double LogProbabilityDensityFunction(double w)
        {
            // For all possible values for W, find how many
            // of them are equal to the requested value.

            int count = 0;
            for (int i = 0; i < table.Length; i++)
                if (table[i] == w) count++;
            return Math.Log(count) - Math.Log(table.Length);
        }
    }
}
