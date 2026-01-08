namespace smart_parking_web.Models;
public class FavoriteViewModel
{
    public string ParkId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double OccupancyRatio { get; set; }
    public string CustomName { get; set; }
     public double CurrentOccupancyRatio { get; set; }
}
