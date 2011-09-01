using System.Runtime.InteropServices;
namespace Accord.Audio.IO
{
	using System;
	using System.IO;
	
	
	/// <summary>
	///   Represents a reader that can read a Wave (.wav) file.
	/// </summary>
	/// <remarks>
	///   <para>
	///      WAV (or WAVE), short for Waveform audio format, also known as Audio
	///      for Windows, is a Microsoft and IBM audio file format standard for storing an
	///      audio bitstream on PCs. It is the main format used on Windows systems for raw
	///      and typically uncompressed audio. The usual bitstream encoding is the Pulse
	///      Code Modulation (PCM) format.
	///   </para>
	///   <para>
	///      In the digital domain, PCM is a very straightforward mechanism to store audio.
	///      The analog audio is sampled in accordance with the Nyquest theorem and the
	///      individual samples are stored sequentially in binary format.
	///   </para>
	///   <para>
	///      The WAVE File Format supports a variety of bit resolutions, sample rates, and
	///      channels of audio. It contains a header which contains most of this information.
	///   </para>
	///   <para>
	///      WAVE files are divided into equal units of measure called "samples." A sample
	///      represents data captured during a single sampling cycle. So, sampling is done at
	///      44 KHz, there will be 44 K samples. Each sample can be represented as 8 bits, 16
	///      bits, 24 bits, or 32 bits. (There is no restriction on how many bits you use for
	///      a sample except that it has to be a multiple of 8.) To some extent, the more the
	///      number of bits in a sample, the better the quality of the audio.
	///      One very important detail to note is that 8-bit samples are represented differently
	///      (as signed values) than from 16-bit and higher samples (as unsigned values).
	/// 
	///      It is also possible to store samples in 32-bit float format, as normalized standard
	///      float format: Windows PCM IEEE Float (0.24 float type 3)
	/// </para>
	/// <para>
	///      Audio channels are interleaved in the WAVE format. When dealing with multi-channel
	///      sounds, the unit of measure containing the sample for each channel is called a frame.
	///      A frame is simply a block containing a sample for each channel, starting with the
	///      left channel.
	/// </para>
	/// <para>
	///      By default, a WaveReader is not thread safe.
	/// </para>
	/// <para>
	///      For more information, see also:
	///      - http://www.microsoft.com/whdc/device/audio/multichaud.mspx
	///      - http://ccrma.stanford.edu/courses/422/projects/WaveFormat/
	///      - http://netghost.narod.ru/gff/graphics/summary/micriff.htm
	///      - http://www.codeguru.com/cpp/g-m/multimedia/audio/article.php/c8935__1/
	///      - http://en.wikipedia.org/wiki/WAV
	/// </para>
	/// </remarks>
	public class WaveFileReader : IDisposable
	{
		private BinaryReader streamReader;
		private bool ownsReading;
		
		private WaveFileFormat.RIFFChunk riff;
		private WaveFileFormat.FormatHeader header;
		private WaveFileFormat.DataHeader data;
		
		private long dataStart;
		private SampleType sampleType;
		private const int bitsPerByte = 8;
		
		
		#region Properties
		/// <summary>
		///   Gets the length of this stream.
		/// </summary>
		public int Length
		{
			get { return riff.Length; }
		}
		
		/// <summary>
		///   Gets the number of channels for this Wave file.
		/// </summary>
		public int Channels
		{
			get { return header.NumOfChannels; }
		}
		
		/// <summary>
		///   Gets the audio format for this Wave file.
		/// </summary>
		/// <remarks>
		///   See <see cref="WaveFileHeader">WaveAudioFormat</see>
		///   for more details about wave file formats.
		/// </remarks>
		public WaveAudioFormat Format
		{
			get {
				if (Enum.IsDefined(typeof(WaveAudioFormat),header.AudioFormat))
					return (WaveAudioFormat)header.AudioFormat;
				else return WaveAudioFormat.Unknown;
			}
		}
		
		/// <summary>
		///   Gets the size of the data block.
		/// </summary>
		public int DataLength
		{
			get { return data.Length; }
		}
		
		/// <summary>
		///   Gets the type of the sample.
		/// </summary>
		public SampleType SampleType
		{
			get { return this.sampleType; }
		}

