﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

#nullable disable

namespace StarWarsProgressBarIssueTracker.Infrastructure.Database.Migrations
{
    [DbContext(typeof(IssueTrackerContext))]
    [Migration("20241223202020_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("IssueLabel", b =>
                {
                    b.Property<Guid>("IssuesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("LabelsId")
                        .HasColumnType("uuid");

                    b.HasKey("IssuesId", "LabelsId");

                    b.HasIndex("LabelsId");

                    b.ToTable("IssueLabel", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Issues.Issue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1500)
                        .HasColumnType("character varying(1500)");

                    b.Property<string>("GitHubId")
                        .HasColumnType("text");

                    b.Property<string>("GitlabId")
                        .HasColumnType("text");

                    b.Property<string>("GitlabIid")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("MilestoneId")
                        .HasColumnType("uuid");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ReleaseId")
                        .HasColumnType("uuid");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid?>("VehicleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("GitHubId")
                        .IsUnique();

                    b.HasIndex("GitlabId")
                        .IsUnique();

                    b.HasIndex("GitlabIid")
                        .IsUnique();

                    b.HasIndex("MilestoneId");

                    b.HasIndex("ReleaseId");

                    b.HasIndex("VehicleId");

                    b.ToTable("Issues", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Issues.IssueLink", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("LinkedIssueId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LinkedIssueId");

                    b.ToTable("IssueLinks", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Labels.Label", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("character varying(7)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("GitHubId")
                        .HasColumnType("text");

                    b.Property<string>("GitlabId")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TextColor")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("character varying(7)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("GitHubId")
                        .IsUnique();

                    b.HasIndex("GitlabId")
                        .IsUnique();

                    b.ToTable("Labels", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Milestones.Milestone", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("GitHubId")
                        .HasColumnType("text");

                    b.Property<string>("GitlabId")
                        .HasColumnType("text");

                    b.Property<string>("GitlabIid")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("GitHubId")
                        .IsUnique();

                    b.HasIndex("GitlabId")
                        .IsUnique();

                    b.HasIndex("GitlabIid")
                        .IsUnique();

                    b.ToTable("Milestones", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Releases.Release", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("GitHubId")
                        .HasColumnType("text");

                    b.Property<string>("GitlabId")
                        .HasColumnType("text");

                    b.Property<string>("GitlabIid")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Notes")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("GitHubId")
                        .IsUnique();

                    b.HasIndex("GitlabId")
                        .IsUnique();

                    b.HasIndex("GitlabIid")
                        .IsUnique();

                    b.ToTable("Releases", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Vehicles.Appearance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("character varying(7)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("GitHubId")
                        .HasColumnType("text");

                    b.Property<string>("GitlabId")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TextColor")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("character varying(7)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<Guid?>("VehicleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("GitHubId")
                        .IsUnique();

                    b.HasIndex("GitlabId")
                        .IsUnique();

                    b.HasIndex("VehicleId");

                    b.ToTable("Appearances", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Vehicles.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("VehicleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("VehicleId");

                    b.ToTable("Photos", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Vehicles.Translation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("character varying(7)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid?>("VehicleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("VehicleId");

                    b.ToTable("Translations", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Vehicles.Vehicle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EngineColor")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Vehicles", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Infrastructure.Models.DbJob", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CronInterval")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsPaused")
                        .HasColumnType("boolean");

                    b.Property<int>("JobType")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("NextExecution")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("JobType")
                        .IsUnique();

                    b.ToTable("Jobs", "issue_tracker");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Infrastructure.Models.DbTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExecuteAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ExecutedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("JobId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("JobId");

                    b.ToTable("Tasks", "issue_tracker");
                });

            modelBuilder.Entity("IssueLabel", b =>
                {
                    b.HasOne("StarWarsProgressBarIssueTracker.Domain.Issues.Issue", null)
                        .WithMany()
                        .HasForeignKey("IssuesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StarWarsProgressBarIssueTracker.Domain.Labels.Label", null)
                        .WithMany()
                        .HasForeignKey("LabelsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Issues.Issue", b =>
                {
                    b.HasOne("StarWarsProgressBarIssueTracker.Domain.Milestones.Milestone", "Milestone")
                        .WithMany("Issues")
                        .HasForeignKey("MilestoneId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("StarWarsProgressBarIssueTracker.Domain.Releases.Release", "Release")
                        .WithMany("Issues")
                        .HasForeignKey("ReleaseId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("StarWarsProgressBarIssueTracker.Domain.Vehicles.Vehicle", "Vehicle")
                        .WithMany()
                        .HasForeignKey("VehicleId");

                    b.Navigation("Milestone");

                    b.Navigation("Release");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Issues.IssueLink", b =>
                {
                    b.HasOne("StarWarsProgressBarIssueTracker.Domain.Issues.Issue", "LinkedIssue")
                        .WithMany("LinkedIssues")
                        .HasForeignKey("LinkedIssueId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("LinkedIssue");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Vehicles.Appearance", b =>
                {
                    b.HasOne("StarWarsProgressBarIssueTracker.Domain.Vehicles.Vehicle", null)
                        .WithMany("Appearances")
                        .HasForeignKey("VehicleId");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Vehicles.Photo", b =>
                {
                    b.HasOne("StarWarsProgressBarIssueTracker.Domain.Vehicles.Vehicle", null)
                        .WithMany("Photos")
                        .HasForeignKey("VehicleId");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Vehicles.Translation", b =>
                {
                    b.HasOne("StarWarsProgressBarIssueTracker.Domain.Vehicles.Vehicle", null)
                        .WithMany("Translations")
                        .HasForeignKey("VehicleId");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Infrastructure.Models.DbTask", b =>
                {
                    b.HasOne("StarWarsProgressBarIssueTracker.Infrastructure.Models.DbJob", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Issues.Issue", b =>
                {
                    b.Navigation("LinkedIssues");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Milestones.Milestone", b =>
                {
                    b.Navigation("Issues");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Releases.Release", b =>
                {
                    b.Navigation("Issues");
                });

            modelBuilder.Entity("StarWarsProgressBarIssueTracker.Domain.Vehicles.Vehicle", b =>
                {
                    b.Navigation("Appearances");

                    b.Navigation("Photos");

                    b.Navigation("Translations");
                });
#pragma warning restore 612, 618
        }
    }
}