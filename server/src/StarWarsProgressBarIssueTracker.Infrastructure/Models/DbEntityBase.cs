namespace StarWarsProgressBarIssueTracker.Infrastructure.Models;

public abstract record DbEntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }
}
