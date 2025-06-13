namespace TripServices.Model;

public class Trip {
    public Guid Id { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureDate { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}
