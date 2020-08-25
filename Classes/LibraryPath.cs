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

        public ObservableCollection<LibraryFileInfo> FileItems { get; set; } = new ObservableCollection<LibraryFileInfo>();

    }

    public class LibraryFileInfo
    {

        public LibraryFileInfo()
        {

        }

        //public LibraryFileInfo(string name, string type, string extension)
        public LibraryFileInfo(string name,string className,  string extension)
        {
            this.Name = name;
            this.ClassName = className;
            this.Extension = extension;
        }

        public string Name { get; set; }

        public string ClassName { get; set; }

        public string Extension { get; set; }

        public string Thumbnail { get; set; } = "";

        public string ItemId { get; set; } = "";
    }
}
