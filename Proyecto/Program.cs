using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Proyecto.Data.Interfaces;
using Proyecto.Modules.Autenticacion;
using Proyecto.Modules.Autenticacion.Entities;
using Proyecto.Modules.Beneficiarios.Interfaces;
using Proyecto.Modules.Beneficiarios.Services;
using Proyecto.Modules.Duplicados.Interfaces;
using Proyecto.Modules.Duplicados.Services;
using Proyecto.Modules.Notificaciones.Interfaces;
using Proyecto.Modules.Notificaciones.Services;
using Proyecto.Modules.Revalidacion.Interfaces;
using Proyecto.Modules.Revalidacion.Services;
using Proyecto.Modules.Reportes.Interfaces;
using Proyecto.Modules.Reportes.Services;
using Proyecto.Modules.Staging.Interfaces;
using Proyecto.Modules.Staging.Services;
using Proyecto.Data;
using Proyecto.Shared.Auditoria;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthorization(options =>
    options.AddPolicy("ComiteAccess", policy =>
        policy.RequireAuthenticatedUser()
            .AddRequirements(new ComiteAccessRequirement())));
builder.Services.AddScoped<IAuthorizationHandler, ComiteAccessHandler>();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IBeneficiarioService, BeneficiarioService>();
builder.Services.AddScoped<IDuplicateDetectionService, DuplicateDetectionService>();
builder.Services.AddScoped<IRevalidacionService, RevalidacionService>();
builder.Services.AddScoped<IReportesService, ReportesService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IStagingService, StagingService>();

var app = builder.Build();

await RoleSeeder.EnsureRolesAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
