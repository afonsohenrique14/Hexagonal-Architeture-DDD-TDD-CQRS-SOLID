using System;
using Domain.Enums;

namespace Domain.ValueObjects;

public class Price
{
    public decimal Value { get; private set; }
    public AcceptedCurrencies Currency { get; private set; }
}
