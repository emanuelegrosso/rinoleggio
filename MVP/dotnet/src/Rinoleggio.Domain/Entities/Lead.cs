using Rinoleggio.Domain.Enums;

namespace Rinoleggio.Domain.Entities;

public class Lead
{
    public Guid Id { get; set; }

    public Guid ListingId { get; set; }

    public Listing Listing { get; set; } = null!;

    public Guid BuyerId { get; set; }

    public AppUser Buyer { get; set; } = null!;

    public Guid SellerId { get; set; }

    public AppUser Seller { get; set; } = null!;

    public string? Title { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public bool PrivacyConsent { get; set; }

    public LeadStatus Status { get; set; } = LeadStatus.New;

    public string? BackofficeNotes { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
}
