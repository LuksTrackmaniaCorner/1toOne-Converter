using Converter;
using Converter.util;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _loadButton_Click(object sender, RoutedEventArgs e)
        {
            var files = FileHelper.GetFilePaths(".gbx");

            if (files == null)
                return;

            foreach (var file in files)
            {
                _converterList.Items.Add(new Converter.Converter(file));
            }
        }

        private void _clearButton_Click(object sender, RoutedEventArgs e)
        {
            _converterList.Items.Clear();
        }

        private void _settingsButton_Click(object sender, RoutedEventArgs e)
        {
            new Settings().Show();
        }
    }
}
