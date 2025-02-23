using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "issue_tracker");

            migrationBuilder.CreateTable(
                name: "EngineColors",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineColors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IssueStates",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTypes",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypes", x => x.Id);
                });

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
                name: "LinkTypes",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkTypes", x => x.Id);
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
                name: "Priorities",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseStates",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseStates", x => x.Id);
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
                name: "TaskStatuses",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatuses", x => x.Id);
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
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appearances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appearances_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "issue_tracker",
                        principalTable: "Vehicles",
                        principalColumn: "Id");
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
                    FilePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "issue_tracker",
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                schema: "issue_tracker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Country = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Text = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "issue_tracker",
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IssueLabel",
                schema: "issue_tracker",
                columns: table => new
                {
                    IssuesId = table.Column<Guid>(type: "uuid", nullable: false),
                    LabelsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueLabel", x => new { x.IssuesId, x.LabelsId });
                    table.ForeignKey(
                        name: "FK_IssueLabel_Issues_IssuesId",
                        column: x => x.IssuesId,
                        principalSchema: "issue_tracker",
                        principalTable: "Issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueLabel_Labels_LabelsId",
                        column: x => x.LabelsId,
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

            migrationBuilder.InsertData(
                schema: "issue_tracker",
                table: "EngineColors",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Blue" },
                    { 2, "Brown" },
                    { 3, "Green" },
                    { 4, "Orange" },
                    { 5, "Purple" },
                    { 6, "Red" },
                    { 7, "Yellow" },
                    { 8, "White" }
                });

            migrationBuilder.InsertData(
                schema: "issue_tracker",
                table: "IssueStates",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Open" },
                    { 2, "Closed" }
                });

            migrationBuilder.InsertData(
                schema: "issue_tracker",
                table: "JobTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "GitlabSync" },
                    { 2, "GitHubSync" }
                });

            migrationBuilder.InsertData(
                schema: "issue_tracker",
                table: "LinkTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Blocks" },
                    { 2, "IsBlockedBy" },
                    { 3, "RelatesTo" },
                    { 4, "IsRelatedTo" },
                    { 5, "Duplicates" },
                    { 6, "IsDuplicatedBy" }
                });

            migrationBuilder.InsertData(
                schema: "issue_tracker",
                table: "Priorities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Minor" },
                    { 2, "Lowest" },
                    { 3, "Low" },
                    { 4, "Medium" },
                    { 5, "High" },
                    { 6, "Highest" },
                    { 7, "Blocker" }
                });

            migrationBuilder.InsertData(
                schema: "issue_tracker",
                table: "ReleaseStates",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Planned" },
                    { 2, "Released" }
                });

            migrationBuilder.InsertData(
                schema: "issue_tracker",
                table: "TaskStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Planned" },
                    { 2, "Running" },
                    { 3, "FailureWaitingForRetry" },
                    { 4, "Error" },
                    { 5, "Completed" }
                });

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
                name: "IX_Appearances_VehicleId",
                schema: "issue_tracker",
                table: "Appearances",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueLabel_LabelsId",
                schema: "issue_tracker",
                table: "IssueLabel",
                column: "LabelsId");

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
                name: "IX_Photos_VehicleId",
                schema: "issue_tracker",
                table: "Photos",
                column: "VehicleId");

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
                name: "IX_Translations_VehicleId",
                schema: "issue_tracker",
                table: "Translations",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appearances",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "EngineColors",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "IssueLabel",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "IssueLinks",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "IssueStates",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "JobTypes",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "LinkTypes",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Photos",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "Priorities",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "ReleaseStates",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "TaskStatuses",
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
