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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Accord.Vision;
using AForge.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Imaging.Filters;
using System.IO;
using Accord.Vision.Detection.Cascades;

namespace FaceDetection
{
    public partial class MainForm : Form
    {
        Bitmap picture = FaceDetection.Properties.Resources.judybats;
        HaarCascade cascade = new FaceHaarCascade();

        public MainForm()
        {
            InitializeComponent();

            pictureBox1.Image = picture;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            HaarObjectDetector detector = new HaarObjectDetector(
                cascade, 30, ObjectDetectorSearchMode.NoOverlap,
                1.5f, ObjectDetectorScalingMode.SmallerToGreater);

            Rectangle[] objects = detector.ProcessFrame(picture);

            if (objects.Length > 0)
            {
                RectanglesMarker marker = new RectanglesMarker(objects, Color.Fuchsia);
                pictureBox1.Image = marker.Apply(picture);
            }
        }

    }
}
