using System.Linq.Expressions;
using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SSO.DataAccess.Portgresql.Context;

namespace Helpdesk.Infrastructure.Impl.Repositories;

public class RoomRepository : IRoomRepository{
    private readonly ApplicationContext _context;
    private readonly IQueryable<Room?> _set;

    public RoomRepository(ApplicationContext dbContext) {
        _context = dbContext;
        _set = dbContext.Set<Room>();
    }

    public async Task<bool> Any(CancellationToken ct) {
        return await _set.AnyAsync(ct);
    }

    public async Task Insert(Room room, CancellationToken ct) {
        await _context.AddAsync(room, ct);
        await _context.SaveChangesAsync(ct);
    }
    
    public async Task<List<Room>> Search(Expression<Func<Room, bool>> filter, CancellationToken ct) {
        var rooms = await _set
            .Where(filter)
            .Include(x => x.Article)
            .ToListAsync(ct);
        return rooms;
    }
}