		/// <summary>
		///   Gets the total number of frames contained in this file.
		/// </summary>
		public int Frames {
			get { return DataLength/FrameSize;}
		}

		/// <summary>
		///   Gets the sampling rate for this Wave file.
		/// </summary>
		public int SampleRate
		{
			get { return header.SampleRate; }
		}
		
		/// <summary>
		///   Gets the number of frames contained in each second of audio.
		/// </summary>
		public int FrameRate
		{
			get { return SampleRate / Channels;}
		}
		
		/// <summary>
		///   Gets the number of samples contained in each second of audio.
		/// </summary>
		/// <remarks>
		///   The average bytes per second can also be calculated by the formula
		///   AvgBytesPerSec = SampleRate * BlockAlign
		/// </remarks>
		public int AverageBytesPerSecond
		{
			// AvgBytesPerSec = SampleRate * BlockAlign
			get {  return header.AvgBytesPerSecond; }
		}
		
		/// <summary>
		///   Gets a value indicating if the Wave file has two channels.
		/// </summary>
		public bool IsStereo {
			get { return Channels == 2; }
		}
		
		/// <summary>
		///   Gets the underlying stream structure for this WaveFileReader
		/// </summary>
		public Stream BaseStream
		{
			get { return streamReader.BaseStream;}
		}
		
		/// <summary>
		///   Gets the size of a frame, in bytes.
		/// </summary>
		/// <remarks>
		///   The size of a frame can also be known as the
		///   block alignment for the WAVE file.
		/// </remarks>
		public int FrameSize
		{
			// SignificantBitsPerSample / 8 * NumChannels
			get { return Channels*SampleSize; }
		}
		
		/// <summary>
		///   Gets the size of a sample, in bytes.
		/// </summary>
		public int SampleSize
		{
			get { return BitsPerSample/bitsPerByte;}
		}
		
		/// <summary>
		///   Gets the number of bits contained in each sample.
		/// </summary>
		/// <remarks>
		///   The ammount of bits per sample must be a power of two.
		/// </remarks>
		public int BitsPerSample
		{
			get { return header.BitsPerSample; }
		}
		
		/// <summary>
		///   Gets the current frame's index.
		/// </summary>
		public int CurrentFrameIndex
		{
			get	{ return (int)(streamReader.BaseStream.Position - dataStart)/FrameSize;	}
			set { streamReader.BaseStream.Position = dataStart + FrameSize*value; }
		}
		
		/// <summary>
		///   Gets the duration of the audio track, in milliseconds.
		/// </summary>
		public int Duration
		{
			get { return (int)(Frames*1000.0/FrameRate); }
		}
		#endregion

		
		
		#region Constructor
		/// <summary>
		///   Initializes a new instance of the WaveReader class.
		/// </summary>
		/// <param name="stream">A stream.</param>
		/// <returns></returns>
		public WaveFileReader(Stream stream)
		{
			// Create a new binary stream reader to make things easier
			streamReader = new BinaryReader(stream);
			int r = 0;
			
			// Start by loading the RIFF chunk of the wav file
			byte [] b = new byte [Marshal.SizeOf( typeof (WaveFileFormat.RIFFChunk))];
			r = stream.Read(b, 0, b.Length);
			this.riff = (WaveFileFormat.RIFFChunk)
				b.RawDeserialize(typeof (WaveFileFormat.RIFFChunk));

			if (r == 0) throw new EndOfStreamException("Unexpected end of file.");
			
			// Now we check if this is a valid file
			if ( new string (this.riff.RiffHeader) != "RIFF"
			    || new string (this.riff.WaveHeader) != "WAVE")
			{
				// Valid Wave files have both "RIFF" and "WAVE" headers.
				throw new FormatException("The RIFF chunk is missing.");
			}
			
			// This is a valid file, proceed reading the format header
			b = new byte[Marshal.SizeOf( typeof (WaveFileFormat.FormatHeader))];
			r = stream.Read(b, 0, b.Length);
			header = (WaveFileFormat.FormatHeader)
				b.RawDeserialize(typeof (WaveFileFormat.FormatHeader));
			
			if (r == 0)	throw new EndOfStreamException("Unexpected end of file.");
			
			
			// Set-up common format file types
			// TODO: move this handling to separate classes
			//       for each format specification in the future
			
			if (Format == WaveAudioFormat.PCM)
			{
				if (SampleSize == sizeof(byte))
					sampleType = SampleType.UInt8;
				else if (SampleSize == sizeof(short))
					sampleType = SampleType.Int16;
				else if (SampleSize == sizeof(int))
					sampleType = SampleType.Int32;
			}
			else if (Format == WaveAudioFormat.Float)
				sampleType = SampleType.Single;
			else 	throw new FormatException("Unsuported file format");
			
			
			
			// Now we are going to search the first data chunk.
			b = new byte [Marshal.SizeOf( typeof (WaveFileFormat.DataHeader))];
			
			do
			{
				r = stream.Read(b, 0, b.Length);
				this.data = (WaveFileFormat.DataHeader)
					b.RawDeserialize(typeof (WaveFileFormat.DataHeader));
				
				if (r == 0)	throw new EndOfStreamException("Data chunk not found.");
			}
			while (new string (data.Header).ToUpper() != "DATA");
			
			dataStart = stream.Position;
			
			// Done.
		}

