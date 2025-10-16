using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
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

            // --- Database
            builder.Services.AddDbContext<FunewsManagementContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("FUNewsManagement")));

            // --- AutoMapper
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            // --- Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            // --- CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.WithOrigins("https://localhost:7121", "https://localhost:7122", "https://localhost:7123")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
            });

            // --- JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
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

            // --- OData
            builder.Services.AddControllers()
                .AddOData(opt => opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
                    .AddRouteComponents("odata", GetEdmModel()))
                .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                     options.JsonSerializerOptions.WriteIndented = true;
                 });

            // --- Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FUNews OData API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            // --- Dependency Injection
            builder.Services.AddScoped<IAccountsRepo, AccountsRepo>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<ICategoriesRepo, CategoriesRepo>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<IArticlesRepo, ArticlesRepo>();
            builder.Services.AddScoped<ArticleService>();
            builder.Services.AddScoped<ITagsRepo, TagsRepo>();
            builder.Services.AddScoped<TagService>();

            // --- Build app
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseRouting(); // ✅ Phải trước Authentication/Authorization

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers(); // ✅ Cuối cùng

            app.Run();


            static IEdmModel GetEdmModel()
            {
                var builder = new ODataConventionModelBuilder();
                builder.EntitySet<SystemAccount>("Accounts");
                builder.EntitySet<Category>("Categories");
                builder.EntitySet<NewsArticle>("Articles");
                builder.EntitySet<Tag>("Tags");
                return builder.GetEdmModel();
            }
        }
    }
}
