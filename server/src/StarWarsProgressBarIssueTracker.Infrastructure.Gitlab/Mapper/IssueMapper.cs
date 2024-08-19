// using System.Text.Json;
// using StarWarsProgressBarIssueTracker.Domain.Appearances;
// using StarWarsProgressBarIssueTracker.Domain.Issues;
// using StarWarsProgressBarIssueTracker.Domain.Milestones;
//
// namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Mapper;
//
// public class IssueMapper
// {
//     private readonly LabelMapper _labelMapper;
//     private readonly MilestoneMapper _milestoneMapper;
//
//     public IssueMapper(LabelMapper labelMapper,
//                        MilestoneMapper milestoneMapper)
//     {
//         _labelMapper = labelMapper ?? throw new ArgumentNullException(nameof(labelMapper));
//         _milestoneMapper = milestoneMapper ?? throw new ArgumentNullException(nameof(milestoneMapper));
//     }
//
//     public Issue CreateIssue(IIssue issueNode)
//     {
//         Milestone? milestone = null;
//         if (issueNode.Milestone != null)
//         {
//             milestone = _milestoneMapper.CreateMilestone(issueNode.Milestone);
//         }
//         return new Issue
//         {
//             Id = issueNode.Id,
//             Iid = issueNode.Iid,
//             ProjectId = issueNode.ProjectId,
//             Title = issueNode.Title,
//             IssueDescription = ParseDescription(issueNode.Description),
//             WebUrl = issueNode.WebUrl,
//             State = MapState(issueNode.State),
//             Labels = issueNode.Labels?.Nodes?.Where(label => label != null).Select(_labelMapper.CreateLabel!)
//                          .OrderBy(label => label.Title).ToList()
//                      ?? new List<Appearance>(),
//             Milestone = milestone
//         };
//     }
//
//     private static IssueState MapState(GraphQL.IssueState state)
//     {
//         return state switch
//         {
//             GraphQL.IssueState.Opened => IssueState.Open,
//             GraphQL.IssueState.Closed => IssueState.Closed,
//             GraphQL.IssueState.Locked => IssueState.Locked,
//             GraphQL.IssueState.All => IssueState.Unknown,
//             _ => IssueState.Unknown
//         };
//     }
//
// private static IssueDescription ParseDescription(string? description)
// {
//     var issueDescription = new IssueDescription();
//     if (description != null)
//     {
//         var fieldsBeginning = description.IndexOf("{", StringComparison.InvariantCulture);
//         if (fieldsBeginning != -1)
//         {
//             var fieldsText = description[fieldsBeginning..];
//             var parsedDescription = JsonSerializer.Deserialize<IssueDescription>(fieldsText);
//
//             if (parsedDescription != null)
//             {
//                 issueDescription = parsedDescription;
//             }
//
//             if (!string.IsNullOrWhiteSpace(description[..fieldsBeginning]))
//             {
//                 issueDescription.Description = description[..fieldsBeginning] + "\n" + issueDescription.Description;
//             }
//         }
//         else
//         {
//             issueDescription.Description = description;
//         }
//     }
//
//     return issueDescription;
// }
// }
