using System.Threading.Tasks;

namespace Digify.Micro.Commands
{
    public interface ICommandHandlerAsync<TCommand, TResult> where TCommand : ICommand
    {
        Task<TResult> HandleAsync(TCommand command);
    }

    public interface ICommandHandlerAsync<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }
}