using StarWarsProgressBarIssueTracker.Domain.Exceptions;

namespace StarWarsProgressBarIssueTracker.Domain.Milestones;

public class MilestoneService(IDataPort<Milestone> dataPort) : IMilestoneService
{
    public async Task<IEnumerable<Milestone>> GetAllMilestonesAsync(CancellationToken cancellationToken = default)
    {
        return await dataPort.GetAllAsync(cancellationToken);
    }

    public async Task<Milestone?> GetMilestoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dataPort.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Milestone> AddMilestoneAsync(Milestone milestone, CancellationToken cancellationToken = default)
    {
        ValidateMilestone(milestone);

        return await dataPort.AddAsync(milestone, cancellationToken);
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

    public async Task<Milestone> UpdateMilestoneAsync(Milestone milestone, CancellationToken cancellationToken = default)
    {
        ValidateMilestone(milestone);

        if (!(await dataPort.ExistsAsync(milestone.Id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Milestone), milestone.Id.ToString());
        }

        return await dataPort.UpdateAsync(milestone, cancellationToken);
    }

    public async Task<Milestone> DeleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await dataPort.ExistsAsync(id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Milestone), id.ToString());
        }

        return await dataPort.DeleteAsync(id, cancellationToken);
    }

    public async Task SynchronizeFromGitlabAsync(IList<Milestone> milestones, CancellationToken cancellationToken = default)
    {
        var existingMilestones = await dataPort.GetAllAsync(cancellationToken);

        var milestonesToAdd = milestones.Where(milestone =>
            !existingMilestones.Any(existingMilestone => milestone.GitlabId!.Equals(existingMilestone.GitlabId)));

        var milestonesToDelete = existingMilestones.Where(existingMilestone => existingMilestone.GitlabId != null &&
                                                                   !milestones.Any(milestone => milestone.GitlabId!.Equals(existingMilestone.GitlabId)));

        await dataPort.AddRangeAsync(milestonesToAdd, cancellationToken);

        await dataPort.DeleteRangeByGitlabIdAsync(milestonesToDelete, cancellationToken);

        // TODO: Update milestone, resolve conflicts
    }
}
