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

        var deserializedBody = ExtendedSerializerExtensions.DeserializeBody<Order>(HttpContext.Request).Result;

        if (deserializedBody == null)
        {
            _logger.LogInformation("to_cookies is undefined. Exiting.");
            await HttpContext.Response.WriteAsJsonAsync("Error: ToCookies is undefined");
            return;
        }

        string[] sentNotifications = Array.Empty<string>();
        foreach (string cookie in deserializedBody.PushToCookies ) {
            if (_socketJar.ContainsKey(cookie))
                _logger.LogInformation(cookie + " found. Writing fulfillment...");

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

                _logger.LogInformation(serializedFulfillment.ToString() + " sent to websocket.");
                sentNotifications.Append(cookie);
            }

        };

        await HttpContext.Response.WriteAsJsonAsync("Notifications sent to " + sentNotifications);
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
            await HttpContext.Response.WriteAsJsonAsync("Error: Try again with a websocket request.");
        }
    }

    private async Task Listen(System.Net.WebSockets.WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        System.Net.WebSockets.WebSocketReceiveResult? useReceiveResult;
        useReceiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
        var fromCookie = "";
        if (useReceiveResult != null && buffer != null)
        {

            var inputStream = new MemoryStream(buffer);
            var deserialized = ExtendedSerializerExtensions.DeserializeFromStream<Order>(inputStream);
            fromCookie = deserialized.FromCookie;
            if(fromCookie == null)
            {
                _logger.LogInformation("FromCookie not in serialized object. Exiting.");
                await HttpContext.Response.WriteAsJsonAsync("FromCookie not in serialized object.");
                return;
            }
            if (!_socketJar.ContainsKey(fromCookie))
            {
                _logger.LogInformation("FromCookie " + fromCookie + " not found in cache. Caching now.");
                _socketJar[fromCookie] = webSocket;
            }
            var useWebsocket = _socketJar[fromCookie];
            _logger.LogInformation("FromCookie" + fromCookie + "found in cache.");
            useReceiveResult = await useWebsocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        while (!useReceiveResult.CloseStatus.HasValue)
        {

        }
        bool tryRemove = false;
        if (fromCookie != null)
        {
            tryRemove = _socketJar.TryRemove(fromCookie, out webSocket);
        }
        if (tryRemove == true)
        {
            _logger.LogInformation("Uncached websocket:" + fromCookie);
        } else
        {
            _logger.LogInformation("Failed to uncache websocket:" + fromCookie);

        }
        _logger.LogInformation("Closing websocket...");

        await webSocket.CloseAsync(
            useReceiveResult.CloseStatus.Value,
            useReceiveResult.CloseStatusDescription,
            CancellationToken.None);
        _logger.LogInformation("Done closing websocket.");

    }
}

