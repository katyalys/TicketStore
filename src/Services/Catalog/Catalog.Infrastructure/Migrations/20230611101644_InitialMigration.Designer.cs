﻿// <auto-generated />
using System;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    [DbContext(typeof(CatalogContext))]
    [Migration("20230611101644_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Catalog.Domain.Entities.Concert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(550)
                        .HasColumnType("nvarchar(550)");

                    b.Property<string>("GenreName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Perfomer")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int?>("PlaceId")
                        .HasColumnType("int");

                    b.Property<string>("PosterLink")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PlaceId");

                    b.ToTable("Concerts");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Place", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("PlaceNumber")
                        .HasColumnType("int");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Places");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Sector", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Name")
                        .HasColumnType("int");

                    b.Property<string>("PictureLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PlaceId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("RowNumber")
                        .HasColumnType("int");

                    b.Property<int>("RowSeatNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlaceId");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Statuses");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ConcertId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("Row")
                        .HasColumnType("int");

                    b.Property<int>("Seat")
                        .HasColumnType("int");

                    b.Property<int?>("SectorId")
                        .HasColumnType("int");

                    b.Property<int?>("StatusId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ConcertId");

                    b.HasIndex("SectorId");

                    b.HasIndex("StatusId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Concert", b =>
                {
                    b.HasOne("Catalog.Domain.Entities.Place", "Place")
                        .WithMany("Concerts")
                        .HasForeignKey("PlaceId");

                    b.Navigation("Place");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Sector", b =>
                {
                    b.HasOne("Catalog.Domain.Entities.Place", "Place")
                        .WithMany("Sectors")
                        .HasForeignKey("PlaceId");

                    b.Navigation("Place");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Ticket", b =>
                {
                    b.HasOne("Catalog.Domain.Entities.Concert", "Concert")
                        .WithMany("Tickets")
                        .HasForeignKey("ConcertId");

                    b.HasOne("Catalog.Domain.Entities.Sector", "Sector")
                        .WithMany("Tickets")
                        .HasForeignKey("SectorId");

                    b.HasOne("Catalog.Domain.Entities.Status", "Status")
                        .WithMany("Tickets")
                        .HasForeignKey("StatusId");

                    b.Navigation("Concert");

                    b.Navigation("Sector");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Concert", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Place", b =>
                {
                    b.Navigation("Concerts");

                    b.Navigation("Sectors");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Sector", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.Status", b =>
                {
                    b.Navigation("Tickets");
                });
#pragma warning restore 612, 618
        }
    }
}
