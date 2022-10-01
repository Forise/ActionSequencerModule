using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ActionSequencerModule.Interfaces;

namespace ActionSequencerModule
{
    public class BaseAsyncSequencer : AAsyncActionSequencer<IAsyncActionItem>{}

    public abstract class AAsyncActionSequencer<T> : IAsyncActionItem, IActionParent where T : IAsyncActionItem
    {
        public virtual event Action CompletedEvent;

        protected readonly Queue<T> actions = new Queue<T>();
        protected bool cancelled;

        public Queue<T> Actions => actions;
        public IActionParent Parent { get; set; }
        public bool Cancelled => cancelled;
        public bool IsCompleted { get; protected set; }
        public bool IsRunning { get; protected set; }

        #region Constructors
        protected AAsyncActionSequencer()
        {
        }

        protected AAsyncActionSequencer(params T[] items)
        {
            actions = new Queue<T>();
            foreach (var item in items)
            {
                actions.Enqueue(item);
            }
        }

        protected AAsyncActionSequencer(Queue<T> items)
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
            foreach (var item in actions)
            {
                await item.Run();
            }
            EndSequence();
        }

        protected virtual void EndSequence()
        {
            CompletedEvent?.Invoke();
            CompletedEvent = null;
            IsCompleted = true;
            IsRunning = false;
        }

        public void StopSequence()
        {
            cancelled = true;
            EndSequence();
        }
    }
}