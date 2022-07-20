using HotelListing.API.Configurations;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Repository;
using HotelListing.API.Data;
using HotelListing.API.Data.Entities;
using HotelListing.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
builder.Services.AddDbContext<HotelListingDbContext>(options =>
{
    options.UseSqlServer(connString);
});

builder.Services.AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ApiUser>>("HotelListingApi")
    .AddEntityFrameworkStores<HotelListingDbContext>()
    .AddDefaultTokenProviders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader()
                                        .AllowAnyOrigin()
                                        .AllowAnyMethod());
});
//API versioning
//builder.Services.AddApiVersioning(options =>
//{
//    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
//    options.AssumeDefaultVersionWhenUnspecified = true;
//    options.ReportApiVersions = true;
//    options.ApiVersionReader = ApiVersionReader.Combine(
//         new QueryStringApiVersionReader("api-version"),
//         new HeaderApiVersionReader("API-Version"),
//         new MediaTypeApiVersionReader("ver")
//    );

//});

//builder.Services.AddVersionedApiExplorer(options =>
//    {
//        options.AssumeDefaultVersionWhenUnspecified = true;
//        options.SubstituteApiVersionInUrl = true;

//    }
//);

//using SeriLog with configuration from appsetting.json
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

//load autoMapperService with config object /configuration/AutoMapperConfig.cs
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

//add generic Repositories to Build scope
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//add specific Repositories to Build scope
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IHotelsRepository, HotelsRepository>();

//add AuthManager for users to Build scope
builder.Services.AddScoped<IAuthManager, AuthManager>();

//add JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,      
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});
// API Response Data Caching 
builder.Services.AddResponseCaching(options => 
    {
        options.MaximumBodySize = 1024;
        options.UseCaseSensitivePaths = true;   
    }
);

builder.Services.AddControllers().AddOData(options =>
    { 
        //options.Select().Filter().OrderBy();
        options.QuerySettings.EnableSelect = true;
        options.QuerySettings.EnableFilter = true;
        options.QuerySettings.EnableOrderBy = true; 
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// API Response Data Caching 
app.UseResponseCaching();
app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(10)
        };
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };

    await next();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
