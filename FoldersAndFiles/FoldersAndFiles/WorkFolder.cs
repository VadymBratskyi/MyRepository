using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldersAndFiles
{
    class WorkFolder : IFolder
    {
        private DirectoryInfo dir = null;
        
        public void CreateDirectoryInfo(string path)
        { 
            try
            {   
                dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                    Console.WriteLine("Folder created");
                }
                else
                {
                    Console.WriteLine("Error!!");
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void DeleteDrectory(string path)
        {
            try
            {
                dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    dir.Delete();
                    Console.WriteLine("Folder deleted");
                }
                else
                {
                    Console.WriteLine("Error!!");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void MoveDirectory(string path)
        {
            dir = new DirectoryInfo(path);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Enter new path: ");
            Console.ForegroundColor = ConsoleColor.White;
            string newPath = Console.ReadLine();
            if (dir.Exists && Directory.Exists(newPath)==false)
            {
                dir.MoveTo(newPath);
                Console.WriteLine("Folder moved");
            }
            else
            {
                Console.WriteLine("Error!!");
            }
        }

        public void GetDirectories(string path)
        {
            try
            {
                string[] d = Directory.GetDirectories(path);
                foreach (var s in d)
                {
                    Console.WriteLine(s);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
