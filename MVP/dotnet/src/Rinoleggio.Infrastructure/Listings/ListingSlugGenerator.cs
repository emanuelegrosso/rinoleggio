using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Rinoleggio.Infrastructure.Listings;

public static partial class ListingSlugGenerator
{
    public static string BuildBaseSlug(string title, string brandName, string model)
    {
        var raw = $"{title}-{brandName}-{model}";
        var normalized = raw.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        var slug = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        slug = NonAlphanumeric().Replace(slug, "-");
        slug = MultiDash().Replace(slug, "-").Trim('-');
        return string.IsNullOrEmpty(slug) ? "annuncio" : slug;
    }

    public static async Task<string> EnsureUniqueSlugAsync(
        Func<string, Task<bool>> slugExists,
        string baseSlug,
        Guid? excludeListingId = null)
    {
        var candidate = $"annunci/{baseSlug}";
        if (!await slugExists(candidate))
        {
            return candidate;
        }

        for (var i = 2; i < 1000; i++)
        {
            candidate = $"annunci/{baseSlug}-{i}";
            if (!await slugExists(candidate))
            {
                return candidate;
            }
        }

        return $"annunci/{baseSlug}-{Guid.NewGuid():N}"[..200];
    }

    [GeneratedRegex(@"[^a-z0-9\-]+")]
    private static partial Regex NonAlphanumeric();

    [GeneratedRegex(@"-{2,}")]
    private static partial Regex MultiDash();
}
