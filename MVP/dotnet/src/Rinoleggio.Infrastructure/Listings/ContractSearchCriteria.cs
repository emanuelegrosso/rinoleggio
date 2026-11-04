using Rinoleggio.Domain.Enums;

namespace Rinoleggio.Infrastructure.Listings;

public class ContractSearchCriteria
{
    public Guid? ProvinceId { get; set; }

    public Guid? BrandId { get; set; }

    public Guid? RentalCompanyId { get; set; }

    public FuelType? FuelType { get; set; }

    public decimal? MonthlyFeeMin { get; set; }

    public decimal? MonthlyFeeMax { get; set; }

    public int? RemainingKmMin { get; set; }

    public string? Query { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 12;
}

public sealed class ContractSearchResult
{
    public required IReadOnlyList<ListingSummary> Items { get; init; }

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }
}

public sealed class ListingSummary
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string SlugPath { get; init; } = string.Empty;

    public string ProvinceName { get; init; } = string.Empty;

    public string BrandName { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public decimal MonthlyFee { get; init; }

    public bool VatIncluded { get; init; }

    public int RemainingKm { get; init; }

    public DateOnly EndDate { get; init; }

    public string RentalCompanyName { get; init; } = string.Empty;

    public FuelType FuelType { get; init; }
}
