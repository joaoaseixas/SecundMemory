using System.Net.Http.Headers;
using System.Net.Http.Json;
using SecundMemory.Application.Features.Auth.Commands.Register;
using SecundMemory.Application.Features.Auth.Dtos;
using SecundMemory.Application.Features.Journals.Dtos;

namespace SecundMemory.Api.IntegrationTests;

public class JournalsFlowTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task OpenToday_TwiceInARow_ReturnsTheSameJournal()
    {
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var first = await (await client.PostAsync("/api/journals/today", null)).Content.ReadFromJsonAsync<JournalDto>();
        var second = await (await client.PostAsync("/api/journals/today", null)).Content.ReadFromJsonAsync<JournalDto>();

        Assert.Equal(first!.PageId, second!.PageId);
    }

    [Fact]
    public async Task OpenByDate_NoEntryYet_CreatesOneInsteadOf404()
    {
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var response = await client.PostAsync("/api/journals/2026-07-04", null);
        response.EnsureSuccessStatusCode();
        var journal = await response.Content.ReadFromJsonAsync<JournalDto>();

        Assert.Equal(new DateOnly(2026, 7, 4), journal!.Date);
        Assert.Equal("2026-07-04", journal.PageTitle);
    }

    [Fact]
    public async Task OpenByDate_ThenGetByDate_FindsTheSameJournal()
    {
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var opened = await (await client.PostAsync("/api/journals/2026-03-15", null)).Content.ReadFromJsonAsync<JournalDto>();
        var fetched = await (await client.GetAsync("/api/journals/2026-03-15")).Content.ReadFromJsonAsync<JournalDto>();

        Assert.Equal(opened!.PageId, fetched!.PageId);
    }

    private static async Task AuthenticateAsync(HttpClient client)
    {
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterCommand($"{Guid.NewGuid()}@test.com", "Integration Test", "password123"));
        registerResponse.EnsureSuccessStatusCode();
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResultDto>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
    }
}
