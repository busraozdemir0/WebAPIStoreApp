using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Presentation.ActionFilters;
using Repositories.EfCore;
using Services;
using Services.Contracts;
using WebAPIStoreApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(),"/nlog.config"));

// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;  // default olarak false gelen yap�d�r. ��erik pazarl��� i�in kullan�l�r. �rne�in csv olarak istek att���nda csv dosyas� olarak d�n�� yapmal�d�r
    config.ReturnHttpNotAcceptable = true;
    config.CacheProfiles.Add("5mins", new CacheProfile() { Duration=300}); // 5 dakikal�k/300 saniye cache profile olu�turuldu
})
     .AddXmlDataContractSerializerFormatters()  // Bu ifadeyle isteyene json format�nda dosyana isteyene de xml format�nda dosya g�nderebiliriz
 .AddCustomCsvFormatter() // CSV format�nda ��kt� verecep yap�y� bu metodda in�aa ettik
 .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly); // controller yap�s�n� Presentation katman�na ta��d���m�z i�in
 //.AddNewtonsoftJson(); // AddNewtonsoftJson => Patch istekleriyle �al��mak i�in


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;  // ModelState isvalid oldu�unda BadRequest d�nerek ge�ersiz k�laca��z/bast�raca��z
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager(); // Extensions'daki Metod tek parametreli dizi oldu�u i�in metoda parametre vermek zorunda de�iliz
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureActionFilters();  // Bu extension metodda validation ve loglama i�in attribute tan�mlar� yer al�yor
builder.Services.ConfigureCors();
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<IBookLinks, BookLinks>();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();

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

app.UseCors("CorsPolicy"); // cors yap�land�rmas� ile herhangi biri api'mize istek atabilir, herhangi bir headeri kullanabilir.
app.UseResponseCaching(); // �n bellekte tutmak i�in (Cors'tan sonra �a��r�lmal�)
app.UseHttpCacheHeaders();

app.UseAuthorization();

app.MapControllers();

app.Run();
