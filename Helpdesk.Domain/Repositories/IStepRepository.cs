using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IStepRepository {
    Task<bool> Any(CancellationToken ct);
    Task Insert(Step step, CancellationToken ct);
    Task<Step> GetByNumber(int number, CancellationToken ct);
    Task<List<Step>> GetAll(CancellationToken ct);
}