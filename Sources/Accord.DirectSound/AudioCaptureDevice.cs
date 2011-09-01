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
    ///   Audio source for local audio capture device (for example microphone).
    /// </summary>
    /// 
    /// <remarks><para>The audio source captures audio data from local audio capture device.
    /// DirectSound is used for capturing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // enumerate audio devices
    /// audioDevices = new FilterInfoCollection( FilterCategory.AudioInputDevice );
    /// // create audio source
    /// AudioCaptureDevice audioSource = new AudioCaptureDevice( audioDevices[0].MonikerString );
    /// // set NewFrame event handler
    /// audioSource.NewFrame += new NewFrameEventHandler( audio_NewFrame );
    /// // start the audio source
    /// audioSource.Start( );
    /// // ...
    /// // signal to stop
    /// audioSource.SignalToStop( );
    /// // ...
    /// 
    /// private void audio_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     float[][] signal = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class AudioCaptureDevice : IAudioSource, IDisposable
    {

        // moniker string of audio capture device
        private Guid device = Guid.Empty;

        // user data associated with the audio source
        private object userData = null;

        // received frames count
        private int framesReceived;

        // recieved byte count
        private int bytesReceived;

        // specifies desired capture frame size
        private int desiredCaptureSize = 4096;

        // specifies the sample rate used in the source
        private int sampleRate = 44100;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;

        /// <summary>
        ///   New frame event.
        /// </summary>
        /// 
        /// <remarks><para>Notifies clients about new available frame from audio source.</para>
        /// 
        /// <para><note>Since audio source may have multiple clients, each client is responsible for
        /// making a copy (cloning) of the passed audio frame, because the audio source disposes its
        /// own original copy after notifying of clients.</note></para>
        /// </remarks>
        /// 
        public event EventHandler<NewFrameEventArgs> NewFrame;


        /// <summary>
        ///   Audio source error event.
        /// </summary>
        /// 
        /// <remarks>This event is used to notify clients about any type of errors occurred in
        /// audio source object, for example internal exceptions.</remarks>
        /// 
        public event EventHandler<AudioSourceErrorEventArgs> AudioSourceError;

        /// <summary>
        ///   Audio source.
        /// </summary>
        /// 
        /// <remarks>Audio source is represented by Guid of audio capture device.</remarks>
        /// 
        public virtual string Source
        {
            get { return device.ToString(); }
            set { device = new Guid(value); }
        }

        /// <summary>
        ///   Gets or sets the desired frame size.
        /// </summary>
        public int DesiredFrameSize
        {
            get { return desiredCaptureSize; }
            set { desiredCaptureSize = value; }
        }

        /// <summary>
        ///   Received frames count.
        /// </summary>
        /// 
        /// <remarks>Number of frames the audio source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int FramesReceived
        {
            get
            {
                int frames = framesReceived;
                framesReceived = 0;
                return frames;
            }
        }

        /// <summary>
        ///   Received bytes count.
        /// </summary>
        /// 
        /// <remarks>Number of bytes the audio source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int BytesReceived
        {
            get
            {
                int bytes = bytesReceived;
                bytesReceived = 0;
                return bytes;
            }
        }

        /// <summary>
        ///   User data.
        /// </summary>
        /// 
        /// <remarks>The property allows to associate user data with audio source object.</remarks>
        /// 
        public object UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        /// <summary>
        ///   State of the audio source.
        /// </summary>
        /// 
        /// <remarks>Current state of audio source object - running or not.</remarks>
        /// 
        public bool IsRunning
        {
            get
            {
                if (thread != null)
                {
                    // check thread status
                    if (thread.Join(0) == false)
                        return true;

                    // the thread is not running, free resources
                    Free();
                }
                return false;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioCaptureDevice"/> class.
        /// </summary>
        /// 
        public AudioCaptureDevice()
        {
            this.device = Guid.Empty;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioCaptureDevice"/> class.
        /// </summary>
        /// 
        /// <param name="device">Global identifier of the audio capture device.</param>
        /// 
        public AudioCaptureDevice(Guid device)
        {
            this.device = device;
        }

        /// <summary>
        ///   Start audio source.
        /// </summary>
        /// 
        /// <remarks>Starts audio source and return execution to caller. audio source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        public void Start()
        {
            if (thread == null)
            {
                // check source
                if (device == null)
                    throw new ArgumentException("Audio source is not specified");

                framesReceived = 0;
                bytesReceived = 0;

                // create events
                stopEvent = new ManualResetEvent(false);

                // create and start new thread
                thread = new Thread(new ThreadStart(WorkerThread));
                thread.Name = device.ToString(); // mainly for debugging
                thread.Start();
            }
        }

        /// <summary>
        ///   Signals audio source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals audio source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        public void SignalToStop()
        {
            // stop thread
            if (thread != null)
            {
                // signal to stop
                stopEvent.Set();
            }
        }

        /// <summary>
        ///   Wait for audio source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop()
        {
            if (thread != null)
            {
                // wait for thread stop
                thread.Join();

                Free();
            }
        }

        /// <summary>
        ///   Stop audio source.
        /// </summary>
        /// 
        /// <remarks><para>Stops audio source aborting its thread.</para>
        /// 
        /// <para><note>Since the method aborts background thread, its usage is highly not preferred
        /// and should be done only if there are no other options. The correct way of stopping camera
        /// is <see cref="SignalToStop">signaling it stop</see> and then
        /// <see cref="WaitForStop">waiting</see> for background thread's completion.</note></para>
        /// </remarks>
        /// 
        public void Stop()
        {
            if (this.IsRunning)
            {
                thread.Abort();
                WaitForStop();
            }
        }

        /// <summary>
        ///   Free resource.
        /// </summary>
        /// 
        private void Free()
        {
            thread = null;

            // release events
            stopEvent.Close();
            stopEvent = null;
        }



        /// <summary>
        ///   Worker thread.
        /// </summary>
        /// 
        private void WorkerThread()
        {
            // Get the selected capture device
            DirectSoundCapture captureDevice = new DirectSoundCapture(device);

            // Set the capture format
            WaveFormat waveFormat = new WaveFormat();
            waveFormat.FormatTag = WaveFormatTag.IeeeFloat;
            waveFormat.BitsPerSample = 32; // Floats are 32 bits
            waveFormat.BlockAlignment = (short)(waveFormat.BitsPerSample / 8);
            waveFormat.Channels = 1;
            waveFormat.SamplesPerSecond = sampleRate;
            waveFormat.AverageBytesPerSecond =
                waveFormat.SamplesPerSecond * waveFormat.BlockAlignment * waveFormat.Channels;

            // Setup the capture buffer
            CaptureBufferDescription bufferDescription = new CaptureBufferDescription();
            bufferDescription.BufferBytes = desiredCaptureSize * sizeof(float);
            bufferDescription.Format = waveFormat;
            bufferDescription.WaveMapped = false;


            // Create the capture buffer
            CaptureBuffer buffer = new CaptureBuffer(captureDevice, bufferDescription);
            buffer.Start(true);

            try
            {
                float[] sample = new float[desiredCaptureSize];

                while (!stopEvent.WaitOne(0, true))
                {
                    buffer.Read(sample, 0, true);
                    OnNewFrame(sample);
                }

                buffer.Stop();
            }
            catch (Exception ex)
            {
                if (AudioSourceError != null)
                {
                    AudioSourceError(this, new AudioSourceErrorEventArgs(ex.Message));
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                buffer.Dispose();
                captureDevice.Dispose();
            }
        }

        /// <summary>
        /// Notifies client about new frame.
        /// </summary>
        /// 
        /// <param name="frame">New frame's audio.</param>
        /// 
        protected void OnNewFrame(float[] frame)
        {
            framesReceived++;
            if ((!stopEvent.WaitOne(0, true)) && (NewFrame != null))
            {
                Signal s = Signal.FromArray(frame, sampleRate);
                NewFrame(this, new NewFrameEventArgs(s));
            }
        }


        /// <summary>
        ///   Gets whether this audio source supports seeking.
        /// </summary>
        /// 
        public bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        ///    This source does not support seeking.
        /// </summary>
        /// 
        public void Seek(int frameIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Gets or sets the desired sample rate for this capturing device.
        /// </summary>
        /// 
        public int SampleRate
        {
            get { return this.sampleRate; }
            set { this.sampleRate = value; }
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
                if (stopEvent != null)
                {
                    stopEvent.Close();
                    stopEvent = null;
                }
            }
        }
        #endregion

    }
}