		/// <summary>
		///   Initializes a new instance of the WaveReader class.
		/// </summary>
		/// <param name="stream">A path to a Wave (.wav) file.</param>
		/// <returns></returns>
		public WaveFileReader(string path)
			: this(new FileStream(path, FileMode.Open,FileAccess.Read))
		{
			this.ownsReading = true;
		}
		#endregion

		
		
		#region Members
		
		#region Read Sample
		/// <summary>
		///   Reads a raw sample from the stream.
		/// </summary>
		/// <returns>The sample as a byte array. See <see cref="BitConverter">
		/// for converting to its suitable format.</see></returns>
		public byte[] ReadSample()
		{
			checkMultichannel();
			
			byte[] buff = new byte[SampleSize];
			streamReader.BaseStream.Read(buff,0,SampleSize);
			return buff;
		}
		
		/// <summary>
		///   Reads a 8-bit sample from the stream.
		/// </summary>
		/// <remarks>
		/// <para>
		///   In Wave files, 8-bit samples are represented as byte values
		///   whereas 16-bit and higher are represented by signed values.
		///   See <see cref="WaveFileReader">WaveFileReader description</see>
		///   for more details.
		/// </para>
		/// <para>
		///   If the underlying stream sample size does not match 8-bit,
		///   if will be automatically converted. It is generally faster
		///   to read a block of samples than one at a time. See
		///   <see cref="ReadBlock">ReadBlock</see> for more details.
		/// </para>
		/// </remarks>
		/// <param name="b">
		///   A unsigned 8-bit unsigned byte containing the sample.
		/// </param>
		public void ReadSample(out byte b)
		{
			checkMultichannel();
			
			switch(sampleType)
			{
				case SampleType.UInt8:
					// Sample size is byte, we want byte - match
					b = streamReader.ReadByte();
					break;
					
				case SampleType.Int16:
					// Sample size is Int16, we want byte
					Int16 s = streamReader.ReadInt16();
					SampleConverter.Convert(s, out b);
					break;
					
				case SampleType.Int32:
					// Sample size is Int32, we want byte
					Int32 i = streamReader.ReadInt32();
					SampleConverter.Convert(i, out b);
					break;
					
				case SampleType.Single:
					// Sample size is Float, we want byte
					float f = streamReader.ReadSingle();
					SampleConverter.Convert(f, out b);
					break;
					
				default:
					throw new NotSupportedException();
			}
		}
		
		/// <summary>
		///   Reads a 32-bit sample from the stream.
		/// </summary>
		/// <remarks>
		///   In Wave files, 8-bit samples are represented as byte values
		///   whereas 16-bit and higher are represented by signed values.
		///   See <see cref="WaveFileReader">WaveFileReader description</see>
		///   for more details.
		/// </remarks>
		/// <param name="i">
		///   A signed 32-bit byte containing the sample.
		/// </param>
		public void ReadSample(out Int32 i)
		{
			// Sample size is Int32, we want Int32 - match
			if (sampleType == SampleType.Int32)
			{
				i = streamReader.ReadInt32();
			}
			else
			{
				i = 0;
			}
		}
		
