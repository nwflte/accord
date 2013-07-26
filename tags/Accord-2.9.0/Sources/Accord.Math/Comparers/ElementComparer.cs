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
    ///   Array element comparer.
    /// </summary>
    /// 
    public class ElementComparer : IComparer<double[]>, IEqualityComparer<double[]>
    {
        /// <summary>
        ///   Gets or sets the element index to compare.
        /// </summary>
        /// 
        public int Index { get; set; }

        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// 
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>. Zero <paramref name="x"/> equals <paramref name="y"/>. Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.-or- <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other. </exception>
        /// 
        public int Compare(double[] x, double[] y)
        {
            return x[Index].CompareTo(y[Index]);
        }

        /// <summary>
        ///   Determines whether two instances are equal.
        /// </summary>
        /// 
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        ///   <c>true</c> if the specified object is equal to the other; otherwise, <c>false</c>.
        /// </returns>
        ///   
        public bool Equals(double[] x, double[] y)
        {
            return x[Index] == y[Index];
        }

        /// <summary>
        ///   Returns a hash code for a given instance.
        /// </summary>
        /// 
        /// <param name="obj">The instance.</param>
        /// 
        /// <returns>
        ///   A hash code for the instance instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public int GetHashCode(double[] obj)
        {
            return obj[Index].GetHashCode();
        }

    }
}
