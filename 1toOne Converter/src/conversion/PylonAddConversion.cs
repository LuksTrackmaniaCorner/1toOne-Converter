using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.chunks;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;
using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;
using _1toOne_Converter.src.util;

namespace _1toOne_Converter.src.conversion
{
    public class PylonAddConversion : Conversion
    {
        public GBXLBS Collection;
        public GBXLBS DefaultAuthor;

        public GBXLBS SinglePylon; //Left One
        public GBXLBS DoublePylon;
        public GBXLBS ForcedPylon;

        public FlagName HeightFlag;
        public FlagName ItemCountStatistic;

        public override void Convert(GBXFile file)
        {
            int itemCount = 0;

            var itemChunk = (Challenge03043040)file.GetChunk(Chunk.challenge03043040Key);

            var pylonMap = new SortedSet<Pylon>[GBXFile.MaxMapXSize + 1, GBXFile.MaxMapZSize + 1, 4];

            foreach(var pylon in file.GetPylons(PylonType.Forced))
            {
                var pylonField = pylonMap[pylon.X, pylon.Z, pylon.Rot] ??= new SortedSet<Pylon>(Pylon.GetComparer());
                pylonField.Add(pylon);
            }

            foreach (var pylon in file.GetPylons(PylonType.Top))
            {
                var pylonField = pylonMap[pylon.X, pylon.Z, pylon.NormalizedRot] ??= new SortedSet<Pylon>(Pylon.GetComparer());
                pylonField.Add(pylon);
            }

            foreach (var pylon in file.GetPylons(PylonType.Bottom))
            {
                var pylonField = pylonMap[pylon.X, pylon.Z, pylon.NormalizedRot];
                pylonField?.Add(pylon);
            }

            foreach (var pylon in file.GetPylons(PylonType.Prevent))
            {
                var pylonField = pylonMap[pylon.X, pylon.Z, pylon.NormalizedRot];
                pylonField?.Add(pylon);
            }

            foreach(var pylonField in pylonMap)
            {
                if (pylonField == null)
                    goto nextPylon;

                bool forced = false; //Forced pillar has already been placed
                byte? topLeft = null, topRight = null;

                foreach(var pylonInfo in pylonField)
                {
                    switch (pylonInfo.Type, pylonInfo.Pos)
                    {
                        case (PylonType.Prevent, PylonPosition.Both):
                            topLeft = topRight = null;
                            break;
                        case (PylonType.Prevent, PylonPosition.Left):
                            topLeft = null;
                            break;
                        case (PylonType.Prevent, PylonPosition.Right):
                            topLeft = null;
                            break;
                        case (PylonType.Top, PylonPosition.Both):
                            topLeft ??= (byte)pylonInfo.Y;
                            topRight ??= (byte)pylonInfo.Y;
                            break;
                        case (PylonType.Top, PylonPosition.Left):
                            topLeft ??= (byte)pylonInfo.Y;
                            break;
                        case (PylonType.Top, PylonPosition.Right):
                            topRight ??= (byte)pylonInfo.Y;
                            break;
                        case (PylonType.Bottom, PylonPosition.Both):
                            itemCount += PlaceBoth(file, pylonInfo, topLeft, topRight, itemChunk);
                            goto nextPylon;
                        case (PylonType.Bottom, PylonPosition.Left):
                            itemCount += PlaceLeft(file, pylonInfo, topLeft, itemChunk);
                            goto nextPylon;
                        case (PylonType.Bottom, PylonPosition.Right):
                            itemCount += PlaceRight(file, pylonInfo, topRight, itemChunk);
                            goto nextPylon;
                        case (PylonType.Forced, _):
                            if (!forced)
                            {
                                itemCount += PlaceForced(file, pylonInfo, itemChunk);
                                forced = true;
                            }
                            break;
                    }
                }

            nextPylon:;
            }

            //add statistic
            if (ItemCountStatistic != null)
            {
                file.AddStatistic(ItemCountStatistic.Name, itemCount);
            }
        }

        private int PlaceForced(GBXFile file, Pylon pylon, Challenge03043040 itemChunk)
        {

            var coords = ((byte) pylon.X, (byte) pylon.Z);
            var (x, z) = coords;
            var top = (byte) pylon.Y;
            var bottom = file.GetFlag(HeightFlag.Name, x, z);

            return PlaceForcedPylons(file, coords, (top, bottom), pylon.Rot, PylonPosition.Both, itemChunk);
        }

