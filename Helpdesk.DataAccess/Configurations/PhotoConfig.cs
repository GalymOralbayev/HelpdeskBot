using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.DataAccess.Configurations;

public class PhotoConfig : IEntityTypeConfiguration<Photo> {
    public void Configure(EntityTypeBuilder<Photo> builder) {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Step);
        builder.Property(x => x.PhotoBytes);
    }
}