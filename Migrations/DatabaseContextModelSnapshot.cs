﻿// <auto-generated />
using System;
using MainBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MainBot.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MainBot.Database.Models.DiscordChannel", b =>
                {
                    b.Property<Guid>("key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("guildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.HasKey("key");

                    b.ToTable("NukeChannels");
                });

            modelBuilder.Entity("MainBot.Database.Models.Guild", b =>
                {
                    b.Property<Guid>("key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("guildSettingskey")
                        .HasColumnType("uuid");

                    b.Property<decimal>("id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("key");

                    b.HasIndex("guildSettingskey");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("MainBot.Database.Models.GuildSettings", b =>
                {
                    b.Property<Guid>("key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal?>("administratorRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("commandLogChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("hiddenRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("messageLogChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("moderatorRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("muteRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("rainbowRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("systemLogChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<long[]>("uglyColours")
                        .HasColumnType("bigint[]");

                    b.Property<decimal?>("userLogChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal?>("verifyRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("key");

                    b.ToTable("GuildSettings");
                });

            modelBuilder.Entity("MainBot.Database.Models.Logs.ErrorLog", b =>
                {
                    b.Property<Guid>("key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("errorTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("extraInformation")
                        .HasColumnType("text");

                    b.Property<string>("message")
                        .HasColumnType("text");

                    b.Property<string>("source")
                        .HasColumnType("text");

                    b.Property<string>("stackTrace")
                        .HasColumnType("text");

                    b.HasKey("key");

                    b.ToTable("Errors");
                });

            modelBuilder.Entity("MainBot.Database.Models.MuteUser", b =>
                {
                    b.Property<Guid>("key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("guildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<DateTime>("muteExpiryDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("muteRoleId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("key");

                    b.ToTable("MutedUsers");
                });

            modelBuilder.Entity("MainBot.Database.Models.Guild", b =>
                {
                    b.HasOne("MainBot.Database.Models.GuildSettings", "guildSettings")
                        .WithMany()
                        .HasForeignKey("guildSettingskey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("guildSettings");
                });
#pragma warning restore 612, 618
        }
    }
}
