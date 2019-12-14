using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src.util
{
    public static class FileHelper
    {
        public static string GetFilePath(string extension)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = extension;
            fileDialog.Filter = extension.ToUpper() + " Files |*" + extension;
            fileDialog.Multiselect = false;

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            else
            {
                return null;
            }
        }

        public static string[] GetFilePaths(string extension)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = extension;
            fileDialog.Filter = extension.ToUpper() + " Files |*" + extension;
            fileDialog.Multiselect = true;

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileNames;
            }
            else
            {
                return null;
            }
        }
    }
}
