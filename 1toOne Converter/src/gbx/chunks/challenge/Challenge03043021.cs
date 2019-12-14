using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class Challenge03043021 : Chunk
    {
        public static readonly string clipIntroKey = "Clip Intro";
        public static readonly string clipGroupIngameKey = "Clip Group Ingame";
        public static readonly string clipGroupEndRaceKey = "Clip Group Endrace";

        public readonly GBXNodeRef clipIntro;
        public readonly GBXNodeRef clipGroupIngame;
        public readonly GBXNodeRef clipGroupEndRace;

        public Challenge03043021(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            clipIntro = list.ReadGBXNodeRef(s, context);
            AddChildDeprevated(clipIntroKey, clipIntro);

            clipGroupIngame = list.ReadGBXNodeRef(s, context);
            AddChildDeprevated(clipGroupIngameKey, clipGroupIngame);

            clipGroupEndRace = list.ReadGBXNodeRef(s, context);
            AddChildDeprevated(clipGroupEndRaceKey, clipGroupEndRace);
        }
    }
}
