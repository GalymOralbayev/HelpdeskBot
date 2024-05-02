using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IPhoneReferenceRepository {
    Task<bool> Any(CancellationToken ct);
    Task InsertRange(IEnumerable<PhoneReference> phoneReference, CancellationToken ct);
    Task<List<PhoneReference>> Search(string searchingText, CancellationToken ct);
}