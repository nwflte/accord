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

namespace Accord.DirectSound
{
    using System;
    using System.Threading;
    using Accord.Audio;
    using SlimDX.DirectSound;
    using SlimDX.Multimedia;


    /// <summary>
    ///   System audio output device.
    /// </summary>
    /// 
    public class AudioOutputDevice : IAudioOutput, IDisposable
    {
        private IntPtr owner;
        private Guid device = Guid.Empty;
        private SecondarySoundBuffer buffer;
        private int samplingRate;
        private int bufferSize;
        private int channels;

        private Thread thread;
        private bool stop;

        private NotificationPosition[] notifications;

        /// <summary>
        ///   Constructs a new Audio Output Device.
        /// </summary>
        /// 
        /// <param name="owner">The owner window handle.</param>
        /// <param name="samplingRate">The sampling rate of the device.</param>
        /// <param name="channels">The number of channels of the device.</param>
        /// 
        public AudioOutputDevice(IntPtr owner, int samplingRate, int channels)
            : this(Guid.Empty, owner, samplingRate, channels)
        {
        }

        /// <summary>
        ///   Constructs a new Audio Output Device.
        /// </summary>
        /// 
        /// <param name="device">Global identifier of the audio output device.</param>
        /// <param name="owner">The owner window handle.</param>
        /// <param name="samplingRate">The sampling rate of the device.</param>
        /// <param name="channels">The number of channels of the device.</param>
        /// 
        public AudioOutputDevice(Guid device, IntPtr owner, int samplingRate, int channels)
        {
            this.owner = owner;
            this.samplingRate = samplingRate;
            this.channels = channels;
            this.device = device;

            DirectSound ds = new DirectSound(device);
            ds.SetCooperativeLevel(owner, CooperativeLevel.Priority);


            // Set the output format
            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.IeeeFloat;
            waveFormat.BitsPerSample = 32;
            waveFormat.BlockAlignment = (short)(waveFormat.BitsPerSample * channels / 8);
            waveFormat.Channels = (short)channels;
            waveFormat.SamplesPerSecond = samplingRate;
            waveFormat.AverageBytesPerSecond =
                waveFormat.SamplesPerSecond *
                waveFormat.BlockAlignment;

            bufferSize = 8 * waveFormat.AverageBytesPerSecond;


            // Setup the secondary buffer
            SoundBufferDescription desc2 = new SoundBufferDescription();
            desc2.Flags =
                BufferFlags.GlobalFocus |
                BufferFlags.ControlPositionNotify |
                BufferFlags.GetCurrentPosition2;
            desc2.SizeInBytes = bufferSize;
            desc2.Format = waveFormat;

            buffer = new SecondarySoundBuffer(ds, desc2);


            notifications = new NotificationPosition[2];

            // Set notification for second half of buffer
            notifications[0].Offset = bufferSize / 2 + 1;
            notifications[0].Event = new AutoResetEvent(false);

            // Set notification for first half of buffer
            notifications[1].Offset = bufferSize - 1;
            notifications[1].Event = new AutoResetEvent(false);

            buffer.SetNotificationPositions(notifications);
        }



        /// <summary>
        ///   Gets the sampling rate for the current output device.
        /// </summary>
        /// 
        public int SamplingRate
        {
            get { return samplingRate; }
        }

        /// <summary>
        ///   Gets the number of channels for the current output device.
        /// </summary>
        /// 
        public int Channels
        {
            get { return channels; }
        }

        /// <summary>
        ///   Gets the parent owner form for the device.
        /// </summary>
        /// 
        public IntPtr Owner
        {
            get { return owner; }
        }

        /// <summary>
        ///   Audio output.
        /// </summary>
        /// 
        /// <remarks>Audio output is represented by Guid of audio output device.</remarks>
        /// 
        public virtual string Output
        {
            get { return device.ToString(); }
            set { device = new Guid(value); }
        }

        /// <summary>
        ///   Raised when a frame starts playing.
        /// </summary>
        /// 
        public event EventHandler<PlayFrameEventArgs> FramePlayingStarted;

        /// <summary>
        ///   Raised when the device stops playing.
        /// </summary>
        /// 
        public event EventHandler Stopped;

        /// <summary>
        ///   Raised when a frame starts playing.
        /// </summary>
        /// 
        public event EventHandler<NewFrameRequestedEventArgs> NewFrameRequested;

        /// <summary>
        ///   Starts playing the current buffer.
        /// </summary>
        /// 
        public void Play(float[] samples)
        {
            if (thread != null && thread.IsAlive)
                return;

            Thread t = new Thread(() =>
            {
                // TODO: Write an alternative to the following
                while (buffer.Status == BufferStatus.Playing)
                    Thread.Sleep(100);

                if (Stopped != null)
                    Stopped.Invoke(this, EventArgs.Empty);
            });

            if (FramePlayingStarted != null)
                FramePlayingStarted(this, PlayFrameEventArgs.Empty);

            buffer.Write(samples, 0, LockFlags.None);
            buffer.Play(0, PlayFlags.None);

            t.Start();
        }

        /// <summary>
        ///   Starts playing the current buffer.
        /// </summary>
        /// 
        public void Play()
        {
            thread = new Thread(WorkerThread);

            if (!thread.IsAlive)
                thread.Start();
        }

        /// <summary>
        ///   Worker thread.
        /// </summary>
        /// 
        private void WorkerThread()
        {
            int samples = bufferSize / sizeof(float);

            // The first write should fill the entire buffer.
            var request = new NewFrameRequestedEventArgs(samples);
            NewFrameRequested.Invoke(this, request);

            buffer.Write(request.Buffer, 0, LockFlags.None);

            if (request.Stop)
            {
                buffer.Play(0, PlayFlags.None);
                return;
            }

            if (FramePlayingStarted != null)
                FramePlayingStarted.Invoke(this, new PlayFrameEventArgs(0));

            // The buffer starts playing.
            buffer.Play(0, PlayFlags.Looping);


            while (!stop)
            {
                // When the first half of the buffer has finished
                //  playing and we have just started playing the
                //  second half, we will write the next samples in
                //  the first half of the buffer again.
                notifications[0].Event.WaitOne();

                request = new NewFrameRequestedEventArgs(samples / 2);
                NewFrameRequested.Invoke(this, request);

                if (request.Stop || stop) break;

                buffer.Write(request.Buffer, 0, LockFlags.None);


                // When the second half of the buffer has finished
                //  playing, the first half of the buffer will
                //  start playing again (since this is a circular
                //  buffer). At this time, we can write the next
                //  samples in the second half of the buffer.
                notifications[1].Event.WaitOne();

                request = new NewFrameRequestedEventArgs(samples / 2);
                NewFrameRequested.Invoke(this, request);

                if (request.Stop || stop) break;

                buffer.Write(request.Buffer, bufferSize / 2, LockFlags.None);
            }

            buffer.Stop();

            if (Stopped != null)
                Stopped.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///   Stops playing the current buffer.
        /// </summary>
        public void Stop()
        {
            stop = true;

            if (buffer != null)
                buffer.Stop();

            if (thread != null)
            {
                thread.Join();
                thread = null;
            }
        }


        #region IDisposable members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (buffer != null)
                {
                    buffer.Dispose();
                    buffer = null;
                }
            }
        }
        #endregion

    }
}
