using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.gbx
{
    public class RefTable : Structure
    {
        public static readonly string numExternalNodesKey = "Number of External Nodes";
        public static readonly string ancestorLevelKey = "Ancestor Level";
        public static readonly string numSubFoldersKey = "Number of SubFolders";
        public static readonly string subFoldersKey = "SubFolders";

        public RefTable(Stream fs)
        {
            var numExternalNodes = new GBXUInt(fs);
            AddChildDeprevated(numExternalNodesKey, numExternalNodes);

            if (numExternalNodes.Value== 0) {
                return;
            }

            //numExternalNodes >= 1
            var ancestorLevel = new GBXUInt(fs);
            AddChildDeprevated(ancestorLevelKey, ancestorLevel);

            var numSubFolders = new GBXUInt(fs);
            AddChildDeprevated(numSubFoldersKey, numSubFolders);

            var subFolders = new Array<Folder>(numSubFolders.Value, () => new Folder(fs));
            AddChildDeprevated(subFoldersKey, subFolders);
        }
    }

    public class Folder : Structure
    {
        public static readonly string nameKey = "Name";
        public static readonly string numSubFoldersKey = "Number of SubFolders";
        public static readonly string subFoldersKey = "SubFolders";

        public Folder(Stream fs)
        {
            var name = new GBXString(fs);
            AddChildDeprevated(nameKey, name);

            var numSubFolders = new GBXUInt(fs);
            AddChildDeprevated(numSubFoldersKey, numSubFolders);

            var subFolders = new Array<Folder>(numSubFolders.Value, () => new Folder(fs));
            AddChildDeprevated(subFoldersKey, subFolders);
        }
    }
}
