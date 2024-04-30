using Helpdesk.Domain.Abstraction;

namespace Helpdesk.Domain.Entities;

public class Photo : IEntity
{
    public Guid Id { get; set; }
    public Step Step { get; protected set; }
    public string PhotoBytes { get; protected set; }

    protected Photo() {
    }
    
    public Photo(Step step, string photoBytes) {
        this.Step = step;
        PhotoBytes = photoBytes;
    }
    
}