namespace StarWarsProgressBarIssueTracker.Domain.Models;

public abstract class DomainBase
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }
}
