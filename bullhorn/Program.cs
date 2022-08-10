using System.Collections.Concurrent;
using System.Net;

var root = Directory.GetCurrentDirectory();
bullhorn.DotEnv.Load(Path.Combine(root, ".env"));
var builder = WebApplication.CreateBuilder(args);

int NUMITEMS = 1000;
int initialCapacity = 1000;
int numProcs = Environment.ProcessorCount;
int concurrencyLevel = numProcs * 2;
var socketJar = new ConcurrentDictionary<string, System.Net.WebSockets.WebSocket>(concurrencyLevel, initialCapacity);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(socketJar);

ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
new HttpClientHandler().ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}


app.UseWebSockets();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5200";

app.Run("http://0.0.0.0:" + port);

