using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;

namespace BCS.CADs.Synchronization.Entities
{
    public class RecentFile
    {
        private string path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments).ToString() + $@"/Broadway/CADDocument/RecentFile";
        /// <summary>
        /// 增加一筆開啟的檔案
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filepath"></param>
        public ObservableCollection<RecentFileProperties> AddRecentFile(List<SearchItem> searchItems)
        {
            string path = @"pack://application:,,,/BCS.CADs.Synchronization;component/Images";
            ObservableCollection<RecentFileProperties>  readData = ReadRecentFile();

            foreach (var item in searchItems.Where(x => !String.IsNullOrEmpty(x.FileName) && !String.IsNullOrEmpty(x.FilePath)))
            {
                readData.Remove(readData.FirstOrDefault(a => a.FileName == item.FileName && a.FilePath == item.FilePath));

                while (readData.Count >= 15)
                {
                    readData.Remove(readData.Last());
                }

                readData.Insert(0, new RecentFileProperties
                {
                    FileName = item.FileName,
                    FilePath = item.FilePath,
                    DrawingTypeImage = Path.Combine(path,item.ClassName+".png"),
                    OpenDate = DateTime.Now.ToString("yyyy/MM/dd"),
                });
            }

            var result = new
            {
                registration_ids = from s in readData select s
            };

            var json = JsonConvert.SerializeObject(result);
            JObject jo = JObject.Parse(json);
            WriteFile(jo);
            return readData;
        }

        /// <summary>
        /// 寫入json檔案
        /// </summary>
        /// <param name="jo"></param>
        private void WriteFile(JObject jo)
        {
            string path = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments).ToString()+$@"/Broadway/CADDocument/RecentFile");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, path);
            }
            using (StreamWriter fs = File.CreateText(Path.Combine(path, "myjson.json")))
            {
                using (JsonTextWriter writer = new JsonTextWriter(fs))
                {
                    jo.WriteTo(writer);
                }
            }
        }

        /// <summary>
        /// 讀取json檔案
        /// </summary>
        public ObservableCollection<RecentFileProperties> ReadRecentFile()
        {
            if (File.Exists(Path.Combine(path, "myjson.json")) == false) return new ObservableCollection<RecentFileProperties>();
            var jsonData = JObject.Parse(File.ReadAllText(Path.Combine(path, "myjson.json"))).Children().First().Values();

            var readRecentFiles = Converter.ToObservableCollection(jsonData.Select(a => new RecentFileProperties
            {
                FileName = a.Value<string>("FileName"),
                FilePath = a.Value<string>("FilePath"),
                OpenDate = a.Value<string>("OpenDate"),
                DrawingTypeImage = a.Value<string>("DrawingTypeImage")
            }));
            return readRecentFiles;
            //foreach (var item in jsonData.Select(a => new { FileName = a.Value<string>("FileName"), OpenDate = a.Value<string>("OpenDate") }))
            //{
            //    Console.WriteLine($@"Name: {item.FileName}  Date: {item.OpenDate}");
            //}
        }
    }
}