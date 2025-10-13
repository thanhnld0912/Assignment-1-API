using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Models.Models;
using Repositories.Implements;
using Repositories.Interfaces;
using Services;
using Services.Mapping;
using System.Security.Claims;
using System.Text;

namespace API_ass1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // EF Core
            builder.Services.AddDbContext<FunewsManagementContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("FUNewsManagement")));


            // AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache(); // 👈 Required for session to store data in memory
            builder.Services.AddSession(); // already present


            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.WithOrigins("https://localhost:7121")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
            });

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                    RoleClaimType = ClaimTypes.Role
                };
            });

            // OData
            builder.Services.AddControllers()
                .AddOData(opt => opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
                    .AddRouteComponents("odata", GetEdmModel())); // ✅ giữ nguyên

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FUNews OData API", Version = "v1" });
            });


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.SameSite = SameSiteMode.None; // Important for cross-origin cookies
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });


            builder.Services.AddScoped<IAccountsRepo, AccountsRepo>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<ICategoriesRepo, CategoriesRepo>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<IArticlesRepo, ArticlesRepo>();
            builder.Services.AddScoped<ArticleService>();
            builder.Services.AddScoped<ITagsRepo, TagsRepo>();
            builder.Services.AddScoped<TagService>();



            var app = builder.Build();

            // ✅ CORS phải chạy TRƯỚC các pipeline xử lý
            app.UseCors("AllowFrontend");
            app.UseRouting();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowFrontend");
            app.UseSession(); // 👈 Add AFTER UseRouting() and BEFORE UseAuthentication()

            app.UseHttpsRedirection();

            app.UseAuthentication();  // ✅ phải trước Authorization
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

            // EDM for OData
            static IEdmModel GetEdmModel()
            {
                var builder = new ODataConventionModelBuilder();
                builder.EntitySet<SystemAccount>("Accounts");
                builder.EntitySet<Category>("Categories");
                builder.EntitySet<NewsArticle>("Articles").EntityType.HasKey(n => n.NewsArticleId).HasMany(n => n.Tags);
                builder.EntityType<Tag>()
              .HasMany(t => t.NewsArticles);
                return builder.GetEdmModel();
            }
             




        }
    }
}

