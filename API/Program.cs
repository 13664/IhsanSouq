using API.Middleware;
using API.Services;
using Core.Entities;
using Core.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<IMulticardService, MulticardService>();

builder.Services.AddDbContext<CharityCaseContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
.AddRoles<IdentityRole>().
AddEntityFrameworkStores<CharityCaseContext>();
builder.Services.AddScoped<ICharityCaseRepository, CharityCaseRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddCors();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200","https://localhost:4200"));
//app.MapControllers();
app.UseRouting();
app.UseAuthorization(); // Make sure this is before UseEndpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapGroup("api").MapIdentityApi<AppUser>(); //api/login
try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CharityCaseContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await CharityCaseSeed.SeedAsync(context, userManager);
}
catch (System.Exception)
{

    throw;
}

app.Run();
