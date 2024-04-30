using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IInstructionRepository {
    Task<bool> Any(CancellationToken ct);
    Task InsertRange(List<Instruction> instructions, CancellationToken ct);
    Task<Instruction?> GetByName(string name, CancellationToken ct);
    Task<List<Instruction?>> GetAll(CancellationToken ct);
}