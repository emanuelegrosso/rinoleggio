using Rinoleggio.Domain.Enums;

namespace Rinoleggio.Domain.Entities;

public class Listing
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public AppUser Owner { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string? DescriptionHtml { get; set; }

    public string SlugPath { get; set; } = string.Empty;

    public Guid ProvinceId { get; set; }

    public Province Province { get; set; } = null!;

    public TransferContributionType TransferContributionType { get; set; } = TransferContributionType.None;

    public decimal TransferContributionAmount { get; set; }

    public ListingBusinessStatus BusinessStatus { get; set; } = ListingBusinessStatus.Active;

    public string? StatusReason { get; set; }

    public StatusChangedBy? StatusChangedBy { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;

    public DateTime? LastModificationTime { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public ListingContract? Contract { get; set; }

    public ListingVehicle? Vehicle { get; set; }

    public ListingServices? Services { get; set; }

    public ICollection<ListingInsuranceType> InsuranceTypes { get; set; } = new List<ListingInsuranceType>();

    public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();

    public ICollection<Lead> Leads { get; set; } = new List<Lead>();
}

public class ListingContract
{
    public Guid ListingId { get; set; }

    public Listing Listing { get; set; } = null!;

    public Guid RentalCompanyId { get; set; }

    public RentalCompany RentalCompany { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal MonthlyFee { get; set; }

    public bool VatIncluded { get; set; }

    public decimal InitialPayment { get; set; }

    public int TotalKm { get; set; }

    public int CurrentKm { get; set; }

    /// <summary>Persisted for filters: max(0, TotalKm - CurrentKm).</summary>
    public int RemainingKm { get; set; }

    public decimal? ExcessKmRate { get; set; }

    public decimal? UnderKmRate { get; set; }
}

public class ListingVehicle
{
    public Guid ListingId { get; set; }

    public Listing Listing { get; set; } = null!;

    public Guid BrandId { get; set; }

    public Brand Brand { get; set; } = null!;

    public string Model { get; set; } = string.Empty;

    public string? Version { get; set; }

    public FuelType FuelType { get; set; }

    public Transmission Transmission { get; set; }

    public short RegistrationYear { get; set; }
}

public class ListingServices
{
    public Guid ListingId { get; set; }

    public Listing Listing { get; set; } = null!;

    public bool MaintenanceIncluded { get; set; }

    public bool InsuranceIncluded { get; set; }

    public bool RoadsideAssistanceIncluded { get; set; }

    public bool ReplacementCarIncluded { get; set; }

    public bool TiresIncluded { get; set; }

    public bool RoadTaxIncluded { get; set; }
}

public class ListingInsuranceType
{
    public Guid ListingId { get; set; }

    public Listing Listing { get; set; } = null!;

    public InsuranceKind InsuranceKind { get; set; }
}

public class ListingImage
{
    public Guid Id { get; set; }

    public Guid ListingId { get; set; }

    public Listing Listing { get; set; } = null!;

    public string StoragePathOrBlobId { get; set; } = string.Empty;

    public byte SortOrder { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
}
