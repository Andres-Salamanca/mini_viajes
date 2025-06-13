namespace TripServices.DTO;

public class CreateTripRequest
{
    public string Destination { get; set; }
    public DateTime DepartureDate { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}
