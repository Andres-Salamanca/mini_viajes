namespace TripServices.DTO;

public class TripResponse
{
    public int Id { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureDate { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public string CreatedBy { get; set; }
}