		/// <summary>
		///   Reads a 16-bit sample from the stream.
		/// </summary>
		/// <remarks>
		///   In Wave files, 8-bit samples are represented as byte values
		///   whereas 16-bit and higher are represented by signed values.
		///   See <see cref="WaveFileReader">WaveFileReader description</see>
		///   for more details.
		/// </remarks>
		/// <param name="i">
		///   A signed 16-bit byte containing the sample.
		/// </param>
		public void ReadSample(out Int16 i)
		{
			// Sample size is Int16, we want Int16 - match
			if (sampleType == SampleType.Int16)
			{
				i = streamReader.ReadInt16();
			}
			else
			{
				i = 0;
			}
		}
		
		/// <summary>
		///   Reads a 32-bit float sample from the stream.
		/// </summary>
		/// <remarks>
		///   In Wave files, 8-bit samples are represented as byte values
		///   whereas 16-bit and higher are represented by signed values.
		///   See <see cref="WaveFileReader">WaveFileReader description</see>
		///   for more details.
		/// </remarks>
		/// <param name="i">
		///   A signed 32-bit float containing the sample.
		/// </param>
		public void ReadSample(out float f)
		{
			// Sample size is float, we want float - match
			if (sampleType == SampleType.Single)
			{
				f = streamReader.ReadSingle();
			}
			else
			{
				f = 0;
			}
		}
		#endregion
		
		
		#region Read Block
		/// <summary>
		///   Reads a raw frame from the stream.
		/// </summary>
		/// <returns>The frame as a byte array. See <see cref="BitConverter">
		/// for converting to its suitable format.</see></returns>
		public byte[][] ReadBlock()
		{
			byte[][] buff = new byte[Channels][];
			for (int i = 0; i < Channels; i++)
			{
				buff[i] = new byte[SampleSize];
				streamReader.BaseStream.Read(buff[i],0,SampleSize);
			}
			return buff;
		}
		
		/// <summary>
		///   Reads a 8-bit frame from the stream.
		/// </summary>
		/// <remarks>
		///   The stream position must be aligned for reading the
		///   n-channel frame correctly.
		/// </remarks>
		/// <param name="frame">The frame containing the samples for each channel of the stream.</param>
		/// <returns></returns>
		public void ReadBlock(byte[] frame)
		{
			switch(sampleType)
			{
					// Sample size is byte, we want byte - match
				case SampleType.UInt8:
					streamReader.Read(frame,0,Channels);
					break;
					
				default:
					throw new NotImplementedException();
			}
		}
		
		/// <summary>
		///   Reads a 16-bit frame from the stream.
		/// </summary>
		/// <remarks>
		///   The stream position must be aligned for reading the
		///   n-channel frame correctly.
		/// </remarks>
		/// <param name="frame">
		///   The frame containing the samples for each channel of the stream.
		/// </param>
		public void ReadBlock(Int16[] frame)
		{
			switch (SampleType)
			{
					// Sample size is Int16, we want Int16 - match
				case SampleType.Int16:
					for (int i = 0; i < Channels; i++)
						frame[i] = streamReader.ReadInt16();
					break;
					
				default:
					throw new NotImplementedException();
			}
		}
		
		/// <summary>
		///   Reads a 32-bit frame from the stream.
		/// </summary>
		/// <remarks>
		///   The stream position must be aligned for reading the n-channel frame correctly.
		///   If the sample size is different from the expected 32-bit format, it will be
		///   automatically converted.
		/// </remarks>
		/// <param name="frame">The frame containing the samples for each channel of the stream.</param>
		public void ReadBlock(Int32[] frame)
		{
			switch(sampleType)
			{
					// Sample size is Int32, we want Int32 - match
				case SampleType.Int32:
					for (int i = 0; i < Channels; i++)
						frame[i] = streamReader.ReadInt32();
					break;
					
				default:
					throw new NotImplementedException();
			}
		}
		
