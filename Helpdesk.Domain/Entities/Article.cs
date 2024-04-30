using Helpdesk.Domain.Abstraction;

namespace Helpdesk.Domain.Entities;

public class Article : IEntity {
    public Guid Id { get; set; }
    public string Number { get; protected set; }
    public string Name { get; protected set; }

    protected Article() {
        
    }
    
    public Article(string number, string name) {
        Number = number;
        Name = name;
    }

    public override string ToString() {
        return $"Номер: {Number} \nНазвание: {Name}";
    }
}