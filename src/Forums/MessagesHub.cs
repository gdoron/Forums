using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Forums
{
    public static class MessagesHub
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> SocketsDictionary =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>>();

        public static void Subscribe(WebSocket webSocket, string socketId, string userId, string postId)
        {
            if (userId != null)
            {
                var userMessagesSockets = SocketsDictionary.GetOrAdd(userId, new ConcurrentDictionary<string, WebSocket>());
                userMessagesSockets.TryAdd(socketId, webSocket);
            }
            var postSockets = SocketsDictionary.GetOrAdd(postId, new ConcurrentDictionary<string, WebSocket>());
            postSockets.TryAdd(socketId, webSocket);
        }

        public static void Unsubscribe(WebSocket webSocket, string socketId, string userId, string postId)
        {
            ConcurrentDictionary<string, WebSocket> sockets;
            WebSocket removedSocket;
            if (SocketsDictionary.TryGetValue(userId, out sockets))
            {
                sockets.TryRemove(socketId, out removedSocket);
            }

            if (SocketsDictionary.TryGetValue(postId, out sockets))
            {
                sockets.TryRemove(socketId, out removedSocket);
            }
        }

        public static async Task PublishAsync(string topic, string message)
        {
            await Task.Run(() =>
            {
                ConcurrentDictionary<string, WebSocket> sockets;
                if (SocketsDictionary.TryGetValue(topic, out sockets))
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