		/// <summary>
		///   Reads a 32-bit float frame from the stream.
		/// </summary>
		/// <remarks>
		///   The stream position must be aligned for reading the n-channel frame correctly
		/// </remarks>
		/// <param name="frame">
		///   The frame containing the samples for each channel of the stream.
		/// </param>
		public void ReadBlock(float[] frame)
		{
			// Sample size is float, we want float - match
			if (SampleSize == sizeof(float))
			{
				for (int i = 0; i < Channels; i++)
					frame[i] = streamReader.ReadSingle();
			}
		}
		#endregion
		
		
		#region Read Audio Frame (blocks)
		/// <summary>
		///   Reads a maximum of count samples from the current stream,
		///   and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the
		///    specified byte array with the values between index and
		///    (index + count -1) replaced by the 8 bit frames read
		///    from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadBlocks(byte[] buffer, int index, int count)
		{
			return streamReader.Read(buffer, index, count);
		}
		
		/// <summary>
		///   Reads a maximum of count samples from the current stream, and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadBlocks(short[] buffer, int index, int count)
		{
			int blockSize = sizeof(short)*count;
			byte[] b = new byte[blockSize];
			int reads = streamReader.Read(b, 0, blockSize-1);
			
			for (int i = index; i < count; i++)
				buffer[i] = BitConverter.ToInt16(b,i*sizeof(short));
			
			return reads;
		}
		
		/// <summary>
		///   Reads a maximum of count samples from the current stream, and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadBlocks(int[] buffer, int index, int count)
		{
			int blockSize = sizeof(int)*count;
			byte[] b = new byte[blockSize];
			int reads = streamReader.Read(b, 0, blockSize-1);
			
			for (int i = index; i < count; i++)
				buffer[i] = BitConverter.ToInt32(b,i*sizeof(int));
			
			return reads;
		}
		
		/// <summary>
		///   Reads a maximum of count samples from the current stream, and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadBlocks(float[] buffer, int index, int count, bool convert)
		{
			int blockSize = BitsPerSample*count*Channels;
			byte[] b = new byte[blockSize];
			int reads = streamReader.Read(b, 0, blockSize-1);
			
			for (int i = index; i < count; i++)
				buffer[i] = BitConverter.ToSingle(b,i*sizeof(float));
			
			return reads;
		}
		
		/// <summary>
		///   Reads a maximum of count samples from the current stream, and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadBlocks(float[][] buffer, int index, int count)
		{
			int reads = 0;
			
			switch(sampleType)
			{
				case SampleType.UInt8:
					{
						byte[][] block = new byte[buffer.Length][];
						reads = ReadBlocks(block, index, count);
						SampleConverter.Convert(block, buffer);
					}
					break;
					
				case SampleType.Int16:
					{
						short[][] block = new short[buffer.Length][];
						reads = ReadBlocks(block, index, count);
						SampleConverter.Convert(block, buffer);
					}
					break;
					
				case SampleType.Single:
					int blockSize = BitsPerSample*Channels*count;
					byte[] b = new byte[blockSize];
					reads = streamReader.Read(b, 0, blockSize-1);

					for (int j = 0; j < Channels; j++)
					{
						if (buffer[j] == null) buffer[j] = new float[count];
						for (int i = index; i < count; i++)
							buffer[j][i] = BitConverter.ToSingle(b,i*sizeof(float)+j);
					}
					break;
			}
			
			return reads;
		}
		
		/// <summary>
		///   Reads a maximum of count frames from the current stream, and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadBlocks(byte[][] buffer, int index, int count)
		{
			int reads = 0;
			
			switch(sampleType)
			{
				case SampleType.UInt8:
					int blockSize = count*Channels;
					byte[] b = new byte[blockSize];
					reads = streamReader.Read(b, 0, blockSize-1);
					
					for (int i = index; i < count; i++)
						for (int j = 0; j < Channels; j++)
						buffer[i][j] = b[i*Channels+j];
					break;
					
				default:
					throw new NotImplementedException();
			}
			
			return reads;
		}
		
