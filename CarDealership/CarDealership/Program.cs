using CarDealership.Data;
using CarDealership.Models;
using CarDealership.Services.Brand;
using CarDealership.Services.Car;
using CarDealership.Services.Color;
using CarDealership.Services.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI().AddDefaultTokenProviders();

builder.Services.AddScoped<ICarService,CarService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IModelService, ModelService>();
builder.Services.AddScoped<IColorService, ColorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
else
{
	app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "getmodels",
        pattern: "Car/GetModels/{brandId}",
        defaults: new { controller = "Car", action = "GetModels" });

    endpoints.MapControllerRoute(
      name: "Search",
      pattern: "Car/Search",
      defaults: new { controller = "Car", action = "Search" }
  );
    endpoints.MapControllerRoute(
        name: "Details",
        pattern: "Car/Details/{id}",
        defaults: new { controller = "Car", action = "Details" });

    endpoints.MapControllerRoute(
    name: "DeletePhoto",
    pattern: "Admin/Photo/Delete/{photoId}",
    defaults: new { controller = "Photo", action = "Delete" }
);

    endpoints.MapControllerRoute(
        name: "AreaRoute",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "Default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapRazorPages();

//Seed Roles
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);

    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

    var dbSeeder = new DbSeeder();

    await dbSeeder.SeedBrands(dbContext);
    await dbSeeder.SeedModels(dbContext);
    await dbSeeder.SeedCarColors(dbContext);
    await dbSeeder.SeedCars(dbContext);
    await dbSeeder.SeedPhotos(dbContext);
}

app.Run();
