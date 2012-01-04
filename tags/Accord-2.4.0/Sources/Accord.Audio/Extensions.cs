// Accord (Experimental) Audio Library
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

namespace Accord.Audio
{
    using System;
    using System.Runtime.InteropServices;
	
	/// <summary>
	///   Useful extension methods
	/// </summary>
    /// 
	public static class Extensions
	{
		/// <summary>
		///  Serializes (converts) any object to a byte array.
		/// </summary>
        /// 
        /// <param name="value">The object to be serialized.</param>
		/// <returns>The byte array containing the serialized object.</returns>
        /// 
		public static byte[] ToByteArray<T>(this T value) where T : struct
		{
			int rawsize = Marshal.SizeOf(value);
			byte[] rawdata = new byte[rawsize];
			GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
			IntPtr buffer = handle.AddrOfPinnedObject();
			Marshal.StructureToPtr(value, buffer, false);
			handle.Free();
			return rawdata;
		}
		
		/// <summary>
		///   Deserializes (converts) a byte array to a given structure type.
		/// </summary>
        /// 
		/// <remarks>
		///  This is a potentiality unsafe operation.
		/// </remarks>
        /// 
		/// <param name="rawData">The byte array containing the serialized object.</param>
		/// <returns>The object stored in the byte array.</returns>
        /// 
		public static T RawDeserialize<T>(this byte[] rawData)
		{
			return RawDeserialize<T>(rawData, 0);
		}

        /// <summary>
        ///   Deserializes (converts) a byte array to a given structure type.
        /// </summary>
        /// 
        /// <remarks>
        ///  This is a potentiality unsafe operation.
        /// </remarks>
        /// 
        /// <param name="rawData">The byte array containing the serialized object.</param>
        /// <param name="position">The starting position in the rawData array where the object is located.</param>
        /// <returns>The object stored in the byte array.</returns>
        /// 
		public static T RawDeserialize<T>(this byte[] rawData, int position)
		{
            Type type = typeof(T);

			int rawsize = Marshal.SizeOf(type);

            if (rawsize > (rawData.Length - position))
            {
                throw new ArgumentException("The given array is smaller than the object size.");
            }

			IntPtr buffer = Marshal.AllocHGlobal(rawsize);
			Marshal.Copy(rawData, position, buffer, rawsize);
			T obj = (T)Marshal.PtrToStructure(buffer, type);
			Marshal.FreeHGlobal(buffer);
			return obj;
		}
	}
}
