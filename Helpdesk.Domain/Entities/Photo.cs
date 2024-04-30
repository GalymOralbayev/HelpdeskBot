using Helpdesk.Domain.Abstraction;

namespace Helpdesk.Domain.Entities;

public class Photo : IEntity
{
    public Guid Id { get; set; }
    public Step Step { get; protected set; }
    public Guid StepId { get; protected set; }
    public byte[] Content { get; set; }
    public string ContentType { get; set; }

    protected Photo() {
    }
    
    public Photo(Guid stepId, byte[] content, string contentType) {
        StepId = stepId;
        Content = content;
        ContentType = contentType;
    }
    
}