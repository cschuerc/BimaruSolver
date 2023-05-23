using Bimaru.Database;
using Bimaru.Database.DbContexts;
using Bimaru.Database.Entities;
using Bimaru.Database.Repositories;
using Bimaru.Interface.Database;
using Bimaru.Interface.Solver;
using Bimaru.Interface.Utility;
using Bimaru.Solver;
using Bimaru.Solver.CombinedRules;
using Bimaru.Solver.FieldChangedRules;
using Bimaru.Solver.TrialAndErrorRules;
using Bimaru.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Reflection;
using Newtonsoft.Json.Serialization;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => policy.AllowAnyMethod().WithOrigins("http://localhost:4200"));
});

builder.Services.AddControllers(
        options => options.ReturnHttpNotAcceptable = true
    )
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Bimaru API",
        Description = "An ASP.NET Core Web API for finding, solving and persisting Bimaru games (see https://en.wikipedia.org/wiki/Battleship_(puzzle))."
    });

    var webapiDocuXmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, webapiDocuXmlFilename));

    var interfacesXmlFilename = $"{Assembly.GetAssembly(typeof(BimaruValue))!.GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, interfacesXmlFilename));
});

builder.Services.AddScoped<IDerivedValueGenerator<GameEntity, int>, GameAlmostUniqueIdGenerator>();
builder.Services.AddScoped<IDerivedValueGenerator<GameEntity, GameSize>, GameSizeGenerator>();
builder.Services.AddScoped<IDerivedValueGenerator<GameEntity, GameDifficulty>, GameDifficultyGenerator>();
builder.Services.AddScoped<IEntityTypeConfiguration<GameEntity>, GameEntityConfiguration>();
builder.Services.AddScoped<IEntityTypeConfiguration<GridValueEntity>, GridValueEntityConfiguration>();
builder.Services.AddDbContext<GameDbContext>
(
    dbContextOptions =>
        dbContextOptions.UseSqlServer(
                builder.Configuration.GetConnectionString("BimaruGameDb"))
);

builder.Services.AddScoped<IGameRepository, GameRepository>();

AddSolverServiceAndDependencies(builder.Services);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    SeedGameDb.Initialize(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();


static void AddSolverServiceAndDependencies(IServiceCollection serviceCollection)
{
    serviceCollection.AddScoped<IFieldValueChangedRule, SetShipEnvironment>();
    serviceCollection.AddScoped<IFieldValueChangedRule, FillRowOrColumnWithWater>();
    serviceCollection.AddScoped<IFieldValueChangedRule, FillRowOrColumnWithShips>();
    serviceCollection.AddScoped<IFieldValueChangedRule, DetermineShipUndetermined>();
    serviceCollection.AddScoped<IFieldValueChangedRule, DetermineShipMiddleNeighbors>();

    serviceCollection.AddScoped<ISolverRule, FillRowOrColumnWithWater>();
    serviceCollection.AddScoped<ISolverRule, FillRowOrColumnWithShips>();

    serviceCollection.AddScoped<ITrialAndErrorRule>(_ => new OneMissingShipOrWater(new BruteForce()));

    serviceCollection.AddScoped(typeof(IBackup<>), typeof(Backup<>));

    serviceCollection.AddScoped<IBimaruSolver, BimaruSolver>();
}