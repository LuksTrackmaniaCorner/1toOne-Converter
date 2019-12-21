using Gbx.Chunks.Challenge;
using Gbx.Parser.info;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Loaders
{
    public class ChallengeLoader : ILoader
    {
        public void Load()
        {
            var challengeInfo = new GbxClassInfo(0x03043000, "Challenge");

            challengeInfo.Add(new GbxChunkInfo(0x002, "Description", true, false, (x) => new ChallengeDescription(x)));
            challengeInfo.Add(new GbxChunkInfo(0x003, "Common", true, false, (x) => new ChallengeCommon(x)));
            challengeInfo.Add(new GbxChunkInfo(0x004, "Version", true, false, (x) => new ChallengeVersion(x)));
            challengeInfo.Add(new GbxChunkInfo(0x005, "Community", true, true, (x) => new ChallengeCommunity(x)));
            //TODO add thumbnail true true
            challengeInfo.Add(new GbxChunkInfo(0x008, "Author", true, false, (x) => new ChallengeAuthor(x)));
            challengeInfo.Add(new GbxChunkInfo(0x00D, "Vehicle", false, false, (x) => new ChallengeVehicle(x)));
            challengeInfo.Add(new GbxChunkInfo(0x011, "Parameter", false, false, (x) => new ChallengeParameter(x)));

            challengeInfo.Add(new GbxChunkInfo(0x024, "Custom Music", false, false, (x) => new ChallengeCustomMusic(x)));
            challengeInfo.Add(new GbxChunkInfo(0x026, "Global Clip", false, false, (x) => new ChallengeGlobalClip(x)));

            GbxInfo.Add(challengeInfo);
            GbxInfo.AddAlias(challengeInfo, 0x24003000);
        }
    }
}
