using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using RaceDay.Api.Data;
using Xunit;

namespace RaceDay.Api.Tests;

public class AuthenticationAndAccessTests : IClassFixture<RaceDayApiFactory>
{
    private readonly RaceDayApiFactory _factory;
    public AuthenticationAndAccessTests(RaceDayApiFactory factory) => _factory = factory;

    [Fact]
    public async Task RegisterAndLoginParticipant_CreatesSession()
    {
        using var client = _factory.CreateClient(new() { HandleCookies = true });
        var email = $"participant-{Guid.NewGuid():N}@example.com";
        var register = await client.PostAsJsonAsync("/api/auth/register", new { firstName = "Test", lastName = "Participant", email, password = "SecurePass123!", role = "Participant", dateOfBirth = "2001-05-12" });
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        var login = await client.PostAsJsonAsync("/api/auth/login", new { email, password = "SecurePass123!" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var profile = await client.GetAsync("/api/profile");
        Assert.Equal(HttpStatusCode.OK, profile.StatusCode);
    }

    [Fact]
    public async Task ParticipantCannotCreateEvent()
    {
        using var client = _factory.CreateClient(new() { HandleCookies = true });
        var email = $"participant-{Guid.NewGuid():N}@example.com";
        await client.PostAsJsonAsync("/api/auth/register", new { firstName = "Test", lastName = "Participant", email, password = "SecurePass123!", role = "Participant", dateOfBirth = "2001-05-12" });
        await client.PostAsJsonAsync("/api/auth/login", new { email, password = "SecurePass123!" });
        var response = await client.PostAsJsonAsync("/api/events", new { name = "Test Run", description = "Test", date = "2027-05-12", location = "Pretoria", distance = 5, eventType = "Run" });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task OrganiserCanCreateEvent_AndParticipantCanEnrol()
    {
        using var organiserClient = _factory.CreateClient(new() { HandleCookies = true });
        var organiserEmail = $"organiser-{Guid.NewGuid():N}@example.com";
        await organiserClient.PostAsJsonAsync("/api/auth/register", new { firstName = "Test", lastName = "Organiser", email = organiserEmail, password = "SecurePass123!", role = "Organiser" });
        await organiserClient.PostAsJsonAsync("/api/auth/login", new { email = organiserEmail, password = "SecurePass123!" });
        var eventResponse = await organiserClient.PostAsJsonAsync("/api/events", new { name = "Integration Run", description = "Test event", date = "2027-05-12", location = "Pretoria", distance = 5, eventType = "Run" });
        Assert.Equal(HttpStatusCode.Created, eventResponse.StatusCode);
        var eventData = await eventResponse.Content.ReadFromJsonAsync<EventDto>();
        Assert.NotNull(eventData);
        var categoryResponse = await organiserClient.PostAsJsonAsync("/api/categories", new { name = "Open 5km", minimumAge = 18, maximumAge = 80, distance = 5, eventId = eventData!.eventId });
        Assert.Equal(HttpStatusCode.Created, categoryResponse.StatusCode);
        var categoryData = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(categoryData);

        using var participantClient = _factory.CreateClient(new() { HandleCookies = true });
        var participantEmail = $"participant-{Guid.NewGuid():N}@example.com";
        await participantClient.PostAsJsonAsync("/api/auth/register", new { firstName = "Test", lastName = "Runner", email = participantEmail, password = "SecurePass123!", role = "Participant", dateOfBirth = "2001-05-12" });
        await participantClient.PostAsJsonAsync("/api/auth/login", new { email = participantEmail, password = "SecurePass123!" });
        var enrol = await participantClient.PostAsJsonAsync("/api/registrations", new { eventId = eventData!.eventId, categoryId = categoryData!.categoryId });
        Assert.Equal(HttpStatusCode.Created, enrol.StatusCode);
        var duplicate = await participantClient.PostAsJsonAsync("/api/registrations", new { eventId = eventData.eventId, categoryId = categoryData.categoryId });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }

    private sealed class EventDto { public int eventId { get; set; } }
    private sealed class CategoryDto { public int categoryId { get; set; } }
}
