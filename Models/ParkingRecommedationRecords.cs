using System.Text.Json.Serialization;
public class ParkingRecommendationResponse
{
    [JsonPropertyName("recommended_parking")]
    public RecommendedParking Recommended_Parking { get; set; }

    [JsonPropertyName("all_parkings")]
    public List<RecommendedParking> All_Parkings { get; set; }
}

public class RecommendedParking
{

    
    [JsonPropertyName("park_id")]
    public string? ParkId { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("occupancy_ratio")]
    public double OccupancyRatio { get; set; }

    [JsonPropertyName("duration_minutes")]
    public double DurationMinutes { get; set; }

    [JsonPropertyName("duration_text")]
    public string DurationText { get; set; }

    [JsonPropertyName("walk_minutes")]
    public double WalkMinutes { get; set; }

    [JsonPropertyName("walk_text")]
    public string? WalkText { get; set; }

    [JsonPropertyName("distance_km")]
    public double DistanceKm { get; set; }

    [JsonPropertyName("score")]
    public double Score { get; set; }
}




