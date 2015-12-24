using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldersAndFiles
{
    interface IFolder
    {
        void CreateDirectoryInfo(string path);
        void DeleteDrectory(string path);
        void MoveDirectory(string path);
        void GetDirectories(string path);
    }
}
