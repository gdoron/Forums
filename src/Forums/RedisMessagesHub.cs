using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.WebSockets.Server;
using StackExchange.Redis;

namespace Forums
{
    public class RedisMessagesHub : IDisposable
    {
        private readonly ConnectionMultiplexer _redis;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _roomsDictionary =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>>();

        private readonly ConcurrentDictionary<string, KeyValuePair<WebSocket, List<string>>> _webSockets =
            new ConcurrentDictionary<string, KeyValuePair<WebSocket, List<string>>>();

        private readonly Timer _timer;

        public RedisMessagesHub(ConnectionMultiplexer redis)
        {
            _redis = redis;
            _timer = new Timer(ScanForDeadSockets, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        private void ScanForDeadSockets(object state)
        {
            foreach (var socket in _webSockets)
            {
                if (socket.Value.Key.State >= WebSocketState.Closed)
                {
                    KeyValuePair<WebSocket, List<string>> doNotNeed;
                    _webSockets.TryRemove(socket.Key, out doNotNeed);

                    foreach (var room in socket.Value.Value)
                    {
                        ConcurrentDictionary<string, WebSocket> removedSocket;
                        _roomsDictionary.TryRemove(room, out removedSocket);
                    }
                }
            }
        }

        public void Subscribe(WebSocket webSocket, string socketId, string room)
        {
            if (string.IsNullOrWhiteSpace(room))
                throw new ArgumentException("Invalid room", nameof(room));

            _webSockets.AddOrUpdate(socketId, new KeyValuePair<WebSocket, List<string>>(webSocket,new List<string> {room}), (key, oldValue) =>
            {
                oldValue.Value.Add(room);
                return oldValue;
            });


            var roomSockets = _roomsDictionary.GetOrAdd(room, roomId =>
            {
                _redis.GetSubscriber().Subscribe(room, (channel, message) => { BroadcastMessageAsync(channel, $"Update in channel {channel}, value, message:{message}"); });
                return new ConcurrentDictionary<string, WebSocket>();
            });
            roomSockets.TryAdd(socketId, webSocket);
        }

        public void Unsubscribe(string socketId, string room)
        {
            ConcurrentDictionary<string, WebSocket> sockets;

            if (_roomsDictionary.TryGetValue(room, out sockets))
            {
                WebSocket removedSocket;
                sockets.TryRemove(socketId, out removedSocket);
            }
        }

        public async Task DisposeWebSocket(string websocketId)
        {
            KeyValuePair<WebSocket, List<string>> socketRooms;
            if (_webSockets.TryGetValue(websocketId, out socketRooms))
            {
                foreach (var room in socketRooms.Value)
                {
                    ConcurrentDictionary<string, WebSocket> roomSockets;
                    if (_roomsDictionary.TryGetValue(room, out roomSockets))
                    {
                        WebSocket removedWebSocket;
                        roomSockets.TryRemove(websocketId, out removedWebSocket);
                    }
                }
            }
            await socketRooms.Key.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye Bye", CancellationToken.None);
        }

        private Task BroadcastMessageAsync(string topic, string message)
        {
            return Task.Run(() =>
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

        public void Dispose()
        {
            _timer.Dispose();
        }
    }

    public static class SocketIniter
    {
        public static void ConfigureForumsSockets(this IApplicationBuilder app, RedisMessagesHub messagesHub, string path)
        {
            app.Map(path, managedWebSocketsApp =>
            {
                // Comment this out to test native server implementations
                managedWebSocketsApp.UseWebSockets(new WebSocketOptions {ReplaceFeature = true, KeepAliveInterval= TimeSpan.FromSeconds(5)});

                managedWebSocketsApp.Use(async (context, next) =>
                {
                    if (!context.WebSockets.IsWebSocketRequest)
                    {
                        await next();
                    }
                    else
                    {
                        var userId = context.User.GetUserId();
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        byte[] buffer = new byte[1024*4];
                        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        string postId = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var socketId = Guid.NewGuid().ToString();

                        if (userId != null)
                            messagesHub.Subscribe(webSocket, socketId, userId);

                        messagesHub.Subscribe(webSocket, socketId, postId);

                        while (!result.CloseStatus.HasValue)
                        {
                            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        }

                        //await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                        await messagesHub.DisposeWebSocket(socketId);
                    }
                });
            });
        }
    }
}