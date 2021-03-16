﻿using Converter.Gbx;
using Converter.Gbx.chunks;
using Converter.Gbx.core;
using Converter.Gbx.core.chunks;
using Converter.Gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Converion
{
    public class TitleConversion : Conversion
    {
        public GBXLBS TitleUID;

        public override void Convert(GBXFile file)
        {
            var commonChunk = (ChallengeCommon)file.GetChunk(Chunk.challengeCommonKey);
            commonChunk.UpdateToVersion11();
            commonChunk.TitleUID = (GBXLBS) TitleUID.DeepClone();

            var communityChunk = (ChallengeCommunity)file.GetChunk(Chunk.challengeCommunityKey);

            if(communityChunk != null)
                communityChunk.CommunityXml.Xml.Title = TitleUID.Content;

            var titlepackChunk = (Challenge03043051)file.GetChunk(Chunk.challenge03043051Key);

            if(titlepackChunk == null)
            {
                titlepackChunk = new Challenge03043051(false);
                file.AddBodyChunk(Chunk.challenge03043051Key, titlepackChunk);
            }

            titlepackChunk.TitlePack.Content = TitleUID.Content;
        }
    }
}