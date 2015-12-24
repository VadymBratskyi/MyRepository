using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldersAndFiles
{
    class WorkFile : IFiles
    {
        private FileInfo fil = null;


        public void CreateFile(string path)
        {
            try
            {
                fil = new FileInfo(path);
                if (!fil.Exists)
                {
                    fil.Create();
                    Console.WriteLine("File created");
                }
                else
                {
                    Console.WriteLine("Error!!!");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteFile(string path)
        {
            try
            {
                fil = new FileInfo(path);
                if (fil.Exists)
                {
                    fil.Delete();
                    Console.WriteLine("File deleted");
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

        public void GetFiles(string path)
        {
            try
            {
                string[] ms = Directory.GetFiles(path);
                foreach (var s in ms)
                {
                    Console.WriteLine(s);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void MoveFile(string path, string namefile)
        {
            try
            {
                fil = new FileInfo(path);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Enter new path for move: ");
                Console.ForegroundColor = ConsoleColor.White;
                string newPath = Console.ReadLine()+"\\"+namefile;
                Console.WriteLine(newPath);
                if (fil.Exists && Directory.Exists(newPath) == false)
                {
                    fil.MoveTo(newPath);
                    Console.WriteLine("File moved");
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

        public void CopyFile(string path, string namefile)
        {
            try
            {
                fil = new FileInfo(path);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Enter new path for copy: ");
                Console.ForegroundColor = ConsoleColor.White;
                string newPath = Console.ReadLine() + "\\" + namefile;
                Console.WriteLine(newPath);
                if (fil.Exists && Directory.Exists(newPath) == false)
                {
                    fil.CopyTo(newPath,true);
                    Console.WriteLine("File copied");
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

        public void ReadFile(string path)
        {
            using (StreamReader str = new StreamReader(path, System.Text.Encoding.Default))
            {
                Console.WriteLine(str.ReadToEnd());
            }
            
        }

        public void WriteFile(string path)
        {
            using (StreamWriter wr = new StreamWriter(path,false,System.Text.Encoding.Default))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Enter you message: ");
                Console.ForegroundColor = ConsoleColor.White;
                var ms = Console.ReadLine();
                wr.WriteLine(ms);
            }
        }
    }
}
