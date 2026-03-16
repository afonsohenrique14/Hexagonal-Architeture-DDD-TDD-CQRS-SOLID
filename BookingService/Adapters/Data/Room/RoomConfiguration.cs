
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities = Domain.Room.Entities;

namespace Data;

public class RoomConfiguration : IEntityTypeConfiguration<Entities.Room>
{
    public void Configure(EntityTypeBuilder<Entities.Room> builder)
    {
        builder.HasKey(x=> x.Id);
        builder.OwnsOne(x=> x.Price)
            .Property(x=> x.Value);
        builder.OwnsOne(x=> x.Price)
            .Property(x=> x.Currency);
    }
}
