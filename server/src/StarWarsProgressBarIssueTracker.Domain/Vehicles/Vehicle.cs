using StarWarsProgressBarIssueTracker.Domain.Models;

namespace StarWarsProgressBarIssueTracker.Domain.Vehicles;

public class Vehicle : DomainBase
{
    public IList<Appearance> Appearances { get; set; } = [];

    public EngineColor EngineColor { get; set; }

    public IList<Translation> Translations { get; set; } = [];

    public IList<Photo> Photos { get; set; } = [];
}
