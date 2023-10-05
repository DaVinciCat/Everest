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

        private readonly HashSet<WebSocketSession> sessions = new HashSet<WebSocketSession>();

        private readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        protected virtual async Task OnCloseAsync(WebSocketSession session)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task OnErrorAsync(WebSocketSession session, Exception ex)
        {
            await Task.CompletedTask;
        }

        protected virtual Task OnMessageAsync(WebSocketSession session, byte[] message)
        {
            throw new NotImplementedException();
        }

        protected virtual Task OnMessageAsync(WebSocketSession session, string message)
        {
            throw new NotImplementedException();
        }

        protected virtual async Task OnOpenAsync(WebSocketSession session)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task SendAsync(WebSocketSession session, byte[] message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            await session.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        protected virtual async Task SendAsync(WebSocketSession session, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var buffer = Encoding.UTF8.GetBytes(message);
            await session.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        protected virtual async Task CloseAsync(WebSocketSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            await session.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        protected virtual async Task ReceiveAsync(WebSocketSession session, CancellationToken token)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            try
            {
                AddSession(session);
                await OnOpenAsync(session);

                var bytes = new byte[ReceiveBufferSize];
                var buffer = new ArraySegment<byte>(bytes);
                while (!token.IsCancellationRequested && session.State == WebSocketState.Open)
                {
                    var result = await session.ReceiveAsync(buffer, token);

                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Binary:
                            await OnMessageAsync(session, bytes);
                            break;

                        case WebSocketMessageType.Text:
                            var text = Encoding.UTF8.GetString(bytes);
                            await OnMessageAsync(session, text);
                            break;

                        default:
                            // If we received an incoming CLOSE message, we'll queue a CLOSE frame to be sent.
                            // We'll give the queued frame some amount of time to go out on the wire, and if a
                            // timeout occurs we'll give up and abort the connection.
                            await Task
                                .WhenAny(CloseAsync(session), Task.Delay(CloseTimeout, token))
                                .ContinueWith(_ => { }, TaskContinuationOptions.ExecuteSynchronously); // swallow exceptions occurring from sending the CLOSE
                            RemoveSession(session);
                            return;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                // ex.CancellationToken never has the token that was actually cancelled
                if (!token.IsCancellationRequested)
                {
                    await OnErrorAsync(session, ex);
                }
            }
            catch (Exception ex)
            {
                if (IsFatalException(ex))
                {
                    await OnErrorAsync(session, ex);
                }
            }
            finally
            {
                try
                {
                    await CloseAsync(session);
                }
                finally
                {
                    await OnCloseAsync(session);
                }
            }
        }

        protected async Task BroadcastAsync(string text)
        {
            try
            {
                await locker.WaitAsync();

#if NET5_0_OR_GREATER
                await Parallel.ForEachAsync(sessions, async (session, _) =>
                {
                    await SendAsync(session, text);
                });
#else
                await sessions.ParallelForEachAsync(async session =>
                {
                    await SendAsync(session, text);
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
                await Parallel.ForEachAsync(sessions, async (session, _) =>
                {
                    await SendAsync(session, bytes);
                });
#else
                await sessions.ParallelForEachAsync(async session =>
                {
                    await SendAsync(session, bytes);
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
                foreach (var session in sessions)
                {
                    await CloseAsync(session);
                }
            }
            finally
            {
                locker.Release();
            }
        }

        private void AddSession(WebSocketSession session)
        {
            try
            {
                locker.Wait();
                sessions.Add(session);
            }
            finally
            {
                locker.Release();
            }
        }

        private void RemoveSession(WebSocketSession session)
        {
            try
            {
                locker.Wait();
                sessions.Remove(session);
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
