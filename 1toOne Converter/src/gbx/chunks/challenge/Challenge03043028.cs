using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge03043028 : Chunk
    {
        public static readonly string archiveGmCamValKey = "Archive GM Cam Value";
        public static readonly string commentsKey = "Comments";

        public readonly GBXBool archiveGmCamVal;
        public readonly GBXString comments;

        public Challenge03043028(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            archiveGmCamVal = new GBXBool(s);
            AddChildDeprevated(archiveGmCamValKey, archiveGmCamVal);

            Trace.Assert(archiveGmCamVal.Value== false, "This chunk variant is not implemented");

            comments = new GBXString(s);
            AddChildDeprevated(commentsKey, comments);
        }
    }
}
