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

namespace Accord.Audio.Formats
{
    using System;
    using System.IO;
    using Accord.Audio;
    using SlimDX.Multimedia;

    /// <summary>
    ///  Wave audio file decoder
    /// </summary>
    /// 
    public class WaveDecoder : IAudioDecoder
    {
        private WaveStream waveStream;
        private int blockAlign;
        private int channels;
        private int numberOfFrames;
        private int sampleRate;
        private int bytes;

        /// <summary>
        ///   Gets the number of channels in this Wave stream.
        /// </summary>
        /// 
        public int Channels
        {
            get { return channels; }
        }

        /// <summary>
        ///   Gets the number of frames in this Wave stream.
        /// </summary>
        /// 
        public int Frames
        {
            get { return numberOfFrames; }
        }

        /// <summary>
        ///   Gets the sample rate for this Wave stream.
        /// </summary>
        /// 
        public int SampleRate
        {
            get { return sampleRate; }
        }

        /// <summary>
        ///   Gets the underlying Wave stream.
        /// </summary>
        /// 
        public WaveStream Stream
        {
            get { return waveStream; }
        }

        /// <summary>
        ///   Gets the number of bytes read from the stream in the
        ///   the last call of any of the <seealso cref="Decode(int)"/>
        ///   overloads.
        /// </summary>
        /// 
        public int Bytes
        {
            get { return bytes; }
        }


        #region Constructors

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
        public WaveDecoder()
        {
        }

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
        public WaveDecoder(WaveStream stream)
        {
            Open(stream);
        }

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
        public WaveDecoder(Stream stream)
        {
            Open(stream);
        }

        /// <summary>
        ///   Constructs a new Wave decoder.
        /// </summary>
        public WaveDecoder(string path)
        {
            Open(path);
        }

        #endregion


        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public int Open(WaveStream stream)
        {
            this.waveStream = stream;
            this.channels = stream.Format.Channels;
            this.numberOfFrames = (int)stream.Length / (stream.Format.BlockAlignment * Channels);
            this.blockAlign = stream.Format.BlockAlignment;
            this.sampleRate = stream.Format.SamplesPerSecond;
            return Frames;
        }

        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public int Open(Stream stream)
        {
            return Open(new WaveStream(stream));
        }

        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="path">Path of file to open as stream.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public int Open(string path)
        {
            return Open(new WaveStream(path));
        }

        /// <summary>
        ///   Navigates to a position in this Wave stream.
        /// </summary>
        /// 
        /// <param name="frameIndex">The index of the sample to navigate to.</param>
        /// 
        public void Seek(int frameIndex)
        {
            waveStream.Position = frameIndex * blockAlign;
        }

        /// <summary>
        ///   Decodes the Wave stream into a Signal object.
        /// </summary>
        /// 
        public Signal Decode()
        {
            // Reads the entire stream into a signal
            return Decode(0, Frames);
        }

        /// <summary>
        ///   Decodes the Wave stream into a Signal object.
        /// </summary>
        /// 
        /// <param name="frames">The number of frames to decode.</param>
        /// 
        public Signal Decode(int frames)
        {
            // Create room to store the samples.
            float[] data = new float[Channels * frames];

            bytes = readAs(data, frames);

            return Signal.FromArray(data, channels, sampleRate);
        }

        /// <summary>
        /// Decodes the Wave stream into a Signal object.
        /// </summary>
        /// 
        /// <param name="index">Audio frame index to start decoding.</param>
        /// <param name="frames">The number of frames to decode.</param>
        /// 
        /// <returns>Returns the decoded signal.</returns>
        /// 
        public Signal Decode(int index, int frames)
        {
            waveStream.Seek(index * channels, SeekOrigin.Begin);
            return Decode(frames);
        }

        /// <summary>
        ///   Closes the underlying stream.
        /// </summary>
        /// 
        public void Close()
        {
            waveStream.Close();
        }




        private int readAs(float[] buffer, int count)
        {
            int reads = 0;

            // Detect the underlying stream format.
            if (waveStream.Format.FormatTag == WaveFormatTag.Pcm)
            {
                // The wave is in standard PCM format. We'll need
                //  to convert it to IeeeFloat.
                switch (waveStream.Format.BitsPerSample)
                {
                    case 8: // Stream is 8 bits
                        {
                            byte[] block = new byte[buffer.Length];
                            reads = read(block, count);
                            SampleConverter.Convert(block, buffer);
                        }
                        break;

                    case 16: // Stream is 16 bits
                        {
                            short[] block = new short[buffer.Length];
                            reads = read(block, count);
                            SampleConverter.Convert(block, buffer);
                        }
                        break;

                    case 32: // Stream is 32 bits
                        {
                            int[] block = new int[buffer.Length];
                            reads = read(block, count);
                            SampleConverter.Convert(block, buffer);
                        }
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
            else if (waveStream.Format.FormatTag == WaveFormatTag.IeeeFloat)
            {
                // Format is Ieee float, just copy to the buffer.
                reads = read(buffer, count);
            }
            else
            {
                throw new NotSupportedException("The wave format isn't supported");
            }

            return reads; // return the number of bytes read
        }


        /// <summary>
        ///   Reads a maximum of count samples from the current stream, and writes the data to buffer, beginning at index.
        /// </summary>
        /// <param name="buffer">
        ///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
        /// </param>
        /// <param name="count">The ammount of frames to read.</param>
        /// <returns>The number of reads performed on the stream.</returns>
        private int read(float[] buffer, int count)
        {
            int reads;

            int blockSize = blockAlign * count * channels;
            byte[] block = new byte[blockSize];
            reads = waveStream.Read(block, 0, blockSize);

            // Convert from byte to float
            for (int j = 0; j < buffer.Length; j++)
                buffer[j] = BitConverter.ToSingle(block, j * blockAlign);

            return reads;
        }

        /// <summary>
        ///   Reads a maximum of count frames from the current stream, and writes the data to buffer, beginning at index.
        /// </summary>
        /// <param name="buffer">
        ///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
        /// </param>
        /// <param name="count">The ammount of frames to read.</param>
        /// <returns>The number of reads performed on the stream.</returns>
        private int read(short[] buffer, int count)
        {
            int reads;

            int blockSize = blockAlign * count * channels;
            byte[] block = new byte[blockSize];
            reads = waveStream.Read(block, 0, blockSize);

            // Convert from byte to short
            for (int j = 0; j < buffer.Length; j++)
                buffer[j] = BitConverter.ToInt16(block, j * blockAlign);

            return reads;
        }

        /// <summary>
        ///   Reads a maximum of count frames from the current stream, and writes the data to buffer, beginning at index.
        /// </summary>
        /// <param name="buffer">
        ///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
        /// </param>
        /// <param name="count">The ammount of frames to read.</param>
        /// <returns>The number of reads performed on the stream.</returns>
        private int read(int[] buffer, int count)
        {
            int reads;

            int blockSize = blockAlign * count * channels;
            byte[] block = new byte[blockSize];
            reads = waveStream.Read(block, 0, blockSize);

            // Convert from byte to int
            for (int j = 0; j < buffer.Length; j++)
                buffer[j] = BitConverter.ToInt32(block, j * blockAlign);

            return reads;
        }

        /// <summary>
        ///   Reads a maximum of count frames from the current stream, and writes the data to buffer, beginning at index.
        /// </summary>
        /// <param name="buffer">
        ///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
        /// </param>
        /// <param name="count">The ammount of frames to read.</param>
        /// <returns>The number of reads performed on the stream.</returns>
        private int read(byte[] buffer, int count)
        {
            return waveStream.Read(buffer, 0, count);
        }

    }
}
