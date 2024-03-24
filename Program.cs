using WebEnterprise.Data;
using WebEnterprise.Initializer;
using WebEnterprise.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//toaster alert message
builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = true,
    PositionClass = ToastPositions.TopRight,
    PreventDuplicates = true,
    CloseButton = true
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddOptions(); 
var mailsettings = builder.Configuration.GetSection("MailSettings"); 
builder.Services.Configure<MailSettings> (mailsettings);
builder.Services.AddTransient<ISendMailService, SendMailService>();



builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thòi gian cookie hiệu lực
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// app.Use(async (context, next) =>
// {
//     if (!context.User.Identity.IsAuthenticated)
//     {
//         context.Response.Redirect("/Identity/Account/Login");
//         return;
//     }
//
//     await next();
// });

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize();
}


app.MapControllerRoute(
    name: "authenticated",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Authenticated" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{area=UnAuthenticated}/{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
