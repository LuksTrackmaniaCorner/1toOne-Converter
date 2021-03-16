using Converter;
using Converter.Converion;
using Converter.util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
            var successCount = 0u;
            var errorCount = 0u;

            foreach(var converter in Converters)
            {
                if (converter.Status == ConverterStatus.MapSaved)
                    continue;

                if (converter.Convert(_conversion) && converter.WriteBack())
                {
                    successCount++;
                }
                else
                {
                    errorCount++;
                }
            }

            new Finished(successCount, errorCount).ShowDialog();
        }
    }
}
