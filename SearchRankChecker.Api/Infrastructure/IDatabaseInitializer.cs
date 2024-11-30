using Microsoft.EntityFrameworkCore;
using SearchRankChecker.Api.Infrastructure.Persistence;

namespace SearchRankChecker.Api.Infrastructure;

public interface IDatabaseInitializer
{
    void Initialize();
}

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ApplicationDbContext _context;

    public DatabaseInitializer(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        _context.Database.Migrate();
    }
}