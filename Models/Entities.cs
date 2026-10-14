using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models;

[Table("EVENT_ORGANISER")]
public class EventOrganiser
{
    [Key] public int OrganiserId { get; set; }
    [Required, MaxLength(50)] public string FirstName { get; set; } = "";
    [Required, MaxLength(50)] public string LastName { get; set; } = "";
    [Required, MaxLength(100)] public string Email { get; set; } = "";
    [Required, MaxLength(255)] public string Password { get; set; } = "";
    [MaxLength(20)] public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ICollection<RaceEvent> Events { get; set; } = new List<RaceEvent>();
}

[Table("PARTICIPANT")]
public class Participant
{
    [Key] public int ParticipantId { get; set; }
    [Required, MaxLength(50)] public string FirstName { get; set; } = "";
    [Required, MaxLength(50)] public string LastName { get; set; } = "";
    [Required, MaxLength(100)] public string Email { get; set; } = "";
    [Required, MaxLength(255)] public string Password { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    [MaxLength(20)] public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}

[Table("EVENT")]
public class RaceEvent
{
    [Key] public int EventId { get; set; }
    public int OrganiserId { get; set; }
    [Required, MaxLength(100)] public string EventName { get; set; } = "";
    [MaxLength(500)] public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    [Required, MaxLength(150)] public string Location { get; set; } = "";
    [Column(TypeName = "decimal(6,2)")] public decimal Distance { get; set; }
    [Required, MaxLength(20)] public string EventType { get; set; } = "Run";
    [MaxLength(20)] public string Status { get; set; } = "Available";
    public EventOrganiser? Organiser { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Weather> WeatherRecords { get; set; } = new List<Weather>();
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}

[Table("CATEGORY")]
public class Category
{
    [Key] public int CategoryId { get; set; }
    public int EventId { get; set; }
    [Required, MaxLength(100)] public string CategoryName { get; set; } = "";
    public int MinimumAge { get; set; }
    public int MaximumAge { get; set; }
    [Column(TypeName = "decimal(6,2)")] public decimal Distance { get; set; }
    public RaceEvent? Event { get; set; }
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}

[Table("WEATHER")]
public class Weather
{
    [Key] public int WeatherId { get; set; }
    public int EventId { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal? Temperature { get; set; }
    [MaxLength(100)] public string? WeatherCondition { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal? WindSpeed { get; set; }
    public DateTime WeatherDate { get; set; }
    public RaceEvent? Event { get; set; }
}

[Table("REGISTRATION")]
public class Registration
{
    [Key] public int RegistrationId { get; set; }
    public int ParticipantId { get; set; }
    public int EventId { get; set; }
    public int CategoryId { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    [MaxLength(20)] public string Status { get; set; } = "Pending";
    public Participant? Participant { get; set; }
    public RaceEvent? Event { get; set; }
    public Category? Category { get; set; }
    public ICollection<Result> Results { get; set; } = new List<Result>();
}

[Table("RESULT")]
public class Result
{
    [Key] public int ResultId { get; set; }
    public int RegistrationId { get; set; }
    public TimeSpan FinishTime { get; set; }
    public int Position { get; set; }
    public Registration? Registration { get; set; }
}
