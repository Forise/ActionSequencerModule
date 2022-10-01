using System;
using System.Collections.Generic;
using ActionSequencerModule.Interfaces;

namespace ActionSequencerModule
{
    public class BaseSequencer : ActionSequencer<IActionItem>{}

    public abstract class AActionSequencer<T> : IActionItem, IActionParent
    {
        protected readonly Queue<T> actions = new Queue<T>();
        protected bool cancelled;

        public Queue<T> Actions => actions;
        public IActionParent Parent { get; set; }

        #region Constructors
        protected AActionSequencer()
        {
        }

        protected AActionSequencer(params T[] items)
        {
            actions = new Queue<T>();
            foreach (var item in items)
            {
                actions.Enqueue(item);
            }
        }

        protected AActionSequencer(Queue<T> items)
        {
            actions = items;
        }
        #endregion Constructors

        public virtual void AddAction(T action)
        {
            actions.Enqueue(action);
        }

        public virtual void Run()
        {
            RunNextAction();
        }

        public abstract bool IsItemLast { get; }

        protected abstract void RunNextAction();

        protected abstract void EndSequence();

        public void StopSequence()
        {
            cancelled = true;
            EndSequence();
        }

        public virtual event Action CompletedEvent;
        public bool IsCompleted { get; protected set; }
    }

    public class ActionSequencer<T> : AActionSequencer<T> where T : IActionItem
    {
        public override bool IsItemLast => !IsCompleted && actions.Count == 0;

        #region ICompletion Realisation
        public override event Action CompletedEvent;
        public bool IsRunning { get; protected set; }
        #endregion ICompletion Realisation

        public override void Run()
        {
            IsRunning = true;
            base.Run();
        }

        protected override void RunNextAction()
        {
            if (!cancelled && actions != null && actions.Count > 0)
            {
                var item = actions.Dequeue();

                if (item != null)
                {
                    item.CompletedEvent += RunNextAction;
                    item.Parent = this;
                    item.Run();
                }
                else
                {
                    EndSequence();
                }
            }
            else
            {
                EndSequence();
            }
        }

        protected override void EndSequence()
        {
            CompletedEvent?.Invoke();
            CompletedEvent = null;
            IsCompleted = true;
            IsRunning = false;
        }
    }

    public class ActionTSequencer<T> : AActionSequencer<IActionItem<T>>
    {
        public override bool IsItemLast => !IsCompleted && actions.Count == 0;

        #region ICompletion Realisation
        public override event Action CompletedEvent;

        public bool IsRunning { get; protected set; }
        #endregion ICompletion Realisation

        public override void Run()
        {
            IsRunning = true;
            base.Run();
        }

        protected override void RunNextAction()
        {
            if (!cancelled && actions != null && actions.Count > 0)
            {
                var item = actions.Dequeue();

                if (item != null)
                {
                    item.CompletedEvent += _ => RunNextAction();
                    item.Parent = this;
                    item.Run();
                }
                else
                {
                    EndSequence();
                }
            }
            else
            {
                EndSequence();
            }
        }

        protected override void EndSequence()
        {
            CompletedEvent?.Invoke();
            CompletedEvent = null;
            IsCompleted = true;
            IsRunning = false;
        }
    }
}