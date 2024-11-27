// using AutoMapper;
// using Microsoft.EntityFrameworkCore;
// using StarWarsProgressBarIssueTracker.Domain;
// using StarWarsProgressBarIssueTracker.Domain.Vehicles;
// using StarWarsProgressBarIssueTracker.Infrastructure.Database;
// using StarWarsProgressBarIssueTracker.Infrastructure.Models;
// using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;
//
// namespace StarWarsProgressBarIssueTracker.App.Vehicles;
//
// public class AppearanceDataPort : IDataPort<Appearance>
// {
//     private readonly IAppearanceRepository _repository;
//     private readonly IMapper _mapper;
//
//     public AppearanceDataPort(IssueTrackerContext context, IAppearanceRepository repository, IMapper mapper)
//     {
//         _repository = repository;
//         _repository.Context = context;
//         _mapper = mapper;
//     }
//
//     public async Task<Appearance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
//     {
//         DbAppearance? dbAppearance = await _repository.GetByIdAsync(id, cancellationToken);
//         return _mapper.Map<Appearance?>(dbAppearance);
//     }
//
//     public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
//     {
//         return await _repository.ExistsAsync(id, cancellationToken);
//     }
//
//     public async Task<IEnumerable<Appearance>> GetAllAsync(CancellationToken cancellationToken = default)
//     {
//         List<DbAppearance> dbAppearances = await _repository.GetAll().ToListAsync(cancellationToken);
//
//         return _mapper.Map<IEnumerable<Appearance>>(dbAppearances);
//     }
//
//     public async Task<Appearance> AddAsync(Appearance domain, CancellationToken cancellationToken = default)
//     {
//         var addedDbAppearance = await _repository.AddAsync(_mapper.Map<DbAppearance>(domain), cancellationToken);
//
//         return _mapper.Map<Appearance>(addedDbAppearance);
//     }
//
//     public async Task AddRangeAsync(IEnumerable<Appearance> domains, CancellationToken cancellationToken = default)
//     {
//         var dbAppearances = _mapper.Map<IEnumerable<DbAppearance>>(domains);
//         await _repository.AddRangeAsync(dbAppearances, cancellationToken);
//     }
//
//
//     public async Task<Appearance> UpdateAsync(Appearance domain, CancellationToken cancellationToken = default)
//     {
//         DbAppearance dbAppearance = (await _repository.GetByIdAsync(domain.Id, cancellationToken))!;
//
//         dbAppearance.Title = domain.Title;
//         dbAppearance.Description = domain.Description;
//         dbAppearance.Color = domain.Color;
//         dbAppearance.TextColor = domain.TextColor;
//
//         DbAppearance updatedAppearance = await _repository.UpdateAsync(dbAppearance, cancellationToken);
//
//         return _mapper.Map<Appearance>(updatedAppearance);
//     }
//
//     public async Task<Appearance> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
//     {
//         DbAppearance appearance = (await _repository.GetByIdAsync(id, cancellationToken))!;
//
//         return _mapper.Map<Appearance>(await _repository.DeleteAsync(appearance, cancellationToken));
//     }
//
//     public async Task DeleteRangeByGitlabIdAsync(IEnumerable<Appearance> domains, CancellationToken cancellationToken = default)
//     {
//         var appearances = await _repository.GetAll().ToListAsync(cancellationToken);
//         var toBeDeleted = appearances.Where(dbAppearance => domains.Any(label => label.GitlabId?.Equals(dbAppearance.GitlabId) ?? false));
//         await _repository.DeleteRangeAsync(toBeDeleted, cancellationToken);
//     }
// }
