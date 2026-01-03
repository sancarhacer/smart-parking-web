using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

public class OneriController : Controller
{
    private readonly HttpClient _http;
    private readonly FirestoreDb _db;

    // 🔥 TEK constructor
    public OneriController(IHttpClientFactory factory, FirestoreDb db)
    {
        _http = factory.CreateClient();
        _db = db;
    }

    // public async Task<IActionResult> Index()
    // {
    //     CollectionReference parkingsRef = _db.Collection("otoparklar");
    //     QuerySnapshot snapshot = await parkingsRef.GetSnapshotAsync();

    //     var parkingList = new List<RecommendedParking>();

    //     foreach (DocumentSnapshot document in snapshot.Documents)
    //     {
    //         if (document.Exists)
    //         {
    //             var data = document.ToDictionary();
    //             parkingList.Add(new RecommendedParking
    //             {
    //                 ParkId = data.ContainsKey("park_id") ? data["park_id"].ToString() : "Bilinmiyor",
    //                 Latitude = data.ContainsKey("latitude") ? Convert.ToDouble(data["latitude"]) : 0,
    //                 Longitude = data.ContainsKey("longitude") ? Convert.ToDouble(data["longitude"]) : 0
    //             });
    //         }
    //     }

    //     return View(parkingList);
    // }

    public async Task<IActionResult> Index()
    {
        var snapshot = await _db.Collection("otoparklar").GetSnapshotAsync();

        var allParkings = snapshot.Documents
            .Where(d => d.Exists)
            .Select(d =>
            {
                var data = d.ToDictionary();
                return new RecommendedParking
                {
                    ParkId = data["park_id"].ToString(),
                    Latitude = Convert.ToDouble(data["latitude"]),
                    Longitude = Convert.ToDouble(data["longitude"])
                };
            }).ToList();

        return View(allParkings); // ⚠️ sadece harita için
    }

    [HttpPost]
    public async Task<IActionResult> GetRecommendation([FromBody] RecommendDto dto)
    {
        var response = await _http.PostAsJsonAsync(
            "http://127.0.0.1:8000/recommend",
            new
            {
                target_lat = dto.TargetLat,
                target_lon = dto.TargetLon,
                user_lat = dto.UserLat,
                user_lon = dto.UserLon,
                max_walk_time = 15
            });

        var result = await response.Content.ReadFromJsonAsync<ParkingRecommendationResponse>();
        return Ok(result);
    }
}
