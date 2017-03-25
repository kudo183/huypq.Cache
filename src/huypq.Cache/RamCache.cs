using System.Collections.Generic;

namespace huypq.Cache
{
    public class RamCache<KeyType, DataType> where DataType : class, new()
    {
        class CacheData
        {
            public DataType Data { get; set; }
            public System.IO.MemoryStream DataAsProtoBufStream { get; set; }
            public int HitCount { get; set; }
            public long LastAccessTime { get; set; }
        }

        private static readonly RamCache<KeyType, DataType> _instance = new RamCache<KeyType, DataType>();

        public static RamCache<KeyType, DataType> Instance
        {
            get { return _instance; }
        }

        public void Init(int maxItem)
        {
            _maxItem = maxItem;
            _cache = new Dictionary<KeyType, CacheData>(_maxItem);
        }

        private int _maxItem;

        private Dictionary<KeyType, CacheData> _cache;

        public DataType Get(KeyType key)
        {
            CacheData data;
            if (_cache.TryGetValue(key, out data) == false)
            {
                return null;
            }

            data.HitCount = data.HitCount + 1;
            data.LastAccessTime = System.DateTime.Now.Ticks;
            data.DataAsProtoBufStream.Position = 0;
            return ProtoBuf.Serializer.Deserialize<DataType>(data.DataAsProtoBufStream);
        }

        public void AddOrUpdate(KeyType key, DataType data)
        {
            CacheData cacheData;
            if (_cache.TryGetValue(key, out cacheData) == false)
            {
                if (_cache.Count == _maxItem)
                {
                    long min = long.MaxValue;
                    KeyType keyOfMinHitItem = default(KeyType);
                    foreach (var item in _cache)
                    {
                        //if (item.Value.HitCount < min)
                        //{
                        //    min = item.Value.HitCount;
                        //    keyOfMinHitItem = item.Key;
                        //}
                        if (item.Value.LastAccessTime < min)
                        {
                            min = item.Value.LastAccessTime;
                            keyOfMinHitItem = item.Key;
                        }
                    }
                    _cache.Remove(keyOfMinHitItem);
                }
                cacheData = new CacheData()
                {
                    Data = data,
                    HitCount = 0,
                    LastAccessTime = System.DateTime.Now.Ticks,
                    DataAsProtoBufStream = new System.IO.MemoryStream()
                };
                ProtoBuf.Serializer.Serialize(cacheData.DataAsProtoBufStream, data);
                _cache.Add(key, cacheData);
            }
            else
            {
                cacheData.Data = data;
                cacheData.LastAccessTime = System.DateTime.Now.Ticks;
                ProtoBuf.Serializer.Serialize(cacheData.DataAsProtoBufStream, data);
            }
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public string GetCacheInfo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("MaxItem: " + _maxItem);
            sb.AppendLine("Item count: " + _cache.Count);
            foreach (var item in _cache)
            {
                sb.AppendLine(string.Format("hitcount: {0} key: {1}", item.Value.HitCount, item.Key));
            }

            return sb.ToString();
        }
    }
}
