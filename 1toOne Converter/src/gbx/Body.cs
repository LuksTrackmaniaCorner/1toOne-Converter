using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx
{
    public class Body : Structure
    {
        public static readonly string uncompressedSizeKey = "Uncompressed Size";
        public static readonly string compressedSizeKey = "Compressed Size";
        public static readonly string mainNodeKey = "Main";

        public readonly GBXUInt uncompressedSize;
        public readonly GBXUInt compressedSize;
        public readonly MainNode mainNode;


        public Body(Stream fs, MainNode mainNode)
        {
            this.mainNode = mainNode;
             
            mainNode.ReadBodyChunks(fs);
        }

        public override void WriteBack(Stream s)
        {
            mainNode.WriteBodyChunk(s);
        }
    }
}
