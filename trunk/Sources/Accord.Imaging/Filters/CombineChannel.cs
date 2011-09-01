// Accord Imaging Library
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

namespace Accord.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using AForge.Imaging;
    using AForge.Imaging.Filters;

    /// <summary>
    ///   Combine channel filter.
    /// </summary>
    /// 
    public class CombineChannel : BaseInPlaceFilter
    {

        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();



        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks>
        ///   <para>The dictionary defines, which pixel formats are supported for
        ///   source images and which pixel format will be used for resulting image.</para>
        /// 
        ///   <para>See <see cref="P:AForge.Imaging.Filters.IFilterInformation.FormatTranslations"/>
        ///   for more information.</para>
        /// </remarks>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        ///   Constructs a new CombineChannel filter.
        /// </summary>
        /// 
        public CombineChannel(params UnmanagedImage[] channels)
        {
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
        }


        protected override void ProcessFilter(UnmanagedImage image)
        {
            throw new NotImplementedException();
        }
    }
}
