using Helpdesk.Domain.Entities;

namespace Helpdesk.Application.Services;

public interface IUserService { 
    Task<BotUser> TryCreateUser(string username, CancellationToken ct);
}