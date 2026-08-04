using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using TourEgypt.API.Middlewares;
using TourEgypt.Core;
using TourEgypt.Core.Common;
using TourEgypt.Core.Entities;
using TourEgypt.Core.Interfaces.Repositories;
using TourEgypt.Core.Interfaces.Services;
using TourEgypt.Data.Context;
using TourEgypt.Infrastructure;
using TourEgypt.Infrastructure.BackgroundJobs;
using TourEgypt.Infrastructure.Mapping;
using TourEgypt.Infrastructure.Repositories;
using TourEgypt.Infrastructure.Seed;
using TourEgypt.Infrastructure.Services;


namespace TourEgypt.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

            builder.Services.AddHttpContextAccessor();

            // ---- Swagger services ----
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TourEgypt API", Version = "v1" });

                // عشان تقدر تبعت الـ JWT Token من واجهة Swagger نفسها
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "أدخل التوكن بالشكل: Bearer {token}"
                });

                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });
            // ---------------------------

            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
                ?? throw new InvalidOperationException("Jwt section is missing in configuration.");

            builder.Services.AddSingleton(jwtOptions);


            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;


                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, option =>
                {
                    option.SaveToken = true;
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Signingkey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IPlaceService, PlaceService>();

            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
            builder.Services.AddScoped<IFavouriteService, FavouriteService>();

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<ICityService, CityService>();
            builder.Services.AddHostedService<CityMetricsUpdateJob>();


            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<TourEgyptProfile>();
            });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                await IdentitySeeder.SeedAsync(roleManager, userManager);
            }

            // ---- Swagger middleware ----
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // -----------------------------

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}