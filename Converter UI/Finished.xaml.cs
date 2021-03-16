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
    public partial class Finished : Window
    {
        public string SuccessMessage { get; }
        public string ErrorMessage { get; }

        public Finished(uint successCount, uint errorCount)
        {
            SuccessMessage = "Converted " + FormatMapCount(successCount);
            if (errorCount > 0)
                ErrorMessage = "Failed to convert " + FormatMapCount(errorCount) + ": See logs";
            else
                ErrorMessage = "No Errors occured";

            InitializeComponent();
        }

        private static string FormatMapCount(uint count)
        {
            if (count == 1)
                return $"{count} map";
            else
                return $"{count} maps";
        }

        private void _closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
