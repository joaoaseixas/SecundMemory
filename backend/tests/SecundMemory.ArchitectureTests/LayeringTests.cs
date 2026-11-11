using System.Reflection;
using NetArchTest.Rules;

namespace SecundMemory.ArchitectureTests;

/// <summary>
/// Makes the Dependency Rule from ARCHITECTURE.md a build-time check instead
/// of something that only holds as long as nobody adds a stray `using`.
/// </summary>
public class LayeringTests
{
    private static readonly Assembly DomainAssembly = typeof(Domain.Common.Entity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(Application.Common.Cqrs.IDispatcher).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.Persistence.ApplicationDbContext).Assembly;
    private static readonly Assembly ApiAssembly = typeof(Api.Controllers.PagesController).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOnApplication()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot().HaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Domain_ShouldNotDependOnInfrastructureOrApi()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot().HaveDependencyOnAny(InfrastructureAssembly.GetName().Name, ApiAssembly.GetName().Name)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructureOrApi()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot().HaveDependencyOnAny(InfrastructureAssembly.GetName().Name, ApiAssembly.GetName().Name)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot().HaveDependencyOn(ApiAssembly.GetName().Name)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void CommandHandlers_ShouldBeNamedConsistently()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That().AreClasses().And().ImplementInterface(typeof(Application.Common.Cqrs.ICommandHandler<,>))
            .Should().HaveNameEndingWith("Handler")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void QueryHandlers_ShouldBeNamedConsistently()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That().AreClasses().And().ImplementInterface(typeof(Application.Common.Cqrs.IQueryHandler<,>))
            .Should().HaveNameEndingWith("Handler")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    private static string Describe(TestResult result) =>
        result.FailingTypes is null ? string.Empty : string.Join(", ", result.FailingTypes.Select(t => t.FullName));
}
