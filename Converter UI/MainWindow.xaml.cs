using Converter;
using Converter.Conversion;
using Converter.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#pragma warning disable IDE1006 // Naming Styles
namespace Converter_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<UIConverter> Converters { get; } = new ObservableCollection<UIConverter>();
        private readonly Conversion _conversion;

        public MainWindow()
        {
            InitializeComponent();

            const string DefaultXmlFilePath = @"Default.xml";
            Directory.SetCurrentDirectory(FileHelper.ProgramPath);
            var xmlFile = DefaultXmlFilePath;
            _conversion = Conversion.LoadConversion<SwitchConversion>(xmlFile);
        }

        private void _loadButton_Click(object sender, RoutedEventArgs e)
        {
            var files = FileHelper.GetFilePaths(".gbx");

            if (files == null)
                return;

            foreach (var file in files)
            {
                Converters.Add(new UIConverter(file));
            }
        }

        private void _clearButton_Click(object sender, RoutedEventArgs e)
        {
            Converters.Clear();
        }

        private void _settingsButton_Click(object sender, RoutedEventArgs e)
        {
            new Settings().Show();
        }

        private void _convertButton_Click(object sender, RoutedEventArgs e)
        {
            var successCount = 0;
            var errorCount = 0;

            var conversionTasks = from converter in Converters
                                  where converter.Status != ConverterStatus.MapSaved
                                 select Task.Run(() =>
                                 {
                                     if (converter.Convert(_conversion) && converter.WriteBack())
                                     {
                                         Interlocked.Increment(ref successCount);
                                     }
                                     else
                                     {
                                         Interlocked.Increment(ref errorCount);
                                     }
                                 });

            Task.WaitAll(conversionTasks.ToArray());

            new Finished(successCount, errorCount).ShowDialog();
        }
    }
}
