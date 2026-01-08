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
        
        _db = FirestoreDb.Create("project-adi");
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

    public IActionResult About()
    {
        return View();  
    }

    public async Task<IActionResult> Analysis(string? parkId)
{
    // 🔹 Otopark listesi
    CollectionReference parkingsRef = _db.Collection("otoparklar");
    QuerySnapshot parkingSnapshot = await parkingsRef.GetSnapshotAsync();

    var parkings = new List<RecommendedParking>();

    foreach (var doc in parkingSnapshot.Documents)
    {
        var data = doc.ToDictionary();
        parkings.Add(new RecommendedParking
        {
            ParkId = data["park_id"].ToString(),
            Latitude = Convert.ToDouble(data["latitude"]),
            Longitude = Convert.ToDouble(data["longitude"])
        });
    }

    // 🔹 Grafik verisi
    var graphData = new List<AnalysisPoint>();

    if (!string.IsNullOrEmpty(parkId))
    {
        CollectionReference analysisRef = _db
            .Collection("analysis")
            .Document(parkId)
            .Collection("hours");

        QuerySnapshot analysisSnap = await analysisRef.GetSnapshotAsync();

        foreach (var doc in analysisSnap.Documents)
        {
            var d = doc.ToDictionary();
            graphData.Add(new AnalysisPoint
            {
                Time = d["time"].ToString(),
                Ratio = Convert.ToDouble(d["ratio"])
            });
        }
    }

    var vm = new ParkingAnalysis
    {
        Parkings = parkings,
        SelectedParkId = parkId,
        GraphData = graphData
    };

    return View(vm);
}

}