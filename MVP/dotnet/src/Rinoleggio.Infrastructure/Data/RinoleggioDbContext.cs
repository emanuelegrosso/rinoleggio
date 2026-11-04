using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rinoleggio.Domain.Entities;

namespace Rinoleggio.Infrastructure.Data;

public class RinoleggioDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public RinoleggioDbContext(DbContextOptions<RinoleggioDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<RentalCompany> RentalCompanies => Set<RentalCompany>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<ListingContract> ListingContracts => Set<ListingContract>();
    public DbSet<ListingVehicle> ListingVehicles => Set<ListingVehicle>();
    public DbSet<ListingServices> ListingServices => Set<ListingServices>();
    public DbSet<ListingInsuranceType> ListingInsuranceTypes => Set<ListingInsuranceType>();
    public DbSet<ListingImage> ListingImages => Set<ListingImage>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<SitePage> SitePages => Set<SitePage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserProfile>(e =>
        {
            e.HasKey(x => x.UserId);
            e.Property(x => x.FullName).HasMaxLength(80).IsRequired();
            e.Property(x => x.PhoneNumber).HasMaxLength(30);
            e.HasOne(x => x.User).WithOne(x => x.Profile).HasForeignKey<UserProfile>(x => x.UserId);
        });

        builder.Entity<RentalCompany>(e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
            e.Property(x => x.WebsiteUrl).HasMaxLength(500);
        });

        builder.Entity<Brand>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
        });

        builder.Entity<Province>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Code).HasMaxLength(2).IsFixedLength().IsRequired();
            e.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<Listing>(e =>
        {
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.SlugPath).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.SlugPath).IsUnique();
            e.HasIndex(x => new { x.IsPublished, x.BusinessStatus, x.ProvinceId });
            e.HasIndex(x => x.OwnerId);
            e.Property(x => x.TransferContributionAmount).HasPrecision(18, 2);
            e.Property(x => x.StatusReason).HasMaxLength(1000);
            e.Property(x => x.ConcurrencyStamp).HasMaxLength(40);
            e.HasOne(x => x.Owner).WithMany(x => x.OwnedListings).HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Province).WithMany(x => x.Listings).HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ListingContract>(e =>
        {
            e.HasKey(x => x.ListingId);
            e.Property(x => x.MonthlyFee).HasPrecision(18, 2);
            e.Property(x => x.InitialPayment).HasPrecision(18, 2);
            e.Property(x => x.ExcessKmRate).HasPrecision(18, 3);
            e.Property(x => x.UnderKmRate).HasPrecision(18, 3);
            e.HasOne(x => x.Listing).WithOne(x => x.Contract).HasForeignKey<ListingContract>(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ListingVehicle>(e =>
        {
            e.HasKey(x => x.ListingId);
            e.Property(x => x.Model).HasMaxLength(80).IsRequired();
            e.Property(x => x.Version).HasMaxLength(120);
            e.HasOne(x => x.Listing).WithOne(x => x.Vehicle).HasForeignKey<ListingVehicle>(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ListingServices>(e =>
        {
            e.HasKey(x => x.ListingId);
            e.HasOne(x => x.Listing).WithOne(x => x.Services).HasForeignKey<ListingServices>(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ListingInsuranceType>(e =>
        {
            e.HasKey(x => new { x.ListingId, x.InsuranceKind });
            e.HasOne(x => x.Listing).WithMany(x => x.InsuranceTypes).HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ListingImage>(e =>
        {
            e.Property(x => x.StoragePathOrBlobId).HasMaxLength(500).IsRequired();
            e.HasIndex(x => x.ListingId);
        });

        builder.Entity<Lead>(e =>
        {
            e.Property(x => x.Title).HasMaxLength(200);
            e.Property(x => x.Message).HasMaxLength(1000).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30);
            e.HasIndex(x => x.ListingId);
            e.HasIndex(x => x.SellerId);
            e.HasIndex(x => new { x.BuyerId, x.ListingId });
            e.HasOne(x => x.Buyer).WithMany(x => x.LeadsAsBuyer).HasForeignKey(x => x.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Seller).WithMany(x => x.LeadsAsSeller).HasForeignKey(x => x.SellerId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Listing).WithMany(x => x.Leads).HasForeignKey(x => x.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SitePage>(e =>
        {
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.SlugPath).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.SlugPath).IsUnique();
        });
    }
}
