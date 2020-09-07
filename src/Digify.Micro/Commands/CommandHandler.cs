using System.Threading.Tasks;

namespace Digify.Micro.Commands
{
    public interface ICommandHandlerAsync<TCommand, TResult> where TCommand : CommandWithResult<TResult>
    {
        Task<TResult> HandleAsync(TCommand command);
    }

    public interface ICommandHandlerAsync<in TCommand> where TCommand : Command
    {
        Task HandleAsync(TCommand command);
    }
}