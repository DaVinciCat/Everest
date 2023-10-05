using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if !NET5_0_OR_GREATER
using Everest.Utils;
#endif

namespace Everest.WebSockets
{
    public abstract class WebSocketHandler
    {
        private static readonly TimeSpan CloseTimeout = TimeSpan.FromMilliseconds(250);

        private const int ReceiveBufferSize = 4 * 1024;

        private readonly HashSet<WebSocket> sockets = new HashSet<WebSocket>();

        private readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        protected virtual async Task OnCloseAsync(WebSocket socket)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task OnErrorAsync(WebSocket socket, Exception ex)
        {
            await Task.CompletedTask;
        }

        protected virtual Task OnMessageAsync(WebSocket socket, byte[] message)
        {
            throw new NotImplementedException();
        }

        protected virtual Task OnMessageAsync(WebSocket socket, string message)
        {
            throw new NotImplementedException();
        }

        protected virtual async Task OnOpenAsync(WebSocket socket)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task SendAsync(WebSocket socket, byte[] message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            await socket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        protected virtual async Task SendAsync(WebSocket socket, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var buffer = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        protected virtual async Task CloseAsync(WebSocket socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        protected virtual async Task ReceiveAsync(WebSocket socket, CancellationToken token)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            try
            {
                AddSocket(socket);
                await OnOpenAsync(socket);

                var bytes = new byte[ReceiveBufferSize];
                var buffer = new ArraySegment<byte>(bytes);
                while (!token.IsCancellationRequested && socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer, token);

                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Binary:
                            await OnMessageAsync(socket, bytes);
                            break;

                        case WebSocketMessageType.Text:
                            var text = Encoding.UTF8.GetString(bytes);
                            await OnMessageAsync(socket, text);
                            break;

                        default:
                            // If we received an incoming CLOSE message, we'll queue a CLOSE frame to be sent.
                            // We'll give the queued frame some amount of time to go out on the wire, and if a
                            // timeout occurs we'll give up and abort the connection.
                            await Task
                                .WhenAny(CloseAsync(socket), Task.Delay(CloseTimeout, token))
                                .ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously); // swallow exceptions occurring from sending the CLOSE
                            RemoveSocket(socket);
                            return;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                // ex.CancellationToken never has the token that was actually cancelled
                if (!token.IsCancellationRequested)
                {
                    await OnErrorAsync(socket, ex);
                }
            }
            catch (Exception ex)
            {
                if (IsFatalException(ex))
                {
                    await OnErrorAsync(socket, ex);
                }
            }
            finally
            {
                try
                {
                    await CloseAsync(socket);
                }
                finally
                {
                    await OnCloseAsync(socket);
                }
            }
        }

        protected async Task BroadcastAsync(string text)
        {
            try
            {
                await locker.WaitAsync();

#if NET5_0_OR_GREATER
                await Parallel.ForEachAsync(sockets, async (socket, _) =>
                {
                    await SendAsync(socket, text);
                });
#else
                await sockets.ParallelForEachAsync(async socket =>
                {
                    await SendAsync(socket, text);
                });
#endif
            }
            finally
            {
                locker.Release();
            }
        }

        protected async Task BroadcastAsync(byte[] bytes)
        {
            try
            {
                await locker.WaitAsync();

#if NET5_0_OR_GREATER
                await Parallel.ForEachAsync(sockets, async (socket, _) =>
                {
                    await SendAsync(socket, bytes);
                });
#else
                await sockets.ParallelForEachAsync(async socket =>
                {
                    await SendAsync(socket, bytes);
                });
#endif
            }
            finally
            {
                locker.Release();
            }
        }

        protected async Task CloseAsync()
        {
            try
            {
                await locker.WaitAsync();
                foreach (var socket in sockets)
                {
                    await CloseAsync(socket);
                }
            }
            finally
            {
                locker.Release();
            }
        }

        private void AddSocket(WebSocket socket)
        {
            try
            {
                locker.Wait();
                sockets.Add(socket);
            }
            finally
            {
                locker.Release();
            }
        }

        private void RemoveSocket(WebSocket socket)
        {
            try
            {
                locker.Wait();
                sockets.Remove(socket);
            }
            finally
            {
                locker.Release();
            }
        }

        // returns true if this is a fatal exception (e.g. OnError should be called)
        private static bool IsFatalException(Exception ex)
        {
            // If this exception is due to the underlying TCP connection going away, treat as a normal close
            // rather than a fatal exception.
            if (ex is COMException ce)
            {
                switch ((uint)ce.ErrorCode)
                {
                    // These are the three error codes we've seen in testing which can be caused by the TCP connection going away unexpectedly.
                    case 0x800703e3:
                    case 0x800704cd:
                    case 0x80070026:
                        return false;
                }
            }

            // unknown exception; treat as fatal
            return true;
        }
    }
}
