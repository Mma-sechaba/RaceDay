using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public class RegisterRequest
{
    [Required, MaxLength(50)] public string FirstName { get; set; } = "";
    [Required, MaxLength(50)] public string LastName { get; set; } = "";
    [Required, EmailAddress, MaxLength(100)] public string Email { get; set; } = "";
    [Required, MinLength(8)] public string Password { get; set; } = "";
    [Required] public string Role { get; set; } = "Participant";
    public DateTime? DateOfBirth { get; set; }
    [MaxLength(20)] public string? Phone { get; set; }
}
public class LoginRequest { [Required, EmailAddress] public string Email { get; set; } = ""; [Required] public string Password { get; set; } = ""; }
public class ProfileUpdateRequest { [Required, MaxLength(50)] public string FirstName { get; set; } = ""; [Required, MaxLength(50)] public string LastName { get; set; } = ""; [Required, EmailAddress, MaxLength(100)] public string Email { get; set; } = ""; [MaxLength(20)] public string? Phone { get; set; } }
public class ChangePasswordRequest { [Required] public string CurrentPassword { get; set; } = ""; [Required, MinLength(8)] public string NewPassword { get; set; } = ""; }
public class OrganiserCreateRequest
{
    [Required, MaxLength(50)] public string FirstName { get; set; } = "";
    [Required, MaxLength(50)] public string LastName { get; set; } = "";
    [Required, EmailAddress, MaxLength(100)] public string Email { get; set; } = "";
    [Required, MinLength(8)] public string Password { get; set; } = "";
    [MaxLength(20)] public string? Phone { get; set; }
}
public class ParticipantCreateRequest
{
    [Required, MaxLength(50)] public string FirstName { get; set; } = "";
    [Required, MaxLength(50)] public string LastName { get; set; } = "";
    [Required, EmailAddress, MaxLength(100)] public string Email { get; set; } = "";
    [Required, MinLength(8)] public string Password { get; set; } = "";
    [Required] public DateTime DateOfBirth { get; set; }
    [MaxLength(20)] public string? Phone { get; set; }
}
public class EventRequest { [Required, MaxLength(100)] public string Name { get; set; } = ""; [MaxLength(500)] public string? Description { get; set; } [Required] public DateTime Date { get; set; } [Required, MaxLength(150)] public string Location { get; set; } = ""; [Range(0.01, 9999)] public decimal Distance { get; set; } [Required] public string EventType { get; set; } = "Run"; public int? OrganiserId { get; set; } }
public class CategoryRequest { [Required, MaxLength(100)] public string Name { get; set; } = ""; [Range(0, 120)] public int MinimumAge { get; set; } [Range(0, 120)] public int MaximumAge { get; set; } [Range(0.01, 9999)] public decimal Distance { get; set; } [Range(1, int.MaxValue)] public int EventId { get; set; } }
public class RegistrationRequest { [Range(1, int.MaxValue)] public int EventId { get; set; } [Range(1, int.MaxValue)] public int CategoryId { get; set; } public int? ParticipantId { get; set; } }
public class RegistrationUpdateRequest { [Range(1, int.MaxValue)] public int? CategoryId { get; set; } [MaxLength(20)] public string? Status { get; set; } }
public class ResultRequest { [Range(1, int.MaxValue)] public int RegistrationId { get; set; } [Required] public TimeSpan FinishTime { get; set; } [Range(1, int.MaxValue)] public int Position { get; set; } }
public class ResultUpdateRequest { [Required] public TimeSpan FinishTime { get; set; } [Range(1, int.MaxValue)] public int Position { get; set; } }
public class WeatherAlertRequest { [Range(1, int.MaxValue)] public int EventId { get; set; } [Required, MaxLength(100)] public string AlertType { get; set; } = ""; [Required, MaxLength(100)] public string Message { get; set; } = ""; }
