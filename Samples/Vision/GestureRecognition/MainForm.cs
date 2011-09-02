// Accord.NET Sample Applications
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

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using Accord.Imaging.Filters;
using Accord.Math;
using Accord.Statistics.Models.Markov;
using Accord.Vision.Detection;
using Accord.Vision.Tracking;
using AForge;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.VFW;
using ZedGraph;
using Accord.Vision.Detection.Cascades;


namespace GestureRecognition
{
    public partial class MainForm : Form
    {

        private enum State
        {
            Idle,
            Detecting,
            Tracking,
        }


        // opened video source
        private IVideoSource videoSource;

        // object detector
        HaarObjectDetector detector;

        // object tracker
        Camshift faceTracker;
        HslBlobTracker leftTracker;
        HslBlobTracker rightTracker;

        RectanglesMarker marker;

        private State state = State.Idle;

        private bool showBackprojecton = false;
        private bool showTrackingBox = true;
        private bool showTrackingAxis = true;
        private bool showTrackingWindow = false;

        // statistics length
        private const int statLength = 15;
        // current statistics index
        private int statIndex = 0;
        // ready statistics values
        private int statReady = 0;
        // statistics array
        private int[] statCount = new int[statLength];


        // Constructor
        public MainForm()
        {
            InitializeComponent();

            HaarCascade cascade = new FaceHaarCascade();
            detector = new HaarObjectDetector(cascade,
                25, ObjectDetectorSearchMode.Single, 1.2f,
                ObjectDetectorScalingMode.GreaterToSmaller);
        }

        // Application's main form is closing
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseVideoSource();
        }

        // "Exit" menu item clicked
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        // "Open" menu item clieck - open AVI file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // create video source
                AVIFileVideoSource fileSource = new AVIFileVideoSource(openFileDialog.FileName);

