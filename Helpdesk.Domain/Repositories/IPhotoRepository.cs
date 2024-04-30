using System.Linq.Expressions;
using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IPhotoRepository {
    Task<bool> Any(CancellationToken ct);
    Task Insert(Photo step, CancellationToken ct);
    Task<List<Photo>> GetByFilter(Expression<Func<Photo, bool>> filter, CancellationToken ct);
}