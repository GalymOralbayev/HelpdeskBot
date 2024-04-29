using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SSO.DataAccess.Portgresql.Context;

namespace Helpdesk.Infrastructure.Impl.Repositories;


public class BotUserRepository : IBotUserRepository{
    private readonly ApplicationContext _context;
    private readonly IQueryable<BotUser?> _set;

    public BotUserRepository(ApplicationContext dbContext) {
        _context = dbContext;
        _set = dbContext.Set<BotUser>();
    }
        
    public async Task<IList<BotUser?>> Get(int limit, CancellationToken ct) {
        return await _set
            .Take(limit)
            .ToListAsync(ct);
    }
        
    public async Task<BotUser?> GetByUserName(string userName, CancellationToken ct) {
        return await _set.SingleOrDefaultAsync(x => x != null && x.TgUserName == userName, ct);
    }
    
    public async Task<BotUser> Insert(BotUser botUser, CancellationToken ct) {
        await _context.AddAsync(botUser, ct);
        await _context.SaveChangesAsync(ct);
        return botUser;
    }
    public async Task<BotUser> Update(BotUser botUser, CancellationToken ct) {
        _context.Update(botUser);
        await _context.SaveChangesAsync(ct);
        return botUser;
    }

    public async Task<Guid> Delete(BotUser botUser, CancellationToken ct) {
        _context.Remove(botUser);
        await _context.SaveChangesAsync(ct);
        return botUser.Id;
    }
}