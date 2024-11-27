using HotChocolate.Pagination;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using KeyNotFoundException = GreenDonut.KeyNotFoundException;

namespace StarWarsProgressBarIssueTracker.App.Milestones;

public class MilestoneService(
    IMilestoneRepository milestoneRepository,
    IMilestoneByIdDataLoader milestoneByIdDataLoader) : IMilestoneService
{
    public async Task<Page<Milestone>> GetAllMilestonesAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await milestoneRepository.GetAllAsync(pagingArguments, cancellationToken);
    }

    public async Task<Milestone?> GetMilestoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await milestoneByIdDataLoader.LoadAsync(id, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            throw new DomainIdNotFoundException(nameof(Milestone), id.ToString());
        }
    }

    public async Task<Milestone> AddMilestoneAsync(Milestone milestone, CancellationToken cancellationToken = default)
    {
        ValidateMilestone(milestone);

        return await milestoneRepository.AddAsync(milestone, cancellationToken);
    }

    private static void ValidateMilestone(Milestone milestone)
    {
        var errors = new List<Exception>();
        if (string.IsNullOrWhiteSpace(milestone.Title))
        {
            errors.Add(new ValueNotSetException(nameof(Milestone.Title)));
        }

        if (milestone.Title.Length < 1)
        {
            errors.Add(new StringTooShortException(milestone.Title, nameof(Milestone.Title),
                $"The length of {nameof(Milestone.Title)} has to be between {MilestoneConstants.MinTitleLength} and {MilestoneConstants.MaxTitleLength}."));
        }

        if (milestone.Title.Length > MilestoneConstants.MaxTitleLength)
        {
            errors.Add(new StringTooLongException(milestone.Title, nameof(Milestone.Title),
                $"The length of {nameof(Milestone.Title)} has to be between {MilestoneConstants.MinTitleLength} and {MilestoneConstants.MaxTitleLength}."));
        }

        if (milestone.Description is not null && milestone.Description.Length > MilestoneConstants.MaxDescriptionLength)
        {
            errors.Add(new StringTooLongException(milestone.Description, nameof(Milestone.Description),
                $"The length of {nameof(Milestone.Description)} has to be less than {MilestoneConstants.MaxDescriptionLength + 1}."));
        }

        if (!Enum.IsDefined(milestone.State) || milestone.State == MilestoneState.Unknown)
        {
            errors.Add(new ValueNotSetException(nameof(Milestone.State)));
        }

        if (errors.Count != 0)
        {
            throw new AggregateException(errors);
        }
    }

    public async Task<Milestone> UpdateMilestoneAsync(Milestone milestone,
        CancellationToken cancellationToken = default)
    {
        ValidateMilestone(milestone);

        if (!(await milestoneRepository.ExistsAsync(milestone.Id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Milestone), milestone.Id.ToString());
        }

        return await milestoneRepository.UpdateAsync(milestone, cancellationToken);
    }

    public async Task<Milestone> DeleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await milestoneRepository.ExistsAsync(id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Milestone), id.ToString());
        }

        return await milestoneRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task SynchronizeFromGitlabAsync(IList<Milestone> milestones,
        CancellationToken cancellationToken = default)
    {
        // var existingMilestones = await milestoneRepository.GetAllAsync(cancellationToken);
        //
        // var milestonesToAdd = milestones.Where(milestone =>
        //     !existingMilestones.Any(existingMilestone => milestone.GitlabId!.Equals(existingMilestone.GitlabId)));
        //
        // var milestonesToDelete = existingMilestones.Where(existingMilestone => existingMilestone.GitlabId != null &&
        //                                                            !milestones.Any(milestone => milestone.GitlabId!.Equals(existingMilestone.GitlabId)));
        //
        // await milestoneRepository.AddRangeAsync(milestonesToAdd, cancellationToken);
        //
        // await milestoneRepository.DeleteRangeByGitlabIdAsync(milestonesToDelete, cancellationToken);

        await Task.CompletedTask;

        // TODO: Update milestone, resolve conflicts
    }
}
