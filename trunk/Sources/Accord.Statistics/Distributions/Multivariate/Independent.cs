// Accord Statistics Library
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

namespace Accord.Statistics.Distributions.Multivariate
{
    using System;
    using Accord.Math;
    using Accord.Statistics.Distributions.Fitting;

    /// <summary>
    ///   Independent joint distribution.
    /// </summary>
    /// 
    [Serializable]
    public class Independent : Independent<IUnivariateDistribution>
    {

        /// <summary>
        ///   Initializes a new instance of the <see cref="Independent&lt;TDistribution&gt;"/> class.
        /// </summary>
        /// 
        /// <param name="components">The components.</param>
        /// 
        public Independent(params IUnivariateDistribution[] components)
            : base(components) { }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public override object Clone()
        {
            var clone = new IUnivariateDistribution[Components.Length];
            for (int i = 0; i < clone.Length; i++)
                clone[i] = (IUnivariateDistribution)Components[i].Clone();

            return new Independent(clone);
        }

    }
}
