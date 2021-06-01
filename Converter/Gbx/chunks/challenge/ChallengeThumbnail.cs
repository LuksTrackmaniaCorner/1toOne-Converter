using Converter.Gbx.Core;
using Converter.Gbx.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Gbx.Chunks.Challenge
{
    public class ChallengeThumbnail : Chunk
    {
        public static readonly string versionKey = "Version";
        public static readonly string thumbSizeKey = "Thumbnail Size";
        public static readonly string thumbBeginKey = "Thumbnail Begins";
        public static readonly string thumbKey = "Thumbnail";
        public static readonly string thumbEndKey = " Thumbnail Ends";
        public static readonly string commentBeginKey = "Comment Begins";
        public static readonly string commentKey = "Comment";
        public static readonly string commentEndKey = "Comment Ends";

        public readonly GBXUInt version;
        public readonly GBXUInt thumbSize;
        public readonly GBXFixedLengthString thumbBegin;
        public readonly Unread thumb;
        public readonly GBXFixedLengthString thumbEnd;
        public readonly GBXFixedLengthString commentBegin;
        public readonly GBXString comment;
        public readonly GBXFixedLengthString commentEnd;

        public ChallengeThumbnail(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            version = new GBXUInt(s);
            AddChildDeprecated(versionKey, version);

            if (version.Value == 0)
                return;
            //version > 0
            thumbSize = new GBXUInt(s);
            AddChildDeprecated(thumbSizeKey, thumbSize);

            thumbBegin = new GBXFixedLengthString(s, 15);
            AddChildDeprecated(thumbBeginKey, thumbBegin);

            thumb = new Unread(s, (int)thumbSize.Value);
            AddChildDeprecated(thumbKey, thumb);

            thumbEnd = new GBXFixedLengthString(s, 16);
            AddChildDeprecated(thumbEndKey, thumbEnd);

            commentBegin = new GBXFixedLengthString(s, 10);
            AddChildDeprecated(commentBeginKey, commentBegin);

            comment = new GBXString(s);
            AddChildDeprecated(commentKey, comment);

            commentEnd = new GBXFixedLengthString(s, 11);
            AddChildDeprecated(commentEndKey, commentEnd);
        }
    }
}