		/// <summary>
		///   Reads a maximum of count frames from the current stream, and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadBlocks(short[][] buffer, int index, int count)
		{
			int reads = 0;
			switch(sampleType)
			{
				case SampleType.Int16:
					{
						int blockSize = sizeof(short)*count*Channels;
						byte[] b = new byte[blockSize];
						reads = streamReader.Read(b, 0, blockSize-1);
						
						for (int i = index; i < count; i++)
							for (int j = 0; j < Channels; j++)
							buffer[i][j] = b[i*sizeof(short)*Channels + j];
					}
					break;
					
				default:
					throw new NotImplementedException();
			}
			return reads;
		}
		#endregion
		
		
		#region Read Sample Block from a Single Channel
		/// <summary>
		///   Reads a maximum of count samples from a single channel from the
		///   current stream, and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="channel">The channel from where samples should be read.</param>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadSampleBlock(float[] buffer, int index, int count, int channel)
		{
			check(channel);
			int reads = 0;
				
			switch (sampleType)
			{
				case SampleType.Single:
					{
						// Sample size is float, we want float - match
						int blockSize = FrameSize*count;
						byte[] block = new byte[blockSize];
						reads = streamReader.Read(block, 0, blockSize-1);
						
						for (int i = 0; i < count; i++)
							buffer[i] = BitConverter.ToSingle(block,channel + i*Channels*FrameSize);
					}
					break;
					
				case SampleType.Int16:
					{
						// Sample size is Int16, we want Float32
						Int16[] block = new short[buffer.Length];
						reads = ReadSampleBlock(block, index, count, channel);
						SampleConverter.Convert(block, buffer);
					}
					break;
					
				case SampleType.Int32:
					{
						// Sample size is Int32, we want Float32
						int[] block = new int[buffer.Length];
						reads = ReadSampleBlock(block, index, count, channel);
						SampleConverter.Convert(block,buffer);
					}
					break;
					
				default:
					throw new NotImplementedException();
			}
			return reads;
		}
		
		/// <summary>
		///   Reads a maximum of count samples from a single channel from the
		///   current stream, and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="channel">The channel from where samples should be read.</param>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadSampleBlock(int[] buffer, int index, int count, int channel)
		{
			check(channel);
			int reads=  0;
			
			switch(sampleType)
			{
				case SampleType.Int32:
					{
						// Sample size is Int32, we want Int32 - match
						int blockSize = FrameSize*count;
						byte[] block = new byte[blockSize];
						reads = streamReader.Read(block, 0, blockSize-1);
						
						for (int i = 0; i < count; i++)
							buffer[i] = BitConverter.ToInt32(block,channel + i*Channels*FrameSize);
					}
					break;
					
				case SampleType.Int16:
					{
						// Sample size is Int16, we want Int32
						short[] block = new short[buffer.Length];
						reads = ReadSampleBlock(block, index, count, channel);
						SampleConverter.Convert(block,buffer);
					}
					break;
					
				default:
					throw new NotImplementedException();
			}
			
			return reads;
		}
		
		/// <summary>
		///   Reads a maximum of count samples from a single channel from the
		///   current stream, and writes the data to buffer, beginning at index.
		/// </summary>
		/// <param name="channel">The channel from where samples should be read.</param>
		/// <param name="buffer">
		///    When this method returns, this parameter contains the specified byte array with the values between index and (index + count -1) replaced by the 8 bit frames read from the current source.
		/// </param>
		/// <param name="index">The index where the copying starts.</param>
		/// <param name="count">The ammount of frames to read.</param>
		/// <returns>The number of reads performed on the stream.</returns>
		public int ReadSampleBlock(short[] buffer, int index, int count, int channel)
		{
			check(channel);
			int reads=  0;
			
			switch(sampleType)
			{
				case SampleType.Int16:
					{
						// Sample size is Int16, we want Int16 - match
						int blockSize = FrameSize*count;
						byte[] block = new byte[blockSize];
						reads = streamReader.Read(block, 0, blockSize-1);
						
						for (int i = 0; i < count; i++)
							buffer[i] = BitConverter.ToInt16(block,channel + i*Channels*FrameSize);
					}
					break;
					
				case SampleType.Int32:
					{
						// Sample size is Int32, we want Int16
						int[] block = new int[buffer.Length];
						reads = ReadSampleBlock(block, index, count, channel);
						SampleConverter.Convert(block,buffer);
					}
					break;
					
				default:
					throw new NotImplementedException();
			}
			return reads;
		}
		#endregion
		
		
		
		/// <summary>
		///   Reads all samples from the current position to the end of the WaveReader and returns them as one byte array.
		/// </summary>
		/// <returns>
		/// A byte array containing all samples from the current position to the end of the WaveReader.
		/// </returns>
		public byte[] ReadToEnd()
		{
			long size = BaseStream.Length - BaseStream.Position;
			byte[] buff = new byte[size];
			streamReader.Read(buff, (int)BaseStream.Position, (int)size);
			return buff;
		}
		
