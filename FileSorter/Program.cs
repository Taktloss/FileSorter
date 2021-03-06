﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify a directory");
                Console.WriteLine("Usage: FileSorter [DIRECTORY] [OPTIONS]");
                Console.WriteLine("Options: -r  scan sub directories too");

                Console.WriteLine("Optional use FileSorter -register to add the application to your context menu.");
                Console.WriteLine("You need to have Admin access!!!");
                Console.ReadKey();
                return;
            }
                
            if (args[0] == "-register")
            {
                Console.WriteLine("Adding FileSorter to context menu.");
                AddContextMenu();
                return;
            }

            string directory = args[0];
            Console.WriteLine("Working Directory: {0}", directory);

            //Create File Type List
            List<FileType> fileTypesList = CreateFileTypeList();

            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            if (args.Length == 2 && args[1] == "-r")
            {
                searchOption = SearchOption.AllDirectories;
            }
            
            //Get all files in Directory
            string[] fileList = Directory.GetFiles(directory, "*.*", searchOption);
            foreach (string file in fileList)
            {

                string dirName = CheckExtension(fileTypesList, file);
                if (dirName != null)
                {
                    string tempPath = $"{directory}\\{dirName}\\{file.Substring(file.LastIndexOf('\\') + 1)}";
                    MoveFile(file, tempPath);
                }
                else
                {
                    Console.WriteLine("Unknown File Type: {0}", file);
                }
            }
            
            Console.ReadKey();
        }

        private static List<FileType> CreateFileTypeList()
        {         
            FileType Video = new FileType("Video", new string[] { "mp4", "avi", "mkv", "mov", "wmv", "divx", "oga", "oga" });
            FileType Audio = new FileType("Audio", new string[] { "ogg", "oga", "mp3", "wav", "flac" });
            FileType Apps = new FileType("Apps", new string[] { "exe", "msi" });
            FileType Documents = new FileType("Documents", new string[] { "txt", "doc", "docx", "pdf", "xls", "odt", "ott", "oth", "odm", "csv", "rtf", "odp", "pptx" });
            FileType Images = new FileType("Images", new string[] { "tif", "jpg", "bmp", "png", "dds", "psd", "gif", "tga", "sgv", "pdn" });
            FileType DiscImages = new FileType("DiscImages", new string[] { "iso", "bin", "cue" });

            List<FileType> fileTypesList = new List<FileType>
            {
                Video,
                Audio,
                Apps,
                Documents,
                Images,
                DiscImages
            };
            return fileTypesList;
        }

        static string CheckExtension(List<FileType> fileTypesList, string file)
        {
            string fileExt = file.Substring(file.LastIndexOf('.') + 1);
            foreach (FileType fileType in fileTypesList)
            {
                if (fileType.Extensions.Contains(fileExt))
                {
                    return fileType.Name;
                }
            }
            return null;
        }

        static void MoveFile(string path, string destinationPath)
        {
            // Ensure that the target does not exist.
            if (!File.Exists(destinationPath))
            {
                // Create Directory and Move the file.
                Directory.CreateDirectory(destinationPath.Substring(0, destinationPath.LastIndexOf('\\')));
                File.Move(path, destinationPath);
                Console.WriteLine("{0} was moved to {1}.", path.Substring(path.LastIndexOf('\\') + 1), destinationPath);
            }
            else
            {
                Console.WriteLine("File already exists.{0}",path);
            }
        }

        static void AddContextMenu()
        {
            //Check if Registry entry already exists
            RegistryKey check = Registry.CurrentUser.OpenSubKey(@"Folder\shell\FileSorter");
            if (check != null)
            {
                Console.WriteLine("Already added to context menu.");
                return;
            }
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;//Environment.GetCommandLineArgs()[0];
            RegistryKey registryKey;
            registryKey = Registry.ClassesRoot.CreateSubKey(@"Folder\shell\FileSorter");
            registryKey = Registry.ClassesRoot.CreateSubKey(@"Folder\shell\FileSorter\command");

            //To get the location the assembly normally resides on disk or the install directory
            registryKey.SetValue("", path + " %1");
        }

    }
}
