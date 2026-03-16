using Entities = Domain.Booking.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Booking.Enums;

namespace Data.Booking;

public class BookingConfigurantion : IEntityTypeConfiguration<Entities.Booking>
{
    public void Configure(EntityTypeBuilder<Entities.Booking> builder)
    {
        builder.HasKey(x=> x.Id);
        builder
            .Property<Status>("Status")
            .HasConversion<string>()
            .IsRequired();
    }
}
