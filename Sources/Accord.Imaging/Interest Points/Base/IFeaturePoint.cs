// Accord Imaging Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © Christopher Evans, 2009-2011
// http://www.chrisevansdev.com/
//
// Copyright © César Souza, 2009-2013
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

namespace Accord.Imaging
{

    /// <summary>
    ///   Common interface for feature points.
    /// </summary>
    /// 
    public interface IFeaturePoint : IFeaturePoint<double[]>
    {
        // This class exists to maintain backward compatibility with
        // the non-generic version of IFeaturePoint (and to provide
        // a more intuitive way of handling standard, double valued
        // feature description vectors.
    }

    /// <summary>
    ///   Common interface for feature points.
    /// </summary>
    /// 
    public interface IFeaturePoint<T>
    {
        /// <summary>
        ///   Gets or sets the descriptor vector
        ///   associated with this point.
        /// </summary>
        /// 
        T Descriptor { get; }

        /// <summary>
        ///   Gets or sets the x-coordinate of this point.
        /// </summary>
        /// 
        double X { get; }

        /// <summary>
        ///   Gets or sets the y-coordinate of this point.
        /// </summary>
        /// 
        double Y { get; }
    }
}
