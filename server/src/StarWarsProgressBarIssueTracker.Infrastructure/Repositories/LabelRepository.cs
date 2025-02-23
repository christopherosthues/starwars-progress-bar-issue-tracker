using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class LabelRepository(IssueTrackerContext context) : IssueTrackerRepositoryBase<Label>(context), ILabelRepository
{
    public override async Task<List<Label>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Milestone)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Release)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Photos)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Translations)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Labels)
            .OrderBy(label => label.Title)
            .ThenBy(label => label.Id)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Page<Label>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Milestone)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Release)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Photos)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Translations)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Labels)
            .OrderBy(label => label.Title)
            .ThenBy(label => label.Id)
            .ToPageAsync(pagingArguments, true, cancellationToken);
    }

    public IQueryable<Label> GetLabelByIds(IReadOnlyList<Guid> ids)
    {
        return DbSet
            .AsNoTracking()
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Milestone)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Release)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Photos)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Translations)
            .Include(dbLabel => dbLabel.Issues)
            .ThenInclude(dbIssue => dbIssue.Labels)
            .Where(label => ids.Contains(label.Id));
    }

    public override async Task<Label> UpdateAsync(Label entity, CancellationToken cancellationToken = default)
    {
        Label dbEntity = (await DbSet.FindAsync([entity.Id],  cancellationToken))!;
        dbEntity.Title = entity.Title;
        dbEntity.Description = entity.Description;
        dbEntity.Color = entity.Color;
        dbEntity.TextColor = entity.TextColor;
        await Context.SaveChangesAsync(cancellationToken);

        return (await DbSet.Include(label => label.Issues)
            .FirstAsync(label => label.Id == entity.Id, cancellationToken));
    }

    public async Task DeleteRangeByGitlabIdAsync(IEnumerable<Label> domains, CancellationToken cancellationToken = default)
    {
        // TODO: pasted from data port. Maybe further adjustments needed
        List<Label> labels = await DbSet.ToListAsync(cancellationToken);
        IEnumerable<Label> toBeDeleted = labels.Where(dbLabel => domains.Any(label => label.GitlabId?.Equals(dbLabel.GitlabId) ?? false));
        await DeleteRangeAsync(toBeDeleted, cancellationToken);
    }
}
