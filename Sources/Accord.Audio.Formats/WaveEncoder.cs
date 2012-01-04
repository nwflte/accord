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
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    ///  Wave audio file encoder
    /// </summary>
    /// 
    public class WaveEncoder : IAudioEncoder
    {
        private Stream waveStream;

        /// <summary>
        ///   Gets the underlying Wave stream.
        /// </summary>
        /// 
        public Stream Stream { get { return waveStream; } }


        #region Constructors

        /// <summary>
        ///   Constructs a new Wave encoder.
        /// </summary>
        /// 
        public WaveEncoder()
        {
        }

        /// <summary>
        ///   Constructs a new Wave encoder.
        /// </summary>
        /// 
        public WaveEncoder(FileStream stream)
        {
            Open(stream);
        }

        /// <summary>
        ///   Constructs a new Wave encoder.
        /// </summary>
        /// 
        public WaveEncoder(Stream stream)
        {
            Open(stream);
        }

        /// <summary>
        ///   Constructs a new Wave encoder.
        /// </summary>
        /// 
        public WaveEncoder(string path)
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
        public void Open(FileStream stream)
        {
            this.waveStream = stream;
        }

        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="stream">Stream to open.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public void Open(Stream stream)
        {
            this.waveStream = stream;
        }

        /// <summary>
        ///   Open specified stream.
        /// </summary>
        /// 
        /// <param name="path">Path of file to open as stream.</param>
        /// 
        /// <returns>Returns number of frames found in the specified stream.</returns>
        /// 
        public void Open(string path)
        {
            Open(new FileStream(path, FileMode.OpenOrCreate));
        }


        /// <summary>
        ///   Encodes the Wave stream into a Signal object.
        /// </summary>
        /// 
        public void Encode(Signal signal)
        {
            byte[] dataStream = signal.RawData;

            DataChunk header = new DataChunk();
            header.Header = new char[] { 'd', 'a', 't', 'a' };
            header.Length = dataStream.Length;
            byte[] dataHeader = header.GetBytes();

            FormatHeader format = new FormatHeader();
            format.FmtHeader = new char[] { 'f', 'm', 't', ' ' };
            format.Length = 16;
            format.Channels = (short)signal.Channels;
            format.FormatTag = (short)WaveFormatTag.IeeeFloat;
            format.SamplesPerSecond = signal.SampleRate;
            format.BitsPerSample = 16;
            format.BlockAlignment = (short)(format.BitsPerSample / 8 * format.Channels);
            format.AverageBytesPerSecond = format.BitsPerSample * format.BlockAlignment;
            byte[] waveFormat = format.GetBytes();

            RIFFChunk riff = new RIFFChunk();
            riff.RiffHeader = new char[] { 'R', 'I', 'F', 'F' };
            riff.Length = dataStream.Length + dataHeader.Length + waveFormat.Length;
            riff.WaveHeader = new char[] { 'W', 'A', 'V', 'E' };
            byte[] riffHeader = riff.GetBytes();

            waveStream.Write(riffHeader, 0, riffHeader.Length);
            waveStream.Write(waveFormat, 0, waveFormat.Length);
            waveStream.Write(dataHeader, 0, dataHeader.Length);
            waveStream.Write(dataStream, 0, dataStream.Length);
        }


        /// <summary>
        ///   Closes the underlying stream.
        /// </summary>
        /// 
        public void Close()
        {
            waveStream.Close();
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct FormatHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] FmtHeader;

        public int Length;

        public short FormatTag;

        public short Channels;

        public int SamplesPerSecond;

        public int AverageBytesPerSecond;

        public short BlockAlignment;

        public short BitsPerSample;

        public byte[] GetBytes()
        {
            int rawsize = Marshal.SizeOf(this);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(this, buffer, false);
            handle.Free();
            return rawdata;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct RIFFChunk
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] RiffHeader;

        public int Length;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] WaveHeader;

        public byte[] GetBytes()
        {
            int rawsize = Marshal.SizeOf(this);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(this, buffer, false);
            handle.Free();
            return rawdata;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    internal struct DataChunk
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]

        public char[] Header;

        public int Length;

        public byte[] GetBytes()
        {
            int rawsize = Marshal.SizeOf(this);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(this, buffer, false);
            handle.Free();
            return rawdata;
        }
    }
}
