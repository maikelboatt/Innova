namespace Innova.Application.Abstractions.Messaging
{
    public interface ICommand<TResult>
    {
    }

    public interface ICommand:ICommand<Unit>
    {
    }
}
