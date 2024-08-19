namespace StarWarsProgressBarIssueTracker.Domain.Vehicles;

public interface IAppearanceService
{
    Task<IEnumerable<Appearance>> GetAllAppearancesAsync(CancellationToken cancellationToken = default);

    Task<Appearance?> GetAppearanceAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Appearance> AddAppearanceAsync(Appearance appearance, CancellationToken cancellationToken = default);

    Task<Appearance> UpdateAppearanceAsync(Appearance appearance, CancellationToken cancellationToken = default);

    Task<Appearance> DeleteAppearanceAsync(Guid id, CancellationToken cancellationToken = default);

    Task SynchronizeFromGitlabAsync(IList<Appearance> appearances, CancellationToken cancellationToken = default);
}
