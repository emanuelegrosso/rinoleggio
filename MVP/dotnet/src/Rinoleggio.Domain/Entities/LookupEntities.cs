namespace Rinoleggio.Domain.Entities;

public class RentalCompany
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? WebsiteUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;

    public ICollection<ListingContract> Contracts { get; set; } = new List<ListingContract>();
}

public class Brand
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ListingVehicle> Vehicles { get; set; } = new List<ListingVehicle>();
}

public class Province
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
