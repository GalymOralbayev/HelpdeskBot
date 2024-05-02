using Helpdesk.Domain.Entities;

namespace Helpdesk.Application.Services;

public interface IUserService { 
    Task<BotUser?> TryGetOrInsertUser(string? username, CancellationToken ct);
    Task<BotUser?> UpdateUserEmail(string userName, string? emailAddress, CancellationToken ct);
}