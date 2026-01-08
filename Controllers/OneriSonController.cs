using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

public class OneriSonController : Controller
{
    private readonly HttpClient _http;
    private readonly FirestoreDb _db;

    // 🔥 TEK constructor
    public OneriSonController(IHttpClientFactory factory, FirestoreDb db)
    {
        _http = factory.CreateClient();
        _db = db;
    }

    public  IActionResult Index()
    {
        

        return View();
    }

}
