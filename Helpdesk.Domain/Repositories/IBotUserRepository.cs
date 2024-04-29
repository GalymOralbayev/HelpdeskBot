using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IBotUserRepository {
    Task<IList<BotUser?>> Get(int limit, CancellationToken cancellationToken);
    Task<BotUser?> GetByUserName(string userName, CancellationToken cancellationToken);
    Task<BotUser> Insert(BotUser busMessage, CancellationToken cancellationToken);
    Task<Guid> Delete(BotUser busMessage, CancellationToken cancellationToken);
}