		/// <summary>
		///   Seeks a sample (or frame for multichannel files) in the file.
		/// </summary>
		/// <param name="sample">The sample index.</param>
		public void Seek(int sample)
		{
			streamReader.BaseStream.Seek(dataStart+sample*FrameSize, SeekOrigin.Begin);
		}
		
		/// <summary>
		///   Seeks a sample in the file.
		/// </summary>
		/// <param name="sample">The sample index.</param>
		/// <param name="sample">The sample channel index.</param>
		public void Seek(int sample, int channel)
		{
			streamReader.BaseStream.Seek(dataStart+sample*Channels+channel, SeekOrigin.Begin);
		}
		
		/// <summary>
		///   Extracts a single channel from a Wave file.
		/// </summary>
		/// <param name="buffer">The buffer containing the channel.</param>
		/// <param name="channel">The channel's index.</param>
		public void GetChannel(out byte[] buffer, int channel)
		{
			streamReader.BaseStream.Position = dataStart+FrameSize;
			buffer = new byte[Frames];
			for (int i = 0; i < Frames; i++)
			{
				buffer[i] = streamReader.ReadByte();
				streamReader.BaseStream.Seek(FrameSize, SeekOrigin.Current);
			}
		}
		
		/// <summary>
		///   Extracts a single channel from a Wave file.
		/// </summary>
		/// <param name="buffer">The buffer containing the channel.</param>
		/// <param name="channel">The channel's index.</param>
		public void GetChannel(out Int16[] buffer, int channel)
		{
			streamReader.BaseStream.Position = dataStart+FrameSize;
			buffer = new short[Frames];
			for (int i = 0; i < Frames; i++)
			{
				buffer[i] = streamReader.ReadInt16();
				streamReader.BaseStream.Seek(FrameSize, SeekOrigin.Current);
			}
		}
		
		/// <summary>
		///   Extracts a single channel from a Wave file.
		/// </summary>
		/// <param name="buffer">The buffer containing the channel.</param>
		/// <param name="channel">The channel's index.</param>
		public void GetChannel(out Int32[] buffer, int channel)
		{
			streamReader.BaseStream.Position = dataStart+FrameSize;
			buffer = new int[Frames];
			for (int i = 0; i < Frames; i++)
			{
				buffer[i] = streamReader.ReadInt32();
				streamReader.BaseStream.Seek(FrameSize, SeekOrigin.Current);
			}
		}
		
		/// <summary>
		///   Extracts a single channel from a Wave file.
		/// </summary>
		/// <param name="buffer">The buffer containing the channel.</param>
		/// <param name="channel">The channel's index.</param>
		public void GetChannel(out float[] buffer, int channel)
		{
			streamReader.BaseStream.Position = dataStart+FrameSize;
			buffer = new float[Frames];
			for (int i = 0; i < Frames; i++)
			{
				buffer[i] = streamReader.ReadSingle();
				streamReader.BaseStream.Seek(FrameSize, SeekOrigin.Current);
			}
		}
				
		/// <summary>
		///   Gets the number of frames contained in a specified time interval.
		/// </summary>
		/// <param name="ms">
		///   Number of milliseconds desired.
		/// </param>
		/// <returns>
		///   Number of frames contained in the specified interval.
		/// </returns>
		public int GetNumberOfFramesInTimeInterval(int ms)
		{
			// TODO: find a better name for this method.
			return (int)Math.Ceiling(FrameRate * 1000.0 * ms);
		}
		
		/// <summary>
		///   Closes the stream.
		/// </summary>
		public void Close()
		{
			streamReader.Close();
		}
		
		public void Dispose()
		{
			if (ownsReading)
				this.streamReader.Close();
		}
		
		#endregion
		
		

		private void check(int channel)
		{
			if (channel >= Channels || channel < 0)
				throw new IndexOutOfRangeException("Invalid channel.");
		}
		
		private void checkMultichannel()
		{
			if (Channels > 1)
				throw new InvalidOperationException("Access of single samples in a multichannel stream isn't supported.");
		}
	}
}