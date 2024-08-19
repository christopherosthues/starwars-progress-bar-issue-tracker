// using StarWarsProgressBarIssueTracker.Domain.Issues;
// using StarWarsProgressBarIssueTracker.Domain.Milestones;
// using IssueState = StarWarsVehiclesTracker.GraphQL.IssueState;
// using Label = StarWarsVehiclesTracker.Labels.Models.Label;
//
// namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Mapper;
//
// public class MilestoneMapper
// {
//     private readonly LabelMapper _labelMapper;
//
//     public MilestoneMapper(LabelMapper labelMapper)
//     {
//         _labelMapper = labelMapper ?? throw new ArgumentNullException(nameof(labelMapper));
//     }
//
//     public Milestone CreateMilestone(IGetMilestones_Project_Milestones_Nodes node, string projectId)
//     {
//         return new Milestone
//         {
//             ProjectId = projectId,
//             Id = node.Id,
//             Iid = node.Iid,
//             Title = node.Title,
//             MilestoneState = MapState(node.State),
//             ClosedIssuesCount = node.Stats?.ClosedIssuesCount ?? 0,
//             TotalIssuesCount = node.Stats?.TotalIssuesCount ?? 0
//         };
//     }
//
//     public Milestone CreateMilestone(IGetMilestone_Milestone milestone, IGetMilestone_Project project)
//     {
//         return new Milestone
//         {
//             ProjectId = project.Id,
//             Id = milestone.Id,
//             Iid = milestone.Iid,
//             Title = milestone.Title,
//             Description = milestone.Description,
//             MilestoneState = MapState(milestone.State),
//             ClosedIssuesCount = milestone.Stats?.ClosedIssuesCount ?? 0,
//             TotalIssuesCount = milestone.Stats?.TotalIssuesCount ?? 0,
//             OpenIssues = project.OpenIssues?.Nodes?.Where(node => node != null).Select(CreateIssue!).ToList() ?? new List<Issue>(),
//             ClosedIssues = project.ClosedIssues?.Nodes?.Where(node => node != null).Select(CreateIssue!).ToList() ?? new List<Issue>()
//         };
//     }
//
//     public Milestone CreateMilestone(IIssueMilestone milestone)
//     {
//         return new Milestone
//         {
//             Id = milestone.Id,
//             Iid = milestone.Iid,
//             Title = milestone.Title
//         };
//     }
//
//     private static MilestoneState MapState(MilestoneStateEnum state)
//     {
//         return state switch
//         {
//             MilestoneStateEnum.Active => MilestoneState.Open,
//             MilestoneStateEnum.Closed => MilestoneState.Closed,
//             _ => MilestoneState.Unknown
//         };
//     }
//
//     private Issue CreateIssue(IGetMilestone_Project_OpenIssues_Nodes issueNode)
//     {
//         return new Issue
//         {
//             Id = issueNode.Id,
//             Iid = issueNode.Iid,
//             Title = issueNode.Title,
//             IssueState = MapState(issueNode.State),
//             Labels = issueNode.Labels?.Nodes?.Where(label => label != null).Select(_labelMapper.CreateLabel!)
//                          .OrderBy(label => label.Title).ToList()
//                      ?? new List<Label>()
//         };
//     }
//
//     private static Issues.Models.IssueState MapState(IssueState state)
//     {
//         return state switch
//         {
//             IssueState.Opened => IssueState.Open,
//             IssueState.Closed => IssueState.Closed,
//             _ => IssueState.Unknown
//         };
//     }
// }
