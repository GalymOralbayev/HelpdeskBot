using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.DataAccess.Configurations;

public class PhotoConfig : IEntityTypeConfiguration<Photo> {
    public void Configure(EntityTypeBuilder<Photo> builder) {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Step).WithMany(x => x.Photos).HasForeignKey(x => x.StepId);
        builder.Property(x => x.Content);
        builder.Property(x => x.ContentType);
    }
}