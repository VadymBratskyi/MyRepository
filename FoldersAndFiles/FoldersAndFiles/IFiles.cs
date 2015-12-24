using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldersAndFiles
{
    interface IFiles
    {
        void CreateFile(string path);
        void DeleteFile(string path);
        void GetFiles(string path);
        void MoveFile(string path, string namefile);
        void CopyFile(string path, string namefile);
        void ReadFile(string path);
        void WriteFile(string path);
    }
}
