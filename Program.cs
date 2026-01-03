// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddHttpClient<FirebaseAuthService>();

// // Add services to the container.
// builder.Services.AddControllersWithViews();
// builder.Services.AddHttpClient();
// // Program.cs içine ekleyin
// string path = Path.Combine(Directory.GetCurrentDirectory(), "firebase_admin_key.json");
// Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseRouting();

// app.UseAuthorization();

// app.MapStaticAssets();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}")
//     .WithStaticAssets();


// app.Run();

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Google.Cloud.Firestore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();



builder.Services.AddHttpClient();

builder.Services.AddSingleton(provider =>
{
    string path = Path.Combine(Directory.GetCurrentDirectory(), "firebase_admin_key.json");
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

    return FirestoreDb.Create("");
});

// 🔥 Firebase Admin INIT
var firebasePath = Path.Combine(
    builder.Environment.ContentRootPath,
    "firebase_admin_key.json"
);

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebasePath)
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();



app.UseAuthentication(); // 👈 ÖNCE
app.UseAuthorization();  // 👈 SONRA

// app.UseMiddleware<FirebaseAuthMiddleware>();
// app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


