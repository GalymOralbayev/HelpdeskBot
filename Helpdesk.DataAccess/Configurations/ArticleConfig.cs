using Helpdesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helpdesk.DataAccess.Configurations;

public class ArticleConfig : IEntityTypeConfiguration<Article> {
    public void Configure(EntityTypeBuilder<Article> builder) {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number);
        builder.Property(x => x.Name);
    }
}