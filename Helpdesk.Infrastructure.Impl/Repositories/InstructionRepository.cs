using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SSO.DataAccess.Portgresql.Context;

namespace Helpdesk.Infrastructure.Impl.Repositories;

public class InstructionRepository : IInstructionRepository{
    private readonly ApplicationContext _context;
    private readonly IQueryable<Instruction?> _set;

    public InstructionRepository(ApplicationContext dbContext) {
        _context = dbContext;
        _set = dbContext.Set<Instruction>();
    }

    public async Task<bool> Any(CancellationToken ct) {
        return await _set.AnyAsync(ct);
    }

    public async Task InsertRange(List<Instruction> instructions, CancellationToken ct) {
        await _context.AddRangeAsync(instructions, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Instruction?> GetByName(string name, CancellationToken ct) {
        return await _set
            .Include(x => x.Steps)
            .ThenInclude(x => x.Photos)
            .FirstOrDefaultAsync(x => x.Name == name, ct);
    }

    public async Task<List<Instruction?>> GetAll(CancellationToken ct) {
        return await _set.ToListAsync(ct);
    }
}