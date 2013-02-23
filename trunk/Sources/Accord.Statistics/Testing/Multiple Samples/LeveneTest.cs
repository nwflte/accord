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

    /// <summary>
    ///   Levene's test for equality of variances.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. Levene's test. Available on:
    ///       http://en.wikipedia.org/wiki/Levene's_test </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="BartlettTest"/>
    /// 
    [Serializable]
    public class LeveneTest : FTest
    {

        /// <summary>
        ///   Tests the null hypothesis that all group variances are equal.
        /// </summary>
        /// 
        /// <param name="samples">The grouped samples.</param>
        /// <param name="median"><c>True</c> to use the median in the Levene calculation.
        /// <c>False</c> to use the mean. Default is false (use the mean). </param>
        /// 
        public LeveneTest(double[][] samples, bool median = false)
        {
            int N = 0, k = samples.Length;

            // Compute group means
            var means = new double[samples.Length];
            if (median)
                for (int i = 0; i < means.Length; i++)
                    means[i] = Accord.Statistics.Tools.Median(samples[i]);
            else
                for (int i = 0; i < means.Length; i++)
                    means[i] = Accord.Statistics.Tools.Mean(samples[i]);

            // Compute absolute centred samples
            var z = new double[samples.Length][];
            for (int i = 0; i < z.Length; i++)
            {
                z[i] = new double[samples[i].Length];
                for (int j = 0; j < z[i].Length; j++)
                    z[i][j] = Math.Abs(samples[i][j] - means[i]);
            }

            // Compute means for the centred samples
            var newMeans = new double[samples.Length];
            for (int i = 0; i < newMeans.Length; i++)
                newMeans[i] = Accord.Statistics.Tools.Mean(z[i]);

            // Compute total mean
            double totalMean = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                for (int j = 0; j < samples[i].Length; j++)
                    totalMean += z[i][j];
                N += samples[i].Length;
            }
            totalMean /= N;

            double sum1 = 0; // Numerator sum
            for (int i = 0; i < samples.Length; i++)
            {
                int n = samples[i].Length;
                double u = (newMeans[i] - totalMean);
                sum1 += n * u * u;
            }

            double sum2 = 0; // Denominator sum
            for (int i = 0; i < samples.Length; i++)
            {
                for (int j = 0; j < samples[i].Length; j++)
                {
                    double u = z[i][j] - newMeans[i];
                    sum2 += u * u;
                }
            }

            double num = (N - k) * sum1;
            double den = (k - 1) * sum2;

            double W = num / den;
            int degree1 = k - 1;
            int degree2 = N - k;

            Compute(W, degree1, degree2, TwoSampleHypothesis.FirstValueIsGreaterThanSecond);
        }
    }
}
