using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SSO.DataAccess.Portgresql.Context;

namespace Helpdesk.Infrastructure.Impl.Repositories;

public class ArticleRepository : IArticleRepository{
    private readonly ApplicationContext _context;
    private readonly IQueryable<Article?> _set;

    public ArticleRepository(ApplicationContext dbContext) {
        _context = dbContext;
        _set = dbContext.Set<Article>();
    }

    public async Task<bool> Any(CancellationToken ct) {
        return await _set.AnyAsync(ct);
    }

    public async Task InsertRange(IEnumerable<Article> rooms, CancellationToken ct) {
        await _context.AddRangeAsync(rooms, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Article> GetByName(string articleName, CancellationToken ct) {
        return await _set.FirstOrDefaultAsync(x => x.Name == articleName, cancellationToken: ct);
    }

    public async Task<List<Article>> SearchByArticleNumber(string articleNumber, CancellationToken ct) {
        var rooms = await _set
            .Where(x => x.Number == articleNumber)
            .ToListAsync(ct);
        return rooms;
    }

    public async Task<List<Article>> GetAll(CancellationToken ct) {
        var rooms = await _set
            .ToListAsync(ct);
        return rooms;
    }
}