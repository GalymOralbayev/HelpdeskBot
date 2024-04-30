using System;
using System.Collections.Generic;

namespace Helpdesk.App.Helpers.InitDtos;

public class InstructionInitDto {
    public List<InstructionInit> Instructions { get; set; }
    public List<StepInit> Steps { get; set; }
    public List<PhotoInit> Photos { get; set; }   
}

public class InstructionInit
{
    public Guid Id { get; set; }
    public string name { get; set; }
}

public class PhotoInit
{
    public Guid Id { get; set; }
    public string StepId { get; set; }
}

public class StepInit
{
    public Guid Id { get; set; }
    public string InstructionId { get; set; }
    public int StepNumber { get; set; }
    public string StepText { get; set; }
}

