using System.Threading.Tasks;

namespace ActionSequencerModule.Interfaces
{
    public interface IActionItem : ICompletion
    {
        IActionParent Parent { get; set; }

        void Run();
    }

    public interface IActionItem<out T> : ICompletion<T>
    {
        IActionParent Parent { get; set; }

        void Run();
    }
    
    public interface IAsyncActionItem : ICompletion
    {
        IActionParent Parent { get; set; }

        Task Run();
    }
    

    public interface IActionParent
    {
        
    }
}