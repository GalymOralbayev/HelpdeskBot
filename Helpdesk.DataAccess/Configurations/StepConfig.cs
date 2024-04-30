using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.DataAccess.Configurations;

public class StepConfig : IEntityTypeConfiguration<Step> {
    public void Configure(EntityTypeBuilder<Step> builder) {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Instruction);
        builder.Property(x => x.StepText);
        builder.HasMany(x => x.Photos);
    }
}