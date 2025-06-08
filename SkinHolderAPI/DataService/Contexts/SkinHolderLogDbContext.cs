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

    public virtual DbSet<logger> loggers { get; set; }

    public virtual DbSet<logplace> logplaces { get; set; }

    public virtual DbSet<logtype> logtypes { get; set; }

    public virtual DbSet<user> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<logger>(entity =>
        {
            entity.HasKey(e => e.LoggerId).HasName("PRIMARY");

            entity.ToTable("logger");

            entity.HasIndex(e => e.LogPlaceID, "fk_LogPlaceID_LogPlace");

            entity.HasIndex(e => e.LogTypeID, "fk_LogTypeID_LogType");

            entity.HasIndex(e => e.UserID, "fk_UserID_Logger");

            entity.Property(e => e.LogDateTime).HasColumnType("datetime");
            entity.Property(e => e.LogDescription).HasMaxLength(5000);

            entity.HasOne(d => d.LogPlace).WithMany(p => p.loggers)
                .HasForeignKey(d => d.LogPlaceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_LogPlaceID_LogPlace");

            entity.HasOne(d => d.LogType).WithMany(p => p.loggers)
                .HasForeignKey(d => d.LogTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_LogTypeID_LogType");

            entity.HasOne(d => d.User).WithMany(p => p.loggers)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserID_Logger");
        });

        modelBuilder.Entity<logplace>(entity =>
        {
            entity.HasKey(e => e.LogPlaceID).HasName("PRIMARY");

            entity.ToTable("logplace");

            entity.Property(e => e.PlaceName).HasMaxLength(7);
        });

        modelBuilder.Entity<logtype>(entity =>
        {
            entity.HasKey(e => e.LogTypeID).HasName("PRIMARY");

            entity.ToTable("logtype");

            entity.Property(e => e.TypeName).HasMaxLength(7);
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("PRIMARY");

            entity.Property(e => e.Username).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
