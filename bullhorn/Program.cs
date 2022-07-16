using System.Collections.Concurrent;

var root = Directory.GetCurrentDirectory();
bullhorn.DotEnv.Load(Path.Combine(root, ".env"));
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
int NUMITEMS = 1000;
int initialCapacity = 1000;
int numProcs = Environment.ProcessorCount;
int concurrencyLevel = numProcs * 2;
var socketJar = new ConcurrentDictionary<string, System.Net.WebSockets.WebSocket>(concurrencyLevel, initialCapacity);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(socketJar);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseWebSockets();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

