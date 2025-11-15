using Alba;
using JasperFx.CommandLine;
using Kursio.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kursio.Tests;

public class Fixture : IAsyncLifetime
{
    public IAlbaHost AlbaHost = null!;

    async Task IAsyncLifetime.InitializeAsync()
    {
        JasperFxEnvironment.AutoStartHost = true;
        
        var configValues = new Dictionary<string, string?>()
        {
            { "ConnectionStrings:postgres", $"Server=127.0.0.1;Port=5432;Database=kursio-{Guid.NewGuid()};User Id=postgres;Password=1q2w3E*;" }
        };
        
        AlbaHost = await Alba.AlbaHost.For<Program>(builder  => {}, ConfigurationOverride.Create(configValues));

        using var scope = AlbaHost.Services.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<KursioDbContext>();
        
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = AlbaHost.Services.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<KursioDbContext>();
        
        await dbContext.Database.EnsureDeletedAsync();
        
        await AlbaHost.DisposeAsync();
    }
}