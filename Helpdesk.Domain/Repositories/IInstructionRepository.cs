using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IInstructionRepository {
    Task<bool> Any(CancellationToken ct);
    Task Insert(Instruction instruction, CancellationToken ct);
    Task<Instruction> GetByName(string name, CancellationToken ct);
    Task<List<Instruction>> GetAll(CancellationToken ct);
}