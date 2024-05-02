using Helpdesk.Domain.Abstraction;

namespace Helpdesk.Domain.Entities;

public class BotUser : IEntity {
    public Guid Id { get; set; }
    public virtual string TgUserName { get; protected set; }
    public virtual string? Email { get; protected set; }

    protected BotUser() {
    }
    
    public BotUser(string tgUserName, string? email) {
        TgUserName = tgUserName;
        Email = email;
    }

    public virtual void SetEmail(string email) {
        Email = email;
    }
}