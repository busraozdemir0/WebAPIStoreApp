using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Presentation.ActionFilters;
using Repositories.EfCore;
using Services;
using Services.Contracts;
using WebAPIStoreApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;  // default olarak false gelen yapýdýr. Ýçerik pazarlýðý için kullanýlýr. Örneðin csv olarak istek attýðýnda csv dosyasý olarak dönüþ yapmalýdýr
    config.ReturnHttpNotAcceptable = true;
    config.CacheProfiles.Add("5mins", new CacheProfile() { Duration = 300 }); // 5 dakikalýk/300 saniye cache profile oluþturuldu
})
     .AddXmlDataContractSerializerFormatters()  // Bu ifadeyle isteyene json formatýnda dosyana isteyene de xml formatýnda dosya gönderebiliriz
 .AddCustomCsvFormatter() // CSV formatýnda çýktý verecep yapýyý bu metodda inþaa ettik
 .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly) // controller yapýsýný Presentation katmanýna taþýdýðýmýz için
 .AddNewtonsoftJson(opt =>
 opt.SerializerSettings.ReferenceLoopHandling =
 Newtonsoft.Json.ReferenceLoopHandling.Ignore
 ); // AddNewtonsoftJson => Patch istekleriyle çalýþmak için


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;  // ModelState isvalid olduðunda BadRequest dönerek geçersiz kýlacaðýz/bastýracaðýz
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager(); // Extensions'daki Metod tek parametreli dizi olduðu için metoda parametre vermek zorunda deðiliz
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureActionFilters();  // Bu extension metodda validation ve loglama için attribute tanýmlarý yer alýyor
builder.Services.ConfigureCors();
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<IBookLinks, BookLinks>();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.AddMemoryCache();  // hýz sýnýrlama için
builder.Services.ConfigureRateLimitingOptions(); // hýz sýnýrlama için
builder.Services.AddHttpContextAccessor(); // hýz sýnýrlama için

builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration); // JWT configurasyonu


var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExcepitonHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())  // development modunda çalýþýyorsa
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {  // version1 ve version2'yi ayýrýyoruz swagger için
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "BTK Akademi v1");
        s.SwaggerEndpoint("/swagger/v2/swagger.json", "BTK Akademi v2");
    });
}

if (app.Environment.IsProduction()) // production modda çalýþýyorsa
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting(); // hýz sýnýrlama için(örn. dakikada 3 istek gibi)
app.UseCors("CorsPolicy"); // cors yapýlandýrmasý ile herhangi biri api'mize istek atabilir, herhangi bir headeri kullanabilir.
app.UseResponseCaching(); // ön bellekte tutmak için (Cors'tan sonra çaðýrýlmalý)
app.UseHttpCacheHeaders();

app.UseAuthentication();  // önce kullanýcý adý ve þifre ile doðrulama iþlemi gerçekleþtirilecek
app.UseAuthorization(); // ardýndan yetkilendirme iþlemi gerçekleþtirilecek

app.MapControllers();

app.Run();
