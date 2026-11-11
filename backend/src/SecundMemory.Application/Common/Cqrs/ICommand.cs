namespace SecundMemory.Application.Common.Cqrs;

public interface ICommand<TResponse>;

public interface ICommand : ICommand<Unit>;
