using Assessment.Api.Business;
using Assessment.Api.Entity;
using Assessment.Api.Repository;
using Assessment.Api.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the containers

//SeriLog
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Assessment API",
        Description = "API for Name and Address CRUD functionality"
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//In-memory Database
builder.Services.AddDbContext<AgDbContext>(c => c.UseInMemoryDatabase(databaseName: "AssessmentDb"));

//Services
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ICacheRepository<Person>, PersonCacheRepository>();
builder.Services.AddScoped<IPersonBL, PersonBL>();

//Auto mapper
builder.Services.AddAutoMapper(typeof(Program));

//Cache
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();
app.Run();
