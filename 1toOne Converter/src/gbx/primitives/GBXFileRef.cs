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
        public readonly string locatorURLKey = "Locator URL";

        public GBXFileRef(Stream s)
        {
            var version = new GBXByte(s);
            AddChildDeprevated(versionKey, version);

            if(version.Value>= 3)
            {
                var checksum = new Unread(s, 32);
                AddChildDeprevated(checksumKey, checksum);
            }

            var filePath = new GBXString(s);
            AddChildDeprevated(filePathKey, filePath);

            //TODO Condition not 100% clear.
            if(version.Value>= 1 && (filePath.Value.Length != 0 || version.Value>= 3))
            {
                var locatorURL = new GBXString(s);
                AddChildDeprevated(locatorURLKey, locatorURL);
            }
        }
    }
}
