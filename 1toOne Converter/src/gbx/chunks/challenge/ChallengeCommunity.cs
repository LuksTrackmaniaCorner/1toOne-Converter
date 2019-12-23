using _1toOne_Converter.src.gbx.core.primitives;
using _1toOne_Converter.src.gbx.primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.gbx.core.chunks
{
    public class ChallengeCommunity : Chunk
    {
        public static readonly string xmlKey = "XML";

        public GBXXml<MapCommunityRoot> CommunityXml;

        public ChallengeCommunity(Stream s, GBXLBSContext context, GBXNodeRefList list) : base(context, list)
        {
            CommunityXml = new GBXXml<MapCommunityRoot>(s);
        }

        public override List<NamedChild> GenerateChildren()
        {
            var result = new List<NamedChild>();
            result.AddChild(xmlKey, CommunityXml);
            return result;
        }
    }

    /* --- Courtesy of Solux --- */
    [XmlRoot(ElementName = "header")]
    public class MapCommunityRoot
    {
        [XmlElement(ElementName = "ident")]
        public Identity Identity { get; set; }
        [XmlElement(ElementName = "desc")]
        public Description Description { get; set; }
        [XmlElement(ElementName = "playermodel")]
        public PlayerModel PlayerModel { get; set; }
        [XmlElement(ElementName = "times")]
        public Times Times { get; set; }
        [XmlElement(ElementName = "deps")]
        public Dependencies Dependencies { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "exever")]
        public string ExecutableVersion { get; set; }
        [XmlAttribute(AttributeName = "exebuild")]
        public string ExecutableBuildDate { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "lightmap")]
        public int Lightmap { get; set; }

        public bool ShouldSerializeLightmap() => Lightmap != 0;
    }

    [XmlRoot(ElementName = "ident")]
    public class Identity
    {
        [XmlAttribute(AttributeName = "uid")]
        public string Uid { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "author")]
        public string Author { get; set; }
        [XmlAttribute(AttributeName = "authorzone")]
        public string AuthorZone { get; set; }
    }

    [XmlRoot(ElementName = "desc")]
    public class Description
    {
        [XmlAttribute(AttributeName = "envir")]
        public string Environment { get; set; }
        [XmlAttribute(AttributeName = "mood")]
        public string Mood { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "maptype")]
        public string MapType { get; set; }
        [XmlAttribute(AttributeName = "mapstyle")]
        public string MapStyle { get; set; }
        [XmlAttribute(AttributeName = "validated")]
        //[Obsolete("Use the property Validated instead of ValidatedB.", false)] //The obsolete-attribute prevents the xml parser to parse
        public string ValidatedB { get; set; }
        [XmlIgnore()]
        public bool Validated { get => this.ValidatedB == "1"; }
        [XmlAttribute(AttributeName = "nblaps")]
        public int LapCount { get; set; }

        [XmlAttribute(AttributeName = "price")]
        public int Price { get; set; }

        public bool ShouldSerializePrice => Price != 0;

        [XmlAttribute(AttributeName = "displaycost")]
        public int DisplayCost { get; set; }

        public bool ShouldSerializeDisplayCost() => DisplayCost != 0;

        [XmlAttribute(AttributeName = "mod")]
        public string Mod { get; set; }
        [XmlAttribute(AttributeName = "hasghostblocks")]
        //[Obsolete("Use the property HasGhostBlocks instead of HasGhostBlocksB.", false)] //The obsolete-attribute prevents the xml parser to parse
        public string HasGhostBlocksB { get; set; }
        [XmlIgnore()]
        public bool HasGhostBlocks { get => this.HasGhostBlocksB == "1"; }
    }

    //Todo: Find out 
    [XmlRoot(ElementName = "playermodel")]
    public class PlayerModel
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "times")]
    public class Times
    {
        [XmlAttribute(AttributeName = "bronze")]
        public int Bronze { get; set; }

        public bool ShouldSerializeBronze() => Bronze != 0;

        [XmlIgnore()]
        public TimeSpan BronzeTimeSpan { get => TimeSpan.FromMilliseconds(this.Bronze); }
        [XmlAttribute(AttributeName = "silver")]
        public int Silver { get; set; }

        public bool ShouldSerializeSilver() => Bronze != 0;

        [XmlIgnore()]
        public TimeSpan SilverTimeSpan { get => TimeSpan.FromMilliseconds(this.Silver); }
        [XmlAttribute(AttributeName = "gold")]
        public int Gold { get; set; }

        public bool ShouldSerializeGold() => Bronze != 0;

        [XmlIgnore()]
        public TimeSpan GoldTimeSpan { get => TimeSpan.FromMilliseconds(this.Gold); }
        [XmlAttribute(AttributeName = "authortime")]
        public int AuthorTime { get; set; }

        public bool ShouldSerializeAuthorTime() => Bronze != 0;

        [XmlIgnore()]
        public TimeSpan AuthorTimeSpan { get => TimeSpan.FromMilliseconds(this.AuthorTime); }
        [XmlAttribute(AttributeName = "authorscore")]
        public int AuthorScore { get; set; }

        public bool ShouldSerializeAuthorScore() => AuthorScore != 0;
    }

    [XmlRoot(ElementName = "deps")]
    public class Dependencies
    {
        [XmlElement(ElementName = "dep")]
        public List<Dependency> Deps { get; set; }
    }

    [XmlRoot(ElementName = "dep")]
    public class Dependency
    {
        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
    }
}
