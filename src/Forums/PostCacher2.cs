using System;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Forums
{
    public class PostsCacher2
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        private const string ViewsKey = ":ViewsCount";

        public PostsCacher2(ConnectionMultiplexer redis, ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _redis = redis;
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task<byte[]> GetGzipPostFromRedisAsync(int rootId)
        {
            var key = rootId.ToString();
            var gzipPosts = await _redis.GetDatabase().HashGetAsync(key, "Posts");
            if (!gzipPosts.HasValue)
            {
                return null;
            }

            gzipPosts = await UpdateHierarchyPostAsync(rootId);

            return gzipPosts;
        }

        private async Task<byte[]> UpdateHierarchyPostAsync(int postId)
        {
            if (!_context.Posts.Any(x => x.Id == postId))
                return null;

            var hierarchyPosts = await _context.GetRootAsync(postId);
            var serializedPosts = JsonConvert.SerializeObject(hierarchyPosts);
            var gzippedPosts = Gzipper.Zip(serializedPosts);
            
            var key = hierarchyPosts.First().RootId.ToString();
            var lastChangeTicks = DateTime.UtcNow.Ticks;
            /*todo: fix this*/
            var views = 999;
            var entries = new[]
                              {
                                  new HashEntry("Posts", gzippedPosts),
                                  new HashEntry("LastChangeTicks", lastChangeTicks),
                                  new HashEntry("Views", views)
                              };
            _redis.GetDatabase().HashSet(key, entries, CommandFlags.FireAndForget);

            var memoryKeyValue = new Tuple<PostsCacher.PostMetadata, byte[]>(
                new PostsCacher.PostMetadata
                    {
                        LastChangeTicks = lastChangeTicks,
                        ViewsCount = views
                    }, gzippedPosts);

            _memoryCache.Set(key, memoryKeyValue);

            return gzippedPosts;
        }

        public async Task<long> IncreaseViewCountAsync(int rootPostId)
        {
            var key = rootPostId.ToString();
            var viewCount = await _redis.GetDatabase().HashIncrementAsync(key, "ViewsCount");
            if (viewCount == 0)
            {
                var post = await _context.Posts.SingleAsync(x => x.Id == rootPostId);
                post.Views++;
                viewCount = post.Views;
                /*todo: remove this when the filter works as it should*/
                _context.SaveChanges();
                _redis.GetDatabase().HashSet(key, "ViewsCount", viewCount, flags: CommandFlags.FireAndForget);
            }

            _memoryCache.Set(key + ViewsKey, viewCount);

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