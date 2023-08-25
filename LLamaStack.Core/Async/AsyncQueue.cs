using System.Collections.Concurrent;

namespace LLamaStack.Core.Async
{
    public class AsyncQueue<T, U> : IDisposable
    {
        /// <summary>
        ///   The function to be called when items are processed in the queue
        /// </summary>
        private readonly Func<T, Task<U>> _processFunction;

        /// <summary>
        ///   The queue
        /// </summary>
        private readonly BlockingCollection<AsyncQueueItem<T, U>> _asyncQueue = new BlockingCollection<AsyncQueueItem<T, U>>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="AsyncQueue" /> class.
        /// </summary>
        /// <param name="processFunction">The function to be called when items are processed in the queue</param>
        public AsyncQueue(Func<T, Task<U>> processFunction)
        {
            _processFunction = processFunction;
            Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
        }

        public bool IsCompleted
        {
            get { return _asyncQueue.IsCompleted; }
        }

        /// <summary>
        ///   Queues an item to be processed
        /// </summary>
        /// <param name="item">The object to be passed to the function.</param>
        /// <returns>The completion task to be awaited on</returns>
        public Task<U> QueueItem(T item)
        {
            var queueItem = new AsyncQueueItem<T, U>(item);
            _asyncQueue.TryAdd(queueItem);
            return queueItem.CompletionSource.Task;
        }

        /// <summary>
        ///   Processes the queue.
        /// </summary>
        private async Task ProcessQueue()
        {
            while (_asyncQueue.TryTake(out var queueItem, Timeout.Infinite))
            {
                var input = queueItem.Item;
                var tcs = queueItem.CompletionSource;

                try
                {
                    tcs.SetResult(await _processFunction(input));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
        }

        public async Task StopProcessing()
        {
            _asyncQueue.CompleteAdding();
            while (!_asyncQueue.IsCompleted)
                await Task.Delay(500);

            _asyncQueue.Dispose();
        }

        public int QueueCount()
        {
            return _asyncQueue.Count;
        }

        public void CloseQueue()
        {
            _asyncQueue.CompleteAdding();
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!IsCompleted)
                CloseQueue();
        }
    }

    public class AsyncQueue<T> : IDisposable
    {
        /// <summary>
        ///   The function to be called when items are processed in the queue
        /// </summary>
        private readonly Func<T, Task> _processFunction;

        /// <summary>
        ///   The queue
        /// </summary>
        private readonly BlockingCollection<T> _processQueue = new BlockingCollection<T>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="AsyncQueue" /> class.
        /// </summary>
        /// <param name="processFunction">The function to be called when items are processed in the queue</param>
        public AsyncQueue(Func<T, Task> processFunction)
        {
            _processFunction = processFunction;
            Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
        }

        public bool IsCompleted
        {
            get { return _processQueue.IsCompleted; }
        }



        /// <summary>
        ///   Queues an item to be processed
        /// </summary>
        /// <param name="item">The object to be passed to the function.</param>
        /// <returns>The completion task to be awaited on</returns>
        public void QueueItem(T item)
        {
            _processQueue.TryAdd(item);
        }

        /// <summary>
        ///   Processes the queue.
        /// </summary>
        private async Task ProcessQueue()
        {
            while (_processQueue.TryTake(out var queueItem, Timeout.Infinite))
                await _processFunction(queueItem);
        }

        public int QueueCount()
        {
            return _processQueue.Count;
        }

        public void CloseQueue()
        {
            _processQueue.CompleteAdding();
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!IsCompleted)
                CloseQueue();
        }
    }


    /// <summary>
    /// Class to encapsulate the item information to be proccesed and its completion task
    /// </summary>
    /// <typeparam name="T">The itme containing the information to be used in the Process function</typeparam>
    /// <typeparam name="U">Th return type of the Process function</typeparam>
    public class AsyncQueueItem<T, U>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="AsyncQueueItem" /> class.
        /// </summary>
        /// <param name="item">The request.</param>
        public AsyncQueueItem(T item)
        {
            Item = item;
            CompletionSource = new TaskCompletionSource<U>();
        }

        /// <summary>
        ///   Gets or sets the object to add to the process call containing the information needed to the Process function.
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        ///   Gets or sets the completion source to return the result from the Process function.
        /// </summary>
        public TaskCompletionSource<U> CompletionSource { get; set; }
    }
}
