using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace BCS.CADs.Synchronization.Models
{
    class MyCache
    {
        private static ObjectCache _cache = null;
        static readonly object locker = new object();

        private MyCache() { }

        public static ObjectCache CacheInstance
        {
            get
            {
                if (_cache == null)
                    lock (locker)
                        if (_cache == null)
                            _cache = MemoryCache.Default;
                return _cache;
            }
        }
    }
}
