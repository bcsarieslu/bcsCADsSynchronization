using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace BCS.CADs.Synchronization.Classes
{
    public class LibraryPath
    {
        public LibraryPath()
        {

        }

        public LibraryPath(string name,string path)
        {
            this.Name = name;
            this.Path = path;
        }

        public string Name { get; set; }
        public string Path { get; set; }

        ObservableCollection<LibraryFileInfo> FileItem { get; set; } = new ObservableCollection<LibraryFileInfo>();

    }

    internal class LibraryFileInfo
    {
        string Name { get; set; }

    }
}
