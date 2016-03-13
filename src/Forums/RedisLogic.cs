//using System;
//using System.Threading.Tasks;
//using Entities;
//using StackExchange.Redis;

//namespace Forums
//{
//    public class RedisLogic
//    {
//        private readonly ConnectionMultiplexer _redis;

//        public RedisLogic(ConnectionMultiplexer redis)
//        {
//            _redis = redis;
//        }

//        //public bool IsPostDirty(int rootPostId)
//        //{
//        //    var database = _redis.GetDatabase();
//        //    var isDirty = database.StringGetrootPostId, 0);
//        //    return isDirty;
//        //}

//        //public void Put(int key, string value)
//        //{
//        //    var strKey = key.ToString();
//        //    Put(strKey, value);
//        //}

//        public Task PutAsync(string key, string value, DateTime dateTime)
//        {
//            var database = _redis.GetDatabase();
//            HashEntry[] arr =
//                {
//                    new HashEntry("Date", dateTime.ToString("yyyyMMddHHmmssffff")),
//                    new HashEntry("Post", value)
//                };
//            database.StringSet()
//                database.KeyExpire()
//            return database.HashSetAsync(key, arr, CommandFlags.FireAndForget);
//        }

//        public Task GetAsync(string key)
//        {
//            var database = _redis.GetDatabase();

//            return database.HashGetAsync(key, "Date", CommandFlags.FireAndForget);
//        }
//    }
//}