using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.DataAccess.Configurations;

public class PhoneReferenceConfig : IEntityTypeConfiguration<PhoneReference> {
    public void Configure(EntityTypeBuilder<PhoneReference> builder) {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Position);
        builder.Property(x => x.Department);
        builder.Property(x => x.FullName);
        builder.Property(x => x.InternalNumber);
        builder.Property(x => x.CityNumber);
        builder.Property(x => x.Email);
        builder.Property(x => x.SearchColumn);
        builder.HasIndex(x => x.SearchColumn).IsUnique();
    }
}