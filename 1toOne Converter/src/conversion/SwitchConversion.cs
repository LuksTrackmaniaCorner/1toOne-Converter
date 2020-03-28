using _1toOne_Converter.src.gbx;
using _1toOne_Converter.src.gbx.core;
using _1toOne_Converter.src.gbx.core.chunks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _1toOne_Converter.src.conversion
{
    [XmlRoot]
    public class SwitchConversion : Conversion
    {
        [XmlElement(ElementName = "Case")]
        public Case[] Cases { get; set; }

        private readonly Dictionary<string, Conversion> _conversions;

        private SwitchConversion()
        {
            _conversions = new Dictionary<string, Conversion>();
        }

        public override void Convert(GBXFile file)
        {
            var commonChunk = (ChallengeCommon)file.GetChunk(Chunk.challengeCommonKey);

            Case currentCase = null;
            foreach (var @case in Cases)
            {
                if (@case.Environment == commonChunk.TrackMeta.Collection.Content)
                {
                    currentCase = @case;
                    break;
                }
            }

            if (currentCase == null)
                throw new UnknownConversionException();

            EnsureIsLoaded(currentCase);

            _conversions[currentCase.Environment].Convert(file);
        }

        private void EnsureIsLoaded(Case currentCase)
        {
            var envi = currentCase.Environment;

            if (!_conversions.ContainsKey(envi))
            {
                lock (currentCase)
                {
                    if (!_conversions.ContainsKey(envi))
                    {
                        _conversions[envi] = Conversion.LoadConversion<ComplexConversion>(currentCase.Conversion);
                    }
                }
            }
        }
    }

    public class Case
    {
        [XmlAttribute]
        public string Environment { get; set; }
        [XmlAttribute]
        public string Conversion { get; set; }
    }
}
