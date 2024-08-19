using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Configuration;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking;

public class RestService
{
    private readonly HttpClient _client;

    public RestService(HttpClient client, IOptions<GitlabConfiguration> configuration)
    {
        var restUri = configuration.Value.RestURL ?? throw new ArgumentException("The REST API Uri must not be null!");
        var token = configuration.Value.Token ?? throw new ArgumentException("The token must not be null!");
        _client = client;
        _client.BaseAddress = new Uri(restUri);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TrimEnd());
    }

    // public async Task UpdateLabel(Label label)
    // {
    //     var request = new RestRequest("projects/{projectId}/labels/{labelId}")
    //         .AddUrlSegment("projectId", label.ProjectId.ToId())
    //         .AddUrlSegment("labelId", label.Id.ToId())
    //         .AddJsonBody(new EditLabel
    //         {
    //             NewName = label.Title,
    //             Description = label.Description,
    //             Color = label.Color
    //         });
    //
    //     var response = await _client.PutAsync(request, CancellationToken.None);
    //     response.ThrowIfError();
    // }
    //
    // public async Task DeleteLabel(string projectId, string labelId)
    // {
    //     var request = new RestRequest("projects/{projectId}/labels/{labelId}")
    //         .AddUrlSegment("projectId", projectId.ToId())
    //         .AddUrlSegment("labelId", labelId.ToId());
    //
    //     var response = await _client.DeleteAsync(request, CancellationToken.None);
    //     response.ThrowIfError();
    // }

    public async Task<IList<LinkIssue>?> GetIssueLinksAsync(int projectId, string issueIid)
    {
        var response = await _client.GetAsync($"projects/{projectId}/issues/{issueIid}/links", CancellationToken.None);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStreamAsync();

        var issueLinks = await JsonSerializer.DeserializeAsync<IList<LinkIssue>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return issueLinks;
    }

    // public async Task CreateIssueLink(int projectId, string issueIid, string targetIssueIid)
    // {
    //     var request = new RestRequest("projects/{id}/issues/{issue_iid}/links")
    //         .AddUrlSegment("id", projectId)
    //         .AddUrlSegment("issue_iid", issueIid)
    //         .AddQueryParameter("target_project_id", projectId)
    //         .AddQueryParameter("target_issue_iid", targetIssueIid);
    //
    //     var response = await _client.PostAsync(request, CancellationToken.None);
    //     response.ThrowIfError();
    // }
    //
    // public async Task DeleteIssueLink(int projectId, string issueIid, string issueLinkId)
    // {
    //     var request = new RestRequest("projects/{id}/issues/{issue_iid}/links/{issue_link_id}")
    //         .AddUrlSegment("id", projectId)
    //         .AddUrlSegment("issue_iid", issueIid)
    //         .AddUrlSegment("issue_link_id", issueLinkId);
    //
    //     var response = await _client.DeleteAsync(request, CancellationToken.None);
    //     response.ThrowIfError();
    // }
    //
    // public async Task DeleteIssue(int projectId, string issueIid)
    // {
    //     var request = new RestRequest("projects/{projectId}/issues/{issueIid}")
    //         .AddUrlSegment("projectId", projectId)
    //         .AddUrlSegment("issueIid", issueIid);
    //
    //     var response = await _client.DeleteAsync(request, CancellationToken.None);
    //     response.ThrowIfError();
    // }
    //
    // public async Task CreateMilestone(Milestone milestone)
    // {
    //     var request = new RestRequest("projects/{projectId}/milestones")
    //         .AddUrlSegment("projectId", milestone.ProjectId.ToId())
    //         .AddJsonBody(new EditMilestone
    //         {
    //             Title = milestone.Title,
    //             Description = milestone.Description
    //         });
    //
    //     var response = await _client.PostAsync(request, CancellationToken.None);
    //     response.ThrowIfError();
    // }
    //
    // public async Task UpdateMilestone(Milestone milestone)
    // {
    //     var request = new RestRequest("projects/{projectId}/milestones/{milestoneId}")
    //         .AddUrlSegment("projectId", milestone.ProjectId.ToId())
    //         .AddUrlSegment("milestoneId", milestone.Id.ToId())
    //         .AddJsonBody(new EditMilestone
    //         {
    //             Title = milestone.Title,
    //             Description = milestone.Description,
    //             StateEvent = milestone.MilestoneState == MilestoneState.Active ? EditMilestone.ActivateEvent : EditMilestone.CloseEvent
    //         });
    //
    //     var response = await _client.PutAsync(request, CancellationToken.None);
    //     response.ThrowIfError();
    // }
    //
    // public async Task DeleteMilestone(string projectId, string milestoneId)
    // {
    //     var request = new RestRequest("projects/{projectId}/milestones/{milestoneId}")
    //         .AddUrlSegment("projectId", projectId.ToId())
    //         .AddUrlSegment("milestoneId", milestoneId.ToId());
    //
    //     var response = await _client.DeleteAsync(request, CancellationToken.None);
    //     response.ThrowIfError();
    // }
}
