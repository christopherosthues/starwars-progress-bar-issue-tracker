namespace StarWarsProgressBarIssueTracker.Infrastructure.Entities;

public abstract record DbEntityBase
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }
}
