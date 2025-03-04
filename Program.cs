using Hermes.Common;
using Hermes.DbCore;
using Hermes.Middleware;
using Hermes.Services.EmailService;
using Hermes.src.Models;
using Hermes.src.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});
var configs= ResponseBuilder.DeserializeFromFile<HermesConfiguration>("HermesConfigs/hermesConfiguration.json");
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfigManager, HermesConfigManager>();
builder.Services.AddSingleton<IAuthClient, AuthClient>();
builder.Services.AddSingleton<IEmailHelper, EmailHelper>();
builder.Services.AddMemoryCache();

var userDbService= InitializeMongoClientAsync<UserDetails>(builder.Configuration.GetSection("UserDetailsDb")).GetAwaiter().GetResult();
builder.Services.AddSingleton<IMongoDbService<UserDetails>>(userDbService);
var otpDbService=InitializeMongoClientAsync<OtpVerification>(builder.Configuration.GetSection("OtpDb")).GetAwaiter().GetResult();
builder.Services.AddSingleton<IMongoDbService<OtpVerification>>(otpDbService);

//email service

builder.Services.AddSingleton<ISmtpClient>(provider =>
{
    var smtpClient = new SmtpClient();
    smtpClient.Connect(
        configs.EmailConfigs.Host,
        configs.EmailConfigs.Port,
        MailKit.Security.SecureSocketOptions.StartTls // Use appropriate security options
    );
    smtpClient.Authenticate(
        configs.EmailConfigs.Username,
        configs.EmailConfigs.Password
    );

    return smtpClient;
});
//Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]??"")),
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline
app.MapControllers();

// Add CORS, authentication and authorization to the middleware pipeline
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

async Task<MongoDbService<T>> InitializeMongoClientAsync<T>(
    IConfigurationSection configurationSection
)
    where T : IMongoDbRecord
{
    var mongoSettings = MongoClientSettings.FromConnectionString(
        configurationSection.GetSection("ConnectionString").Value
    );
    mongoSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
    mongoSettings.RetryWrites = true;
    mongoSettings.RetryReads = true;
    var databaseName = configurationSection.GetSection("DatabaseName").Value;
    var collectionName = configurationSection.GetSection("CollectionName").Value;
    var client = new MongoClient(mongoSettings);
    return new MongoDbService<T>(client, databaseName, collectionName);
}