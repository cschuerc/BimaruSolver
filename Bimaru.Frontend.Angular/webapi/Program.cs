using Bimaru.Interface.Solver;
using Bimaru.Interface.Utility;
using Bimaru.Solver;
using Bimaru.Solver.CombinedRules;
using Bimaru.Solver.FieldChangedRules;
using Bimaru.Solver.TrialAndErrorRules;
using Bimaru.Utility;
using Microsoft.EntityFrameworkCore;
using webapi.DbContexts;
using webapi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers
    (
        options => options.ReturnHttpNotAcceptable = true
    )
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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