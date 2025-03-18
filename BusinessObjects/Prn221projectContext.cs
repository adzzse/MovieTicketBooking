using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessObjects;

public class Prn221projectContext : DbContext
{
    private readonly string _connectionString;
    private readonly IConfiguration? configuration;
    
    public Prn221projectContext()
    {
        _connectionString = "Server=(local);uid=sa;pwd=123456789;database=TicketMovieBooking;TrustServerCertificate=True";
    }

    public Prn221projectContext(DbContextOptions<Prn221projectContext> options, IConfiguration configuration)
        : base(options)
    {
        this.configuration = configuration;
        _connectionString = this.configuration.GetConnectionString("DB") ?? "Server=(local);uid=sa;pwd=123456789;database=TicketMovieBooking;TrustServerCertificate=True";
    }

    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Bill> Bills { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<CinemaRoom> CinemaRooms { get; set; }
    public virtual DbSet<Movie> Movies { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Seat> Seats { get; set; }
    public virtual DbSet<ShowTime> ShowTimes { get; set; }
    public virtual DbSet<Ticket> Tickets { get; set; }
    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }
    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Wallet).HasDefaultValue(0.0f);

            entity.HasOne(d => d.Role)
                .WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.TotalPrice).IsRequired();

            entity.HasOne(d => d.Account)
                .WithMany(p => p.Bills)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bill_Account");

            entity.HasOne(d => d.Ticket)
                .WithMany(p => p.Bills)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Bill_Ticket");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Type).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<CinemaRoom>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.RoomName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Capacity).IsRequired();
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Image).HasMaxLength(255);

            entity.HasOne(d => d.Category)
                .WithMany(p => p.Movies)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Movie_Category");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.SeatNumber).IsRequired();

            entity.HasOne(d => d.CinemaRoom)
                .WithMany(p => p.Seats)
                .HasForeignKey(d => d.CinemaRoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Seat_CinemaRoom");
        });

        modelBuilder.Entity<ShowTime>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.TicketQuantity).IsRequired();
            entity.Property(e => e.ShowDateTime).IsRequired();
            entity.Property(e => e.AvailableSeats).IsRequired();

            entity.HasOne(d => d.CinemaRoom)
                .WithMany(p => p.ShowTimes)
                .HasForeignKey(d => d.CinemaRoomId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ShowTime_CinemaRoom");

            entity.HasOne(d => d.Movie)
                .WithMany(p => p.ShowTimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ShowTime_Movie");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Price).IsRequired();

            entity.HasOne(d => d.Movie)
                .WithMany(p => p.Tickets)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_Movie");

            entity.HasOne(d => d.Seat)
                .WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_Seat");

            entity.HasOne(d => d.Showtime)
                .WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_ShowTime");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Bill)
                .WithOne(p => p.Transaction)
                .HasForeignKey<Transaction>(d => d.BillId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transaction_Bill");

            entity.HasOne(d => d.Type)
                .WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Transaction_Type");
        });

        modelBuilder.Entity<TransactionHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Price).IsRequired();
            entity.Property(e => e.Time).HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.Transaction)
                .WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Transaction_history_Transaction");

            entity.HasOne(d => d.Account)
                .WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Transaction_history_Account");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        });
    }
}
