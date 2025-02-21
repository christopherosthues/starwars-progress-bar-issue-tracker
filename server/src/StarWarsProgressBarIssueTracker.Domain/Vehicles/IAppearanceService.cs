using GreenDonut.Data;

namespace StarWarsProgressBarIssueTracker.Domain.Vehicles;

public interface IAppearanceService
{
    Task<Page<Appearance>> GetAllAppearancesAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default);

    Task<Appearance?> GetAppearanceAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Appearance> AddAppearanceAsync(Appearance appearance, CancellationToken cancellationToken = default);

    Task<Appearance> UpdateAppearanceAsync(Appearance appearance, CancellationToken cancellationToken = default);

    Task<Appearance> DeleteAppearanceAsync(Guid id, CancellationToken cancellationToken = default);

    Task SynchronizeFromGitlabAsync(IList<Appearance> appearances, CancellationToken cancellationToken = default);
}
