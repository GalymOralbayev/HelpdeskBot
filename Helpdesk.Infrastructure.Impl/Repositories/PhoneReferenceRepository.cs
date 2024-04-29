using System.Collections.ObjectModel;
using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SSO.DataAccess.Portgresql.Context;

namespace Helpdesk.Infrastructure.Impl.Repositories;

public class PhoneReferenceRepository : IPhoneReferenceRepository{
    private readonly ApplicationContext _context;
    private readonly IQueryable<PhoneReference?> _set;

    public PhoneReferenceRepository(ApplicationContext dbContext) {
        _context = dbContext;
        _set = dbContext.Set<PhoneReference>();
    }

    public async Task<bool> Any(CancellationToken ct) {
        return await _set.AnyAsync(ct);
    }

    public async Task InsertRange(IEnumerable<PhoneReference> phoneReference, CancellationToken ct) {
        await _context.AddRangeAsync(phoneReference, ct);
        await _context.SaveChangesAsync(ct);
    }
    
    public async Task<List<PhoneReference>> Search(string searchingText, CancellationToken ct) {
        var phoneReferences = await _set
            .Where(x => x!.SearchColumn!.Contains(searchingText))
            .OrderBy(x => x.Department)
            .ThenBy(x => x.Position)
            .ThenBy(x => x.FullName)
            .ToListAsync(ct);
        return phoneReferences;
    }
}