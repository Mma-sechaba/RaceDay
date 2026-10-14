using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data;

public class RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : DbContext(options)
{
    public DbSet<EventOrganiser> Organisers => Set<EventOrganiser>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<RaceEvent> Events => Set<RaceEvent>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Weather> WeatherRecords => Set<Weather>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Match the Part 1 SQL script's VARCHAR columns instead of EF's default NVARCHAR.
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties()).Where(p => p.ClrType == typeof(string)))
            property.SetIsUnicode(false);
        modelBuilder.Entity<EventOrganiser>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Participant>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Registration>().HasIndex(x => new { x.ParticipantId, x.EventId }).IsUnique();
        modelBuilder.Entity<EventOrganiser>().HasMany(x => x.Events).WithOne(x => x.Organiser).HasForeignKey(x => x.OrganiserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RaceEvent>().HasMany(x => x.Categories).WithOne(x => x.Event).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RaceEvent>().HasMany(x => x.WeatherRecords).WithOne(x => x.Event).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Participant>().HasMany(x => x.Registrations).WithOne(x => x.Participant).HasForeignKey(x => x.ParticipantId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RaceEvent>().HasMany(x => x.Registrations).WithOne(x => x.Event).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Category>().HasMany(x => x.Registrations).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Registration>().HasMany(x => x.Results).WithOne(x => x.Registration).HasForeignKey(x => x.RegistrationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Participant>().Property(x => x.DateOfBirth).HasColumnType("date");
        modelBuilder.Entity<RaceEvent>().Property(x => x.EventDate).HasColumnType("date");
        modelBuilder.Entity<Weather>().Property(x => x.WeatherDate).HasColumnType("date");
        modelBuilder.Entity<EventOrganiser>().Property(x => x.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<Participant>().Property(x => x.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<Registration>().Property(x => x.RegistrationDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<RaceEvent>().Property(x => x.Status).HasDefaultValue("Available");
        modelBuilder.Entity<Registration>().Property(x => x.Status).HasDefaultValue("Pending");
    }
}
