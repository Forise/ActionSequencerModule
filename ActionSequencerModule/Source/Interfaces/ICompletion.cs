namespace ActionSequencerModule.Interfaces
{
    public interface ICompletion
    {
        event System.Action CompletedEvent;
        bool IsCompleted { get; }
    }

    public interface ICompletion<out T>
    {
        event System.Action<T> CompletedEvent;
        bool IsCompleted { get; }
    }
}