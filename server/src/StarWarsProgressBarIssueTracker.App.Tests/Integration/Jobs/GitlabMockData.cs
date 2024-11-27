using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Tests.Integration.Jobs;

public static class GitlabMockData
{
    public const string LabelResponse = """
                                        {
                                            "data": {
                                                "project": {
                                                    "__typename": "Project",
                                                    "id": "gid://gitlab/Project/1",
                                                    "labels": {
                                                        "__typename": "LabelConnection",
                                                        "nodes": [
                                                            {
                                                                "__typename": "Label",
                                                                "color": "#f7e7ce",
                                                                "description": "Vehicles appearing in Andor",
                                                                "id": "gid://gitlab/ProjectLabel/1",
                                                                "textColor": "#1F1E24",
                                                                "title": "Andor",
                                                                "updatedAt": "2023-02-25T12:57:46Z"
                                                            },
                                                            {
                                                                "__typename": "Label",
                                                                "color": "#cd5b45",
                                                                "description": "",
                                                                "id": "gid://gitlab/ProjectLabel/2",
                                                                "textColor": "#FFFFFF",
                                                                "title": "Bad Batch",
                                                                "updatedAt": "2022-11-02T15:01:41Z"
                                                            },
                                                            {
                                                                "__typename": "Label",
                                                                "color": "#FA7320",
                                                                "description": "The old Clone Wars series",
                                                                "id": "gid://gitlab/ProjectLabel/3",
                                                                "textColor": "#FFFFFF",
                                                                "title": "Clone Wars",
                                                                "updatedAt": "2023-03-10T22:26:55Z"
                                                            }
                                                        ]
                                                    }
                                                }
                                            }
                                        }
                                        """;

    public const string AppearanceResponse = """
                                             {
                                                 "data": {
                                                     "project": {
                                                         "__typename": "Project",
                                                         "id": "gid://gitlab/Project/1",
                                                         "labels": {
                                                             "__typename": "LabelConnection",
                                                             "nodes": [
                                                                 {
                                                                     "__typename": "Label",
                                                                     "color": "#f7e7ce",
                                                                     "description": "Vehicles appearing in Andor",
                                                                     "id": "gid://gitlab/ProjectLabel/4",
                                                                     "textColor": "#1F1E24",
                                                                     "title": "Appearance: Andor",
                                                                     "updatedAt": "2023-02-25T12:57:46Z"
                                                                 },
                                                                 {
                                                                     "__typename": "Label",
                                                                     "color": "#cd5b45",
                                                                     "description": "",
                                                                     "id": "gid://gitlab/ProjectLabel/5",
                                                                     "textColor": "#FFFFFF",
                                                                     "title": "Appearance: Bad Batch",
                                                                     "updatedAt": "2022-11-02T15:01:41Z"
                                                                 },
                                                                 {
                                                                     "__typename": "Label",
                                                                     "color": "#FA7320",
                                                                     "description": "The old Clone Wars series",
                                                                     "id": "gid://gitlab/ProjectLabel/6",
                                                                     "textColor": "#FFFFFF",
                                                                     "title": "Appearance: Clone Wars",
                                                                     "updatedAt": "2023-03-10T22:26:55Z"
                                                                 }
                                                             ]
                                                         }
                                                     }
                                                 }
                                             }
                                             """;

    public const string MilestoneResponse = """
                                            {
                                                "data": {
                                                    "project": {
                                                        "__typename": "Project",
                                                        "id": "gid://gitlab/Project/1",
                                                        "milestones": {
                                                            "__typename": "MilestoneConnection",
                                                            "nodes": [
                                                                {
                                                                    "__typename": "Milestone",
                                                                    "description": "",
                                                                    "id": "gid://gitlab/Milestone/1",
                                                                    "iid": "1",
                                                                    "state": "closed",
                                                                    "title": "Features",
                                                                    "updatedAt": "2022-12-30T11:49:41Z"
                                                                },
                                                                {
                                                                    "__typename": "Milestone",
                                                                    "description": "",
                                                                    "id": "gid://gitlab/Milestone/2",
                                                                    "iid": "2",
                                                                    "state": "active",
                                                                    "title": "Yuuzhan-Vong",
                                                                    "updatedAt": "2022-11-02T22:12:15Z"
                                                                },
                                                                {
                                                                    "__typename": "Milestone",
                                                                    "description": "Desc3",
                                                                    "id": "gid://gitlab/Milestone/3",
                                                                    "iid": "3",
                                                                    "state": "active",
                                                                    "title": "Mandalorians",
                                                                    "updatedAt": "2022-11-02T14:44:21Z"
                                                                }
                                                            ]
                                                        }
                                                    }
                                                }
                                            }
                                            """;

