using System.Linq.Expressions;
using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IPhotoRepository {
    Task<bool> Any(CancellationToken ct);
    Task Insert(Photo step, CancellationToken ct);
    Task<Step> GetByFilter(Expression<Func<Step, bool>> filter, CancellationToken ct);
}