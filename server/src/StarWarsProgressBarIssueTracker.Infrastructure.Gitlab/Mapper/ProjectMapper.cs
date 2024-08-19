// using Label = StarWarsVehiclesTracker.Labels.Models.Label;
//
// namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Mapper;
//
// public class ProjectMapper
// {
//     private readonly IssueMapper _issueMapper;
//     private readonly LabelMapper _labelMapper;
//     private readonly MilestoneMapper _milestoneMapper;
//     private readonly ReleaseMapper _releaseMapper;
//
//     public ProjectMapper(IssueMapper issueMapper,
//                          LabelMapper labelMapper,
//                          MilestoneMapper milestoneMapper,
//                          ReleaseMapper releaseMapper)
//     {
//         _issueMapper = issueMapper ?? throw new ArgumentNullException(nameof(issueMapper));
//         _labelMapper = labelMapper ?? throw new ArgumentNullException(nameof(labelMapper));
//         _milestoneMapper = milestoneMapper ?? throw new ArgumentNullException(nameof(milestoneMapper));
//         _releaseMapper = releaseMapper ?? throw new ArgumentNullException(nameof(releaseMapper));
//     }
//
//     public Project CreateProjectWithIssue(IGetEditIssue_Project project)
//     {
//         var result = CreateProject(project);
//         var issue = project.Issue;
//         if (issue != null)
//         {
//             result.Issue = _issueMapper.CreateIssue(issue);
//         }
//
//         return result;
//     }
//
//     public Project CreateProject(IProject project)
//     {
//         var result = new Project();
//         result.Labels = project.Labels?.Nodes?.Where(l => l != null).Select(_labelMapper.CreateLabel!)
//                             .OrderBy(label => label.Title).ToList()
//                         ?? new List<Label>();
//
//         result.Milestones = project.Milestones?.Nodes?.Where(m => m != null).Select(_milestoneMapper.CreateMilestone!)
//                                 .OrderBy(milestone => milestone.Title).ToList()
//                         ?? new List<Milestone>();
//
//         result.Releases = project.Releases?.Nodes?.Where(node => node != null).Select(_releaseMapper.CreateRelease!)
//                               .OrderBy(release => release.Title).ToList()
//                           ?? new List<Release>();
//
//         return result;
//     }
// }
