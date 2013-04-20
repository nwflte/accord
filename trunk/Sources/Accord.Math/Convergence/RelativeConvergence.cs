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

namespace Accord.Math
{
    using System;
    using System.Linq;

    /// <summary>
    ///   Absolute convergence criteria.
    /// </summary>
    /// 
    public class RelativeConvergence
    {

        private double tolerance = 0;
        private int maxIterations = 100;
        private double newValue;


        /// <summary>
        ///   Gets or sets the maximum change in the watched value
        ///   after an iteration of the algorithm used to detect convergence.
        /// </summary>
        /// 
        public double Tolerance
        {
            get { return tolerance; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Tolerance should be positive.");

                tolerance = value;
            }
        }

        /// <summary>
        ///   Gets or sets the maximum number of iterations
        ///   performed by the iterative algorithm.
        /// </summary>
        /// 
        public int Iterations
        {
            get { return maxIterations; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "The maximum number of iterations should be positive.");

                maxIterations = value;
            }
        }

        /// <summary>
        ///   Gets or sets the watched value before the iteration.
        /// </summary>
        /// 
        public double OldValue { get; private set; }

        /// <summary>
        ///   Gets or sets the watched value after the iteration.
        /// </summary>
        /// 
        public double NewValue
        {
            get { return newValue; }
            set
            {
                OldValue = newValue;
                newValue = value;
                CurrentIteration++;
            }
        }

        /// <summary>
        ///   Gets or sets the current iteration number.
        /// </summary>
        /// 
        public int CurrentIteration { get; private set; }

        /// <summary>
        ///   Gets whether the algorithm has converged.
        /// </summary>
        /// 
        public bool HasConverged
        {
            get
            {
                // Update and verify stop criteria
                if (tolerance > 0)
                {
                    // Stopping criteria is likelihood convergence
                    double delta = Math.Abs(OldValue - NewValue);

                    if (delta <= tolerance * OldValue)
                        return true;

                    if (maxIterations > 0)
                    {
                        // Maximum iterations should also be respected
                        if (CurrentIteration >= maxIterations)
                            return true;
                    }
                }
                else
                {
                    // Stopping criteria is number of iterations
                    if (CurrentIteration >= maxIterations)
                        return true;
                }

                // Check if we have reached an invalid or perfectly separable answer
                if (Double.IsNaN(NewValue) || Double.IsInfinity(NewValue))
                {
                    return true;
                }

                return false;
            }
        }


        /// <summary>
        ///   Clears this instance.
        /// </summary>
        /// 
        public void Clear()
        {
            CurrentIteration = 0;
            NewValue = 0;
            OldValue = 0;
        }
    }
}
