using Gbx.Chunks.Challenge;
using Gbx.Parser.Info;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gbx.Chunks.Loaders
{
    public class ChallengeLoader : Loader
    {
        protected override void LoadNeededClasses()
        {
            
        }

        protected override IEnumerable<GbxChunkInfo> CreateChunkInfos()
        {
            yield return new GbxChunkInfo(0x002, "Description", true, false, (x) => new ChallengeDescription(x));
            yield return new GbxChunkInfo(0x003, "Common", true, false, (x) => new ChallengeCommon(x));
            yield return new GbxChunkInfo(0x004, "Version", true, false, (x) => new ChallengeVersion(x));
            yield return new GbxChunkInfo(0x005, "Community", true, true, (x) => new ChallengeCommunity(x));
            //TODO add thumbnail true true
            yield return new GbxChunkInfo(0x008, "Author", true, false, (x) => new ChallengeAuthor(x));
            yield return new GbxChunkInfo(0x00D, "Vehicle", false, false, (x) => new ChallengeVehicle(x));
            yield return new GbxChunkInfo(0x011, "Parameter", false, false, (x) => new ChallengeParameter(x));

            yield return new GbxChunkInfo(0x024, "Custom Music", false, false, (x) => new ChallengeCustomMusic(x));
            yield return new GbxChunkInfo(0x026, "Global Clip", false, false, (x) => new ChallengeGlobalClip(x));
        }

        protected override GbxClassInfo CreateClassInfo()
        {
            return new GbxClassInfo(0x03043000, "Challenge");
        }

        protected override IEnumerable<uint> GetAliases()
        {
            yield return 0x24003000;
        }
    }
}
