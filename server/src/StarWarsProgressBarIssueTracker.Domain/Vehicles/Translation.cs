using StarWarsProgressBarIssueTracker.Domain.Models;

namespace StarWarsProgressBarIssueTracker.Domain.Vehicles;

public class Translation : DomainBase
{
    public required string Country { get; set; }

    public required string Text { get; set; }
}
