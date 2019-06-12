﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ParkingSpotsManager.Shared.Database;

namespace ParkingSpotsManager.API.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085");

            modelBuilder.Entity("ParkingSpotsManager.Shared.Models.Parking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Columns");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("Rows");

                    b.HasKey("Id");

                    b.ToTable("Parkings");
                });

            modelBuilder.Entity("ParkingSpotsManager.Shared.Models.Spot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsOccupiedByDefault");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime?>("OccupiedAt");

                    b.Property<int?>("OccupiedBy");

                    b.Property<int?>("OccupiedByDefaultBy");

                    b.Property<int>("ParkingId");

                    b.Property<DateTime?>("ReleasedAt");

                    b.HasKey("Id");

                    b.HasIndex("OccupiedBy");

                    b.HasIndex("OccupiedByDefaultBy");

                    b.HasIndex("ParkingId");

                    b.ToTable("Spots");
                });

            modelBuilder.Entity("ParkingSpotsManager.Shared.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthToken");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Firstname");

                    b.Property<string>("Lastname");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ParkingSpotsManager.Shared.Models.UserParking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IsAdmin");

                    b.Property<int>("ParkingId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ParkingId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersParkings");
                });

            modelBuilder.Entity("ParkingSpotsManager.Shared.Models.Spot", b =>
                {
                    b.HasOne("ParkingSpotsManager.Shared.Models.User", "Occupier")
                        .WithMany()
                        .HasForeignKey("OccupiedBy");

                    b.HasOne("ParkingSpotsManager.Shared.Models.User", "OccupierByDefault")
                        .WithMany()
                        .HasForeignKey("OccupiedByDefaultBy");

                    b.HasOne("ParkingSpotsManager.Shared.Models.Parking", "Parking")
                        .WithMany("Spots")
                        .HasForeignKey("ParkingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ParkingSpotsManager.Shared.Models.UserParking", b =>
                {
                    b.HasOne("ParkingSpotsManager.Shared.Models.Parking", "Parking")
                        .WithMany("UserParkings")
                        .HasForeignKey("ParkingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ParkingSpotsManager.Shared.Models.User", "User")
                        .WithMany("UserParkings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
