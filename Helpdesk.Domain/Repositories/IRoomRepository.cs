using System.Linq.Expressions;
using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IRoomRepository {
    Task<bool> Any(CancellationToken ct);
    Task Insert(Room room, CancellationToken ct);
    Task<List<Room>> Search(Expression<Func<Room, bool>> filter, CancellationToken ct);
}