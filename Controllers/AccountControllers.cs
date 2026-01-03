using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Google.Cloud.Firestore;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> FirebaseLogin([FromBody] TokenDto dto)
    {
        var decoded = await FirebaseAuth.DefaultInstance
            .VerifyIdTokenAsync(dto.Token);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, decoded.Uid),
            new Claim(ClaimTypes.Email, decoded.Claims.TryGetValue("email", out var email) ? email.ToString() : ""),
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return Ok();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProfile()
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

        FirestoreDb db = FirestoreDb.Create("");
       

        await db.Collection("users").Document(uid).SetAsync(new
        {
            email = User.FindFirstValue(ClaimTypes.Email),
            role = "user",
            createdAt = Timestamp.GetCurrentTimestamp()
        });

        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login");
    }


}
