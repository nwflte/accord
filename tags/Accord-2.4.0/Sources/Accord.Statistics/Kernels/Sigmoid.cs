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

namespace Accord.Statistics.Kernels
{
    using System;

    /// <summary>
    ///   Sigmoid Kernel.
    /// </summary>
    /// 
    /// <remarks>
    ///   Sigmoid kernel of the form k(x,z) = tanh(a * x'z + c). Sigmoid kernels are only
    ///   conditionally positive definite for some values of a and c, and therefore may not
    ///   induce a reproducing kernel Hilbert space. However, they have been successfully 
    ///   used in practice (Scholkopf and Smola, 2002).
    /// </remarks>
    /// 
    [Serializable]
    public sealed class Sigmoid : IKernel
    {
        private double alpha;
        private double constant;

        /// <summary>
        ///   Constructs a Sigmoid kernel.
        /// </summary>
        /// 
        public Sigmoid()
            : this(2.0, -0.5) { }

        /// <summary>
        ///   Constructs a Sigmoid kernel.
        /// </summary>
        /// 
        /// <param name="alpha">
        ///   Alpha parameter. Typically should be set to
        ///   a positive value. Default is 2.</param>
        /// <param name="constant">
        ///   Constant parameter. Typically should be set to
        ///   a negative value. Default is -0.5.</param>
        /// 
        public Sigmoid(double alpha, double constant)
        {
            this.alpha = alpha;
            this.constant = constant;
        }

        /// <summary>
        ///   Gets or sets the kernel's alpha parameter.
        /// </summary>
        /// 
        /// <remarks>
        ///   In a sigmoid kernel, alpha is a inner product
        ///   coefficient for the hyperbolic tangent function.
        /// </remarks>
        /// 
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        ///   Gets or sets the kernel's constant term.
        /// </summary>
        /// 
        public double Constant
        {
            get { return constant; }
            set { constant = value; }
        }

        /// <summary>
        ///   Sigmoid kernel function.
        /// </summary>
        /// 
        /// <param name="x">Vector <c>x</c> in input space.</param>
        /// <param name="y">Vector <c>y</c> in input space.</param>
        /// <returns>Dot product in feature (kernel) space.</returns>
        /// 
        public double Function(double[] x, double[] y)
        {
            double sum = 0.0;
            for (int i = 0; i < x.Length; i++)
                sum += x[i] * y[i];

            return Math.Tanh(alpha * sum + constant);
        }

    }
}
