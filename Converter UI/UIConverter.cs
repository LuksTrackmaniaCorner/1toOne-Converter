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
using Converter.Converion;
using Converter.Gbx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Converter;

namespace Converter_UI
{
    public class UIConverter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private GBXFile _file { get; }
        private string InputFilePath { get; }

        private ConverterStatus _status;
        public ConverterStatus Status
        {
            get => _status;
            private set
            {
                if (value == ConverterStatus.MapSaved)
                {
                    StatusImagePath = "./Image/Status/Checkmark.png";
                }
                else if (value == ConverterStatus.ErrorLoading ||
                         value == ConverterStatus.ErrorConverting ||
                         value == ConverterStatus.ErrorSaving)
                {
                    StatusImagePath = "./Image/Status/Cross.png";
                }
                else
                {
                    StatusImagePath = null;
                }

                _status = value;
            }
        }
        protected Exception Error { get; private set; }

        public MapEnvironment? Envi { get; }
        public string MapName { get; }
        public string Author { get; }
        public string AuthorTime { get; }
        public string Mood { get; }
        public MapEnvironment? CarEnvi { get; }

        public Brush Color { get; }
        public string IconPath { get; }
        public string BannerPath { get; }
        private string _statusImagePath;
        public string StatusImagePath
        {
            get => _statusImagePath;
            private set {
                _statusImagePath = value;
                OnPropertyChanged();
            }
        }
        public BitmapSource Thumbnail { get; }


        public UIConverter(string inputFilePath)
        {
            InputFilePath = inputFilePath;

            try
            {
                using var fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _file = new GBXFile(fs);
                Status = ConverterStatus.MapLoaded;
            }
            catch (Exception e)
            {
                Status = ConverterStatus.ErrorLoading;
                Error = e;
            }

            var descChunk = (ChallengeDesc)_file.GetChunk(Chunk.challengeDescKey);
            var commonChunk = (ChallengeCommon)_file.GetChunk(Chunk.challengeCommonKey);
            var thumbnailChunk = (ChallengeThumbnail)_file.GetChunk(Chunk.challengeThumbnailKey);
            var carChunk = (Challenge0304300D)_file.GetChunk(Chunk.challenge0304300DKey);
            var chunk0304301F = (Challenge0304301F)_file.GetChunk(Chunk.challenge0304301FKey);

            
            if(commonChunk.TrackMeta.Collection.Content is string enviContent
                    && Enum.TryParse<MapEnvironment>(enviContent, out MapEnvironment enviResult))
            {
                // We have a valid Envi!
                Envi = enviResult;
                IconPath = $"./Image/Icons/Icon{enviResult}.png";
                BannerPath = $"./Image/Banner/Banner{enviResult}.png";
                Color = enviResult.GetColor();
            }
            else
            {
                Status = ConverterStatus.ErrorLoading;
                Error = new Exception("The envi is not supported by the converter.");
            }
            MapName = commonChunk.TrackName.Value.RemoveTrackmaniaFormat();
            Author = commonChunk.TrackMeta.Author.Content.RemoveTrackmaniaFormat();
            var authorTime = descChunk.AuthorTime.Value;
            if (authorTime == uint.MaxValue)
                AuthorTime = "Not validated";
            else
            {
                var timeSpan = TimeSpan.FromMilliseconds(descChunk.AuthorTime.Value);
                if (timeSpan.TotalHours < 1)
                    AuthorTime = string.Format("{0:mm\\:ss\\.fff}", TimeSpan.FromMilliseconds(descChunk.AuthorTime.Value));
                else
                    AuthorTime = string.Format("{0:hh\\:mm\\:ss\\.fff}", TimeSpan.FromMilliseconds(descChunk.AuthorTime.Value));
            }
            //TODO better stuff
            Mood = chunk0304301F?.DecorationMeta.ID.Content;
            if (carChunk != null && carChunk.Meta.ID.Content is string carContent &&
                    Enum.TryParse<EnvironmentCar>(carContent, out EnvironmentCar carResult))
                CarEnvi = carResult.GetEnvironment();
            else
                CarEnvi = Envi;

            if (thumbnailChunk != null)
            {
                Thumbnail = (BitmapSource)new ImageSourceConverter().ConvertFrom(thumbnailChunk.thumb.Value);
            }
        }

        public bool WriteBack()
        {
            if (Status != ConverterStatus.MapConverted && Status != ConverterStatus.MapLoaded)
                return false;

            try
            {
                var fileName = Path.GetFileName(InputFilePath);
                var outputName = fileName.Replace(".Challenge.Gbx", ".Map.Gbx");
                var filePath = Path.GetDirectoryName(InputFilePath);
                var outputPath = GetOutputPath(filePath);
                var outputMapFile = Path.Combine(outputPath, outputName);

                var createFileMode = Converter.Settings.GetSettings().OverwriteExistingFiles ? FileMode.Create : FileMode.CreateNew;
                using var fs = new FileStream(outputMapFile, createFileMode, FileAccess.Write);
                _file.WriteBack(fs);
                Status = ConverterStatus.MapSaved;
            }
            catch(Exception e)
            {
                Status = ConverterStatus.ErrorSaving;
                Error = e;
            }

            return true;
        }

        public bool Convert(Conversion conversion)
        {
            if (Status != ConverterStatus.MapLoaded)
                return false;

            try
            {
                conversion.Convert(_file);
                Status = ConverterStatus.MapConverted;
            }
            catch(Exception e)
            {
                Status = ConverterStatus.ErrorConverting;
                Error = e;
            }

            return true;
        }

        public string GetStatistics()
        {
            return _file.GetStatistics();
        }

        public string GetOutputPath(string mapPath)
        {
            return Converter.Settings.GetSettings().GetOutputPath(_file, mapPath);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public enum ConverterStatus
    {
        NotInitialized = 0,
        MapLoaded,
        MapConverted,
        MapSaved,
        ErrorLoading,
        ErrorConverting,
        ErrorSaving
    }
}
