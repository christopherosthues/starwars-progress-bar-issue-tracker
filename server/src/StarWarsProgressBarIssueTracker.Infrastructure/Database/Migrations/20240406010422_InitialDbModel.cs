using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialDbModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "issue_tracker");

            migrationBuilder.CreateTable(
                name: "Jobs",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CronInterval = table.Column<string>(type: "text", nullable: false),
                    IsPaused = table.Column<bool>(type: "boolean", nullable: false),
                    NextExecution = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    JobType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    TextColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    GitlabId = table.Column<string>(type: "text", nullable: true),
                    GitHubId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Milestones",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    GitlabId = table.Column<string>(type: "text", nullable: true),
                    GitlabIid = table.Column<string>(type: "text", nullable: true),
                    GitHubId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Releases",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GitlabId = table.Column<string>(type: "text", nullable: true),
                    GitlabIid = table.Column<string>(type: "text", nullable: true),
                    GitHubId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EngineColor = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExecuteAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "issue_tracker",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appearances",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    TextColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    GitlabId = table.Column<string>(type: "text", nullable: true),
                    GitHubId = table.Column<string>(type: "text", nullable: true),
                    AppearanceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appearances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appearances_Vehicles_AppearanceId",
                        column: x => x.AppearanceId,
                        principalSchema: "issue_tracker",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    MilestoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReleaseId = table.Column<Guid>(type: "uuid", nullable: true),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    GitlabId = table.Column<string>(type: "text", nullable: true),
                    GitlabIid = table.Column<string>(type: "text", nullable: true),
                    GitHubId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issues_Milestones_MilestoneId",
                        column: x => x.MilestoneId,
                        principalSchema: "issue_tracker",
                        principalTable: "Milestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Issues_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalSchema: "issue_tracker",
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Issues_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "issue_tracker",
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Vehicles_PhotoId",
                        column: x => x.PhotoId,
                        principalSchema: "issue_tracker",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Country = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Text = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TranslationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Vehicles_TranslationId",
                        column: x => x.TranslationId,
                        principalSchema: "issue_tracker",
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DbIssueDbLabel",
                schema: "issue_tracker",
                columns: table => new
                {
                    IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabelId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbIssueDbLabel", x => new { x.IssueId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_DbIssueDbLabel_Issues_LabelId",
                        column: x => x.LabelId,
                        principalSchema: "issue_tracker",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DbIssueDbLabel_Labels_IssueId",
                        column: x => x.IssueId,
                        principalSchema: "issue_tracker",
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueLinks",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    LinkedIssueId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueLinks_Issues_LinkedIssueId",
                        column: x => x.LinkedIssueId,
                        principalSchema: "issue_tracker",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appearances_AppearanceId",
                schema: "issue_tracker",
                table: "Appearances",
                column: "AppearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Appearances_GitHubId",
                schema: "issue_tracker",
                table: "Appearances",
                column: "GitHubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appearances_GitlabId",
                schema: "issue_tracker",
                table: "Appearances",
                column: "GitlabId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DbIssueDbLabel_LabelId",
                schema: "issue_tracker",
                table: "DbIssueDbLabel",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueLinks_LinkedIssueId",
                schema: "issue_tracker",
                table: "IssueLinks",
                column: "LinkedIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_GitHubId",
                schema: "issue_tracker",
                table: "Issues",
                column: "GitHubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_GitlabId",
                schema: "issue_tracker",
                table: "Issues",
                column: "GitlabId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_GitlabIid",
                schema: "issue_tracker",
                table: "Issues",
                column: "GitlabIid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_MilestoneId",
                schema: "issue_tracker",
                table: "Issues",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ReleaseId",
                schema: "issue_tracker",
                table: "Issues",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_VehicleId",
                schema: "issue_tracker",
                table: "Issues",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobType",
                schema: "issue_tracker",
                table: "Jobs",
                column: "JobType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_GitHubId",
                schema: "issue_tracker",
                table: "Labels",
                column: "GitHubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_GitlabId",
                schema: "issue_tracker",
                table: "Labels",
                column: "GitlabId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_GitHubId",
                schema: "issue_tracker",
                table: "Milestones",
                column: "GitHubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_GitlabId",
                schema: "issue_tracker",
                table: "Milestones",
                column: "GitlabId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_GitlabIid",
                schema: "issue_tracker",
                table: "Milestones",
                column: "GitlabIid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoId",
                schema: "issue_tracker",
                table: "Photos",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_GitHubId",
                schema: "issue_tracker",
                table: "Releases",
                column: "GitHubId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Releases_GitlabId",
                schema: "issue_tracker",
                table: "Releases",
                column: "GitlabId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Releases_GitlabIid",
                schema: "issue_tracker",
                table: "Releases",
                column: "GitlabIid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_JobId",
                schema: "issue_tracker",
                table: "Tasks",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_TranslationId",
                schema: "issue_tracker",
                table: "Translations",
                column: "TranslationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appearances",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "DbIssueDbLabel",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "IssueLinks",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Photos",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Translations",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Labels",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Issues",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Jobs",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Milestones",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Releases",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Vehicles",
                schema: "issue_tracker");
        }
    }
}
