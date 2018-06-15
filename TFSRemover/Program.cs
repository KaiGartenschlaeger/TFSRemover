using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFSRemover
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Syntax: TFSRemover.exe <root directory of project>");
                return 1;
            }
            else if (!Directory.Exists(args[0]))
            {
                Console.WriteLine($"The directory \"{args[0]}\" does not exists.");
                return 1;
            }

            // todo: process projects

            return 0;
        }
    }
}
