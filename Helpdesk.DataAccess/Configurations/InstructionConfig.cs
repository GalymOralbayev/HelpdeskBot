using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.DataAccess.Configurations;

public class InstructionConfig : IEntityTypeConfiguration<Instruction> {
    public void Configure(EntityTypeBuilder<Instruction> builder) {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name);
        builder.HasMany(x => x.Steps);
    }
}