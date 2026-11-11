using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SecundMemory.Application.Features.Auth.Commands.Register;
using SecundMemory.Application.Features.Auth.Dtos;
using SecundMemory.Application.Features.Blocks.Dtos;
using SecundMemory.Application.Features.Pages.Dtos;
using SecundMemory.Api.Contracts.Blocks;
using SecundMemory.Api.Contracts.Pages;

namespace SecundMemory.Api.IntegrationTests;

/// <summary>
/// End-to-end through the real HTTP pipeline (routing, JWT auth, the custom
/// dispatcher, EF Core) — the thing no unit test, however thorough, actually
/// proves: that a request for a real use case works all the way through.
/// </summary>
public class AuthAndPagesFlowTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task Register_ThenCreatePage_ThenCreateBlock_ThenReadBackTree()
    {
        var client = factory.CreateClient();

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterCommand($"{Guid.NewGuid()}@test.com", "Integration Test", "password123"));
        registerResponse.EnsureSuccessStatusCode();
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResultDto>();
        Assert.NotNull(auth);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var createPageResponse = await client.PostAsJsonAsync("/api/pages", new CreatePageRequest("Integration Page"));
        createPageResponse.EnsureSuccessStatusCode();
        var page = await createPageResponse.Content.ReadFromJsonAsync<PageDto>();
        Assert.NotNull(page);

        var createBlockResponse = await client.PostAsJsonAsync("/api/blocks", new CreateBlockRequest(page!.Id, null, "Hello from the integration test"));
        createBlockResponse.EnsureSuccessStatusCode();

        var treeResponse = await client.GetAsync($"/api/pages/{page.Id}/blocks");
        treeResponse.EnsureSuccessStatusCode();
        var tree = await treeResponse.Content.ReadFromJsonAsync<List<BlockTreeNodeDto>>();

        Assert.Single(tree!);
        Assert.Equal("Hello from the integration test", tree![0].Block.Content);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/pages");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_SamePasswordTwice_SecondReturns400()
    {
        var client = factory.CreateClient();
        var email = $"{Guid.NewGuid()}@test.com";
        await client.PostAsJsonAsync("/api/auth/register", new RegisterCommand(email, "First", "password123"));

        var secondResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterCommand(email, "Second", "password123"));

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }
}
