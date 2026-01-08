using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Google.Cloud.Firestore;


public class AnalysisController : Controller
{
    private readonly FirebaseService _firebase;
    private readonly PredictionService _prediction;

    public AnalysisController(FirebaseService firebase, PredictionService prediction)
    {
        _firebase = firebase;
        _prediction = prediction;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Parkings = await _firebase.GetAllParkingsAsync();
        return View();
    }

    [HttpGet]
public async Task<IActionResult> Get24HourPrediction(string parkId)
{
    try
    {
        if (string.IsNullOrEmpty(parkId))
            return BadRequest(new { error = "parkId boş geldi" });

        var now = DateTime.Now;
        var list = new List<object>();

        for (int i = 0; i < 24; i++)
        {
            var time = now.AddHours(i);
            var ratio = await _prediction.PredictAsync(parkId, time);

            list.Add(new
            {
                time = time.ToString("HH:mm"),
                ratio = Math.Round(ratio * 100, 1)
            });
        }

        return Json(list);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            error = ex.Message,
            stack = ex.StackTrace
        });
    }
}


}
