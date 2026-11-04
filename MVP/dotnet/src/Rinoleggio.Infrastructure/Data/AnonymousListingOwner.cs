namespace Rinoleggio.Infrastructure.Data;

/// <summary>Fixed identity for contracts inserted without user login (MVP).</summary>
public static class AnonymousListingOwner
{
    public static readonly Guid UserId = Guid.Parse("a1a1a1a1-b2b2-c3c3-d4d4-e5e5e5e5e5e5");

    public const string Email = "anonymous@rinoleggio.local";
}
