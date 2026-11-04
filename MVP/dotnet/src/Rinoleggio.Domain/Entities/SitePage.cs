namespace Rinoleggio.Domain.Entities;

public class SitePage
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string SlugPath { get; set; } = string.Empty;

    public string? BodyHtml { get; set; }

    public bool IsHomePage { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
}
