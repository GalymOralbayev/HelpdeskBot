using System.Linq.Expressions;
using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SSO.DataAccess.Portgresql.Context;

namespace Helpdesk.Infrastructure.Impl.Repositories;

public class PhotoRepository : IPhotoRepository{
    private readonly ApplicationContext _context;
    private readonly IQueryable<Photo?> _set;

    public PhotoRepository(ApplicationContext dbContext) {
        _context = dbContext;
        _set = dbContext.Set<Photo>();
    }

    public async Task<bool> Any(CancellationToken ct) {
        return await _set.AnyAsync(ct);
    }

    public async Task Insert(Photo step, CancellationToken ct) {
        await _context.AddAsync(step, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<Photo>> GetByFilter(Expression<Func<Photo, bool>> filter, CancellationToken ct) {
        var photos = await _set
            .Where(filter)
            .Include(x => x.Step)
            .ToListAsync(ct);
        return photos;
    }
}