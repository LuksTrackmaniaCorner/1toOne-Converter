using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1toOne_Converter.src
{
    public interface IDumpable
    {
        LinkedList<string> Dump();
    }

    public static class DumpableExtensions
    {
        public static void DumpToConsole(this IDumpable dumpable)
        {
            foreach (var currentString in dumpable.Dump()) {
                Console.WriteLine(currentString);
            }
        }

        public static void DumpToFile(this IDumpable dumpable, System.IO.StreamWriter file)
        {
            foreach (var currentString in dumpable.Dump())
            {
                file.WriteLine(currentString);
            }
        }
    }
}
