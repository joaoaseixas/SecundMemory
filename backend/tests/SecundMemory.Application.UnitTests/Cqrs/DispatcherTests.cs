using Microsoft.Extensions.DependencyInjection;
using SecundMemory.Application.Common.Cqrs;

namespace SecundMemory.Application.UnitTests.Cqrs;

public class DispatcherTests
{
    private record PingCommand(string Message) : ICommand<string>;

    private class PingHandler : ICommandHandler<PingCommand, string>
    {
        public Task<string> Handle(PingCommand command, CancellationToken cancellationToken) =>
            Task.FromResult($"pong:{command.Message}");
    }

    private class TrackingBehavior(List<string> log, string name) : IPipelineBehavior<PingCommand, string>
    {
        public async Task<string> Handle(PingCommand request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
        {
            log.Add($"{name}:before");
            var result = await next();
            log.Add($"{name}:after");
            return result;
        }
    }

    [Fact]
    public async Task Send_RoutesToTheRegisteredHandler()
    {
        var services = new ServiceCollection();
        services.AddScoped<ICommandHandler<PingCommand, string>, PingHandler>();
        services.AddScoped<IDispatcher, Dispatcher>();
        var provider = services.BuildServiceProvider();
        var sut = provider.GetRequiredService<IDispatcher>();

        var result = await sut.Send(new PingCommand("hello"));

        Assert.Equal("pong:hello", result);
    }

    [Fact]
    public async Task Send_RunsBehaviorsInRegistrationOrder_WrappingTheHandler()
    {
        var log = new List<string>();
        var services = new ServiceCollection();
        services.AddScoped<ICommandHandler<PingCommand, string>, PingHandler>();
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IPipelineBehavior<PingCommand, string>>(_ => new TrackingBehavior(log, "first"));
        services.AddScoped<IPipelineBehavior<PingCommand, string>>(_ => new TrackingBehavior(log, "second"));
        var provider = services.BuildServiceProvider();
        var sut = provider.GetRequiredService<IDispatcher>();

        await sut.Send(new PingCommand("x"));

        Assert.Equal(["first:before", "second:before", "second:after", "first:after"], log);
    }
}
