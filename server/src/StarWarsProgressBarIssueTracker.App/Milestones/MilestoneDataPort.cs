using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

namespace StarWarsProgressBarIssueTracker.App.Milestones;

public class MilestoneDataPort : IDataPort<Milestone>
{
    private readonly IRepository<DbMilestone> _repository;
    private readonly IMapper _mapper;

    public MilestoneDataPort(IssueTrackerContext context, IRepository<DbMilestone> repository, IMapper mapper)
    {
        _repository = repository;
        _repository.Context = context;
        _mapper = mapper;
    }

    public async Task<Milestone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        DbMilestone? dbMilestone = await _repository.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<Milestone?>(dbMilestone);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Milestone>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<DbMilestone> dbMilestones = await _repository.GetAll().ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Milestone>>(dbMilestones);
    }

    public async Task<Milestone> AddAsync(Milestone domain, CancellationToken cancellationToken = default)
    {
        var addedDbMilestone = await _repository.AddAsync(_mapper.Map<DbMilestone>(domain), cancellationToken);

        return _mapper.Map<Milestone>(addedDbMilestone);
    }

    public async Task AddRangeAsync(IEnumerable<Milestone> domains, CancellationToken cancellationToken = default)
    {
        var dbMilestones = _mapper.Map<IEnumerable<DbMilestone>>(domains);
        await _repository.AddRangeAsync(dbMilestones, cancellationToken);
    }

    public async Task<Milestone> UpdateAsync(Milestone domain, CancellationToken cancellationToken = default)
    {
        DbMilestone dbMilestone = (await _repository.GetByIdAsync(domain.Id, cancellationToken))!;

        dbMilestone.Title = domain.Title;
        dbMilestone.Description = domain.Description;
        dbMilestone.State = domain.State;

        DbMilestone updatedMilestone = await _repository.UpdateAsync(dbMilestone, cancellationToken);

        return _mapper.Map<Milestone>(updatedMilestone);
    }

    public async Task<Milestone> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        DbMilestone milestone = (await _repository.GetByIdAsync(id, cancellationToken))!;

        milestone.Issues.Clear();

        await _repository.UpdateAsync(milestone, cancellationToken);

        return _mapper.Map<Milestone>(await _repository.DeleteAsync(milestone, cancellationToken));
    }

    public async Task DeleteRangeByGitlabIdAsync(IEnumerable<Milestone> domains, CancellationToken cancellationToken = default)
    {
        var milestones = await _repository.GetAll().ToListAsync(cancellationToken);
        var toBeDeleted = milestones.Where(dbMilestone => domains.Any(milestone => milestone.GitlabId?.Equals(dbMilestone.GitlabId) ?? false));
        await _repository.DeleteRangeAsync(toBeDeleted, cancellationToken);
    }
}
