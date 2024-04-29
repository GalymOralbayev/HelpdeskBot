using Helpdesk.Application.Services;
using Helpdesk.Domain.Entities;
using Helpdesk.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Helpdesk.Infrastructure.Impl.Services;

public class UserService : IUserService {
    private readonly ILogger<UserService> _logger;
    private readonly IBotUserRepository _botUserRepository;
    
    public UserService(ILogger<UserService> logger, IBotUserRepository botUserRepository) {
        _logger = logger;
        _botUserRepository = botUserRepository;
    }

    public async Task<BotUser> TryCreateUser(string userName, CancellationToken ct) {
        var existingUser = await _botUserRepository.GetByUserName(userName, ct);
        if (existingUser is not null) return existingUser;
        
        var user = await _botUserRepository.Insert(new BotUser(userName, null), ct);
        return user;
    }

}