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

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}
