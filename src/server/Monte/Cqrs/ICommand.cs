using MediatR;

namespace Monte.Cqrs;

public interface ICommand : IRequest
{
}

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}