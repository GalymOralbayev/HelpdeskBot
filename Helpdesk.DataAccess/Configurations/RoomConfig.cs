using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.DataAccess.Configurations;

public class RoomConfig : IEntityTypeConfiguration<Room> {
    public void Configure(EntityTypeBuilder<Room> builder) {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RoomNumber);
        builder.Property(x => x.IpAddress);
        builder.Property(x => x.MacAddress);
        builder.HasOne(x => x.Article);
    }
}