    public const string ReleaseResponse = """
                                          {
                                              "data": {
                                                  "project": {
                                                      "__typename": "Project",
                                                      "id": "gid://gitlab/Project/1",
                                                      "issues": {
                                                          "__typename": "IssueConnection",
                                                          "count": 3,
                                                          "nodes": [
                                                              {
                                                                  "__typename": "Issue",
                                                                  "description": "Hello there",
                                                                  "dueDate": null,
                                                                  "id": "gid://gitlab/Issue/1",
                                                                  "iid": "11",
                                                                  "labels": {
                                                                      "__typename": "LabelConnection",
                                                                      "nodes": [
                                                                      ]
                                                                  },
                                                                  "milestone": null,
                                                                  "projectId": 1,
                                                                  "state": "closed",
                                                                  "title": "Release 1.0.0",
                                                                  "dueDate": "2024-02-25T19:50:01Z",
                                                                  "updatedAt": "2023-03-05T13:54:06Z",
                                                                  "webUrl": "https://gitlab.com/osthues.christopher/starwars-progress-bar/-/issues/11"
                                                              },
                                                              {
                                                                  "__typename": "Issue",
                                                                  "description": "",
                                                                  "dueDate": null,
                                                                  "id": "gid://gitlab/Issue/2",
                                                                  "iid": "22",
                                                                  "labels": {
                                                                      "__typename": "LabelConnection",
                                                                      "nodes": [
                                                                      ]
                                                                  },
                                                                  "milestone": null,
                                                                  "projectId": 1,
                                                                  "state": "opened",
                                                                  "title": "Release 1.1.0",
                                                                  "dueDate": null,
                                                                  "updatedAt": "2023-04-22T19:07:14Z",
                                                                  "webUrl": "https://gitlab.com/osthues.christopher/starwars-progress-bar/-/issues/22"
                                                              },
                                                              {
                                                                  "__typename": "Issue",
                                                                  "description": "Desc3",
                                                                  "dueDate": null,
                                                                  "id": "gid://gitlab/Issue/3",
                                                                  "iid": "33",
                                                                  "labels": {
                                                                      "__typename": "LabelConnection",
                                                                      "nodes": [
                                                                      ]
                                                                  },
                                                                  "milestone": null,
                                                                  "projectId": 1,
                                                                  "state": "opened",
                                                                  "title": "Release 1.2.0",
                                                                  "dueDate": null,
                                                                  "updatedAt": "2023-04-22T19:06:51Z",
                                                                  "webUrl": "https://gitlab.com/osthues.christopher/starwars-progress-bar/-/issues/33"
                                                              }
                                                          ],
                                                          "pageInfo": {
                                                              "__typename": "PageInfo",
                                                              "endCursor": "eyJ0aXRsZSI6ImNrLTYgc3dvb3AiLCJpZCI6IjExOTQ3MjY1NyJ9",
                                                              "hasNextPage": false,
                                                              "hasPreviousPage": false
                                                          }
                                                      }
                                                  }
                                              }
                                          }
                                          """;

