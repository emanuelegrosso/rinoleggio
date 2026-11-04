using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rinoleggio.Infrastructure.Data;
using Rinoleggio.Infrastructure.Listings;

namespace Rinoleggio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRinoleggioInfrastructure(
        this IServiceCollection services,
        string sqliteConnectionString)
    {
        services.AddDbContext<RinoleggioDbContext>(options =>
            options.UseSqlite(sqliteConnectionString));

        services.AddScoped<ListingService>();

        return services;
    }
}
