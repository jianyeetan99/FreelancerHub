using FreelancerHub.UI;
using FreelancerHub.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";
    });

builder.Services.AddAuthorization();

builder.Services.AddRazorPages(options => { options.Conventions.AuthorizeFolder("/Freelancers"); });
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<TokenStorage>();

builder.Services.AddHttpClient("Api", client => { client.BaseAddress = new Uri("http://localhost:5058"); });

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();