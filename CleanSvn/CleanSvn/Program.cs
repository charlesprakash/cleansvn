///////////////////////////////////////////////////////////////////////////////
// Copyright © Charles Prakash Dasari. 2014. All rights reserved.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CleanSvn
{
    // Imagine you have moved out of SVN to either a different source control
    // system or some other mechanism of managing your files. You will have an
    // annoying .svn file in every folder that you have ever created to the
    // deepest nesting level possible. This program tries to to clean up that
    // folder as you don't have any use for it anymore.
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

            try
            {
                foreach (var child in Directory.GetDirectories(parentFolder))
                {
                    RemoveSvnFolder(child);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Could not access the folder {0}", parentFolder);
            }
        }

        static void ForceDeleteFolder(string svnFolder)
        {
            try
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
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Could not access folder {0}", svnFolder);
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool DeleteFile(string lpszLongPath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool SetFileAttributes(string lpFileName, uint dwFileAttributes);
    }
}
