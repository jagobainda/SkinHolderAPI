using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.Models;

namespace SkinHolderAPI.DataService.Contexts;

public partial class SkinHolderDbContext : DbContext
{
    public SkinHolderDbContext(DbContextOptions<SkinHolderDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<item> items { get; set; }

    public virtual DbSet<itemprecio> itemprecios { get; set; }

    public virtual DbSet<registro> registros { get; set; }

    public virtual DbSet<registrotype> registrotypes { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<useritem> useritems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<item>(entity =>
        {
            entity.HasKey(e => e.ItemID).HasName("PRIMARY");

            entity.Property(e => e.GamerPayNombre).HasMaxLength(300);
            entity.Property(e => e.HashNameSteam).HasMaxLength(300);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<itemprecio>(entity =>
        {
            entity.HasKey(e => e.ItemPrecioID).HasName("PRIMARY");

            entity.ToTable("itemprecio");

            entity.HasIndex(e => e.RegistroID, "fk_RegistroID_ItemPrecio");

            entity.HasIndex(e => e.UserItemID, "fk_UserItemID_ItemPrecio");

            entity.Property(e => e.PrecioGamerPay).HasPrecision(10, 2);
            entity.Property(e => e.PrecioSteam).HasPrecision(10, 2);

            entity.HasOne(d => d.Registro).WithMany(p => p.itemprecios)
                .HasForeignKey(d => d.RegistroID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RegistroID_ItemPrecio");

            entity.HasOne(d => d.UserItem).WithMany(p => p.itemprecios)
                .HasForeignKey(d => d.UserItemID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserItemID_ItemPrecio");
        });

        modelBuilder.Entity<registro>(entity =>
        {
            entity.HasKey(e => e.RegistroID).HasName("PRIMARY");

            entity.HasIndex(e => e.RegistroTypeID, "fk_UserID_RegistroType");

            entity.HasIndex(e => e.UserID, "fk_UserID_Registros");

            entity.Property(e => e.FechaHora).HasColumnType("datetime");
            entity.Property(e => e.TotalGamerPay).HasPrecision(10, 2);
            entity.Property(e => e.TotalSteam).HasPrecision(10, 2);

            entity.HasOne(d => d.RegistroType).WithMany(p => p.registros)
                .HasForeignKey(d => d.RegistroTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserID_RegistroType");

            entity.HasOne(d => d.User).WithMany(p => p.registros)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserID_Registros");
        });

        modelBuilder.Entity<registrotype>(entity =>
        {
            entity.HasKey(e => e.RegistroTypeID).HasName("PRIMARY");

            entity.ToTable("registrotype");

            entity.Property(e => e.Type).HasMaxLength(8);
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("PRIMARY");

            entity.Property(e => e.PasswordHash).HasMaxLength(70);
            entity.Property(e => e.Username).HasMaxLength(20);
        });

        modelBuilder.Entity<useritem>(entity =>
        {
            entity.HasKey(e => e.UserItemID).HasName("PRIMARY");

            entity.HasIndex(e => e.ItemID, "fk_ItemID_UserItems");

            entity.HasIndex(e => e.UserID, "fk_UserID_UserItems");

            entity.Property(e => e.PrecioMedioCompra).HasPrecision(10, 2);

            entity.HasOne(d => d.Item).WithMany(p => p.useritems)
                .HasForeignKey(d => d.ItemID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ItemID_UserItems");

            entity.HasOne(d => d.User).WithMany(p => p.useritems)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserID_UserItems");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
