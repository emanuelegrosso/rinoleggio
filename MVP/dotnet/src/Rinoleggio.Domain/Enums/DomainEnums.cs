namespace Rinoleggio.Domain.Enums;

public enum ListingBusinessStatus
{
    Active,
    Reserved,
    Closed,
    Suspended,
    Deleted
}

public enum TransferContributionType
{
    None,
    Incentive,
    Compensation
}

public enum StatusChangedBy
{
    Seller,
    BackOffice
}

public enum LeadStatus
{
    New,
    Contacted,
    Closed
}

public enum AccountType
{
    Private,
    Business
}

public enum FuelType
{
    Petrol,
    Diesel,
    MildHybrid,
    PlugInHybrid,
    Electric,
    LPG,
    CNG
}

public enum Transmission
{
    Manual,
    Automatic
}

public enum InsuranceKind
{
    RCA,
    Kasko,
    FurtoIncendio,
    Cristalli,
    Infortuni,
    Altro
}
