﻿// <auto-generated />
using System;
using DataAccessLayers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLayers.Migrations
{
    [DbContext(typeof(MovieprojectContext))]
    [Migration("20250325092008_removewallet")]
    partial class removewallet
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BusinessObjects.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.Property<byte?>("Status")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("BusinessObjects.Bill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<int?>("PromotionId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int?>("TicketId")
                        .HasColumnType("int");

                    b.Property<int>("TotalPrice")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("TicketId");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("BusinessObjects.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("BusinessObjects.CinemaRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("RoomName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CinemaRooms");
                });

            modelBuilder.Entity("BusinessObjects.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DirectorName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte?>("Status")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("BusinessObjects.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("BusinessObjects.Seat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CinemaRoomId")
                        .HasColumnType("int");

                    b.Property<string>("SeatNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CinemaRoomId");

                    b.ToTable("Seats");
                });

            modelBuilder.Entity("BusinessObjects.ShowTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AvailableSeats")
                        .HasColumnType("int");

                    b.Property<int>("CinemaRoomId")
                        .HasColumnType("int");

                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ShowDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("TicketQuantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CinemaRoomId");

                    b.HasIndex("MovieId");

                    b.ToTable("ShowTimes");
                });

            modelBuilder.Entity("BusinessObjects.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("MovieId")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("SeatId")
                        .HasColumnType("int");

                    b.Property<int>("ShowtimeId")
                        .HasColumnType("int");

                    b.Property<byte?>("Status")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.HasIndex("SeatId");

                    b.HasIndex("ShowtimeId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("BusinessObjects.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BillId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BillId")
                        .IsUnique()
                        .HasFilter("[BillId] IS NOT NULL");

                    b.HasIndex("TypeId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("BusinessObjects.TransactionHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Time")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<int?>("TransactionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("TransactionId");

                    b.ToTable("TransactionHistories");
                });

            modelBuilder.Entity("BusinessObjects.TransactionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TransactionTypes");
                });

            modelBuilder.Entity("BusinessObjects.Account", b =>
                {
                    b.HasOne("BusinessObjects.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_Account_Role");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BusinessObjects.Bill", b =>
                {
                    b.HasOne("BusinessObjects.Account", "Account")
                        .WithMany("Bills")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_Bill_Account");

                    b.HasOne("BusinessObjects.Ticket", "Ticket")
                        .WithMany("Bills")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Bill_Ticket");

                    b.Navigation("Account");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("BusinessObjects.Movie", b =>
                {
                    b.HasOne("BusinessObjects.Category", "Category")
                        .WithMany("Movies")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_Movie_Category");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("BusinessObjects.Seat", b =>
                {
                    b.HasOne("BusinessObjects.CinemaRoom", "CinemaRoom")
                        .WithMany("Seats")
                        .HasForeignKey("CinemaRoomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Seat_CinemaRoom");

                    b.Navigation("CinemaRoom");
                });

            modelBuilder.Entity("BusinessObjects.ShowTime", b =>
                {
                    b.HasOne("BusinessObjects.CinemaRoom", "CinemaRoom")
                        .WithMany("ShowTimes")
                        .HasForeignKey("CinemaRoomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_ShowTime_CinemaRoom");

                    b.HasOne("BusinessObjects.Movie", "Movie")
                        .WithMany("ShowTimes")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_ShowTime_Movie");

                    b.Navigation("CinemaRoom");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("BusinessObjects.Ticket", b =>
                {
                    b.HasOne("BusinessObjects.Movie", "Movie")
                        .WithMany("Tickets")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_Ticket_Movie");

                    b.HasOne("BusinessObjects.Seat", "Seat")
                        .WithMany("Tickets")
                        .HasForeignKey("SeatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Ticket_Seat");

                    b.HasOne("BusinessObjects.ShowTime", "Showtime")
                        .WithMany("Tickets")
                        .HasForeignKey("ShowtimeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Ticket_ShowTime");

                    b.Navigation("Movie");

                    b.Navigation("Seat");

                    b.Navigation("Showtime");
                });

            modelBuilder.Entity("BusinessObjects.Transaction", b =>
                {
                    b.HasOne("BusinessObjects.Bill", "Bill")
                        .WithOne("Transaction")
                        .HasForeignKey("BusinessObjects.Transaction", "BillId")
                        .HasConstraintName("FK_Transaction_Bill");

                    b.HasOne("BusinessObjects.TransactionType", "Type")
                        .WithMany("Transactions")
                        .HasForeignKey("TypeId")
                        .HasConstraintName("FK_Transaction_TransactionType");

                    b.Navigation("Bill");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("BusinessObjects.TransactionHistory", b =>
                {
                    b.HasOne("BusinessObjects.Account", "Account")
                        .WithMany("TransactionHistories")
                        .HasForeignKey("AccountId")
                        .HasConstraintName("FK_TransactionHistory_Account");

                    b.HasOne("BusinessObjects.Transaction", "Transaction")
                        .WithMany("TransactionHistories")
                        .HasForeignKey("TransactionId")
                        .HasConstraintName("FK_TransactionHistory_Transaction");

                    b.Navigation("Account");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("BusinessObjects.Account", b =>
                {
                    b.Navigation("Bills");

                    b.Navigation("TransactionHistories");
                });

            modelBuilder.Entity("BusinessObjects.Bill", b =>
                {
                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("BusinessObjects.Category", b =>
                {
                    b.Navigation("Movies");
                });

            modelBuilder.Entity("BusinessObjects.CinemaRoom", b =>
                {
                    b.Navigation("Seats");

                    b.Navigation("ShowTimes");
                });

            modelBuilder.Entity("BusinessObjects.Movie", b =>
                {
                    b.Navigation("ShowTimes");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("BusinessObjects.Role", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("BusinessObjects.Seat", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("BusinessObjects.ShowTime", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("BusinessObjects.Ticket", b =>
                {
                    b.Navigation("Bills");
                });

            modelBuilder.Entity("BusinessObjects.Transaction", b =>
                {
                    b.Navigation("TransactionHistories");
                });

            modelBuilder.Entity("BusinessObjects.TransactionType", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
