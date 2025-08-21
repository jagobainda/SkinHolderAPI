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

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Useritem> Useritems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PRIMARY");

            entity.ToTable("items");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Gamerpaynombre)
                .HasMaxLength(300)
                .HasColumnName("gamerpaynombre");
            entity.Property(e => e.Hashnamesteam)
                .HasMaxLength(300)
                .HasColumnName("hashnamesteam");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Itemprecio>(entity =>
        {
            entity.HasKey(e => e.Itemprecioid).HasName("PRIMARY");

            entity.ToTable("itemprecio");

            entity.HasIndex(e => e.Registroid, "fk_registroid_itemprecio");

            entity.HasIndex(e => e.Useritemid, "fk_useritemid_itemprecio");

            entity.Property(e => e.Itemprecioid).HasColumnName("itemprecioid");
            entity.Property(e => e.Preciocsfloat)
                .HasPrecision(10, 2)
                .HasColumnName("preciocsfloat");
            entity.Property(e => e.Preciogamerpay)
                .HasPrecision(10, 2)
                .HasColumnName("preciogamerpay");
            entity.Property(e => e.Preciosteam)
                .HasPrecision(10, 2)
                .HasColumnName("preciosteam");
            entity.Property(e => e.Registroid).HasColumnName("registroid");
            entity.Property(e => e.Useritemid).HasColumnName("useritemid");

            entity.HasOne(d => d.Registro).WithMany(p => p.Itemprecios)
                .HasForeignKey(d => d.Registroid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_registroid_itemprecio");

            entity.HasOne(d => d.Useritem).WithMany(p => p.Itemprecios)
                .HasForeignKey(d => d.Useritemid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_useritemid_itemprecio");
        });

        modelBuilder.Entity<Registro>(entity =>
        {
            entity.HasKey(e => e.Registroid).HasName("PRIMARY");

            entity.ToTable("registros");

            entity.HasIndex(e => e.Userid, "fk_userid_registros");

            entity.Property(e => e.Registroid).HasColumnName("registroid");
            entity.Property(e => e.Fechahora)
                .HasColumnType("datetime")
                .HasColumnName("fechahora");
            entity.Property(e => e.Totalcsfloat)
                .HasPrecision(10, 2)
                .HasColumnName("totalcsfloat");
            entity.Property(e => e.Totalgamerpay)
                .HasPrecision(10, 2)
                .HasColumnName("totalgamerpay");
            entity.Property(e => e.Totalsteam)
                .HasPrecision(10, 2)
                .HasColumnName("totalsteam");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Registros)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_userid_registros");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("createdat");
            entity.Property(e => e.Isactive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("isactive");
            entity.Property(e => e.Isbanned).HasColumnName("isbanned");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(128)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Useritem>(entity =>
        {
            entity.HasKey(e => e.Useritemid).HasName("PRIMARY");

            entity.ToTable("useritems");

            entity.HasIndex(e => e.Itemid, "fk_itemid_useritems");

            entity.HasIndex(e => e.Userid, "fk_userid_useritems");

            entity.Property(e => e.Useritemid).HasColumnName("useritemid");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Preciomediocompra)
                .HasPrecision(10, 2)
                .HasColumnName("preciomediocompra");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Item).WithMany(p => p.Useritems)
                .HasForeignKey(d => d.Itemid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_itemid_useritems");

            entity.HasOne(d => d.User).WithMany(p => p.Useritems)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_userid_useritems");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
