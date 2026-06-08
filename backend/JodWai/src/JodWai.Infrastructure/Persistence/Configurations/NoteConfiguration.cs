using JodWai.Domain.Entities;
using JodWai.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JodWai.Infrastructure.Persistence.Configurations;

internal class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("Notes");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasConversion(
                id => id.Value,
                value => NoteId.From(value))
            .ValueGeneratedNever();

        builder.OwnsOne(n => n.Title, title =>
        {
            title.Property(t => t.Value)
                .HasColumnName("Title")
                .HasMaxLength(200)
                .IsRequired();

            title.HasIndex(t => t.Value);
        });

        builder.OwnsOne(n => n.Content, content =>
        {
            content.Property(c => c.Value)
                .HasColumnName("Content")
                .HasMaxLength(NoteContent.MaxLength)
                .IsRequired();
        });

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.UpdatedAt)
            .IsRequired();

        builder.OwnsMany(n => n.Tags, tag =>
        {
            tag.WithOwner()
                .HasForeignKey("NoteId");

            tag.Property<Guid>("Id");
            tag.HasKey("Id");

            tag.Property(t => t.Value)
                .HasColumnName("Tag")
                .HasMaxLength(Tag.MaxLength)
                .IsRequired();

            // Prevent duplicate tags per note at database level
            tag.HasIndex("NoteId", nameof(Tag.Value))
                .IsUnique();
        });

        builder.OwnsMany(n => n.Links, link =>
        {
            link.WithOwner()
                .HasForeignKey("NoteId");

            link.Property<Guid>("Id");
            link.HasKey("Id");

            link.Property(l => l.TargetId)
                .HasConversion(
                    id => id.Value,
                    value => NoteId.From(value))
                .IsRequired();

            link.UsePropertyAccessMode(PropertyAccessMode.Field);

            // Prevent duplicate links per note at database level
            link.HasIndex("NoteId", nameof(NoteLink.TargetId))
                .IsUnique();
        });

        builder.Navigation(n => n.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(n => n.Links)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
