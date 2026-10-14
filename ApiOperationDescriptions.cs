using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RaceDay.Api;

/// <summary>Adds human-readable purpose and common HTTP outcomes to the generated Swagger operations.</summary>
public sealed class ApiOperationDescriptions : IOperationFilter
{
    private static readonly Dictionary<string, string> Descriptions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["AuthController.Register"] = "Creates a Participant or Organiser account. Passwords are hashed before storage; duplicate email addresses are rejected.",
        ["AuthController.Login"] = "Checks the supplied email and password and creates a server-side session containing the user ID and role.",
        ["AuthController.Logout"] = "Clears the current authenticated session.",
        ["ProfileController.Get"] = "Returns the current signed-in user's profile without exposing password data.",
        ["ProfileController.Update"] = "Updates the current signed-in user's name, email address and phone number.",
        ["ProfileController.ChangePassword"] = "Verifies the current password and stores a hash of the new password.",
        ["OrganisersController.GetAll"] = "Returns organiser directory information without password fields.",
        ["OrganisersController.Get"] = "Returns one organiser's public profile and associated events.",
        ["OrganisersController.Create"] = "Creates an organiser account with a hashed password.",
        ["OrganisersController.Update"] = "Updates the authenticated Organiser's own profile; users cannot update another account.",
        ["OrganisersController.Delete"] = "Deletes the authenticated Organiser's own account when database relationships allow it.",
        ["ParticipantsController.GetAll"] = "Returns participant directory information for Organisers only.",
        ["ParticipantsController.Get"] = "Returns a participant profile to that Participant or an authenticated Organiser.",
        ["ParticipantsController.Create"] = "Creates a Participant account with a hashed password.",
        ["ParticipantsController.Update"] = "Updates the authenticated Participant's own profile.",
        ["ParticipantsController.Delete"] = "Deletes a participant account when database relationships allow it.",
        ["EventsController.GetAll"] = "Lists events for browsing, ordered by event date.",
        ["EventsController.Get"] = "Returns an event with its categories and saved weather records.",
        ["EventsController.Create"] = "Creates an event owned by the authenticated Organiser.",
        ["EventsController.Update"] = "Updates an event only when it belongs to the authenticated Organiser.",
        ["EventsController.Delete"] = "Deletes an event when related database records do not prevent deletion.",
        ["EventsController.Enrolments"] = "Lists enrolments for an event owned by the authenticated Organiser.",
        ["CategoriesController.GetAll"] = "Lists event categories.",
        ["CategoriesController.Get"] = "Returns one category by its identifier.",
        ["CategoriesController.Create"] = "Creates a category for an event owned by the authenticated Organiser.",
        ["CategoriesController.Update"] = "Updates a category belonging to an event owned by the authenticated Organiser.",
        ["CategoriesController.Delete"] = "Deletes a category when no registration depends on it.",
        ["WeatherController.Get"] = "Returns stored weather records, optionally filtered by event ID or location. This endpoint does not fetch live external weather.",
        ["WeatherController.Forecast"] = "Returns saved weather records for an event and optional date.",
        ["WeatherController.CreateAlert"] = "Records a weather alert description against an event using the existing WEATHER table.",
        ["RegistrationsController.GetAll"] = "Lists registrations for events owned by the authenticated Organiser.",
        ["RegistrationsController.Get"] = "Returns a registration only to its Participant or the Organiser who owns the event.",
        ["RegistrationsController.Create"] = "Enrols the authenticated Participant in an event and category; duplicate event registrations are rejected.",
        ["RegistrationsController.Update"] = "Updates an accessible registration; only Organisers may change its status.",
        ["RegistrationsController.Delete"] = "Cancels a registration for its Participant or the event's Organiser.",
        ["RegistrationsController.Enrol"] = "Alias route that enrols the authenticated Participant in an event and category.",
        ["RegistrationsController.CancelEnrolment"] = "Alias route that cancels an accessible enrolment.",
        ["ResultsController.GetAll"] = "Lists published race results without exposing account credentials.",
        ["ResultsController.MyResults"] = "Returns only the authenticated Participant's result history.",
        ["ResultsController.Get"] = "Returns a result to its Participant or the Organiser who owns the event.",
        ["ResultsController.Create"] = "Records a finish time and position for a registration belonging to the authenticated Organiser's event.",
        ["ResultsController.Update"] = "Updates a result for an event owned by the authenticated Organiser.",
        ["ResultsController.Delete"] = "Deletes a result for an event owned by the authenticated Organiser."
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var key = $"{context.MethodInfo.DeclaringType?.Name}.{context.MethodInfo.Name}";
        if (Descriptions.TryGetValue(key, out var description))
        {
            operation.Summary = key[(key.IndexOf('.') + 1)..];
            operation.Description = description;
        }
        operation.Responses.TryAdd("400", new OpenApiResponse { Description = "Bad Request - request validation failed." });
        operation.Responses.TryAdd("404", new OpenApiResponse { Description = "Not Found - the requested record does not exist." });
        var isAuthorized = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() || context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true;
        if (isAuthorized)
        {
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized - a valid session is required." });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden - the signed-in role or account does not have permission." });
        }
        operation.Responses.TryAdd("409", new OpenApiResponse { Description = "Conflict - the operation conflicts with existing data." });
    }
}
