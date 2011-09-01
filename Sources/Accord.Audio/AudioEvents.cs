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

    /// <summary>
    ///   Arguments for audio source error event from audio source.
    /// </summary>
    ///
    public class AudioSourceErrorEventArgs : EventArgs
    {
        private string description;

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioSourceErrorEventArgs"/> class.
        /// </summary>
        ///
        /// <param name="description">Error description.</param>
        ///
        public AudioSourceErrorEventArgs(string description)
        {
            this.description = description;
        }

        /// <summary>
        ///   Audio source error description.
        /// </summary>
        ///
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        ///   Represents an event with no event data.
        /// </summary>
        public new static readonly AudioSourceErrorEventArgs Empty;

    }

    /// <summary>
    ///   Arguments for new frame request from an audio output device.
    /// </summary>
    /// 
    public class NewFrameRequestedEventArgs : EventArgs
    {
        private float[] buffer;

        /// <summary>
        ///   Gets or sets the buffer to be played in the audio source.
        /// </summary>
        /// 
        public float[] Buffer
        {
            get { return buffer; }
            set
            {
                if (value.Length > Samples)
                    throw new ArgumentException("The length of the buffer should be equal or less than the requested number of samples.");

                buffer = value;
            }
        }

        /// <summary>
        ///   Gets or sets whether the playing should stop.
        /// </summary>
        /// 
        public bool Stop { get; set; }

        /// <summary>
        ///   Gets the number of samples which should be placed in the buffer.
        /// </summary>
        /// 
        public int Samples { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NewFrameRequestedEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples being requested.</param>
        /// 
        public NewFrameRequestedEventArgs(int samples)
        {
            this.Samples = samples;
        }

    }

    /// <summary>
    ///   Arguments for new block event from audio source.
    /// </summary>
    ///
    public class NewFrameEventArgs : EventArgs
    {
        private Signal signal;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewFrameEventArgs"/> class.
        /// </summary>
        ///
        /// <param name="signal">New signal frame.</param>
        ///
        public NewFrameEventArgs(Signal signal)
        {
            this.signal = signal;
        }

        /// <summary>
        ///   New Frame from audio source.
        /// </summary>
        ///
        public Signal Signal
        {
            get { return signal; }
        }

    }

    /// <summary>
    ///   Arguments for new block event from audio source.
    /// </summary>
    ///
    public class PlayFrameEventArgs : EventArgs
    {
        private int frameIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewFrameEventArgs"/> class.
        /// </summary>
        ///
        /// <param name="frameIndex">New frame.</param>
        ///
        public PlayFrameEventArgs(int frameIndex)
        {
            this.frameIndex = frameIndex;
        }

        /// <summary>
        ///   New block from audio source.
        /// </summary>
        ///
        public int FrameIndex
        {
            get { return frameIndex; }
        }

        /// <summary>
        ///   Represents an event with no event data.
        /// </summary>
        /// 
        public static readonly new PlayFrameEventArgs Empty = new PlayFrameEventArgs(0);

    }
}