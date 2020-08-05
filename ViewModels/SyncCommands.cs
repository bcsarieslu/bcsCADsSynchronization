using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BCS.CADs.Synchronization.ViewModels
{
    public class SyncCommand
    {
        private readonly Dictionary<string, EventHandler> _commands;

        private SyncCommand()
        {
            _commands = new Dictionary<string, EventHandler>();
        }

        private readonly static Lazy<SyncCommand> _lazyInstance
            = new Lazy<SyncCommand>(() => new SyncCommand());

        private static SyncCommand Instance
        {
            get
            {
                return _lazyInstance.Value;
            }
        }

        public static void Add(string key, EventHandler handler)
        {
            if (Instance._commands.ContainsKey(key))
            {
                return;
            }

            Instance._commands.Add(key, handler);
        }


        public static void Remove(string key)
        {
            Instance._commands.Remove(key);
        }



        public static void Invoke(string key, object sender, EventArgs args)
        {
            if (!Instance._commands.ContainsKey(key))
            {
                throw new ArgumentException($"{key} handler is not exist!!");
            }

            Instance._commands[key]?.Invoke(sender, args);

        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
