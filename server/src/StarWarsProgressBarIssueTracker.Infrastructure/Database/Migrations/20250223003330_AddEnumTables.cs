using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEnumTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EngineColors",
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
                name: "Priorities",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "ReleaseStates",
                schema: "issue_tracker");

            migrationBuilder.DropTable(
                name: "TaskStatuses",
                schema: "issue_tracker");
        }
    }
}
