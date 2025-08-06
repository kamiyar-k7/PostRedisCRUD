using PostRedisCRUD.Data;
using PostRedisCRUD.Repositories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductRepository, ProductRepository>();


#region redis

builder.Services.AddSingleton<IConnectionMultiplexer>(op =>
{
    var cnf = builder.Configuration.GetSection("ConnectionStrings")["RedisConnectionString"];
    return ConnectionMultiplexer.Connect(cnf);
});

#endregion



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.MigrateDatabase<Program>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
