// Accord (Experimental) Audio Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
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

namespace Accord.Audio
{
    using System;
    using Accord.Math;
    using AForge.Math;

    /// <summary>
    ///   Tool functions for audio processing.
    /// </summary>
    /// 
    public static class Tools
    {
        /// <summary>
        ///   Interleaves the channels into a single array.
        /// </summary>
        /// 
        public static float[] Interleave(this float[][] channels)
        {
            if (channels == null) throw new ArgumentNullException("channels");
            if (channels.Length == 0) return new float[0];

            int c = channels.Length;
            int n = channels[0].Length;

            float[] data = new float[c * n];

            for (int i = 0, k = 0; i < n; i++)
                for (int j = 0; j < c; j++)
                    data[k++] = channels[j][i];

            return data;
        }

        /// <summary>
        ///   Interleaves the channels into a single array.
        /// </summary>
        /// 
        public static float[] Interleave(this float[,] channels)
        {
            float[] result = new float[channels.Length];
            Buffer.BlockCopy(channels, 0, result, 0, result.Length * sizeof(float));
            return result;
        }

        /// <summary>
        ///   Computes the Magnitude spectrum of a complex signal.
        /// </summary>
        /// 
        public static double[] GetMagnitudeSpectrum(Complex[] fft)
        {
            if (fft == null) throw new ArgumentNullException("fft");

            // assumes fft is symmetric

            // In a two-sided spectrum, half the energy is displayed at the positive frequency,
            // and half the energy is displayed at the negative frequency. Therefore, to convert
            // from a two-sided spectrum to a single-sided spectrum, discard the second half of
            // the array and multiply every point except for DC by two.

            int numUniquePts = (int)System.Math.Ceiling((fft.Length + 1) / 2.0);
            double[] mx = new double[numUniquePts];

            mx[0] = fft[0].Magnitude / fft.Length;
            for (int i = 0; i < numUniquePts; i++)
            {
                mx[i] = fft[i].Magnitude * 2 / fft.Length;
            }

            return mx;
        }

        /// <summary>
        ///   Computes the Power spectrum of a complex signal.
        /// </summary>
        /// 
        public static double[] GetPowerSpectrum(Complex[] fft)
        {
            if (fft == null) throw new ArgumentNullException("fft");

            int n = (int)System.Math.Ceiling((fft.Length + 1) / 2.0);

            double[] mx = new double[n];

            mx[0] = fft[0].SquaredMagnitude / fft.Length;

            for (int i = 1; i < n; i++)
                mx[i] = fft[i].SquaredMagnitude * 2.0 / fft.Length;

            return mx;
        }

        /// <summary>
        ///   Computes the Phase spectrum of a complex signal.
        /// </summary>
        /// 
        public static double[] GetPhaseSpectrum(Complex[] fft)
        {
            if (fft == null) throw new ArgumentNullException("fft");

            int n = (int)System.Math.Ceiling((fft.Length + 1) / 2.0);

            double[] mx = new double[n];

            for (int i = 0; i < n; i++)
                mx[i] = fft[i].Phase;

            return mx;
        }

        /// <summary>
        ///   Creates an evenly spaced frequency vector (assuming a symmetric FFT)
        /// </summary>
        /// 
        public static double[] GetFrequencyVector(int length, int sampleRate)
        {
            int numUniquePts = (int)System.Math.Ceiling((length + 1) / 2.0);
            double[] freq = new double[numUniquePts];
            for (int i = 0; i < numUniquePts; i++)
            {
                freq[i] = (double)i * sampleRate / length;
            }
            return freq;
        }

        /// <summary>
        ///   Gets the spectral resolution for a signal of given sampling rate and number of samples.
        /// </summary>
        /// 
        public static double GetSpectralResolution(int samplingRate, int samples)
        {
            return samplingRate / samples;
        }

        /// <summary>
        ///   Gets the power Cepstrum for a complex signal.
        /// </summary>
        /// 
        public static double[] GetPowerCepstrum(Complex[] signal)
        {
            if (signal == null) throw new ArgumentNullException("signal");

            FourierTransform.FFT(signal, FourierTransform.Direction.Backward);

            Complex[] logabs = new Complex[signal.Length];
            for (int i = 0; i < logabs.Length; i++)
                logabs[i].Re = System.Math.Log(signal[i].Magnitude);

            FourierTransform.FFT(logabs, FourierTransform.Direction.Forward);

            return logabs.Re();
        }

    }
}
