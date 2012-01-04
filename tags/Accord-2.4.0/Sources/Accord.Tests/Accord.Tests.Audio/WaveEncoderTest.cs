// Accord Unit Tests
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

namespace Accord.Tests.Audio
{
    using Accord.DirectSound;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Audio;
    using Accord.Audio.Formats;
    using System.IO;

    [TestClass()]
    public class WaveEncoderTest
    {

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void WaveEncoderConstructorTest()
        {
            UnmanagedMemoryStream sourceStream = Properties.Resources.Grand_Piano___Fazioli___major_A_middle;
            MemoryStream destStream = new MemoryStream();

            WaveDecoder decoder = new WaveDecoder(sourceStream);
            WaveEncoder encoder = new WaveEncoder(destStream);


            Signal wave = decoder.Decode();

            encoder.Encode(wave);

            sourceStream.Seek(0, SeekOrigin.Begin);
            destStream.Seek(0, SeekOrigin.Begin);

            WaveDecoder decoder2 = new WaveDecoder(destStream);

            Signal wave2 = decoder2.Decode();


            Assert.AreEqual(wave.Length, wave2.Length);
            Assert.AreEqual(wave.SampleFormat, wave2.SampleFormat);
            Assert.AreEqual(wave.SampleRate, wave2.SampleRate);
            Assert.AreEqual(wave.Samples, wave2.Samples);

            for (int i = 0; i < wave.RawData.Length; i++)
            {
                byte actual = wave.RawData[i];
                byte expected = wave2.RawData[i];
                Assert.AreEqual(expected, actual);
            }

        }

    }
}
