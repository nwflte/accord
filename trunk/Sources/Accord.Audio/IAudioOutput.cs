// Accord (Experimental) Audio Library
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2011
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
	using Accord;
	
	/// <summary>
	///   Audio Output Device Interface
	/// </summary>
    /// 
	public interface IAudioOutput
	{

        /// <summary>
        ///   Starts playing the buffer
        /// </summary>
        /// 
		void Play();

        /// <summary>
        ///   Stops playing the buffer
        /// </summary>
        /// 
		void Stop();

        /// <summary>
        ///   Audio output.
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>
        ///   The meaning of the property depends on particular audio output.
        ///   Depending on audio source it may be a file name, driver guid, URL
        ///   or any other string describing the audio source.</para>
        /// </remarks>
        /// 
        string Output { get; set; }

        /// <summary>
        ///   Indicates a frame has started execution.
        /// </summary>
        /// 
		event EventHandler<PlayFrameEventArgs> FramePlayingStarted;

        /// <summary>
        ///   Indicates the audio output is requesting a new sample.
        /// </summary>
        /// 
        event EventHandler<NewFrameRequestedEventArgs> NewFrameRequested;

	}
}
