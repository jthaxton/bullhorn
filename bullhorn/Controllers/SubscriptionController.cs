using Microsoft.AspNetCore.Mvc;
namespace bullhorn.Controllers;
using bullhorn.Models;
using System.Collections.Concurrent;

[ApiController]
[Route("[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ILogger<SubscriptionController> _logger;
    private ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> _socketJar;
    public SubscriptionController(ILogger<SubscriptionController> logger, ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> socketJar)
    {
        _logger = logger;
        _socketJar = socketJar;
    }

    [HttpPost("/addNotification")]
    public async void Register()
    {

        _logger.LogInformation(Request.Path.Value);
        var deserializedBody = ExtendedSerializerExtensions.DeserializeBody<Order>(HttpContext.Request).Result;

        if (deserializedBody == null)
        {
            _logger.LogInformation("to_cookies is undefined. Exiting.");
            return;
        }

        foreach (string cookie in deserializedBody.PushToCookies ) {
            if (_socketJar.ContainsKey(cookie))
            {
                var webSocket = _socketJar[cookie];
                var fulfillment = new Fulfillment(
                    resourceType: deserializedBody.ResourceType,
                    meta: deserializedBody.Meta.ToString()
                );

                var serializedFulfillment = ExtendedSerializerExtensions.Serialize(fulfillment);
                _logger.LogInformation(serializedFulfillment.ToString());
                await webSocket.SendAsync(
                    new ArraySegment<byte>(serializedFulfillment, 0, 1024 * 4),
                    0,
                    true,
                    CancellationToken.None);

                _logger.LogInformation(_socketJar.ToString());
            }

        };
        await HttpContext.Response.WriteAsJsonAsync(_socketJar.Keys.First<string>().ToString());
    }

    [HttpGet("/getregister")]
    public async Task? GetRegister()
    {
        var sub = new Order(actionType: "subscribe", fromCookie: "c00k!3", resourceType: null, pushToCookies: null, meta: null);
        await HttpContext.Response.WriteAsJsonAsync(sub);
    }


    [HttpGet("/wssubscribe")]
    public async Task SubscribeClient()
    {
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
        System.Net.WebSockets.WebSocketReceiveResult? useReceiveResult;
        useReceiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
        if (useReceiveResult != null && buffer != null)
        {
            var inputStream = new MemoryStream(buffer);
            var deserialized = ExtendedSerializerExtensions.DeserializeFromStream<Order>(inputStream);
            var fromCookie = deserialized.FromCookie;
            if (fromCookie != null && !_socketJar.ContainsKey(fromCookie))
            {
                _socketJar[fromCookie] = webSocket;
            }
            _logger.LogInformation(deserialized.ToString());
            _logger.LogInformation(_socketJar.Keys.First<string>().ToString());
            var useWebsocket = _socketJar[fromCookie];
            useReceiveResult = await useWebsocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        _logger.LogInformation(_socketJar.Keys.First<string>());
        while (!useReceiveResult.CloseStatus.HasValue)
        {

        }

        await webSocket.CloseAsync(
            useReceiveResult.CloseStatus.Value,
            useReceiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}

