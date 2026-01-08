using DTBWebServer.DataBase;
using DTBWebServer.GameLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using DTBWebServer.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging.EventLog;
using NLog;
using NLog.Web;
using DTBWebServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

NLog4Init.LoadConfig();
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options =>
 {
     options.Limits.MaxConcurrentConnections = 8192;
     options.Limits.MaxConcurrentUpgradedConnections = 8192;
 });

#if DEBUG
var connectionString = builder.Configuration.GetRequiredSection("TestDb").Get<DataBaseConnectConfig>().GetConnectStr();
var serverVersion = new MySqlServerVersion(new Version(5, 7, 0));
#else
var connectionString = builder.Configuration.GetRequiredSection("ReleaseDb").Get<DataBaseConnectConfig>().GetConnectStr();
var serverVersion = new MySqlServerVersion(new Version(5, 7, 0));
#endif

builder.Services.AddDbContextPool<WorldDBContext>((DbContextOptionsBuilder dbContextOptions) => dbContextOptions
        .UseMySql(connectionString, serverVersion, mySqlOptionsAction =>
        {
            //mySqlOptionsAction.CommandTimeout(300);
        })
#if DEBUG
         .EnableSensitiveDataLogging()
#endif
         .EnableDetailedErrors()
, 8192);

builder.Services.AddMvcCore().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
});

var jwtconfig = builder.Configuration.GetRequiredSection("Jwt").Get<JwtConfig>();
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.AddScheme<LimitChallengeAuthenticationHandler>(LimitChallengeAuthenticationHandler.SchemeName, "LimitChallenge");
})
.AddJwtBearer(options =>
{
/*    options.ForwardDefaultSelector = content =>
    {
        return content.Request.Path.Value!.Contains(LimitChallengeAuthenticationHandler.SchemeName) ? LimitChallengeAuthenticationHandler.SchemeName : null;
    };*/
    
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtconfig.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtconfig.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = JwtHelper.GetSymetricSecurityKey(jwtconfig.SigningKey),
        ValidateLifetime = true,
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddAuthorization(options => {
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .AddAuthenticationSchemes(new string[] { JwtBearerDefaults.AuthenticationScheme, LimitChallengeAuthenticationHandler.SchemeName })
    .Build();
});


builder.Services.AddOptions().Configure<JwtConfig>(builder.Configuration.GetRequiredSection("Jwt"));
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddControllers();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
builder.Host.UseNLog();

builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

WebApplication app = builder.Build();
app.UseStatusCodePages();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

GameLogicMain.Instance.Init(app);

string localUrl = app.Configuration.GetRequiredSection("LocalUrl").Value;
app.Urls.Add(localUrl);
app.UseResponseCaching();
app.UseResponseCompression();
app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();

//----------------------Begin----------------------------

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello World!");
});

app.MapPost("/", async context =>
{
    await context.Response.WriteAsync("Hello World!  Post");
});

app.MapControllers();

//------------------------End---------------------------------
try
{
    app.Run();
}
catch (Exception exception)
{
    NLog.LogManager.GetCurrentClassLogger().Error(exception, "Stopped program because of exception");
}
finally
{
    NLog.LogManager.Shutdown();
}
