namespace Digify.Micro.Commands
{
    public interface ICommand { }
    public interface ICommand<out TResult> { }
}
