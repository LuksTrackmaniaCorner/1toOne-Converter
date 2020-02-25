using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx.core.primitives
{
    public class GBXFileRef : Structure
    {
        public readonly string versionKey = "Version";
        public readonly string checksumKey = "Checksum";
        public readonly string filePathKey = "FilePath";
        public readonly string locatorUrlKey = "Locator URL";

        public GBXByte version;
        public Unread checksum;
        public GBXString filePath;
        public GBXString locatorUrl;

        public GBXFileRef(Stream s)
        {
            version = new GBXByte(s);

            if(version.Value>= 3)
            {
                checksum = new Unread(s, 32);
            }

            filePath = new GBXString(s);

            //TODO Condition not 100% clear.
            if(version.Value>= 1 && (filePath.Value.Length != 0 || version.Value>= 3))
            {
                locatorUrl = new GBXString(s);
            }
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(versionKey, version);
            result.AddChild(checksumKey, checksum);
            result.AddChild(filePathKey, filePath);
            result.AddChild(locatorUrlKey, locatorUrl);
            return result;
        }

        public void Clear()
        {
            version.Value = 2;
            checksum = null;
            filePath.Value = "";
            locatorUrl = null;

            MarkAsChanged();
        }
    }
}
