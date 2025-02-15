using System.Text.RegularExpressions;
using HotChocolate.Pagination;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Labels;

public partial class LabelService(ILabelRepository labelRepository, ILabelByIdDataLoader labelByIdDataLoader)
    : ILabelService
{
    public async Task<Page<Label>> GetAllLabelsAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await labelRepository.GetAllAsync(pagingArguments, cancellationToken);
    }

    public async Task<Label?> GetLabelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await labelByIdDataLoader.LoadAsync(id, cancellationToken);
        }
        catch (GreenDonut.KeyNotFoundException)
        {
            throw new DomainIdNotFoundException(nameof(Label), id.ToString());
        }
    }

    public async Task<Label> AddLabelAsync(Label label, CancellationToken cancellationToken = default)
    {
        ValidateLabel(label);

        return await labelRepository.AddAsync(label, cancellationToken);
    }

    private static void ValidateLabel(Label label)
    {
        List<Exception> errors = [];
        if (string.IsNullOrWhiteSpace(label.Title))
        {
            errors.Add(new ValueNotSetException(nameof(Label.Title)));
        }

        if (label.Title.Length < LabelConstants.MinTitleLength)
        {
            errors.Add(new StringTooShortException(label.Title, nameof(Label.Title),
                $"The length of {nameof(Label.Title)} has to be between {LabelConstants.MinTitleLength} and {LabelConstants.MaxTitleLength}."));
        }

        if (label.Title.Length > LabelConstants.MaxTitleLength)
        {
            errors.Add(new StringTooLongException(label.Title, nameof(Label.Title),
                $"The length of {nameof(Label.Title)} has to be between {LabelConstants.MinTitleLength} and {LabelConstants.MaxTitleLength}."));
        }

        if (label.Description is not null && label.Description.Length > LabelConstants.MaxDescriptionLength)
        {
            errors.Add(new StringTooLongException(label.Description, nameof(Label.Description),
                $"The length of {nameof(Label.Description)} has to be less than {LabelConstants.MaxDescriptionLength + 1}."));
        }

        if (string.IsNullOrWhiteSpace(label.Color))
        {
            errors.Add(new ValueNotSetException(nameof(Appearance.Color)));
        }

        Regex regexMatcher = ColorHexCodeRegex();
        if (!regexMatcher.Match(label.Color).Success)
        {
            errors.Add(new ColorFormatException(label.Color, nameof(Label.Color)));
        }

        if (string.IsNullOrWhiteSpace(label.TextColor))
        {
            errors.Add(new ValueNotSetException(nameof(Label.TextColor)));
        }

        if (!regexMatcher.Match(label.TextColor).Success)
        {
            errors.Add(new ColorFormatException(label.TextColor, nameof(Label.TextColor)));
        }

        if (errors.Count != 0)
        {
            throw new AggregateException(errors);
        }
    }

    public async Task<Label> UpdateLabelAsync(Label label, CancellationToken cancellationToken = default)
    {
        ValidateLabel(label);

        if (!(await labelRepository.ExistsAsync(label.Id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Label), label.Id.ToString());
        }

        return await labelRepository.UpdateAsync(label, cancellationToken);
    }

    public async Task<Label> DeleteLabelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await labelRepository.ExistsAsync(id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Label), id.ToString());
        }

        return await labelRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task SynchronizeFromGitlabAsync(IList<Label> labels, CancellationToken cancellationToken = default)
    {
        // var existingLabels = await labelRepository.GetAllAsync(cancellationToken);
        //
        // var labelsToAdd = labels.Where(label =>
        //     !existingLabels.Any(existingLabel => label.GitlabId!.Equals(existingLabel.GitlabId)));
        //
        // var labelsToDelete = existingLabels.Where(existingLabel => existingLabel.GitlabId != null &&
        //     !labels.Any(label => label.GitlabId!.Equals(existingLabel.GitlabId)));
        //
        // await labelRepository.AddRangeAsync(labelsToAdd, cancellationToken);
        //
        // await labelRepository.DeleteRangeByGitlabIdAsync(labelsToDelete, cancellationToken);

        await Task.CompletedTask;
        // TODO: Update label, resolve conflicts
    }

    [GeneratedRegex("^#[a-fA-F0-9]{6}$")]
    private static partial Regex ColorHexCodeRegex();
}
