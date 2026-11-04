using System.ComponentModel.DataAnnotations;
using Rinoleggio.Domain.Enums;

namespace Rinoleggio.Infrastructure.Listings;

public class ListingEditorModel
{
    public Guid? ListingId { get; set; }

    [Required(ErrorMessage = "Il titolo è obbligatorio.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? DescriptionHtml { get; set; }

    [Required]
    public Guid ProvinceId { get; set; }

    public TransferContributionType TransferContributionType { get; set; } = TransferContributionType.None;

    public decimal TransferContributionAmount { get; set; }

    [Required]
    public Guid RentalCompanyId { get; set; }

    public DateOnly? StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MonthlyFee { get; set; }

    public bool VatIncluded { get; set; } = true;

    public decimal InitialPayment { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalKm { get; set; }

    [Range(0, int.MaxValue)]
    public int CurrentKm { get; set; }

    public decimal? ExcessKmRate { get; set; }

    public decimal? UnderKmRate { get; set; }

    [Required]
    public Guid BrandId { get; set; }

    [Required]
    [StringLength(80)]
    public string Model { get; set; } = string.Empty;

    public string? Version { get; set; }

    public FuelType FuelType { get; set; } = FuelType.Petrol;

    public Transmission Transmission { get; set; } = Transmission.Manual;

    public short RegistrationYear { get; set; } = (short)DateTime.UtcNow.Year;

    public bool MaintenanceIncluded { get; set; }

    public bool InsuranceIncluded { get; set; }

    public bool RoadsideAssistanceIncluded { get; set; }

    public bool ReplacementCarIncluded { get; set; }

    public bool TiresIncluded { get; set; }

    public bool RoadTaxIncluded { get; set; }
}
