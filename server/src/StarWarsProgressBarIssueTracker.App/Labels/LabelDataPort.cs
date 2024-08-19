using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

namespace StarWarsProgressBarIssueTracker.App.Labels;

public class LabelDataPort : IDataPort<Label>
{
    private readonly IRepository<DbLabel> _repository;
    private readonly IMapper _mapper;

    public LabelDataPort(IssueTrackerContext context, IRepository<DbLabel> repository, IMapper mapper)
    {
        _repository = repository;
        _repository.Context = context;
        _mapper = mapper;
    }

    public async Task<Label?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        DbLabel? dbLabel = await _repository.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<Label?>(dbLabel);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Label>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<DbLabel> dbLabels = await _repository.GetAll().ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Label>>(dbLabels);
    }

    public async Task<Label> AddAsync(Label domain, CancellationToken cancellationToken = default)
    {
        var addedDbLabel = await _repository.AddAsync(_mapper.Map<DbLabel>(domain), cancellationToken);

        return _mapper.Map<Label>(addedDbLabel);
    }

    public async Task AddRangeAsync(IEnumerable<Label> domains, CancellationToken cancellationToken = default)
    {
        var dbLabels = _mapper.Map<IEnumerable<DbLabel>>(domains);
        await _repository.AddRangeAsync(dbLabels, cancellationToken);
    }

    public async Task<Label> UpdateAsync(Label domain, CancellationToken cancellationToken = default)
    {
        DbLabel dbLabel = (await _repository.GetByIdAsync(domain.Id, cancellationToken))!;

        dbLabel.Title = domain.Title;
        dbLabel.Description = domain.Description;
        dbLabel.Color = domain.Color;
        dbLabel.TextColor = domain.TextColor;

        DbLabel updatedLabel = await _repository.UpdateAsync(dbLabel, cancellationToken);

        return _mapper.Map<Label>(updatedLabel);
    }

    public async Task<Label> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        DbLabel label = (await _repository.GetByIdAsync(id, cancellationToken))!;

        return _mapper.Map<Label>(await _repository.DeleteAsync(label, cancellationToken));
    }

    public async Task DeleteRangeByGitlabIdAsync(IEnumerable<Label> domains, CancellationToken cancellationToken = default)
    {
        var labels = await _repository.GetAll().ToListAsync(cancellationToken);
        var toBeDeleted = labels.Where(dbLabel => domains.Any(label => label.GitlabId?.Equals(dbLabel.GitlabId) ?? false));
        await _repository.DeleteRangeAsync(toBeDeleted, cancellationToken);
    }
}
