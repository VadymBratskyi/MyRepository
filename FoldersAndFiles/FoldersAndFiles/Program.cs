using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldersAndFiles
{
    class Program
    {
        public static DriveInfo[] dr = DriveInfo.GetDrives();
        public static Dictionary<string,string> disk = new Dictionary<string, string>();

        public static string namefolder;
        public static string namefile;

        static void Main(string[] args)
        {

            MainMenu();
           
            Console.ReadKey();
        }

        public static void MainMenu()
        {
            GetDisks();
            var menu = new Dictionary<int, string>()
            {
                [1] = "Work with Folders",
                [2] = "Work with Files"
            };

            Console.ForegroundColor = ConsoleColor.Green;

            foreach (var m in menu)
            {
                Console.WriteLine(m);
            }

            PrintRed("Select one of them: ");
            while (true)
            {
                var select = Console.ReadLine();
                switch (select)
                {
                    case "1":
                        Console.Clear();
                        GetDisks();
                        FolderMenu();
                        return;
                    case "2":
                        Console.Clear();
                        GetDisks();
                        FileMenu();
                        return;
                    default:
                        PrintRed("Error!!!");
                        break;
                }

            }
        }

        public static void FolderMenu()
        {
            WorkFolder fold = new WorkFolder();

            var menuFO = new Dictionary<int, string>()
            {
                [1] = "Create Folder",
                [2] = "Delete Folder",
                [3] = "Move Folder",
                [4] = "Get all Folder in",
                [5] = "Go back"
            };

            Console.ForegroundColor = ConsoleColor.Green;

            foreach (var m in menuFO)
            {
                Console.WriteLine(m);
            }

            
            while (true)
            {
                PrintRed("Select one of them: ");
                var select = Console.ReadLine();

                switch (select)
                {
                    case "1":
                        fold.CreateDirectoryInfo(PathDisk(fold));
                        break;
                    case "2":
                        fold.DeleteDrectory(PathDisk(fold));
                        break;
                    case "3":
                        fold.MoveDirectory(PathDisk(fold));
                        break;
                    case "4":
                        fold.GetDirectories(PathDisk(fold));
                        break;
                    case "5":
                        Console.Clear();
                        MainMenu();
                        return;
                    default:
                        PrintRed("Error!!");
                        break;
                }
            }
        }

        public static void FileMenu()
        {
            WorkFile fl = new WorkFile();

            var menuFI = new Dictionary<int, string>()
            {
                [1] = "Create File",
                [2] = "Delete File",
                [3] = "Get all Files",
                [4] = "Move File",
                [5] = "Copy File",
                [6] = "Read File",
                [7] = "Write File",
                [8] = "Go beck"
            };
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var m in menuFI)
            {
                Console.WriteLine(m);
            }
            while (true)
            {
                PrintRed("Select one of them: ");
                var select = Console.ReadLine();
                switch (select)
                {
                    case "1":
                        fl.CreateFile(PathDisk(fl));
                        break;
                    case "2":
                        fl.DeleteFile(PathDisk(fl));
                        break;
                    case "3":
                        fl.GetFiles(PathDisk(fl));
                        break;
                    case "4":
                        fl.MoveFile(PathDisk(fl), namefile);
                        break;
                    case "5":
                        fl.CopyFile(PathDisk(fl),namefile);
                        break;
                    case "6":
                        fl.ReadFile(PathDisk(fl));
                        break;
                    case "7":
                        fl.WriteFile(PathDisk(fl));
                        break;
                    case "8":
                        Console.Clear();
                        MainMenu();
                        return;
                    default:break;
                }
            }
            
        }

        public static void GetDisks()
        {
            PrintRed("Your DISKs: ");
            foreach (var d in dr)
            {
                Console.Write(" ("+d.ToString().ToLower()[0]+") "+d.Name);
                disk[d.ToString().ToLower()[0].ToString()] = d.Name;
            }
            Console.WriteLine();
        }
        public static void PrintRed(string ms)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(ms);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static string PathDisk(object ob)
        {
            PrintRed("Select one of the DISK: ");
            var path = Console.ReadLine();
            foreach (var d in disk)
            {
                if (d.Key.Contains(path) && ob is WorkFolder)
                {
                    return d.Value + NameFolder();
                }else if (d.Key.Contains(path) && ob is WorkFile)
                {
                    return d.Value + NameFolder() + NameFile();
                }
            }
            return null;
        }
        public static string NameFolder()
        {
            PrintRed("Enter folder's name: ");
            namefolder = Console.ReadLine();
            return  namefolder;
        }
        public static string NameFile()
        {
            PrintRed("Enter file's name: ");
            var ss = Console.ReadLine();
            namefile = ss != "" ? ss + ".txt" : "";
            return namefile;
        }
    }
}
