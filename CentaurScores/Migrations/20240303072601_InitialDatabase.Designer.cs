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
    [Migration("20240303072601_InitialDatabase")]
    partial class InitialDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

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

                    b.Property<int>("ArrowsPerEnd")
                        .HasColumnType("int");

                    b.Property<bool>("AutoProgressAfterEachArrow")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("GroupsJSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LijnenJSON")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MatchCode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MatchName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("NumberOfEnds")
                        .HasColumnType("int");

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

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("CentaurScores.Persistence.ParticipantEntity", b =>
                {
                    b.Property<int>("Id")
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

                    b.Property<string>("Lijn")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("MatchId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

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

            modelBuilder.Entity("CentaurScores.Persistence.ParticipantEntity", b =>
                {
                    b.HasOne("CentaurScores.Persistence.MatchEntity", "Match")
                        .WithMany("Participants")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Match");
                });

            modelBuilder.Entity("CentaurScores.Persistence.MatchEntity", b =>
                {
                    b.Navigation("Participants");
                });
#pragma warning restore 612, 618
        }
    }
}