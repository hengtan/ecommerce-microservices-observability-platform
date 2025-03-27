namespace Orders.Domain.Entities;

public class Address(string street, string city, string state, string country, string zipCode)
{
    public string Street { get; } = street;
    public string City { get; } = city;
    public string State { get; } = state;
    public string Country { get; } = country;
    public string ZipCode { get; } = zipCode;
}