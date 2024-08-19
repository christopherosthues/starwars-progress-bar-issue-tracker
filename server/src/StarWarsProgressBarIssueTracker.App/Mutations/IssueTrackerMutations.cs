using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations(
    IAppearanceService appearanceService,
    ILabelService labelService,
    IIssueService issueService,
    IMilestoneService milestoneService,
    IReleaseService releaseService);
