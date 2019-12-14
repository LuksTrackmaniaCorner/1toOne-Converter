using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _1toOne_Converter.Compression;

namespace _1toOne_Converter_Tests
{
    [TestClass]
    public class LZOTests
    {
        [TestMethod]
        public void TestCompressionAndDecompression()
        {
            LZO LZO = new LZO();

            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < 1000; i++)
                SB.Append("Test-Data");

            byte[] Source = Encoding.Default.GetBytes(SB.ToString());
            Console.WriteLine("Uncompressed Length: " + Source.Length + " Bytes");

            byte[] Compressed = LZO.Compress(Source);
            Console.WriteLine("Compressed Length: " + Compressed.Length + " Bytes");
            Assert.IsTrue(Compressed.Length < Source.Length, "Compressed Data is larger than uncompressed Data");

            byte[] Decompressed = LZO.Decompress(Compressed, Source.Length);
            CollectionAssert.AreEqual(Source, Decompressed, "Decompressed Data is not Equal to Original Data");
        }

        [TestMethod]
        public void TestVersions()
        {
            string Version = LZO.Version;
            Console.WriteLine(Version);
            double VersionNumber = double.Parse(Version, System.Globalization.CultureInfo.InvariantCulture);
            Assert.IsTrue(0 < VersionNumber && VersionNumber < 5, Version + " / " + VersionNumber);

            Console.WriteLine(LZO.VersionDate);
            Assert.IsNotNull(LZO.VersionDate);
        }
    }
}
