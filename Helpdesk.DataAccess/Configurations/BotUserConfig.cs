using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.DataAccess.Configurations;

public class BotUserConfig : IEntityTypeConfiguration<BotUser> {
    public void Configure(EntityTypeBuilder<BotUser> builder) {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email);
        builder.Property(x => x.TgUserName);
    }
}