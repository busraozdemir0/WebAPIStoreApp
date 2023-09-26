using AspNetCoreRateLimit;
using Entities.DataTransferObjects;
using Entities.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentation.ActionFilters;
using Presentation.Controllers;
using Repositories.Contracts;
using Repositories.EfCore;
using Services;
using Services.Contracts;
using System.Text;

namespace WebAPIStoreApp.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>    // IoC'ye DbContext tanımını yapmış olduk (DI çerçevesi için)
                   options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
             services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();


        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerService, LoggerManager>();    // AddSingleton => logger bir kere üretilecek herkes aynı logger'ı kullanacak

        public static void ConfigureActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>(); // IoC
            services.AddSingleton<LogFilterAttribute>(); // loglama işlemi için bir tane nesnenin oluşması yeterli
            services.AddScoped<ValidateMediaTypeAttribute>(); 
        }

        public static void ConfigureCors(this IServiceCollection services) // cors yapılandırması ile herhangi biri api'mize istek atabilir, herhangi bir headeri kullanabilir.
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => 
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination")
                );
            });
        }

        public static void ConfigureDataShaper(this IServiceCollection services)
        {
            services.AddScoped<IDataShaper<BookDto>, DataShaper<BookDto>>();
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config
                .OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();  // OfType kullanma amacımız bir filtreleme yapmak

                if(systemTextJsonOutputFormatter is not null)
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.hateoas+json");  // json desteği

                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.apiroot+json");
                }

                var xmlOutputFormatter = config
                .OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if(xmlOutputFormatter is not null)
                {
                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.hateoas+xml"); // xml desteği

                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.btkakademi.apiroot+xml");
                }
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified= true;
                opt.DefaultApiVersion = new ApiVersion(1, 0); // default versiyon 1.0 olsun
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version"); // versiyon bilgisini header'e taşıma

                // -- Aşağıdaki şekilde kullanım yaparsak controller üzerşnde ApiVersion diyerek attribute kullanmamıza gerek kalmaz.
                opt.Conventions.Controller<BooksController>()
                    .HasApiVersion(new ApiVersion(1, 0));

                opt.Conventions.Controller<BooksV2Controller>()
                    .HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) =>
            services.AddResponseCaching(); // istek gönderdikten sonra o istek 60 saniye boyunca ön bellekte duracak ve +
                                           // 60 saniye içinde tekrar istek gönderirsek ön bellekten gelecek ve log oluşmayacak.
    
        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)=>
            services.AddHttpCacheHeaders(expirationOpt =>
            {
                expirationOpt.MaxAge = 90; // default değer 60 saniye olduğu için burada 90 saniye yapmayı denedik
                expirationOpt.CacheLocation = CacheLocation.Public;
            },
              validationOpt =>
              {
                  validationOpt.MustRevalidate = false; // yeniden validate etme zorunluluğu olmasın
              });

        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>() { 
                new RateLimitRule()
                {
                    Endpoint="*",
                    Limit=60,  // dakikada 60 istek olabilsin
                    Period="1m" // 1 dakika içerisinde yalnızca 3 istek alınabilecek
                }
            };

            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        // Identity kütüphanesi için configuration
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(opts =>
            {
                opts.Password.RequireDigit = true;  // şifrede rakam olsun mu
                opts.Password.RequireLowercase = false; // küçük harf olsun mu
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false; // alfa nümerik karakterler olsun mu
                opts.Password.RequiredLength = 6; // uzunluğu 6 karakter olsun

                opts.User.RequireUniqueEmail = true;  // bir email bir kere kullanılabilsin
            }).AddEntityFrameworkStores<RepositoryContext>()
              .AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];

            services.AddAuthentication(opt =>
            {
                // Kullanılacak olan şema default olarak belirtiliyor
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,  // bu key'i kim ürettiyse bunu doğrula
                    ValidateAudience = true, // geçerli bir alıcı mı doğrular
                    ValidateLifetime = true,  // geçerlilik süresi
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title="BTK Akademi", 
                    Version="v1",
                    Description="BTK Akademi ASP.NET Core Web API",
                    TermsOfService= new Uri("https://www.btkakademi.gov.tr/"),
                    Contact=new OpenApiContact
                    {
                        Name="Büşra",
                        Email="busraozdm1@gmail.com",
                        Url=new Uri("https://busraozdm.com/")
                    }
                });
                s.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "BTK Akademi",
                    Version = "v2"
                });


                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },
                            Name = "Bearer"
                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IAuthenticationService, AuthenticationManager>();
        }
    }
}
