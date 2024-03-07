using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;


namespace Defra.Trade.ReMoS.AssuranceService.API.Data.Persistence.Context;

[ExcludeFromCodeCoverage]
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }
}
