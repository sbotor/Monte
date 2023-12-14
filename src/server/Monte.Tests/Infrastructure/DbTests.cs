using Microsoft.EntityFrameworkCore;

namespace Monte.Tests.Infrastructure;

public abstract class DbTests : IDisposable
{
    private const string ConnectionStringTemplate = "Data Source=Monte-{0};Mode=Memory;Cache=Shared";
    
    private readonly Guid _databaseId = Guid.NewGuid();
    private bool _disposed;
    
    protected MonteDbContext Db { get; }

    protected DbTests()
    {
        Db = CreateDbContext();
    }

    protected MonteDbContext CreateDbContext(string? id = null)
    {
        if (string.IsNullOrEmpty(id))
        {
            id = _databaseId.ToString();
        }

        var conStr = string.Format(ConnectionStringTemplate, id);
        
        var options = new DbContextOptionsBuilder<MonteDbContext>()
            .UseSqlite(conStr)
            .Options;

        var dbContext = new MonteDbContext(options);
        dbContext.Database.OpenConnection();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        
        if (disposing)
        {
            Db.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
