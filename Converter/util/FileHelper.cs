using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Converter.util
{
    public static class FileHelper
    {
        public static string ProgramPath { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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
