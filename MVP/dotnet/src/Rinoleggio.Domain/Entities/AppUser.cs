using Microsoft.AspNetCore.Identity;

namespace Rinoleggio.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public UserProfile? Profile { get; set; }

    public ICollection<Listing> OwnedListings { get; set; } = new List<Listing>();

    public ICollection<Lead> LeadsAsBuyer { get; set; } = new List<Lead>();

    public ICollection<Lead> LeadsAsSeller { get; set; } = new List<Lead>();
}
