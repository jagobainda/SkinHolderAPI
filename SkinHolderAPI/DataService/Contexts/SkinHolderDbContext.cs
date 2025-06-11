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

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Itemprecio> Itemprecios { get; set; }

    public virtual DbSet<Registro> Registros { get; set; }

    public virtual DbSet<Registrotype> Registrotypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Useritem> Useritems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PRIMARY");

            entity.ToTable("items");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.GamerPayNombre).HasMaxLength(300);
            entity.Property(e => e.HashNameSteam).HasMaxLength(300);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Itemprecio>(entity =>
        {
            entity.HasKey(e => e.ItemPrecioId).HasName("PRIMARY");

            entity.ToTable("itemprecio");

            entity.HasIndex(e => e.RegistroId, "fk_RegistroID_ItemPrecio");

            entity.HasIndex(e => e.UserItemId, "fk_UserItemID_ItemPrecio");

            entity.Property(e => e.ItemPrecioId).HasColumnName("ItemPrecioID");
            entity.Property(e => e.PrecioGamerPay).HasPrecision(10, 2);
            entity.Property(e => e.PrecioSteam).HasPrecision(10, 2);
            entity.Property(e => e.RegistroId).HasColumnName("RegistroID");
            entity.Property(e => e.UserItemId).HasColumnName("UserItemID");

            entity.HasOne(d => d.Registro).WithMany(p => p.Itemprecios)
                .HasForeignKey(d => d.RegistroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RegistroID_ItemPrecio");

            entity.HasOne(d => d.UserItem).WithMany(p => p.Itemprecios)
                .HasForeignKey(d => d.UserItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserItemID_ItemPrecio");
        });

        modelBuilder.Entity<Registro>(entity =>
        {
            entity.HasKey(e => e.RegistroId).HasName("PRIMARY");

            entity.ToTable("registros");

            entity.HasIndex(e => e.RegistroTypeId, "fk_UserID_RegistroType");

            entity.HasIndex(e => e.UserId, "fk_UserID_Registros");

            entity.Property(e => e.RegistroId).HasColumnName("RegistroID");
            entity.Property(e => e.FechaHora).HasColumnType("datetime");
            entity.Property(e => e.RegistroTypeId).HasColumnName("RegistroTypeID");
            entity.Property(e => e.TotalGamerPay).HasPrecision(10, 2);
            entity.Property(e => e.TotalSteam).HasPrecision(10, 2);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.RegistroType).WithMany(p => p.Registros)
                .HasForeignKey(d => d.RegistroTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserID_RegistroType");

            entity.HasOne(d => d.User).WithMany(p => p.Registros)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserID_Registros");
        });

        modelBuilder.Entity<Registrotype>(entity =>
        {
            entity.HasKey(e => e.RegistroTypeId).HasName("PRIMARY");

            entity.ToTable("registrotype");

            entity.Property(e => e.RegistroTypeId).HasColumnName("RegistroTypeID");
            entity.Property(e => e.Type).HasMaxLength(8);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.PasswordHash).HasMaxLength(128);
            entity.Property(e => e.Username).HasMaxLength(20);
        });

        modelBuilder.Entity<Useritem>(entity =>
        {
            entity.HasKey(e => e.UserItemId).HasName("PRIMARY");

            entity.ToTable("useritems");

            entity.HasIndex(e => e.ItemId, "fk_ItemID_UserItems");

            entity.HasIndex(e => e.UserId, "fk_UserID_UserItems");

            entity.Property(e => e.UserItemId).HasColumnName("UserItemID");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.PrecioMedioCompra).HasPrecision(10, 2);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Item).WithMany(p => p.Useritems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ItemID_UserItems");

            entity.HasOne(d => d.User).WithMany(p => p.Useritems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UserID_UserItems");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
