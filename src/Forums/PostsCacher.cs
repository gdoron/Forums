using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Forums
{
    public class PostsCacher
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly object _postSyncer = new object();

        public PostsCacher(ConnectionMultiplexer redis, ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _redis = redis;
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task<List<HierarchyPost>> GetHierarchyPost(int postId)
        {
            Tuple<PostMetadata, List<HierarchyPost>> memoryChaceValue;
            if (!_memoryCache.TryGetValue(postId, out memoryChaceValue))
            {
                var posts = await GetPostFromRedis(postId);
                if (posts == null)
                    return UpdateHierarchyPost(postId);

                memoryChaceValue = new Tuple<PostMetadata, List<HierarchyPost>>(new PostMetadata(), posts);
                _memoryCache.Set(postId, memoryChaceValue);
            }

            return memoryChaceValue.Item2;
        }

        private async Task<List<HierarchyPost>> GetPostFromRedis(int rootId)
        {
            var key = rootId.ToString();
            var serializedPosts = await _redis.GetDatabase().HashGetAsync(key, "Posts");
            if (!serializedPosts.HasValue)
            {
                return null;
            }
            var posts = JsonConvert.DeserializeObject<List<HierarchyPost>>(serializedPosts);
            return posts;
        }

        private List<HierarchyPost> UpdateHierarchyPost(int postId)
        {
            if (!_context.Posts.Any(x => x.Id == postId))
                return null;

            lock (_postSyncer)
            {
                var hierarchyPosts = _context.GetRootAsync(postId).Result;
                var serializedPosts = JsonConvert.SerializeObject(hierarchyPosts);

                byte[] byteArray = Encoding.UTF8.GetBytes(serializedPosts);
                string gzippedPosts;
                using (var finalStream = new MemoryStream(byteArray))
                using (var stream = new MemoryStream(byteArray))
                using (var compressedStream = new GZipStream(finalStream, CompressionLevel.Optimal))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    /*todo: Change to async and remove the lock*/
                    stream.CopyTo(compressedStream);
                    StreamReader reader = new StreamReader(finalStream);
                    gzippedPosts = reader.ReadToEnd();
                }



                var key = hierarchyPosts.First().RootId.ToString();
                var lastChangeTicks = DateTime.UtcNow.Ticks;
                /*todo: fix this*/
                var views = 999;
                var entries = new[]
                                  {
                                      new HashEntry("Posts", gzippedPosts),
                                      new HashEntry("LastChangeTicks", lastChangeTicks),
                                      new HashEntry("Views", views),
                                  };
                _redis.GetDatabase().HashSet(key, entries, CommandFlags.FireAndForget);

                var memoryKeyValue = new Tuple<PostMetadata, string>(
                    new PostMetadata
                        {
                            LastChangeTicks = lastChangeTicks,
                            ViewsCount = views
                        }, gzippedPosts);

                _memoryCache.Set(key, memoryKeyValue);

                return hierarchyPosts;
            }
        }

        public async Task<long> IncreaseViewCount(int rootPostId)
        {
            var key = rootPostId.ToString();
            var viewCount = await _redis.GetDatabase().HashIncrementAsync(key, "ViewsCount");
            return viewCount;
        }

        public async Task<PostMetadata> GetPostMetadata(int rootPostId)
        {
            var key = rootPostId.ToString();
            var fields = new RedisValue[]
                             {
                                 "ViewsCount",
                                 "LastChangeTicks"
                             };

            RedisValue[] redisValues = await _redis.GetDatabase().HashGetAsync(key, fields);
            var metadata = new PostMetadata
                               {
                                   ViewsCount = long.Parse(redisValues[0]),
                                   LastChangeTicks = long.Parse(redisValues[1])
                               };
            return metadata;
        }

        public class PostMetadata
        {
            public long ViewsCount { get; set; }
            public long LastChangeTicks { get; set; }
        }
    }
}