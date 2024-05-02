using Helpdesk.Domain.Abstraction;

namespace Helpdesk.Domain.Entities;

public class Instruction : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; protected set; }
    public List<Step> Steps { get; protected set; } = new();
    
    protected Instruction() {
        
    }
    
    public Instruction(string name) {
        Name = name;
    }   
}