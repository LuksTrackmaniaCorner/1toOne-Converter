using Converter.Gbx.core;
using Converter.util;
using Converter.Gbx.core.chunks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using Converter.Gbx.chunks;

namespace Converter_UI
{
    public class UIConverter : Converter.Converter
    {
        public MapEnvironment Envi { get; }
        public string MapName { get; }
        public string Author { get; }
        public TimeSpan AuthorTime { get; }
        public string Mood { get; }
        public MapEnvironment CarEnvi { get; }

        public Brush Color { get; }
        public string IconPath { get; }
        public string BannerPath { get; }
        public BitmapSource Thumbnail { get; }

        public UIConverter(string inputFilePath) : base(inputFilePath)
        {
            var descChunk = (ChallengeDesc)_file.GetChunk(Chunk.challengeDescKey);
            var commonChunk = (ChallengeCommon)_file.GetChunk(Chunk.challengeCommonKey);
            var thumbnailChunk = (ChallengeThumbnail)_file.GetChunk(Chunk.challengeThumbnailKey);
            var carChunk = (Challenge0304300D)_file.GetChunk(Chunk.challenge0304300DKey);

            Envi = Enum.Parse<MapEnvironment>(commonChunk.TrackMeta.Collection.Content);
            MapName = commonChunk.TrackName.Value.RemoveTrackmaniaFormat();
            Author = commonChunk.TrackMeta.Author.Content.RemoveTrackmaniaFormat();
            AuthorTime = TimeSpan.FromMilliseconds(descChunk.AuthorTime.Value);
            //TODO
            Mood = "TODO";
            if (carChunk != null && carChunk.Meta.ID.Content is string content)
                CarEnvi = Enum.Parse<EnvironmentCar>(content).GetEnvironment();
            else
                CarEnvi = Envi;

            IconPath = $"./Image/{Envi}.png";
            BannerPath = $"./Image/Banner/Banner{Envi}.png";
            Color = Envi.GetColor();
            if(thumbnailChunk != null)
            {
                Thumbnail = (BitmapSource)new ImageSourceConverter().ConvertFrom(thumbnailChunk.thumb.Value);
            }
        }
    }
}
