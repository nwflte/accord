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
// Based on code from the:
//   Motion Detection sample application
//   AForge.NET Framework
//   http://www.aforgenet.com/framework/
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GloveTracking
{
    public partial class MotionRegionsForm : Form
    {
        // Video frame sample to show
        public Bitmap VideoFrame
        {
            set { defineRegionsControl.BackgroundImage = value; }
        }

        // Motion rectangles
        public Rectangle[] ObjectRectangles
        {
            get { return defineRegionsControl.Rectangles; }
            set { defineRegionsControl.Rectangles = value; }
        }

        // Class constructor
        public MotionRegionsForm( )
        {
            InitializeComponent( );

            defineRegionsControl.OnNewRectangle += new NewRectangleHandler( defineRegionsControl_NewRectangleHandler );
        }

        // On first displaying of the form
        protected override void OnLoad( EventArgs e )
        {
            // get video frame dimension
            if ( defineRegionsControl.BackgroundImage != null )
            {
                int imageWidth  = defineRegionsControl.BackgroundImage.Width;
                int imageHeight = defineRegionsControl.BackgroundImage.Height;

                // resize region definition control
                defineRegionsControl.Size = new Size( imageWidth + 2, imageHeight + 2 );
                // resize window
                this.Size = new Size( imageWidth + 2 + 26, imageHeight + 2 + 118 );
            }

            base.OnLoad( e );
        }

        // New rectangle definition was finished
        private void defineRegionsControl_NewRectangleHandler( object sender, Rectangle rect )
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}