        public int PlaceBoth(GBXFile file, Pylon bottom, byte? topLeft, byte? topRight, Challenge03043040 itemChunk)
        {
            int itemCount = 0;

            var pos = ((byte) bottom.X, (byte)bottom.Z);
            var rot = bottom.Rot;

            switch(topLeft, topRight)
            {
                case (byte topL, byte topR):
                    if (topL == topR)
                    {
                        itemCount += PlacePylons(file, pos, (topL, (byte)bottom.Y), rot, PylonPosition.Both, itemChunk);
                    }
                    else if(topL > topR)
                    {
                        itemCount += PlacePylons(file, pos, (topL, topR), rot, PylonPosition.Left, itemChunk);
                        itemCount += PlacePylons(file, pos, (topR, (byte)bottom.Y), rot, PylonPosition.Both, itemChunk);
                    }
                    else
                    {
                        itemCount += PlacePylons(file, pos, (topR, topL), rot, PylonPosition.Right, itemChunk);
                        itemCount += PlacePylons(file, pos, (topL, (byte)bottom.Y), rot, PylonPosition.Both, itemChunk);
                    }
                    break;
                case (byte topL, null):
                    itemCount += PlacePylons(file, pos, (topL, (byte)bottom.Y), rot, PylonPosition.Left, itemChunk);
                    break;
                case (null, byte topR):
                    itemCount += PlacePylons(file, pos, (topR, (byte)bottom.Y), rot, PylonPosition.Right, itemChunk);
                    break;
            }

            return itemCount;
        }

        internal int PlaceRight(GBXFile file, Pylon bottom, byte? topRight, Challenge03043040 itemChunk)
        {
            var pos = ((byte)bottom.X, (byte)bottom.Z);
            var rot = bottom.Rot;

            if (topRight is byte top)
            {
                return PlacePylons(file, pos, (top, (byte)bottom.Y), rot, PylonPosition.Right, itemChunk);
            }

            return 0;
        }

        internal int PlaceLeft(GBXFile file, Pylon bottom, byte? topLeft, Challenge03043040 itemChunk)
        {
            var pos = ((byte) bottom.X, (byte) bottom.Z);
            var rot = bottom.Rot;

            if (topLeft is byte top)
            {
                return PlacePylons(file, pos, (top, (byte) bottom.Y), rot, PylonPosition.Left, itemChunk);
            }

            return 0;
        }

        private int PlacePylons(GBXFile file, (byte x, byte z) pos, (byte top, byte bottom) y, byte rot, PylonPosition pylonPos, Challenge03043040 itemChunk)
        {
            var itemName = pylonPos == PylonPosition.Both ? DoublePylon : SinglePylon;

            for (byte i = y.bottom; i < y.top; i++)
            {
                //Place Pylon
                itemChunk.AddItem(
                    new Meta((GBXLBS)itemName.DeepClone(), (GBXLBS)Collection.DeepClone(), (GBXLBS)DefaultAuthor.DeepClone()),
                    ConvertRot(rot, pylonPos),
                    new GBXByte3(((byte)pos.x, (byte)i, (byte)pos.z)),
                    file.ConvertPylonCoords((pos.x, i, pos.z), rot)
                );
            }

            return y.top - y.bottom;
        }

        private int PlaceForcedPylons(GBXFile file, (byte x, byte z) pos, (byte top, byte bottom) y, byte rot, PylonPosition pylonPos, Challenge03043040 itemChunk)
        {
            GBXLBS itemName = ForcedPylon;

            for (byte i = y.bottom; i < y.top; i++)
            {
                //Place Pylon
                itemChunk.AddItem(
                    new Meta((GBXLBS)itemName.DeepClone(), (GBXLBS)Collection.DeepClone(), (GBXLBS)DefaultAuthor.DeepClone()),
                    ConvertRot(rot, pylonPos),
                    new GBXByte3(((byte)pos.x, (byte)i, (byte)pos.z)),
                    file.ConvertCoords((pos.x, i, pos.z)) //Not ConvertPylonCoords
                );
            }

            return y.top - y.bottom;
        }

        private static GBXVec3 ConvertRot(byte rot, PylonPosition pylonPos)
        {
            if (pylonPos == PylonPosition.Left)
            {
                rot += 2;
                rot %= 4;
            }
            return GBXFile.ConvertRot(rot);
        }
    }
}
