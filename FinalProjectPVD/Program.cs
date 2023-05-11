using Microsoft.OpenApi.Models;
using UPB.CoreLogic.Managers;
using UPB.FinalProjectPVD.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configurationBuilder = new ConfigurationBuilder()
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
        
IConfiguration Configuration = configurationBuilder.Build();
string siteTitle = Configuration.GetSection("Title").Value;
string providersFilePath = Configuration.GetSection("Path:ProvidersFilePath").Value;
string backingService = Configuration.GetSection("BackingService").Value;

builder.Services.AddTransient<ProviderManager>(_ => new ProviderManager(providersFilePath, backingService));


// Create the logger and setup your sinks, filters and properties
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(Configuration)
    .CreateLogger();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = siteTitle
    });
});

var app = builder.Build();

// Log application environment
Log.Information("Application running in {Environment} environment", builder.Environment.EnvironmentName);

app.UseGlobalExceptionHandler(Log.Logger);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "QA" || app.Environment.EnvironmentName == "UAT")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
