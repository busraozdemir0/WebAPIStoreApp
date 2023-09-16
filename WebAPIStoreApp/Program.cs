using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Presentation.ActionFilters;
using Repositories.EfCore;
using Services.Contracts;
using WebAPIStoreApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(),"/nlog.config"));

// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader= true;  // default olarak false gelen yapýdýr. Ýçerik pazarlýðý için kullanýlýr. Örneðin csv olarak istek attýðýnda csv dosyasý olarak dönüþ yapmalýdýr
    config.ReturnHttpNotAcceptable = true;
})
 .AddCustomCsvFormatter() // CSV formatýnda çýktý verecep yapýyý bu metodda inþaa ettik
 .AddXmlDataContractSerializerFormatters()  // Bu ifadeyle isteyene json formatýnda dosyana isteyene de xml formatýnda dosya gönderebiliriz
 .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly) // controller yapýsýný Presentation katmanýna taþýdýðýmýz için
 .AddNewtonsoftJson(); // AddNewtonsoftJson => Patch istekleriyle çalýþmak için


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;  // ModelState isvalid olduðunda BadRequest dönerek geçersiz kýlacaðýz/bastýracaðýz
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager(); // Extensions'daki Metod tek parametreli dizi olduðu için metoda parametre vermek zorunda deðiliz
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureActionFilters();  // Bu extension metodda validation ve loglama için attribute tanýmlarý yer alýyor

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExcepitonHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())  // development modunda çalýþýyorsa
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(app.Environment.IsProduction()) // production modda çalýþýyorsa
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
