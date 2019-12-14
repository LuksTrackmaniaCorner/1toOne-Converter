using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.chunks
{
    public class BlockSkin03059002 : Chunk
    {

        public static readonly string unknownKey = "Unknown";
        public static readonly string packDescKey = "Pack Desc";
        public static readonly string parentPackDescKey = "Parent Pack Desc";

        public readonly GBXString unknown;
        public readonly GBXFileRef packDesc;
        public readonly GBXFileRef parentPackDesc;

        public BlockSkin03059002(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            unknown = new GBXString(s);
            AddChildDeprecated(unknownKey, unknown);

            packDesc = new GBXFileRef(s);
            AddChildDeprecated(packDescKey, packDesc);

            parentPackDesc = new GBXFileRef(s);
            AddChildDeprecated(parentPackDescKey, parentPackDesc);
        }
    }
}
