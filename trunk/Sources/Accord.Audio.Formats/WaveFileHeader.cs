namespace Accord.Audio.IO
{
	using System;
	using System.Runtime.InteropServices;
	
	/// <summary>
	/// http://www.sichbo.ca/Free_Code/C_Sharp_Audio_CD_Writer_for_Windows_XP
	/// </summary>
	public static class WaveFileFormat
	{
		
		[StructLayout( LayoutKind.Sequential, Pack=1, CharSet=CharSet.Ansi)]
		public struct RIFFChunk
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
			public char[] RiffHeader;
			public int Length;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
			public char[] WaveHeader;
		}

		[StructLayout( LayoutKind.Sequential, Pack=1, CharSet=CharSet.Ansi)]
		public struct FormatHeader
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
			public char []Header;
			
			public short AudioFormat;
			public short NumOfChannels;
			public int SampleRate;
			public int AvgBytesPerSecond;
			public short BytesPerSample;
			public short BitsPerSample;
		}

		[StructLayout( LayoutKind.Sequential, Pack=1, CharSet=CharSet.Ansi)]
		public struct DataHeader
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
			public char[] Header;
			public int Length;
		}
	}
	
	public enum WaveChannel : short
		{
			FrontLeft,
			FrontRight,
			FrontCenter,
			LowFrequency,
			BackLeft,
			BackRight,
			FrontLeftOfCenter,
			FrontRightOfCenter,
			BackCenter,
			SideLeft,
			SideRight,
			TopCenter,
			TopFronLeft,
			TopFrontCenter,
			TopFrontRight,
			TopBackLeft,
			TopBackCenter,
			TopBackRight,
		}
		
		public enum WaveAudioFormat : short
		{
			Unknown = 0,
			PCM = 1,
			ADPCM = 2,
			Float =	3,
			Alaw= 6,
			MuLaw = 7,
			ImaADPCM = 17,
			G723ADPCM = 20,
			GSM610 = 49,
			G721ADPCM = 64,
			MPEG = 80,
		}
}
