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
            //Header Chunks
            yield return new GbxChunkInfo(0x002, "Description", true, false, (x) => new ChallengeDescription(x));
            yield return new GbxChunkInfo(0x003, "Common", true, false, (x) => new ChallengeCommon(x));
            yield return new GbxChunkInfo(0x004, "Version", true, false, (x) => new ChallengeVersion(x));
            yield return new GbxChunkInfo(0x005, "Community", true, true, (x) => new ChallengeCommunity(x));
            //TODO add thumbnail true true
            yield return new GbxChunkInfo(0x008, "Author", true, false, (x) => new ChallengeAuthor(x));

            //Body Chunks
            yield return new GbxChunkInfo(0x00D, "Vehicle", false, false, (x) => new ChallengeVehicle(x));
            yield return new GbxChunkInfo(0x011, "Parameter", false, false, (x) => new ChallengeParameter(x));
            //TODO add 0x13
            yield return new GbxChunkInfo(0x014, "Old Password", false, true, (x) => new ChallengePasswordOld(x));
            yield return new GbxChunkInfo(0x017, "Checkpoints", false, true, (x) => new ChallengeCheckpoints(x));
            yield return new GbxChunkInfo(0x018, "Lap Count", false, true, (x) => new ChallengeLapCount(x));
            yield return new GbxChunkInfo(0x019, "Modpack", false, true, (x) => new ChallengeModpack(x));
            yield return new GbxChunkInfo(0x01C, "Playmode", false, true, (x) => new ChallengePlaymode(x));
            yield return new GbxChunkInfo(0x01F, "Blocks", false, false, (x) => new ChallengeBlocks(x));
            yield return new GbxChunkInfo(0x021, "Mediatracker", false, false, (x) => new ChallengeMediatracker(x));
            yield return new GbxChunkInfo(0x022, "Unknown 022", false, false, (x) => new ChallengeUnknown022(x));
            yield return new GbxChunkInfo(0x024, "Custom Music", false, false, (x) => new ChallengeCustomMusic(x));
            yield return new GbxChunkInfo(0x025, "Map Coordinates", false, false, (x) => new ChallengeMapCoordinates(x));
            yield return new GbxChunkInfo(0x026, "Global Clip", false, false, (x) => new ChallengeGlobalClip(x));
            yield return new GbxChunkInfo(0x028, "Thumbnail Camera", false, false, (x) => new ChallengeThumnailCamera(x));
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
