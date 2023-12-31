﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RFIDify.Database;

#nullable disable

namespace RFIDify.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231229095849_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("RFIDify.RFID.DataTypes.RFIDTag", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("SpotifyUri")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("RFIDs");
                });

            modelBuilder.Entity("RFIDify.Spotify.DataTypes.SpotifyAccessToken", b =>
                {
                    b.Property<string>("RefreshToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExpiresAtUtc")
                        .HasColumnType("TEXT");

                    b.HasKey("RefreshToken");

                    b.ToTable("SpotifyAccessToken");
                });

            modelBuilder.Entity("RFIDify.Spotify.DataTypes.SpotifyCredentials", b =>
                {
                    b.Property<string>("ClientId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientSecret")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RedirectUri")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ClientId");

                    b.ToTable("SpotifyCredentials");
                });
#pragma warning restore 612, 618
        }
    }
}
