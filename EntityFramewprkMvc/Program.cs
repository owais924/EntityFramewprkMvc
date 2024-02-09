
using Microsoft.EntityFrameworkCore;
using MyApp.DAL;
using MyApp.DAL.Infrastructure.IRepository;
using MyApp.DAL.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MyApp.CommonHelper;
using Stripe;
using MyApp.DAL.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "786840119584338";
    options.AppSecret = "fe0de105e42cb26ebb4fc9fb8c65c2eb";
});
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "604115909657-ea8f12he9fl2nk3uum9r09b2b3oahpfs.apps.googleusercontent.com";
    options.ClientSecret = "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";
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("PaymentSettings"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>//cookie are added
{
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
});
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("PaymentSettings:SecretKey").Get<string>();
dataSeeding();



app.UseAuthentication();;

app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
void dataSeeding()
{
   using(var scope = app.Services.CreateScope())
    {
        var DbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        DbInitializer.Initialize();
    }
}
