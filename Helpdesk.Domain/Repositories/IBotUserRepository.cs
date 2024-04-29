using Helpdesk.Domain.Entities;

namespace Helpdesk.Domain.Repositories;

public interface IBotUserRepository {
    Task<BotUser?> GetByUserName(string userName, CancellationToken cancellationToken);
    Task<BotUser> Insert(BotUser user, CancellationToken cancellationToken);
    Task<BotUser> Update(BotUser user, CancellationToken cancellationToken);
}