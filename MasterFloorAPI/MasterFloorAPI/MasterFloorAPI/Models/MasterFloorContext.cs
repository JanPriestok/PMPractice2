using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MasterFloorAPI.Models;

public partial class MasterFloorContext : DbContext
{
    public MasterFloorContext()
    {
    }

    public MasterFloorContext(DbContextOptions<MasterFloorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<PartnerRatingHistory> PartnerRatingHistories { get; set; }

    public virtual DbSet<PartnerType> PartnerTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Sale> Sale { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MasterFloor;Username=postgres;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("addresses_pkey");

            entity.ToTable("addresses");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Area)
                .HasMaxLength(100)
                .HasColumnName("area");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.House)
                .HasMaxLength(20)
                .HasColumnName("house");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            entity.Property(e => e.Street)
                .HasMaxLength(100)
                .HasColumnName("street");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("materials_pkey");

            entity.ToTable("materials");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DefectRate)
                .HasPrecision(5, 2)
                .HasColumnName("defect_rate");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("partners_pkey");

            entity.ToTable("partners");

            entity.HasIndex(e => e.Inn, "partners_inn_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DirectorFirstName)
                .HasMaxLength(50)
                .HasColumnName("director_first_name");
            entity.Property(e => e.DirectorLastName)
                .HasMaxLength(50)
                .HasColumnName("director_last_name");
            entity.Property(e => e.DirectorMiddleName)
                .HasMaxLength(50)
                .HasColumnName("director_middle_name");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Inn)
                .HasMaxLength(20)
                .HasColumnName("inn");
            entity.Property(e => e.LegalAddressId).HasColumnName("legal_address_id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.LegalAddress).WithMany(p => p.Partners)
                .HasForeignKey(d => d.LegalAddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partners_legal_address_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Partners)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partners_type_id_fkey");
        });

        modelBuilder.Entity<PartnerRatingHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("partner_rating_history_pkey");

            entity.ToTable("partner_rating_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy)
                .HasMaxLength(100)
                .HasColumnName("changed_by");
            entity.Property(e => e.NewRating).HasColumnName("new_rating");
            entity.Property(e => e.OldRating).HasColumnName("old_rating");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");

            entity.HasOne(d => d.Partner).WithMany(p => p.PartnerRatingHistories)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partner_rating_history_partner_id_fkey");
        });

        modelBuilder.Entity<PartnerType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("partner_types_pkey");

            entity.ToTable("partner_types");

            entity.HasIndex(e => e.Name, "partner_types_name_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Article).HasName("products_pkey");

            entity.ToTable("products");

            entity.HasIndex(e => e.Name, "products_name_key").IsUnique();

            entity.Property(e => e.Article)
                .HasMaxLength(20)
                .HasColumnName("article");
            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.MinPartnerPrice)
                .HasPrecision(12, 2)
                .HasColumnName("min_partner_price");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.HasOne(d => d.Material).WithMany(p => p.Products)
                .HasForeignKey(d => d.MaterialId)
                .HasConstraintName("products_material_id_fkey");

            entity.HasOne(d => d.Type).WithMany(p => p.Products)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("products_type_id_fkey");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_types_pkey");

            entity.ToTable("product_types");

            entity.HasIndex(e => e.Name, "product_types_name_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Coefficient)
                .HasPrecision(10, 2)
                .HasColumnName("coefficient");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        // Настройка сущности Sale
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sales_pkey");

            entity.ToTable("sales");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.ProductArticle)
                .HasMaxLength(20)
                .HasColumnName("product_article");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SaleDate).HasColumnName("sale_date");

            entity.HasOne(d => d.Partner)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sales_partner_id_fkey");

            entity.HasOne(d => d.ProductArticleNavigation)
                .WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductArticle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sales_product_article_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
