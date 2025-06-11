using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.Models.Logs;

namespace SkinHolderAPI.DataService.Contexts;

public partial class SkinHolderLogDbContext : DbContext
{
    public SkinHolderLogDbContext(DbContextOptions<SkinHolderLogDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Logger> Loggers { get; set; }

    public virtual DbSet<Logplace> Logplaces { get; set; }

    public virtual DbSet<Logtype> Logtypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Logger>(entity =>
        {
            entity.HasKey(e => e.LoggerId).HasName("PRIMARY");

            entity.ToTable("logger");

            entity.HasIndex(e => e.LogPlaceId, "fk_LogPlaceID_LogPlace");

            entity.HasIndex(e => e.LogTypeId, "fk_LogTypeID_LogType");

            entity.HasIndex(e => e.UserId, "fk_UserID_Logger");

            entity.Property(e => e.LogDateTime).HasColumnType("datetime");
            entity.Property(e => e.LogDescription).HasMaxLength(5000);
            entity.Property(e => e.LogPlaceId).HasColumnName("LogPlaceID");
            entity.Property(e => e.LogTypeId).HasColumnName("LogTypeID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.LogPlace).WithMany(p => p.Loggers)
                .HasForeignKey(d => d.LogPlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_LogPlaceID_LogPlace");

            entity.HasOne(d => d.LogType).WithMany(p => p.Loggers)
                .HasForeignKey(d => d.LogTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_LogTypeID_LogType");

            entity.HasOne(d => d.User).WithMany(p => p.Loggers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserID_Logger");
        });

        modelBuilder.Entity<Logplace>(entity =>
        {
            entity.HasKey(e => e.LogPlaceId).HasName("PRIMARY");

            entity.ToTable("logplace");

            entity.Property(e => e.LogPlaceId).HasColumnName("LogPlaceID");
            entity.Property(e => e.PlaceName).HasMaxLength(7);
        });

        modelBuilder.Entity<Logtype>(entity =>
        {
            entity.HasKey(e => e.LogTypeId).HasName("PRIMARY");

            entity.ToTable("logtype");

            entity.Property(e => e.LogTypeId).HasColumnName("LogTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(7);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Username).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
