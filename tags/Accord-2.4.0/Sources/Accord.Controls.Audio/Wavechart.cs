// Accord Control Library
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

namespace Accord.Controls
{
    using System.Collections;
    using System.Drawing;
    using System.Windows.Forms;
    using Accord.Math;
    using AForge;

    /// <summary>
    ///   Waveform chart control.
    /// </summary>
    /// 
    /// <remarks><para>The Waveform chart control allows to display multiple
    /// waveforms at time.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create data series array
    /// float[] testValues = new float[128];
    /// // fill data series
    /// for ( int i = 0; i &lt; 128; i++ )
    /// {
    ///     testValues[i] = Math.Sin( i / 18.0 * Math.PI );
    /// }
    /// // add new waveform to the chart
    /// chart.AddWaveform( "Test", Color.DarkGreen, 3 );
    /// // update the chart
    /// chart.UpdateWaveform( "Test", testValues );
    /// </code>
    /// </remarks>
    /// 
    public class Wavechart : System.Windows.Forms.Control
    {
        // waveform data
        private class Waveform
        {
            public float[] data = null;
            public Color color = Color.Blue;
            public int width = 1;
            public bool updateYRange = true;
        }

        // data series table
        Hashtable waveTable = new Hashtable();

        private Pen blackPen = new Pen(Color.Black);
        private Brush whiteBrush = new SolidBrush(Color.White);

        private DoubleRange rangeX = new DoubleRange(0, 1);
        private DoubleRange rangeY = new DoubleRange(0, 1);

        /// <summary>
        /// Chart's X range.
        /// </summary>
        /// 
        /// <remarks><para>The value sets the X range of data to be displayed on the chart.</para></remarks>
        /// 
        public DoubleRange RangeX
        {
            get { return rangeX; }
            set
            {
                rangeX = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Chart's Y range.
        /// </summary>
        /// 
        /// <remarks>The value sets the Y range of data to be displayed on the chart.</remarks>
        ///
        public DoubleRange RangeY
        {
            get { return rangeY; }
            set
            {
                rangeY = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wavechart"/> class.
        /// </summary>
        /// 
        public Wavechart()
        {
            // This call is required by the Signals.Forms Form Designer.
            InitializeComponent();

            // update control style
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                     ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                // free graphics resources
                blackPen.Dispose();
                whiteBrush.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Chart
            // 
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        ///  Paints the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int clientWidth = ClientRectangle.Width;
            int clientHeight = ClientRectangle.Height;

            // fill with white background
            g.FillRectangle(whiteBrush, 0, 0, clientWidth - 1, clientHeight - 1);

            // draw a black rectangle
            g.DrawRectangle(blackPen, 0, 0, clientWidth - 1, clientHeight - 1);

            if (DesignMode)
                return;

            // check if there are any data series
            if ((rangeY != null) && (rangeX.Length != 0))
            {
                DoubleRange rangeClientX = new DoubleRange(0, clientWidth);
                DoubleRange rangeClientY = new DoubleRange(clientHeight, 0);

                // walk through all data series
                foreach (Waveform waveform in waveTable.Values)
                {
                    // get data of the waveform
                    float[] data = waveform.data;

                    // check for available data
                    if (data == null || rangeY.Length == 0)
                        continue;

                    Pen pen = new Pen(waveform.color, waveform.width);

                    int xPrev = 0;
                    int yPrev = (int)rangeY.Scale(rangeClientY, data[0]);

                    for (int x = 0; x < clientWidth; x++)
                    {
                        int index = (int)rangeClientX.Scale(rangeX, x);
                        if (index < 0 || index >= data.Length)
                            index = data.Length - 1;
                        int y = (int)rangeY.Scale(rangeClientY, data[index]);

                        g.DrawLine(pen, xPrev, yPrev, x, y);


                        xPrev = x;
                        yPrev = y;
                    }

                    pen.Dispose();
                }
            }
        }

        /// <summary>
        /// Add Waveform to the chart.
        /// </summary>
        /// 
        /// <param name="name">Waveform name.</param>
        /// <param name="color">Waveform color.</param>
        /// <param name="width">Waveform width.</param>
        /// 
        /// <remarks><para>Adds new empty waveform to the collection of waves. To update this
        /// wave the <see cref="UpdateWaveform"/> method should be used.</para>
        /// </remarks>
        /// 
        public void AddWaveform(string name, Color color, int width)
        {
            AddWaveform(name, color, width, true);
        }

        /// <summary>
        /// Add Waveform to the chart.
        /// </summary>
        /// 
        /// <param name="name">Waveform name.</param>
        /// <param name="color">Waveform color.</param>
        /// <param name="width">Waveform width.</param>
        /// <param name="updateYRange">Specifies if <see cref="RangeY"/> should be updated.</param>
        /// 
        /// <remarks><para>Adds new empty waveform to the collection of waves. To update this
        /// wave the <see cref="UpdateWaveform"/> method should be used.</para>
        /// </remarks>
        /// 
        /// <remarks><para>Adds new empty data series to the collection of data series.</para>
        /// 
        /// <para>The <b>updateYRange</b> parameter specifies if the waveform may affect displayable
        /// Y range. If the value is set to false, then displayable Y range is not updated, but used the
        /// range, which was specified by user (see <see cref="RangeY"/> property). In the case if the
        /// value is set to true, the displayable Y range is recalculated to fully fit the new data
        /// series.</para>
        /// </remarks>
        /// 
        public void AddWaveform(string name, Color color, int width, bool updateYRange)
        {
            // create new series definition ...
            Waveform series = new Waveform();
            // ... add fill it
            series.color = color;
            series.width = width;
            series.updateYRange = updateYRange;
            // add to series table
            waveTable.Add(name, series);
        }

        /// <summary>
        /// Update data series on the chart.
        /// </summary>
        /// 
        /// <param name="name">Data series name to update.</param>
        /// <param name="data">Data series values.</param>
        /// 
        public void UpdateWaveform(string name, float[] data)
        {
            // get data series
            Waveform series = (Waveform)waveTable[name];
            if (series == null)
                return;

            // update data
            series.data = data;

            // update Y range
            if (series.updateYRange)
                UpdateYRange();
            // invalidate the control
            Invalidate();
        }

        /// <summary>
        /// Remove a waveform from the chart.
        /// </summary>
        /// 
        /// <param name="name">Waveform name to remove.</param>
        /// 
        public void RemoveWaveform(string name)
        {
            // remove data series from table
            waveTable.Remove(name);
            // invalidate the control
            Invalidate();
        }

        /// <summary>
        /// Remove all waveforms from the chart.
        /// </summary>
        public void RemoveAllWaveforms()
        {
            // remove all data series from table
            waveTable.Clear();
            // invalidate the control
            Invalidate();
        }

        /// <summary>
        /// Update Y range.
        /// </summary>
        private void UpdateYRange()
        {
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            // walk through all data series
            foreach (Waveform wave in waveTable.Values)
            {
                // get data of the series
                float[] data = wave.data;

                if ((wave.updateYRange) && (data != null))
                {
                    // Let the compiler perform optimizations.
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] > maxY)
                            maxY = data[i];
                        if (data[i] < minY)
                            minY = data[i];
                    }
                }
            }

            // update Y range, if there are any data
            if ((minY != float.MaxValue) || (maxY != float.MinValue))
            {
                rangeY = new DoubleRange(minY, maxY);
            }
        }
    }
}
