using Microsoft.EntityFrameworkCore;
using Rinoleggio.Domain.Entities;
using Rinoleggio.Domain.Enums;
using Rinoleggio.Infrastructure.Data;

namespace Rinoleggio.Infrastructure.Listings;

public class ListingService(RinoleggioDbContext db)
{
    public async Task<IReadOnlyList<Listing>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default) =>
        await db.Listings
            .AsNoTracking()
            .Include(l => l.Vehicle)!.ThenInclude(v => v!.Brand)
            .Include(l => l.Contract)
            .Where(l => l.OwnerId == ownerId && l.BusinessStatus != ListingBusinessStatus.Deleted)
            .OrderByDescending(l => l.CreationTime)
            .ToListAsync(ct);

    public async Task<Listing?> GetOwnedListingAsync(Guid listingId, Guid ownerId, CancellationToken ct = default) =>
        await db.Listings
            .Include(l => l.Contract)
            .Include(l => l.Vehicle)
            .Include(l => l.Services)
            .FirstOrDefaultAsync(l => l.Id == listingId && l.OwnerId == ownerId, ct);

    public async Task<(bool Success, string? Error, Guid? ListingId)> SaveDraftAsync(
        Guid ownerId,
        ListingEditorModel input,
        CancellationToken ct = default)
    {
        var validationError = ValidateDraft(input);
        if (validationError is not null)
        {
            return (false, validationError, null);
        }

        var brand = await db.Brands.AsNoTracking().FirstOrDefaultAsync(b => b.Id == input.BrandId && b.IsActive, ct);
        if (brand is null)
        {
            return (false, "Marca non valida.", null);
        }

        Listing listing;
        if (input.ListingId is { } id)
        {
            listing = await db.Listings
                .Include(l => l.Contract)
                .Include(l => l.Vehicle)
                .Include(l => l.Services)
                .FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == ownerId, ct)
                ?? throw new InvalidOperationException("Annuncio non trovato.");

            listing.Title = input.Title.Trim();
            listing.DescriptionHtml = input.DescriptionHtml;
            listing.ProvinceId = input.ProvinceId;
            listing.TransferContributionType = input.TransferContributionType;
            listing.TransferContributionAmount = input.TransferContributionAmount;
            listing.LastModificationTime = DateTime.UtcNow;
        }
        else
        {
            var baseSlug = ListingSlugGenerator.BuildBaseSlug(input.Title, brand.Name, input.Model);
            var slug = await ListingSlugGenerator.EnsureUniqueSlugAsync(
                async s => await db.Listings.AnyAsync(l => l.SlugPath == s && l.Id != input.ListingId, ct),
                baseSlug);

            listing = new Listing
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = input.Title.Trim(),
                DescriptionHtml = input.DescriptionHtml,
                SlugPath = slug,
                ProvinceId = input.ProvinceId,
                TransferContributionType = input.TransferContributionType,
                TransferContributionAmount = input.TransferContributionAmount,
                BusinessStatus = ListingBusinessStatus.Active,
                IsPublished = false,
                CreationTime = DateTime.UtcNow
            };
            db.Listings.Add(listing);
        }

        var remainingKm = Math.Max(0, input.TotalKm - input.CurrentKm);

        if (listing.Contract is null)
        {
            listing.Contract = new ListingContract { ListingId = listing.Id };
            db.ListingContracts.Add(listing.Contract);
        }

        listing.Contract.RentalCompanyId = input.RentalCompanyId;
        listing.Contract.StartDate = input.StartDate;
        listing.Contract.EndDate = input.EndDate;
        listing.Contract.MonthlyFee = input.MonthlyFee;
        listing.Contract.VatIncluded = input.VatIncluded;
        listing.Contract.InitialPayment = input.InitialPayment;
        listing.Contract.TotalKm = input.TotalKm;
        listing.Contract.CurrentKm = input.CurrentKm;
        listing.Contract.RemainingKm = remainingKm;
        listing.Contract.ExcessKmRate = input.ExcessKmRate;
        listing.Contract.UnderKmRate = input.UnderKmRate;

        if (listing.Vehicle is null)
        {
            listing.Vehicle = new ListingVehicle { ListingId = listing.Id };
            db.ListingVehicles.Add(listing.Vehicle);
        }

        listing.Vehicle.BrandId = input.BrandId;
        listing.Vehicle.Model = input.Model.Trim();
        listing.Vehicle.Version = input.Version;
        listing.Vehicle.FuelType = input.FuelType;
        listing.Vehicle.Transmission = input.Transmission;
        listing.Vehicle.RegistrationYear = input.RegistrationYear;

        if (listing.Services is null)
        {
            listing.Services = new ListingServices { ListingId = listing.Id };
            db.ListingServices.Add(listing.Services);
        }

        listing.Services.MaintenanceIncluded = input.MaintenanceIncluded;
        listing.Services.InsuranceIncluded = input.InsuranceIncluded;
        listing.Services.RoadsideAssistanceIncluded = input.RoadsideAssistanceIncluded;
        listing.Services.ReplacementCarIncluded = input.ReplacementCarIncluded;
        listing.Services.TiresIncluded = input.TiresIncluded;
        listing.Services.RoadTaxIncluded = input.RoadTaxIncluded;

        await db.SaveChangesAsync(ct);
        return (true, null, listing.Id);
    }

    public async Task<bool> SoftDeleteAsync(Guid listingId, Guid ownerId, string reason, CancellationToken ct = default)
    {
        var listing = await db.Listings.FirstOrDefaultAsync(l => l.Id == listingId && l.OwnerId == ownerId, ct);
        if (listing is null)
        {
            return false;
        }

        listing.BusinessStatus = ListingBusinessStatus.Deleted;
        listing.StatusReason = reason.Trim();
        listing.StatusChangedBy = StatusChangedBy.Seller;
        listing.IsPublished = false;
        listing.LastModificationTime = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static string? ValidateDraft(ListingEditorModel input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
        {
            return "Il titolo è obbligatorio.";
        }

        if (input.TotalKm < 1)
        {
            return "I km totali devono essere almeno 1.";
        }

        if (input.CurrentKm < 0)
        {
            return "I km percorsi non possono essere negativi.";
        }

        if (input.TransferContributionType == TransferContributionType.None && input.TransferContributionAmount > 0)
        {
            return "Con contributo 'Nessuno' l'importo deve essere zero.";
        }

        if (input.TransferContributionType != TransferContributionType.None && input.TransferContributionAmount <= 0)
        {
            return "Indica un importo per il contributo di subentro.";
        }

        if (input.StartDate is { } start && input.EndDate <= start)
        {
            return "La data di fine deve essere successiva alla data di inizio.";
        }

        return null;
    }

    public static ListingEditorModel FromListing(Listing listing)
    {
        var model = new ListingEditorModel
        {
            ListingId = listing.Id,
            Title = listing.Title,
            DescriptionHtml = listing.DescriptionHtml,
            ProvinceId = listing.ProvinceId,
            TransferContributionType = listing.TransferContributionType,
            TransferContributionAmount = listing.TransferContributionAmount
        };

        if (listing.Contract is not null)
        {
            model.RentalCompanyId = listing.Contract.RentalCompanyId;
            model.StartDate = listing.Contract.StartDate;
            model.EndDate = listing.Contract.EndDate;
            model.MonthlyFee = listing.Contract.MonthlyFee;
            model.VatIncluded = listing.Contract.VatIncluded;
            model.InitialPayment = listing.Contract.InitialPayment;
            model.TotalKm = listing.Contract.TotalKm;
            model.CurrentKm = listing.Contract.CurrentKm;
            model.ExcessKmRate = listing.Contract.ExcessKmRate;
            model.UnderKmRate = listing.Contract.UnderKmRate;
        }

        if (listing.Vehicle is not null)
        {
            model.BrandId = listing.Vehicle.BrandId;
            model.Model = listing.Vehicle.Model;
            model.Version = listing.Vehicle.Version;
            model.FuelType = listing.Vehicle.FuelType;
            model.Transmission = listing.Vehicle.Transmission;
            model.RegistrationYear = listing.Vehicle.RegistrationYear;
        }

        if (listing.Services is not null)
        {
            model.MaintenanceIncluded = listing.Services.MaintenanceIncluded;
            model.InsuranceIncluded = listing.Services.InsuranceIncluded;
            model.RoadsideAssistanceIncluded = listing.Services.RoadsideAssistanceIncluded;
            model.ReplacementCarIncluded = listing.Services.ReplacementCarIncluded;
            model.TiresIncluded = listing.Services.TiresIncluded;
            model.RoadTaxIncluded = listing.Services.RoadTaxIncluded;
        }

        return model;
    }
}
