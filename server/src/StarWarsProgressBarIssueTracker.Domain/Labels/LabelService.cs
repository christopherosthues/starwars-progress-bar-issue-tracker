using System.Text.RegularExpressions;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Domain.Labels;

public partial class LabelService(IDataPort<Label> dataPort) : ILabelService
{
    public async Task<IEnumerable<Label>> GetAllLabelsAsync(CancellationToken cancellationToken = default)
    {
        return await dataPort.GetAllAsync(cancellationToken);
    }

    public async Task<Label?> GetLabelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dataPort.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Label> AddLabelAsync(Label label, CancellationToken cancellationToken = default)
    {
        ValidateLabel(label);

        return await dataPort.AddAsync(label, cancellationToken);
    }

    private static void ValidateLabel(Label label)
    {
        var errors = new List<Exception>();
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

        var regexMatcher = ColorHexCodeRegex();
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

        if (!(await dataPort.ExistsAsync(label.Id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Label), label.Id.ToString());
        }

        return await dataPort.UpdateAsync(label, cancellationToken);
    }

    public async Task<Label> DeleteLabelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await dataPort.ExistsAsync(id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Label), id.ToString());
        }

        return await dataPort.DeleteAsync(id, cancellationToken);
    }

    public async Task SynchronizeFromGitlabAsync(IList<Label> labels, CancellationToken cancellationToken = default)
    {
        var existingLabels = await dataPort.GetAllAsync(cancellationToken);

        var labelsToAdd = labels.Where(label =>
            !existingLabels.Any(existingLabel => label.GitlabId!.Equals(existingLabel.GitlabId)));

        var labelsToDelete = existingLabels.Where(existingLabel => existingLabel.GitlabId != null &&
            !labels.Any(label => label.GitlabId!.Equals(existingLabel.GitlabId)));

        await dataPort.AddRangeAsync(labelsToAdd, cancellationToken);

        await dataPort.DeleteRangeByGitlabIdAsync(labelsToDelete, cancellationToken);

        // TODO: Update label, resolve conflicts
    }

    [GeneratedRegex("^#[a-fA-F0-9]{6}$")]
    private static partial Regex ColorHexCodeRegex();
}
