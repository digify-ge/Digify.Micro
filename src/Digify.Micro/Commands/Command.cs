namespace Digify.Micro.Commands
{
    public interface ICommand { }
    public interface ICommand<out TResult> { }

    public abstract class Command : ICommand
    {
        public virtual T[] Validate<T>()
        {
            return null;
        }
    }

    public abstract class CommandWithResult<TResult> : ICommand<TResult>
    {
        public virtual T[] Validate<T>()
        {
            return null;
        }
    }
}
