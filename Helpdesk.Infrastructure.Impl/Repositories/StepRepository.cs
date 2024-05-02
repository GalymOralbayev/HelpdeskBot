using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SSO.DataAccess.Portgresql.Context;

namespace Helpdesk.Infrastructure.Impl.Repositories;

public class StepRepository : IStepRepository{
    private readonly ApplicationContext _context;
    private readonly IQueryable<Step?> _set;

    public StepRepository(ApplicationContext dbContext) {
        _context = dbContext;
        _set = dbContext.Set<Step>();
    }

    public async Task<bool> Any(CancellationToken ct) {
        return await _set.AnyAsync(ct);
    }

    public async Task InsertRange(List<Step> steps, CancellationToken ct) {
        await _context.AddRangeAsync(steps, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Step> GetByNumber(int number, CancellationToken ct) {
        return await _set.FirstOrDefaultAsync(x => x.StepNumber == number, ct);
    }

    public async Task<List<Step?>> GetAll(CancellationToken ct) {
        return await _set.ToListAsync(ct);
    }
}