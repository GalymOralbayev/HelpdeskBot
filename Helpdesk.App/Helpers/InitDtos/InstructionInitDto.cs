using System.Collections.Generic;

namespace Helpdesk.App.Helpers.InitDtos;

public class InstructionInitDto {
    public List<Instruction> Instructions { get; set; }
    public List<Step> Steps { get; set; }
    public List<Photo> Photos { get; set; }   
}

public class Instruction
{
    public string Id { get; set; }
    public string name { get; set; }
}

public class Photo
{
    public string Id { get; set; }
    public string StepId { get; set; }
}

public class Step
{
    public string Id { get; set; }
    public string InstructionId { get; set; }
    public string StepText { get; set; }
}

