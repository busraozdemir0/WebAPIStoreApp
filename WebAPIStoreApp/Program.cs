using Microsoft.EntityFrameworkCore;
using NLog;
using Repositories.EfCore;
using Services.Contracts;
using WebAPIStoreApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(),"/nlog.config"));

// Add services to the container.

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly) // controller yap�s�n� Presentation katman�na ta��d���m�z i�in
    .AddNewtonsoftJson(); // AddNewtonsoftJson => Patch istekleriyle �al��mak i�in

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager(); // Extensions'daki Metod tek parametreli dizi oldu�u i�in metoda parametre vermek zorunda de�iliz
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExcepitonHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())  // development modunda �al���yorsa
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(app.Environment.IsProduction()) // production modda �al���yorsa
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
