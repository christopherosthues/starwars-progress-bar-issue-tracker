// using StarWarsProgressBarIssueTracker.Domain.Releases;
// using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking.Models;
//
// namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Mapper;
//
// public class ReleaseMapper
// {
//     public Release CreateRelease(IRelease issueNode)
//     {
//         return new Release
//         {
//             Id = issueNode.Id,
//             Iid = issueNode.Iid,
//             ProjectId = issueNode.ProjectId,
//             Title = issueNode.Title,
//             ReleaseNotes = issueNode.Description,
//             ReleaseState = MapState(issueNode.State),
//             ReleaseDate = issueNode.DueDate != null ? DateTime.Parse(issueNode.DueDate) : null
//         };
//     }
//
//     private static ReleaseState MapState(IssueState state)
//     {
//         return state switch
//         {
//             IssueState.Opened => ReleaseState.Open,
//             IssueState.Closed => ReleaseState.Closed,
//             _ => ReleaseState.Unknown
//         };
//     }
//
//     public Release? CreateRelease(LinkIssue? linkIssue)
//     {
//         if (linkIssue == null)
//         {
//             return null;
//         }
//
//         return new Release
//         {
//             Iid = linkIssue.Iid.ToString(),
//             Id = linkIssue.Id.ToString(),
//             IssueLinkId = linkIssue.IssueLinkId,
//             ProjectId = linkIssue.ProjectId,
//             ReleaseState = MapState(linkIssue.State),
//             Title = linkIssue.Title,
//             ReleaseNotes = linkIssue.Description,
//             ReleaseDate = linkIssue.DueDate != null ? DateTime.Parse(linkIssue.DueDate) : null
//         };
//     }
//
//     private static ReleaseState MapState(string state)
//     {
//         if (state.Equals(IssueState.Opened.ToString()))
//         {
//             return ReleaseState.Open;
//         }
//         if (state.Equals(IssueState.Closed.ToString()))
//         {
//             return ReleaseState.Closed;
//         }
//
//         return ReleaseState.Unknown;
//     }
// }
