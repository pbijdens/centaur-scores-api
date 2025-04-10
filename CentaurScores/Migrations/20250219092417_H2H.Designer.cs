﻿// <auto-generated />
using System;
using CentaurScores.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CentaurScores.Migrations
{
    [DbContext(typeof(CentaurScoresDbContext))]
    [Migration("20250219092417_H2H")]
    partial class H2H
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AccountEntityAclEntity", b =>
                {
                    b.Property<int>("ACLsId")
                        .HasColumnType("int");

                    b.Property<int>("AccountsId")
                        .HasColumnType("int");

                    b.HasKey("ACLsId", "AccountsId");

                    b.HasIndex("AccountsId");

                    b.ToTable("AccountEntityAclEntity");
                });

            modelBuilder.Entity("CentaurScores.Persistence.AccountEntity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("SaltedPasswordHash")
                        .IsRequired()
                        .HasMaxLength(72)
                        .HasColumnType("varchar(72)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("CentaurScores.Persistence.AclEntity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ACLs");
                });

            modelBuilder.Entity("CentaurScores.Persistence.CompetitionEntity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTimeOffset?>("EndDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("ParticipantListId")
                        .HasColumnType("int");

                    b.Property<string>("RulesetGroupName")
                        .HasColumnType("longtext");

                    b.Property<string>("RulesetParametersJSON")
                        .HasColumnType("longtext");

                    b.Property<DateTimeOffset?>("StartDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantListId");

                    b.ToTable("Competitions");
                });

            modelBuilder.Entity("CentaurScores.Persistence.CsSetting", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("JsonValue")
                        .HasColumnType("longtext");

                    b.HasKey("Name");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("CentaurScores.Persistence.MatchEntity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ActiveRound")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int>("ArrowsPerEnd")
                        .HasColumnType("int");

                    b.Property<bool>("AutoProgressAfterEachArrow")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool?>("ChangedRemotely")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("CompetitionId")
                        .HasColumnType("int");

                    b.Property<string>("GroupsJSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LijnenJSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MatchCode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<uint>("MatchFlags")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned")
                        .HasDefaultValue(0u);

                    b.Property<string>("MatchName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("NumberOfEnds")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfRounds")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(4);

                    b.Property<string>("RulesetCode")
                        .HasColumnType("longtext");

                    b.Property<string>("ScoreValuesJson")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("SubgroupsJSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TargetsJSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CompetitionId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("CentaurScores.Persistence.ParticipantEntity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DeviceID")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EndsJSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("HeadToHeadJSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Lijn")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("MatchId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("ParticipantListEntryId")
                        .HasColumnType("int");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<string>("Subgroup")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Target")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("CentaurScores.Persistence.ParticipantListEntity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("ParticipantLists");
                });

            modelBuilder.Entity("CentaurScores.Persistence.ParticipantListEntryEntity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeactivated")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("ListId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Subgroup")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("ListId");

                    b.ToTable("ParticipantListEntries");
                });

            modelBuilder.Entity("CentaurScores.Persistence.PersonalBestsListEntity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CompetitionFormat")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<int?>("ParticipantListId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParticipantListId");

                    b.ToTable("PersonalBestLists");
                });

            modelBuilder.Entity("CentaurScores.Persistence.PersonalBestsListEntryEntity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("AchievedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Discipline")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("ListId")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("ParticipantId")
                        .HasColumnType("int");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ListId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("PersonalBestListEntries");
                });

            modelBuilder.Entity("AccountEntityAclEntity", b =>
                {
                    b.HasOne("CentaurScores.Persistence.AclEntity", null)
                        .WithMany()
                        .HasForeignKey("ACLsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CentaurScores.Persistence.AccountEntity", null)
                        .WithMany()
                        .HasForeignKey("AccountsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CentaurScores.Persistence.CompetitionEntity", b =>
                {
                    b.HasOne("CentaurScores.Persistence.ParticipantListEntity", "ParticipantList")
                        .WithMany("Competitions")
                        .HasForeignKey("ParticipantListId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("ParticipantList");
                });

            modelBuilder.Entity("CentaurScores.Persistence.MatchEntity", b =>
                {
                    b.HasOne("CentaurScores.Persistence.CompetitionEntity", "Competition")
                        .WithMany("Matches")
                        .HasForeignKey("CompetitionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Competition");
                });

            modelBuilder.Entity("CentaurScores.Persistence.ParticipantEntity", b =>
                {
                    b.HasOne("CentaurScores.Persistence.MatchEntity", "Match")
                        .WithMany("Participants")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Match");
                });

            modelBuilder.Entity("CentaurScores.Persistence.ParticipantListEntryEntity", b =>
                {
                    b.HasOne("CentaurScores.Persistence.ParticipantListEntity", "List")
                        .WithMany("Entries")
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("List");
                });

            modelBuilder.Entity("CentaurScores.Persistence.PersonalBestsListEntity", b =>
                {
                    b.HasOne("CentaurScores.Persistence.ParticipantListEntity", "ParticipantList")
                        .WithMany("PersonalBestLists")
                        .HasForeignKey("ParticipantListId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("ParticipantList");
                });

            modelBuilder.Entity("CentaurScores.Persistence.PersonalBestsListEntryEntity", b =>
                {
                    b.HasOne("CentaurScores.Persistence.PersonalBestsListEntity", "List")
                        .WithMany("Entries")
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CentaurScores.Persistence.ParticipantListEntryEntity", "Participant")
                        .WithMany("PersonalBests")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("List");

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("CentaurScores.Persistence.CompetitionEntity", b =>
                {
                    b.Navigation("Matches");
                });

            modelBuilder.Entity("CentaurScores.Persistence.MatchEntity", b =>
                {
                    b.Navigation("Participants");
                });

            modelBuilder.Entity("CentaurScores.Persistence.ParticipantListEntity", b =>
                {
                    b.Navigation("Competitions");

                    b.Navigation("Entries");

                    b.Navigation("PersonalBestLists");
                });

            modelBuilder.Entity("CentaurScores.Persistence.ParticipantListEntryEntity", b =>
                {
                    b.Navigation("PersonalBests");
                });

            modelBuilder.Entity("CentaurScores.Persistence.PersonalBestsListEntity", b =>
                {
                    b.Navigation("Entries");
                });
#pragma warning restore 612, 618
        }
    }
}
