
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using System.Security.Claims;

[Authorize]
public class FavoritesController : Controller
{
    private readonly FirestoreDb _db;

    public FavoritesController()
    {
        _db = FirestoreDb.Create("");
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

        var list = snapshot.Documents.Select(d => new FavoriteViewModel
        {
            ParkId = d.GetValue<string>("park_id"),
            Latitude = d.GetValue<double>("latitude"),
            Longitude = d.GetValue<double>("longitude"),
            OccupancyRatio = d.GetValue<double>("occupancy_ratio"),
            CustomName = d.ContainsField("custom_name")
                ? d.GetValue<string>("custom_name")
                : ""
        }).ToList();

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
            custom_name = dto.CustomName ?? "",
            addedAt = Timestamp.GetCurrentTimestamp()
        });

        return Ok();
    }
}
