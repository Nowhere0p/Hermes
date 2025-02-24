using Hermes.DbCore;
using Hermes.Middleware;
using MongoDB.Driver;

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

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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