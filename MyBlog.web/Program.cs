using Microsoft.EntityFrameworkCore;
using MyBlog.core.Services;
using MyBlog.infrastructure;
using MyBlog.infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("con")
    ));

builder.Services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
// one object for one requests of the same type
builder.Services.AddScoped<PostServices>();
// new object for each request
//builder.Services.AddTransient<PostServices>();
// one object for all requests
//builder.Services.AddSingleton<PostServices>();


var app = builder.Build();

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