    public static IList<Label> AddedLabels()
    {
        return
        [
            new Label
            {
                Title = "Andor",
                Description = "Vehicles appearing in Andor",
                TextColor = "#1F1E24",
                Color = "#f7e7ce",
                GitlabId = "gid://gitlab/ProjectLabel/1",
                LastModifiedAt = new DateTime(2023, 2, 25, 12, 57, 46, DateTimeKind.Utc)
            },
            new Label
            {
                Title = "Bad Batch",
                Description = string.Empty,
                TextColor = "#FFFFFF",
                Color = "#cd5b45",
                GitlabId = "gid://gitlab/ProjectLabel/2",
                LastModifiedAt = new DateTime(2022, 11, 2, 15, 1, 41, DateTimeKind.Utc)
            },
            new Label
            {
                Title = "Clone Wars",
                Description = "The old Clone Wars series",
                TextColor = "#FFFFFF",
                Color = "#FA7320",
                GitlabId = "gid://gitlab/ProjectLabel/3",
                LastModifiedAt = new DateTime(2023, 3, 10, 22, 26, 55, DateTimeKind.Utc)
            },
        ];
    }

    public static IList<Appearance> AddedAppearances()
    {
        return
        [
            new Appearance
            {
                Title = "Andor",
                Description = "Vehicles appearing in Andor",
                TextColor = "#1F1E24",
                Color = "#f7e7ce",
                GitlabId = "gid://gitlab/ProjectLabel/4",
                LastModifiedAt = new DateTime(2023, 2, 25, 12, 57, 46, DateTimeKind.Utc)
            },
            new Appearance
            {
                Title = "Bad Batch",
                Description = string.Empty,
                TextColor = "#FFFFFF",
                Color = "#cd5b45",
                GitlabId = "gid://gitlab/ProjectLabel/5",
                LastModifiedAt = new DateTime(2022, 11, 2, 15, 1, 41, DateTimeKind.Utc)
            },
            new Appearance
            {
                Title = "Clone Wars",
                Description = "The old Clone Wars series",
                TextColor = "#FFFFFF",
                Color = "#FA7320",
                GitlabId = "gid://gitlab/ProjectLabel/6",
                LastModifiedAt = new DateTime(2023, 3, 10, 22, 26, 55, DateTimeKind.Utc)
            },
        ];
    }

    public static IList<Milestone> AddedMilestones()
    {
        return
        [
            new Milestone
            {
                Title = "Features",
                Description = string.Empty,
                GitlabId = "gid://gitlab/Milestone/1",
                GitlabIid = "1",
                State = MilestoneState.Closed,
                LastModifiedAt = new DateTime(2022, 12, 30, 11, 49, 41, DateTimeKind.Utc)
            },
            new Milestone
            {
                Title = "Yuuzhan-Vong",
                Description = string.Empty,
                GitlabId = "gid://gitlab/Milestone/2",
                GitlabIid = "2",
                State = MilestoneState.Open,
                LastModifiedAt = new DateTime(2022, 11, 2, 22, 12, 15, DateTimeKind.Utc)
            },
            new Milestone
            {
                Title = "Mandalorians",
                Description = "Desc3",
                GitlabId = "gid://gitlab/Milestone/3",
                GitlabIid = "3",
                State = MilestoneState.Open,
                LastModifiedAt = new DateTime(2022, 11, 2, 14, 44, 21, DateTimeKind.Utc)
            },
        ];
    }

    public static IList<Release> AddedReleases()
    {
        return
        [
            new Release
            {
                Title = "Release 1.0.0",
                Notes = "Hello there",
                Date = new DateTime(2024, 2, 25, 19, 0, 1, DateTimeKind.Utc),
                GitlabId = "gid://gitlab/Issue/1",
                GitlabIid = "11",
                State = ReleaseState.Released,
                LastModifiedAt = new DateTime(2023, 3, 5, 13, 54, 6, DateTimeKind.Utc)
            },
            new Release
            {
                Title = "Release 1.1.0",
                Notes = string.Empty,
                GitlabId = "gid://gitlab/Issue/2",
                GitlabIid = "22",
                State = ReleaseState.Planned,
                LastModifiedAt = new DateTime(2023, 4, 22, 19, 7, 14, DateTimeKind.Utc)
            },
            new Release
            {
                Title = "Release 1.2.0",
                Notes = "Desc3",
                GitlabId = "gid://gitlab/Issue/3",
                GitlabIid = "33",
                State = ReleaseState.Planned,
                LastModifiedAt = new DateTime(2023, 4, 22, 19, 6, 51, DateTimeKind.Utc)
            },
        ];
    }
}
