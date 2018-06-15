using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TFSRemover
{
    class Program
    {
        static void FixSolutionFile(FileInfo file)
        {
            file.Attributes = file.Attributes & ~FileAttributes.ReadOnly;

            var content = File.ReadAllText(file.FullName);

            var regex = new Regex(@"GlobalSection\(TeamFoundationVersionControl\).+EndGlobalSection", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var match = regex.Match(content);
            if (match.Success)
            {
                content = content.Remove(match.Index, match.Length);
                File.WriteAllText(file.FullName, content);
            }
        }

        static void FixProjectFile(FileInfo file)
        {
            file.Attributes = file.Attributes & ~FileAttributes.ReadOnly;

            var originalContent = File.ReadAllText(file.FullName);

            var content = originalContent;
            content = content.Replace("<SccProjectName>SAK</SccProjectName>", string.Empty);
            content = content.Replace("<SccLocalPath>SAK</SccLocalPath>", string.Empty);
            content = content.Replace("<SccAuxPath>SAK</SccAuxPath>", string.Empty);
            content = content.Replace("<SccProvider>SAK</SccProvider>", string.Empty);

            if (content != originalContent)
            {
                File.WriteAllText(file.FullName, content);
            }
        }

        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Syntax: TFSRemover.exe <root directory of project>");
                return 1;
            }

            var directory = new DirectoryInfo(args[0]);

            if (!directory.Exists)
            {
                Console.WriteLine($"The directory \"{args[0]}\" does not exists.");
                return 1;
            }

            // process solution files
            foreach (var file in directory.GetFiles("*.sln", SearchOption.AllDirectories))
            {
                Console.WriteLine("Process solution file \"{0}\"..", file.FullName);
                FixSolutionFile(file);
            }

            // process project files
            foreach (var file in directory.GetFiles("*.csproj", SearchOption.AllDirectories))
            {
                Console.WriteLine("Process project file \"{0}\"..", file.FullName);
                FixProjectFile(file);
            }

            // delete tfs files
            foreach (var file in directory.GetFiles("*.*", SearchOption.AllDirectories))
            {
                if (file.Extension == ".vssscc" ||
                    file.Extension == ".vspscc")
                {
                    Console.WriteLine("Delete tfs file \"{0}\"..", file.FullName);

                    file.Attributes = file.Attributes & ~FileAttributes.ReadOnly;
                    file.Delete();
                }
            }

            return 0;
        }
    }
}
