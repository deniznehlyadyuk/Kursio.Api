using Kursio.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kursio.Api.Infrastructure;

public class KursioDbContext(DbContextOptions<KursioDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students { get; set; }
}