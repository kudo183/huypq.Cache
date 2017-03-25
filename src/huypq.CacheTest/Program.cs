using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace huypq.CacheTest
{
    public class Program
    {
        [ProtoBuf.ProtoContract]
        public class TestData
        {
            [ProtoBuf.ProtoMember(1)]
            public string Name { get; set; }
        }

        public static void Main(string[] args)
        {
            string info;
            huypq.Cache.RamCache<string, TestData>.Instance.Init(5);
            info = huypq.Cache.RamCache<string, TestData>.Instance.GetCacheInfo();
            Console.WriteLine(info);

            Add("data1", "huy");
            Add("data1", "heo");
            Add("k1", "heo");
            Add("k2", "heo");
            Add("k3", "heo");
            Add("k4", "heo");
            Add("data1", "ga2");
            Add("k5", "heo");
            Add("k6", "heo");

            Console.Read();
        }

        static void Add(string key, string name)
        {
            huypq.Cache.RamCache<string, TestData>.Instance.AddOrUpdate(key, new TestData() { Name = name });
            var text = huypq.Cache.RamCache<string, TestData>.Instance.Get(key).Name;
            Console.WriteLine(text);
            var info = huypq.Cache.RamCache<string, TestData>.Instance.GetCacheInfo();
            Console.WriteLine(info);
        }
    }
}
