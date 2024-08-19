namespace StarWarsProgressBarIssueTracker.Domain.Models;

public abstract class DomainBase
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }
}
