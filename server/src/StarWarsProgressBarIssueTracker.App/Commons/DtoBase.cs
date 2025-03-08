namespace StarWarsProgressBarIssueTracker.App.Commons;

public class DtoBase
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }
}
