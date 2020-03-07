using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src
{
    public class IOManager : IDisposable
    {
        private readonly object _ioLock = new object();
        private readonly object _explorerLock = new object();

        private bool _logLoaded;
        private TextWriter _log;
        private readonly int _nrOfFiles;
        private int _currentFileNr;
        private bool _keepConsoleOpen;
        private readonly HashSet<String> _outputFolders;

        public IOManager(int nrOfFiles)
        {
            _nrOfFiles = nrOfFiles;
            _outputFolders = new HashSet<string>();

            var settings = Settings.GetSettings();
            if (settings.DisplayMode == DisplayMode.Full)
                _keepConsoleOpen = true;

#if DEBUG
            _keepConsoleOpen = true;
#endif
        }

        private void WriteToLog(string message)
        {
            LoadLog();

            if (_log == null)
                return;

            _log.WriteLine(message);
        }

        private void LoadLog()
        {
            if (_logLoaded)
                return;

            var settings = Settings.GetSettings();
            if (settings.LogMode != DisplayMode.None)
            {
                _log = new StreamWriter(settings.LogFilePath, settings.AppendToLog);
                _log.WriteLine();
                _log.WriteLine("==========   " + DateTime.Now + "   ==========");
                _log.WriteLine();
            }
            _logLoaded = true;
        }

        private void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string fileName, string errorMessage)
        {
            lock (_ioLock)
            {
                _currentFileNr++;
                var message = _currentFileNr + " / " + _nrOfFiles + " failed: " + fileName;
                WriteToConsole(message);
                WriteToConsole(errorMessage);
                WriteToLog(message);
                WriteToLog(errorMessage);
                _keepConsoleOpen = true;
            }
        }

        public void Success(string fileName, string outputPath, string statistics)
        {
            var settings = Settings.GetSettings();

            lock (_ioLock)
            {
                _currentFileNr++;
                var message = _currentFileNr + " / " + _nrOfFiles + " converted: " + fileName;
                switch (settings.DisplayMode)
                {
                    case DisplayMode.TracksOnly:
                        WriteToConsole(message);
                        break;
                    case DisplayMode.Full:
                        WriteToConsole(message);
                        WriteToConsole(statistics);
                        break;
                }

                switch (settings.LogMode)
                {
                    case DisplayMode.TracksOnly:
                        WriteToLog(message);
                        break;
                    case DisplayMode.Full:
                        WriteToLog(message);
                        WriteToLog(statistics);
                        break;
                }
            }

            lock(_explorerLock)
            {
                if(!_outputFolders.Contains(outputPath))
                {
                    _outputFolders.Add(outputPath);
                }
            }
        }

        public void KeepConsoleOpen()
        {
            if (Settings.GetSettings().DisplayMode == DisplayMode.Full)
            {
                Console.WriteLine("Remember to calculate shadows for the converted maps!");
            }

            if (_keepConsoleOpen)
            {
                Console.ReadKey();
            }
        }

        public void OpenFolders()
        {
            if (!Settings.GetSettings().OpenFolderAfterFinished)
                return;

            lock(_explorerLock)
            {
                foreach(var outputFolder in _outputFolders)
                {
                    Process.Start(outputFolder);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                //dispose managed state (managed objects).
                _log?.Dispose();
            }

            disposedValue = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
