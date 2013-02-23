// Accord Math Library
// The Accord.NET Framework
// http://accord.googlecode.com
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

namespace Accord.Math.Comparers
{
    using System.Collections.Generic;

    /// <summary>
    ///   Directions for the General Comparer.
    /// </summary>
    /// 
    public enum ComparerDirection
    {
        /// <summary>
        ///   Sorting will be performed in ascending order.
        /// </summary>
        /// 
        Ascending = +1,

        /// <summary>
        ///   Sorting will be performed in descending order.
        /// </summary>
        /// 
        Descending = -1
    };

    /// <summary>
    ///   General comparer which supports multiple directions
    ///   and comparison of absolute values.
    /// </summary>
    /// 
    public class GeneralComparer : IComparer<double>, IComparer<int>
    {
        private bool absolute;
        private int direction = 1;

        /// <summary>
        ///   Gets or sets the sorting direction
        ///   used by this comparer.
        /// </summary>
        /// 
        public ComparerDirection Direction
        {
            get { return (ComparerDirection)direction; }
            set { direction = (int)value; }
        }

        /// <summary>
        ///   Constructs a new General Comparer.
        /// </summary>
        /// 
        /// <param name="direction">The direction to compare.</param>
        /// 
        public GeneralComparer(ComparerDirection direction)
            : this(direction, false) { }

        /// <summary>
        ///   Constructs a new General Comparer.
        /// </summary>
        /// 
        /// <param name="direction">The direction to compare.</param>
        /// <param name="useAbsoluteValues">True to compare absolute values, false otherwise. Default is false.</param>
        /// 
        public GeneralComparer(ComparerDirection direction, bool useAbsoluteValues)
        {
            this.direction = (int)direction;
            this.absolute = useAbsoluteValues;
        }

        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than,
        ///    equal to, or greater than the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        public int Compare(double x, double y)
        {
            if (absolute)
                return direction * (System.Math.Abs(x).CompareTo(System.Math.Abs(y)));
            else
                return direction * (x.CompareTo(y));
        }

        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than,
        ///    equal to, or greater than the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        public int Compare(int x, int y)
        {
            if (absolute)
                return direction * (System.Math.Abs(x).CompareTo(System.Math.Abs(y)));
            else
                return direction * (x.CompareTo(y));
        }

    }
}
