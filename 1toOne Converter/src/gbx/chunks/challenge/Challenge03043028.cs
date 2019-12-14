using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;
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

        private GBXBool archiveGmCamVal;
        private GBXByte @byte;
        private GBXMat3 snapshotRotation;
        private GBXVec3 snapshotPosition;
        private GBXFloat float1; //-1
        private GBXFloat float2; //-1
        private GBXFloat float3; //0
        private GBXString comments;

        public GBXBool ArchiveGmCamVal { get => archiveGmCamVal; set { archiveGmCamVal = value; AddChildNew(value); } }
        public GBXByte Byte { get => @byte; set { @byte = value; AddChildNew(value); } }
        public GBXMat3 SnapshotRotation { get => snapshotRotation; set { snapshotRotation = value; AddChildNew(value); } }
        public GBXVec3 SnapshotPosition { get => snapshotPosition; set { snapshotPosition = value; AddChildNew(value); } }
        public GBXFloat Float1 { get => float1; set { float1 = value; AddChildNew(value); } }
        public GBXFloat Float2 { get => float2; set { float2 = value; AddChildNew(value); } }
        public GBXFloat Float3 { get => float3; set { float3 = value; AddChildNew(value); } }
        public GBXString Comments { get => comments; set { comments = value; AddChildNew(value); } }

        public Challenge03043028(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            ArchiveGmCamVal = new GBXBool(s);

            if(ArchiveGmCamVal.Value == true)
            {
                @byte = new GBXByte(s);
                snapshotRotation = new GBXMat3(s);
                snapshotPosition = new GBXVec3(s);
                float1 = new GBXFloat(s);
                float2 = new GBXFloat(s);
                float3 = new GBXFloat(s);
            }

            Comments = new GBXString(s);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(archiveGmCamValKey, ArchiveGmCamVal);
            result.AddChild("Byte", @byte);
            result.AddChild("Matrix", SnapshotRotation);
            result.AddChild("Vec", SnapshotPosition);
            result.AddChild("Float 1", Float1);
            result.AddChild("Float 2", Float2);
            result.AddChild("Float 3", Float3);
            result.AddChild("Comments", Comments);
            return result;
        }
    }
}
