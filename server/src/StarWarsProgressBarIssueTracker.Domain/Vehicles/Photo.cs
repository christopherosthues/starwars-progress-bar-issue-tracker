using StarWarsProgressBarIssueTracker.Domain.Models;

namespace StarWarsProgressBarIssueTracker.Domain.Vehicles;

public class Photo : DomainBase
{
    public required string FilePath { get; set; }
}
