
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using System.Security.Claims;
namespace smart_parking_web.Models;

[Authorize]
public class FavoritesController : Controller
{
    private readonly FirestoreDb _db;
    private readonly PredictionService _prediction;

    public FavoritesController(PredictionService prediction)
    {
        _prediction = prediction;
        _db = FirestoreDb.Create("project-adi");
    }

    [Authorize]
public async Task<IActionResult> Index()
{
    var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

    var snapshot = await _db
        .Collection("users")
        .Document(uid)
        .Collection("favorites")
        .GetSnapshotAsync();

    var list = new List<FavoriteViewModel>();

    foreach (var doc in snapshot.Documents)
    {
        var parkId = doc.GetValue<string>("park_id");

        // 🔥 ŞU ANKİ TAHMİN
        var ratio = await _prediction.PredictAsync(parkId, DateTime.Now);

        list.Add(new FavoriteViewModel
        {
            ParkId = parkId,
            CustomName = doc.ContainsField("custom_name")
                ? doc.GetValue<string>("custom_name")
                : "",
            CurrentOccupancyRatio = ratio
        });
    }

    return View(list);
}

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] FavoriteAddDto dto)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var favRef = _db
            .Collection("users")
            .Document(uid)
            .Collection("favorites")
            .Document(dto.ParkId);

        await favRef.SetAsync(new
        {
            park_id = dto.ParkId,
            latitude = dto.Latitude,
            longitude = dto.Longitude,
            occupancy_ratio = dto.OccupancyRatio,
            custom_name = dto.ParkId ?? "",
            addedAt = Timestamp.GetCurrentTimestamp()
        });

        return Ok();
    }


    [HttpPost]
    public async Task<IActionResult> EditName([FromBody] FavoriteEdit edit)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var docRef = _db
            .Collection("users")
            .Document(uid)
            .Collection("favorites")
            .Document(edit.ParkId);

        await docRef.UpdateAsync("custom_name", edit.CustomName);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromBody] DeleteFavorite delete)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _db
            .Collection("users")
            .Document(uid)
            .Collection("favorites")
            .Document(delete.ParkId)
            .DeleteAsync();

        return Ok();
    }



}
