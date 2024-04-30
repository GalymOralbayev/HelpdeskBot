using Helpdesk.Domain.Abstraction;

namespace Helpdesk.Domain.Entities;

public class Step : IEntity {
    public Guid Id { get; set; }
    public Instruction Instruction { get; protected set; }
    public string StepText { get; protected set; }
    public int StepNumber { get; protected set; }
    public List<Photo> Photos { get; protected set; } = new();

    protected Step() {
    }
    
    public Step(string stepText, Instruction instruction, int stepNumber) {
        StepText = stepText;
        Instruction = instruction;
        StepNumber = stepNumber;
    }
}