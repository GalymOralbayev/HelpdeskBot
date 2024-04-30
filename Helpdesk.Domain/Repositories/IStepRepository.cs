using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IStepRepository {
    Task<bool> Any(CancellationToken ct);
    Task InsertRange(List<Step> steps, CancellationToken ct);
    Task<Step> GetByNumber(int number, CancellationToken ct);
    Task<List<Step?>> GetAll(CancellationToken ct);
}