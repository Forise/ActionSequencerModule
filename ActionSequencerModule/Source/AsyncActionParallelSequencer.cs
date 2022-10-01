using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ActionSequencerModule.Interfaces;

namespace ActionSequencerModule
{
    public class BaseAsyncParallelSequencer : AAsyncActionParallelSequencer<IAsyncActionItem>{}

    public abstract class AAsyncActionParallelSequencer<T> : IAsyncActionItem, IActionParent where T : IAsyncActionItem
    {
        public virtual event Action CompletedEvent;

        protected readonly Queue<T> actions = new Queue<T>();
        private readonly List<Task> tasks = new List<Task>();
        protected bool cancelled;
        CancellationTokenSource ts = new CancellationTokenSource();

        public Queue<T> Actions => actions;
        public IActionParent Parent { get; set; }
        public bool Cancelled => cancelled;

        public bool IsCompleted { get; protected set; }
        public bool IsRunning { get; protected set; }

        #region Constructors
        protected AAsyncActionParallelSequencer()
        {
        }

        protected AAsyncActionParallelSequencer(params T[] items)
        {
            actions = new Queue<T>();
            foreach (var item in items)
            {
                actions.Enqueue(item);
            }
        }

        protected AAsyncActionParallelSequencer(Queue<T> items)
        {
            actions = items;
        }
        #endregion Constructors

        public virtual void AddAction(T action)
        {
            actions.Enqueue(action);
        }

        public virtual async Task Run()
        {
            IsRunning = true;
            tasks.Clear();
            foreach (var item in actions)
            {
                tasks.Add(item.Run());
            }

            Task cancelTask = Task.Run(() =>
            {
                int i = 0;
                while (ts.IsCancellationRequested)
                {
                    //DO NOTHING;
                    i = 0;
                }
            });

            await Task.WhenAny(Task.WhenAll(tasks), cancelTask);
            EndSequence();
            tasks.Clear();
        }

        protected virtual void EndSequence()
        {
            ts = new CancellationTokenSource();
            CompletedEvent?.Invoke();
            CompletedEvent = null;
            IsCompleted = true;
            IsRunning = false;
        }

        public void StopSequence()
        {
            ts.Cancel();
            cancelled = true;
        }
    }
}