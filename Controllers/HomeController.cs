using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly FirestoreDb _db;

    public HomeController()
    {
        // JSON anahtar dosyanızın yolunu buraya eklemeyi unutmayın
        string path = Path.Combine(Directory.GetCurrentDirectory(), "firebase_admin_key.json");
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        
        _db = FirestoreDb.Create("");
    }

    public async Task<IActionResult> Index()
    {
        CollectionReference parkingsRef = _db.Collection("otoparklar");
        QuerySnapshot snapshot = await parkingsRef.GetSnapshotAsync();

        var parkingList = new List<RecommendedParking>();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            if (document.Exists)
            {
                var data = document.ToDictionary();
                parkingList.Add(new RecommendedParking
                {
                    ParkId = data.ContainsKey("park_id") ? data["park_id"].ToString() : "Bilinmiyor",
                    Latitude = data.ContainsKey("latitude") ? Convert.ToDouble(data["latitude"]) : 0,
                    Longitude = data.ContainsKey("longitude") ? Convert.ToDouble(data["longitude"]) : 0,
                    // Diğer alanları boş bırakıyoruz çünkü sadece listeleme yapıyoruz
                });
            }
        }

        // Sadece listeyi gönderiyoruz
        return View(parkingList);
    }
}