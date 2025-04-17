using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBlog.core.Services;
using MyBlog.domain.Models;
using MyBlog.infrastructure;
using MyBlog.infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("con")
    ));

// add identity 
builder.Services.AddIdentity<User, IdentityRole>(
    options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";
        options.Password.RequiredLength = 3;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 0;
        options.Lockout.AllowedForNewUsers = false;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
        options.Lockout.MaxFailedAccessAttempts = 3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//builder.Services.AddDefaultIdentity<User>(options =>
//{

//}).AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();


builder.Services.AddScoped(typeof(IUnitOfWork),typeof(UintOfWork));  // core 
builder.Services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
builder.Services.AddScoped(typeof(IPostRepo),typeof(PostRepo));
// one object for one requests of the same type
builder.Services.AddScoped<PostServices>();
builder.Services.AddScoped<CategoryServices>();
// new object for each request
//builder.Services.AddTransient<PostServices>();
// one object for all requests
//builder.Services.AddSingleton<PostServices>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DbIntializer.SeedAdminAsync(userManager,roleManager);
}





    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization(); // hint  => identity

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // url 
// domain.com/Post/ Posts

app.Run();
