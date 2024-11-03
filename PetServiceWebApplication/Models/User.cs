using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models;

public class User
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }


    [JsonIgnore]
    public ICollection<Pet>? Pets { get; set; }
    [JsonIgnore]
    public ICollection<Booking>? Bookings { get; set; }
    [JsonIgnore]
    public ICollection<Review>? Reviews { get; set; }
}

