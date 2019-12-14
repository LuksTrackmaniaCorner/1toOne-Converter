using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.core.primitives;

namespace _1toOne_Converter.src.conversion
{
    public class GridDefineConversion : Conversion
    {
        public GBXVec3 GridSize;
        public GBXVec3 GridOffset;

        public override void Convert(GBXFile file)
        {
            file.GridSize = (GBXVec3)this.GridSize.DeepClone();
            file.GridOffset = (GBXVec3)this.GridOffset.DeepClone();
        }
    }
}
