﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WakaBot.Core.Data;

#nullable disable

namespace WakaBot.Core.Migrations.PostgreSql
{
    [DbContext(typeof(PostgreSqlContext))]
    [Migration("20230319210044_InitalCreate")]
    partial class InitalCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DiscordGuildDiscordUser", b =>
                {
                    b.Property<decimal>("GuildsId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("UsersId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("GuildsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("DiscordGuildDiscordUser");
                });

            modelBuilder.Entity("WakaBot.Core.Models.DiscordGuild", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.ToTable("DiscordGuilds");
                });

            modelBuilder.Entity("WakaBot.Core.Models.DiscordUser", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("WakaUserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("WakaUserId")
                        .IsUnique();

                    b.ToTable("DiscordUsers");
                });

            modelBuilder.Entity("WakaBot.Core.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("WakaName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WakaBot.Core.Models.WakaUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AccessToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<string>("Scope")
                        .HasColumnType("text");

                    b.Property<string>("State")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("usingOAuth")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("WakaUsers");
                });

            modelBuilder.Entity("DiscordGuildDiscordUser", b =>
                {
                    b.HasOne("WakaBot.Core.Models.DiscordGuild", null)
                        .WithMany()
                        .HasForeignKey("GuildsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WakaBot.Core.Models.DiscordUser", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WakaBot.Core.Models.DiscordUser", b =>
                {
                    b.HasOne("WakaBot.Core.Models.WakaUser", "WakaUser")
                        .WithMany()
                        .HasForeignKey("WakaUserId");

                    b.Navigation("WakaUser");
                });
#pragma warning restore 612, 618
        }
    }
}