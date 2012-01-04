// Accord Statistics Library
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

namespace Accord.Statistics.Models.Fields.Functions
{
    using System;
    using Accord.Statistics.Models.Fields.Features;

    /// <summary>
    ///   Base implementation for <see cref="IPotentialFunction{T}">potential functions</see>.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the observations modeled.</typeparam>
    /// 
    [Serializable]
    public abstract class BasePotentialFunction<T>
    {

        /// <summary>
        ///   Gets the factor potentials (also known as clique potentials) 
        ///   functions composing this potential function.
        /// </summary>
        /// 
        public FactorPotential<T>[] Factors { get; protected set; }

        /// <summary>
        ///   Gets the number of output classes assumed by this function.
        /// </summary>
        /// 
        public int Outputs { get; protected set; }

        /// <summary>
        ///   Gets the number of symbols assumed by this function.
        /// </summary>
        /// 
        public int Symbols { get; protected set; }

        /// <summary>
        ///   Gets or sets the set of weights for each feature function.
        /// </summary>
        /// 
        /// <value>The weights for each of the feature functions.</value>
        /// 
        public double[] Weights { get; set; }

        /// <summary>
        /// Gets the feature functions composing this potential function.
        /// </summary>
        /// 
        public IFeature<T>[] Features { get; protected set; }


    }
}
