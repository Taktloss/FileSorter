using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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

                Console.WriteLine("Argument missing.");
                return;
            }
                
            if (args[0] == "-register")
            {
                Console.WriteLine("Adding FileSorter to context menu.");
                //Check if Registry entry already exists
                RegistryKey check = Registry.CurrentUser.OpenSubKey(@"Folder\shell\FileSorter");
                if(check != null)
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

                return;
            }

            string directory = args[0];

            Console.WriteLine("Working Directory: {0}", directory);
            Console.WriteLine();

            //Create File Type List
            List<FileType> fileTypesList = CreateFileTypeList();

            //Get all files in Directory
            string[] fileList = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            foreach (string file in fileList)
            {
                string dirName = CheckExtension(fileTypesList, file);
                if (dirName != null)
                {
                    string tempPath = string.Format("{0}\\{1}\\{2}", directory, dirName, file.Substring(file.LastIndexOf('\\') + 1));
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
            string[] videoFiles = { "mp4", "avi", "mkv", "mov", "wmv", "divx", "oga", "oga" };
            string[] audioFiles = { "ogg", "oga", "mp3", "wav", "flac" };
            string[] applicationFiles = { "exe", "msi" };
            string[] documentFiles = { "txt", "doc", "docx", "pdf", "xls", "odt", "ott", "oth", "odm", "csv", "rtf", "odp", "pptx" };
            string[] imageFiles = { "tif", "jpg", "bmp", "png", "dds", "psd", "gif", "tga", "sgv" };
            string[] discImageFiles = { "iso", "bin", "cue" };

            FileType Video = new FileType("Video", videoFiles);
            FileType Audio = new FileType("Audio", audioFiles);
            FileType Apps = new FileType("Apps", applicationFiles);
            FileType Documents = new FileType("Documents", documentFiles);
            FileType Images = new FileType("Images", imageFiles);
            FileType DiscImages = new FileType("DiscImages", discImageFiles);

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
            foreach(FileType fileType in fileTypesList)
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
            //if (File.Exists(destinationPath))
            //File.Delete(destinationPath);
            if (!File.Exists(destinationPath))
            {
                // Create Directory and Move the file.
                Directory.CreateDirectory(destinationPath.Substring(0, destinationPath.LastIndexOf('\\')));
                File.Move(path, destinationPath);
                Console.WriteLine("{0} was moved to {1}.", path.Substring(path.LastIndexOf('\\') + 1), destinationPath);
            }
        }

    }
}
