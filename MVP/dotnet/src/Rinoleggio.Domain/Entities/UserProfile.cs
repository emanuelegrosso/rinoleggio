using Rinoleggio.Domain.Enums;

namespace Rinoleggio.Domain.Entities;

public class UserProfile
{
    public Guid UserId { get; set; }

    public AppUser User { get; set; } = null!;

    public string FullName { get; set; } = string.Empty;

    /// <summary>Contact phone; never shown on public listings.</summary>
    public string? PhoneNumber { get; set; }

    public AccountType AccountType { get; set; } = AccountType.Private;

    public bool PrivacyConsent { get; set; }

    public DateTime? PrivacyConsentAt { get; set; }
}
