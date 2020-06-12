using Converter.util;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Converter_UI
{
    public static class EnviStyles
    {
        public static Brush AlpineBrush { get; } = new SolidColorBrush(Color.FromRgb(0x00, 0xBA, 0xFF));
        public static Brush SpeedBrush  { get; } = new SolidColorBrush(Color.FromRgb(0xF2, 0xA6, 0x44));
        public static Brush RallyBrush  { get; } = new SolidColorBrush(Color.FromRgb(0x00, 0xC2, 0x0E));
        public static Brush BayBrush    { get; } = new SolidColorBrush(Color.FromRgb(0x48, 0x68, 0x72));
        public static Brush CoastBrush  { get; } = new SolidColorBrush(Color.FromRgb(0xE5, 0x00, 0x00));
        public static Brush IslandBrush { get; } = new SolidColorBrush(Color.FromRgb(0xB3, 0x3C, 0x8D));

        /*
        public static BitmapSource AlpineIcon { get; }
        public static BitmapSource SpeedIcon  { get; }
        public static BitmapSource RallyIcon  { get; }
        public static BitmapSource BayIcon    { get; }
        public static BitmapSource CoastIcon  { get; }
        public static BitmapSource IslandIcon { get; }

        public static BitmapSource AlpineBanner { get; }
        public static BitmapSource SpeedBanner  { get; }
        public static BitmapSource RallyBanner  { get; }
        public static BitmapSource BayBanner    { get; }
        public static BitmapSource CoastBanner  { get; }
        public static BitmapSource IslandBanner { get; }

        static EnviStyles()
        {
            var alpineIcon = new BitmapImage();
            alpineIcon.BeginInit();
            alpineIcon.UriSource = new Uri("Image/Alpine.png", UriKind.Relative);
            alpineIcon.EndInit();
            AlpineIcon = alpineIcon;
        }
        */

        public static Brush GetColor(this MapEnvironment envi)
        {
            return envi switch
            {
                MapEnvironment.Alpine => AlpineBrush,
                MapEnvironment.Speed  => SpeedBrush,
                MapEnvironment.Rally  => RallyBrush,
                MapEnvironment.Bay    => BayBrush,
                MapEnvironment.Coast  => CoastBrush,
                MapEnvironment.Island => IslandBrush,
                _ => throw new Exception()
            };
        }

        /*
        public static ImageSource GetIcon(this MapEnvironment envi)
        {
            return envi switch
            {
                MapEnvironment.Alpine => AlpineIcon,
                MapEnvironment.Speed  => SpeedIcon,
                MapEnvironment.Rally  => RallyIcon,
                MapEnvironment.Bay    => BayIcon,
                MapEnvironment.Coast  => CoastIcon,
                MapEnvironment.Island => IslandIcon,
                _ => throw new Exception()
            };
        }

        public static ImageSource GetBanner(this MapEnvironment envi)
        {
            return envi switch
            {
                MapEnvironment.Alpine => AlpineBanner,
                MapEnvironment.Speed  => SpeedBanner,
                MapEnvironment.Rally  => RallyBanner,
                MapEnvironment.Bay    => BayBanner,
                MapEnvironment.Coast  => CoastBanner,
                MapEnvironment.Island => IslandBanner,
                _ => throw new Exception()
            };
        }
        */
    }
}
