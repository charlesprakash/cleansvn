using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CleanSvn
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootFolder = string.Empty;

            if (args.Length == 0)
            {
                Console.Write("Root folder: ");
                rootFolder = Console.ReadLine();
            }
            else
            {
                rootFolder = args[0];
            }

            rootFolder = string.IsNullOrEmpty(rootFolder) ? string.Empty : rootFolder.Trim();

            if (!Directory.Exists(rootFolder))
            {
                Console.WriteLine("Folder {0} doesn't exist", rootFolder);
                return;
            }

            RemoveSvnFolder(rootFolder);
        }

        static void RemoveSvnFolder(string parentFolder)
        {
            string svnFolder = Path.Combine(parentFolder, ".svn");
            if (Directory.Exists(svnFolder))
            {
                Console.WriteLine(svnFolder);
                ForceDeleteFolder(svnFolder);
            }

            foreach (var child in Directory.GetDirectories(parentFolder))
            {
                RemoveSvnFolder(child);
            }
        }

        static void ForceDeleteFolder(string svnFolder)
        {
            foreach (string file in Directory.GetFiles(svnFolder, "*", SearchOption.AllDirectories))
            {
                var longFileName = @"\\?\" + file;
                var attributesSet = SetFileAttributes(longFileName, (uint)0x20);  // Set ARCHIVE only attribute
                var deleted = DeleteFile(longFileName);
                Console.WriteLine("{0} - {1}", file, attributesSet && deleted);
            }
            Directory.Delete(svnFolder, true);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool DeleteFile(string lpszLongPath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool SetFileAttributes(string lpFileName, uint dwFileAttributes);
    }
}
