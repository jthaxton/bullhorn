using Microsoft.AspNetCore.Mvc;
using System.Collections;
namespace bullhorn.Controllers;
using System.Text;
using Newtonsoft.Json;
using bullhorn.Models;
using System.Text.Json;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private Dictionary<string, Queue<object>> _cookieJar;
    private Dictionary<string, System.Net.WebSockets.WebSocket> _socketJar;
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
        _cookieJar = new Dictionary<string, Queue<object>>();
        _socketJar = new Dictionary<string, System.Net.WebSockets.WebSocket>();
    }

    [HttpPost("/addNotification")]
    public async void Register()
    {

        _logger.LogInformation(Request.Path.Value);
        var deserializedBody = ExtendedSerializerExtensions.DeserializeBody<Order>(HttpContext.Request).Result;

        if (deserializedBody == null)
        {
            _logger.LogInformation(deserializedBody.ToString());
            _logger.LogInformation("to_cookies is undefined. Exiting.");
            return;
        }

        _logger.LogInformation(deserializedBody.PushToCookies.ToString());
        foreach(string cookie in deserializedBody.PushToCookies ) {

            if (_socketJar.ContainsKey(cookie))
            {
                var webSocket = _socketJar[cookie];
                var fulfillment = new Fulfillment(
                    resourceType: "testType",
                    meta: deserializedBody.Meta
                );

                var serializedFulfillment = ExtendedSerializerExtensions.Serialize(fulfillment);
                await webSocket.SendAsync(
                    new ArraySegment<byte>(serializedFulfillment, 0, 1024 * 4),
                    0,
                    true,
                    CancellationToken.None);

                _logger.LogInformation(_socketJar.ToString());
            }

        };
        await HttpContext.Response.WriteAsJsonAsync("success");
    }

    [HttpGet("/getregister")]
    public async Task? GetRegister()
    {
        //var ser = JsonConvert.SerializeObject(_cookieJar);
        var sub = new Order(actionType: "subscribe", fromCookie: "c00k!3", resourceType: null, pushToCookies: null, meta: null);
        await HttpContext.Response.WriteAsJsonAsync(sub);
    }


    [HttpGet("/wssubscribe")]
    public async Task SubscribeClient()
    {
        //_logger.LogInformation(HttpContext.Request.Body.ToString());
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Listen(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task Listen(System.Net.WebSockets.WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
        if (receiveResult != null && buffer != null)
        {
            var inputStream = new MemoryStream(buffer);
            var deserialized = ExtendedSerializerExtensions.DeserializeFromStream<Order>(inputStream);
            var toCookie = deserialized.FromCookie;
            if (toCookie != null && !_socketJar.ContainsKey(toCookie))
            {
                _socketJar[toCookie] = webSocket;
            }
            _logger.LogInformation(deserialized.ToString());
        }

        while (!receiveResult.CloseStatus.HasValue)
        {

        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}

