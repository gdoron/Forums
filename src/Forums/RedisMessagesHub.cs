using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Forums
{
    public class RedisMessagesHub
    {
        private readonly ConnectionMultiplexer _redis;

        //private ConcurrentDictionary<string, List<ISubscriber>> _subscribersDictionary = new ConcurrentDictionary<string, List<ISubscriber>>();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _roomsDictionary =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>>();

        //private readonly ConcurrentDictionary<string, WebSocket> _webSockets = new ConcurrentDictionary<string, WebSocket>(); 

        public RedisMessagesHub(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public void Subscribe(WebSocket webSocket, string socketId, string userId, string postId)
        {
            if (userId != null)
            {
                var userMessagesSockets = _roomsDictionary.GetOrAdd(userId, roomId =>
                {
                    _redis.GetSubscriber().Subscribe(userId, (channel, value) =>
                    {
                        PublishAsync(channel, $"New update in channel {channel}");
                    });
                    return new ConcurrentDictionary<string, WebSocket>();
                });
                userMessagesSockets.TryAdd(socketId, webSocket);
            }

            var postSockets = _roomsDictionary.GetOrAdd(postId, roomId =>
            {
                _redis.GetSubscriber().Subscribe(postId, (channel, value) =>
                {
                    PublishAsync(channel, $"New update in channel {channel}");
                });
                return new ConcurrentDictionary<string, WebSocket>();
            });
            postSockets.TryAdd(socketId, webSocket);

            //_webSockets.TryAdd(socketId, webSocket);
        }

        public void Unsubscribe(WebSocket webSocket, string socketId, string userId, string postId)
        {
            WebSocket removedSocket;
            //_webSockets.TryRemove(socketId, out removedSocket);
            ConcurrentDictionary<string, WebSocket> sockets;
            if (_roomsDictionary.TryGetValue(userId, out sockets))
            {
                sockets.TryRemove(socketId, out removedSocket);
            }

            if (_roomsDictionary.TryGetValue(postId, out sockets))
            {
                sockets.TryRemove(socketId, out removedSocket);
            }
        }

        public async Task PublishAsync(string topic, string message)
        {
            await Task.Run(() =>
            {
                ConcurrentDictionary<string, WebSocket> sockets;
                if (_roomsDictionary.TryGetValue(topic, out sockets))
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    var seg = new ArraySegment<byte>(buffer, 0, buffer.Length);
                    var tasks = sockets.Values.Select(webSocket => webSocket.SendAsync(seg, WebSocketMessageType.Text, true, CancellationToken.None)).ToArray();
                    Task.WaitAll(tasks);
                }
            });
        }
    }
}