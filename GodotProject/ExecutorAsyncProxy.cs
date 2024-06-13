using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LLama.Common.ChatHistory;

namespace GodotSample
{
    /// <summary>
    /// Async proxy to execute Executor under a thread
    /// </summary>
    public class ExecutorAsyncProxy : IDisposable
    {
        private Executor _executor = null;

        private ManualResetEvent _threadMessageProcessing =  new ManualResetEvent(false);

        private ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();

        private Thread _thread;

        private CancellationTokenSource _cancellationTokenSource;

        #region Disposable

        // Flag to indicate if the object has been disposed.
        private bool _disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                if(_executor != null)
                {
                    _executor.Dispose();
                    _executor = null;
                }
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }

        ~ExecutorAsyncProxy()
        {
            Dispose(false);
        }

        #endregion

        public ExecutorAsyncProxy()
        {
            // Start and keep always running
            Start();
        }

        private void Start()
        {
            // Create a CancellationTokenSource
            _cancellationTokenSource = new CancellationTokenSource();

            _thread = new Thread(() => WorkThreadFunction(_cancellationTokenSource.Token));
            _thread.Start();
        }

        private void WorkThreadFunction(CancellationToken token)
        {
            try
            {
                ReportMessage("Loading model");

                // Start by loading executor
                _executor = new Executor();
                _executor.Load();

                ReportMessage("Model loaded");

                // Start loop to process messages
                while (true)
                {
                    ProcessMessage();

                    if (token.IsCancellationRequested)
                    {
                        ReportMessage("Cancel thread");

                        // Terminate the operation
                        token.ThrowIfCancellationRequested();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                ReportMessage("Thread cancelled");
            }
            catch(Exception ex)
            {
                ReportMessage($"Failed '{ex.Message}'");
            }
        }

        private void ReportMessage(string message)
        {
            var threadId = _thread.ManagedThreadId;
            GD.Print($"[{threadId}]: {message}");
        }

        private void ProcessMessage()
        {
            // wait for signal
            _threadMessageProcessing.WaitOne();

            if( _messages.TryDequeue(out var message))
            {
                ReportMessage($"-> '{message}'");

                // send message for processing
                var response = _executor.SendMessage(message).Result;

                ReportMessage($"<- '{response}'");

                if ( ResponseReceivedMessageDelegate != null)
                {
                    ResponseReceivedMessageDelegate(response);
                }
            }

            // block again
            _threadMessageProcessing.Reset();
        }

        public void Terminate()
        {
            _cancellationTokenSource.Cancel();

            // unblock anything
            _threadMessageProcessing.Set();
        }

        // Thread safe
        public void SendMessage(string message)
        {
            _messages.Enqueue(message);

            _threadMessageProcessing.Set();
        }

        // Declare a delegate
        public delegate void ResponseReceivedMessage(string message);
        public ResponseReceivedMessage ResponseReceivedMessageDelegate;
    }
}