                OpenVideoSource(fileSource);
            }
        }


        // Open local video capture device
        private void localVideoCaptureDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VideoCaptureDeviceForm form = new VideoCaptureDeviceForm();

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                // create video source
                VideoCaptureDevice videoSource = new VideoCaptureDevice(form.VideoDevice);

                // set frame size
                videoSource.DesiredFrameSize = new Size(320, 240);

                // open it
                OpenVideoSource(videoSource);
            }
        }

        // Open video file using DirectShow
        private void openVideoFileusingDirectShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // create video source
                FileVideoSource fileSource = new FileVideoSource(openFileDialog.FileName);

                // open it
                OpenVideoSource(fileSource);
            }

        }

        // Open video source
        private void OpenVideoSource(IVideoSource source)
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // close previous video source
            CloseVideoSource();

            // start new video source
            videoSourcePlayer.VideoSource = source;
            videoSourcePlayer.Start();

            // reset statistics
            statIndex = statReady = 0;

            // start timers
            timer.Start();
            alarmTimer.Start();

            videoSource = source;

            this.Cursor = Cursors.Default;
        }

        // Close current video source
        private void CloseVideoSource()
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            videoSourcePlayer.SignalToStop();

            // wait 2 seconds until camera stops
            for (int i = 0; (i < 50) && (videoSourcePlayer.IsRunning); i++)
            {
                Thread.Sleep(100);
            }
            if (videoSourcePlayer.IsRunning)
                videoSourcePlayer.Stop();

            // stop timers
            timer.Stop();
            alarmTimer.Stop();

            // reset motion detector
            faceTracker = new Camshift();
            leftTracker = new HslBlobTracker();
            rightTracker = new HslBlobTracker();

            leftTracker.Filter.Hue = new IntRange(347, 8);
            leftTracker.Filter.Saturation = new Range(0.353f, 1.0f);
            leftTracker.Filter.Luminance = new Range(0.125f, 1.0f);
            leftTracker.MinHeight = 25;
            leftTracker.MinWidth = 25;
            leftTracker.MaxHeight = 200;
            leftTracker.MaxWidth = 200;
            leftTracker.Extract = true;
            
            rightTracker.MinHeight = 25;
            rightTracker.MinWidth = 25;
            rightTracker.Filter.Hue = new IntRange(117, 177);
            rightTracker.Filter.Saturation = new Range(0.169f, 0.906f);
            rightTracker.Filter.Luminance = new Range(0.110f, 0.624f);
            rightTracker.Extract = true;

            faceTracker.Conservative = false;
            faceTracker.Extract = true;

            videoSourcePlayer.BorderColor = Color.Black;
            this.Cursor = Cursors.Default;
        }


        private int task;

        // New frame received by the player
        private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {
            lock (this)
            {
                if (state == State.Idle)
                {
                    // Do nothing.
                }
                else if (state == State.Detecting)
                {
                    state = State.Idle;

                    // Searching faces
                    Rectangle[] regions = detector.ProcessFrame(image);

                    // If a face has been found
                    if (regions.Length > 0)
                    {
                        // Reset the tracker
                        faceTracker.Reset();

                        // Will track the first face found
                        faceTracker.SearchWindow = regions[0];

                        // Show the face markers
                        marker = new RectanglesMarker(faceTracker.SearchWindow);

                        image = marker.Apply(image);

                        state = State.Tracking;
                    }
                    else
                    {
                        state = State.Detecting;
                    }
                }
                else if (state == State.Tracking)
                {
                    UnmanagedImage im = UnmanagedImage.FromManagedImage(image);

                    // Track the object
                    if (task == 0)
                    {
                        faceTracker.ProcessFrame(im);
                    }
                    else if (task == 1)
                    {
                        leftTracker.ProcessFrame(im);
                        rightTracker.ProcessFrame(im);
                        task = -1;
                    }

                    task++;

                    // Get the object position
                    var face = faceTracker.TrackingObject;
                    var leftHand = leftTracker.TrackingObject;
                    var rightHand = rightTracker.TrackingObject;

                    /*
                    if (recognizer != null)
                    {
                        if (leftHand.Center != new IntPoint(0, 0))
                            recognizer.Current = leftHand.Center;

                        PointsMarker pmarker = new PointsMarker(recognizer.GetWindowPoints());
                        pmarker.Width = 5;
                        pmarker.ApplyInPlace(im);
                    }*/

                    if (face.Image != null)
                        pbFace.Image = face.Image.ToManagedImage();

                    if (leftHand.Image != null)
                        pbLeft.Image = leftHand.Image.ToManagedImage();

                    if (rightHand.Image != null)
                        pbRight.Image = rightHand.Image.ToManagedImage();
                    /*  Crop crop = new Crop(face.Rectangle);
                      crop.Rectangle = face.Rectangle;
                      var faceImg = crop.Apply(im);
                      pbFace.Image = faceImg.ToManagedImage();

                      if (leftHand.Rectangle.Width > 0)
                      {
                          crop.Rectangle = leftHand.Rectangle;
                          var leftHandImg = crop.Apply(im);

                          FastCornersDetector s = new FastCornersDetector(40);

                          var pt = s.ProcessImage(leftHandImg);

                          PointsMarker pm = new PointsMarker(pt.ToArray());
                          pm.ApplyInPlace(leftHandImg);
                          pbLeft.Image = leftHandImg.ToManagedImage();
                      }

                      if (rightHand.Rectangle.Width > 0)
                      {
                          crop.Rectangle = rightHand.Rectangle;
                          var rightHandImg = crop.Apply(im);
                          pbRight.Image = rightHandImg.ToManagedImage();
                      }
                      */




                    marker = new RectanglesMarker(
                        new Rectangle[]
                            { 
                                face.Rectangle,
                                leftHand.Rectangle,
                                rightHand.Rectangle,
                            });


                    marker.ApplyInPlace(im);
                    image = im.ToManagedImage();
                }
                else
                {
                    if (marker != null)
                        image = marker.Apply(image);
                }

            }
        }



        // On timer event - gather statistics
        private void timer_Tick(object sender, EventArgs e)
        {
            IVideoSource videoSource = videoSourcePlayer.VideoSource;

            if (videoSource != null)
            {
                // get number of frames for the last second
                statCount[statIndex] = videoSource.FramesReceived;

                // increment indexes
                if (++statIndex >= statLength)
                    statIndex = 0;
                if (statReady < statLength)
                    statReady++;

                float fps = 0;

                // calculate average value
                for (int i = 0; i < statReady; i++)
                {
                    fps += statCount[i];
                }
                fps /= statReady;

                statCount[statIndex] = 0;

                fpsLabel.Text = fps.ToString("F2") + " fps";
            }
        }





        // On opening of Tools menu
        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            localVideoCaptureSettingsToolStripMenuItem.Enabled =
                ((videoSource != null) && (videoSource is VideoCaptureDevice));
        }

        // Display properties of local capture device
        private void localVideoCaptureSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((videoSource != null) && (videoSource is VideoCaptureDevice))
            {
                try
                {
                    ((VideoCaptureDevice)videoSource).DisplayPropertyPage(this.Handle);
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("The video source does not support configuration property page.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void videoSourcePlayer_Click(object sender, EventArgs e)
        {
            state = State.Detecting;
        }

        private void drawTrackingWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showTrackingWindow = !showTrackingWindow;
        }

        private void drawObjectAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showTrackingAxis = !showTrackingAxis;
        }

        private void drawObjectBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showTrackingBox = !showTrackingBox;
        }

        private void displayBackprojectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showBackprojecton = !showBackprojecton;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void videoSourcePlayer_Click_1(object sender, EventArgs e)
        {
            if (state == State.Idle)
                state = State.Detecting;
            else if (state == State.Tracking)
                state = State.Idle;

        }

        private void loadClassifierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        public void CreateBarGraph(ZedGraphControl zgc, double[] responses)
        {
            GraphPane myPane = zgc.GraphPane;

            myPane.CurveList.Clear();

            myPane.Title.IsVisible = false;
            myPane.Legend.IsVisible = false;
            myPane.Border.IsVisible = false;
            myPane.Border.IsVisible = false;
            myPane.Margin.Bottom = 20f;
            myPane.Margin.Right = 20f;
            myPane.Margin.Left = 20f;
            myPane.Margin.Top = 30f;

            myPane.YAxis.Title.IsVisible = true;
            myPane.YAxis.IsVisible = true;
            myPane.YAxis.MinorGrid.IsVisible = false;
            myPane.YAxis.MajorGrid.IsVisible = false;
            myPane.YAxis.IsAxisSegmentVisible = false;
            myPane.YAxis.Scale.Max = responses.Length + 0.5;
            myPane.YAxis.Scale.Min = -0.5;
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            myPane.YAxis.Title.Text = "Classes";
            myPane.YAxis.MinorTic.IsOpposite = false;
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsInside = false;
            myPane.YAxis.MajorTic.IsInside = false;
            myPane.YAxis.MinorTic.IsOutside = false;
            myPane.YAxis.MajorTic.IsOutside = false;

            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.XAxis.MajorTic.IsOpposite = false;
            myPane.XAxis.Title.IsVisible = true;
            myPane.XAxis.Title.Text = "Relative class response";
            myPane.XAxis.IsVisible = true;
            myPane.XAxis.Scale.Min = -100;
            myPane.XAxis.Scale.Max = 100;
            myPane.XAxis.IsAxisSegmentVisible = false;
            myPane.XAxis.MajorGrid.IsVisible = false;
            myPane.XAxis.MajorGrid.IsZeroLine = false;
            myPane.XAxis.MinorTic.IsOpposite = false;
            myPane.XAxis.MinorTic.IsInside = false;
            myPane.XAxis.MinorTic.IsOutside = false;
            myPane.XAxis.Scale.Format = "0'%";


            // Create data points for three BarItems using Random data
            PointPairList list = new PointPairList();

            for (int i = 0; i < responses.Length; i++)
                list.Add(responses[i] * 100, i + 1);

            BarItem myCurve = myPane.AddBar("b", list, Color.DarkBlue);


            // Set BarBase to the YAxis for horizontal bars
            myPane.BarSettings.Base = BarBase.Y;


            zgc.AxisChange();
            zgc.Invalidate();

        }

    }
}