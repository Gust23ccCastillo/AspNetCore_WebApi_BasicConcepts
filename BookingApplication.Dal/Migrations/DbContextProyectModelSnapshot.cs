﻿// <auto-generated />
using System;
using BookingApplication.Dal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookingApplication.Dal.Migrations
{
    [DbContext(typeof(DbContextProyect))]
    partial class DbContextProyectModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.23")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BookingApplication.Domain.Models.Hotel", b =>
                {
                    b.Property<Guid>("HotelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("HotelName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("StarsAssigned")
                        .HasColumnType("int");

                    b.HasKey("HotelId");

                    b.ToTable("_TableHotels");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.HotelReservationDate", b =>
                {
                    b.Property<Guid>("HotelReservationDateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ReservationDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ReservationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("HotelReservationDateId");

                    b.HasIndex("ReservationId");

                    b.ToTable("_TableHotelReservationDates");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.Reservation", b =>
                {
                    b.Property<Guid>("ReservationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Customer")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("HotelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("RoomId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ReservationId");

                    b.HasIndex("HotelId");

                    b.HasIndex("RoomId");

                    b.ToTable("_TableReservations");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.Room", b =>
                {
                    b.Property<Guid>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("HotelId")
                        .IsRequired()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("NeedRepair")
                        .IsRequired()
                        .HasColumnType("bit");

                    b.Property<int?>("RoomNumber")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<double?>("RoomSize")
                        .IsRequired()
                        .HasColumnType("float");

                    b.HasKey("RoomId");

                    b.HasIndex("HotelId");

                    b.ToTable("_TableRooms");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.RoomReservationDate", b =>
                {
                    b.Property<Guid>("RoomReservationDateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ReservationDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RoomId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("RoomReservationDateId");

                    b.HasIndex("RoomId");

                    b.ToTable("_TableRoomReservationDates");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.HotelReservationDate", b =>
                {
                    b.HasOne("BookingApplication.Domain.Models.Reservation", "Reservation")
                        .WithMany("ListToDateReservatedInHotel")
                        .HasForeignKey("ReservationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.Reservation", b =>
                {
                    b.HasOne("BookingApplication.Domain.Models.Hotel", "HotelReservated")
                        .WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BookingApplication.Domain.Models.Room", "RoomReservated")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("HotelReservated");

                    b.Navigation("RoomReservated");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.Room", b =>
                {
                    b.HasOne("BookingApplication.Domain.Models.Hotel", "Hotel")
                        .WithMany("ListOfRooms")
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.RoomReservationDate", b =>
                {
                    b.HasOne("BookingApplication.Domain.Models.Room", "Room")
                        .WithMany("DateReservationForClient")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.Hotel", b =>
                {
                    b.Navigation("ListOfRooms");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.Reservation", b =>
                {
                    b.Navigation("ListToDateReservatedInHotel");
                });

            modelBuilder.Entity("BookingApplication.Domain.Models.Room", b =>
                {
                    b.Navigation("DateReservationForClient");
                });
#pragma warning restore 612, 618
        }
    }
}
