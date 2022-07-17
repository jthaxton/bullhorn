# Bullhorn
A general purpose websocket microservice.

## Prerequisites
* C#
* .NET 6.0

## Recommended
* Visual Studio

## Instructions
1. A client should subscribe to the socket first
1. The server should POST to addNotification and list all user tokens/cookies in `PushToCookies`. For each token that exists in Bullhorn, it will receive a message.

## API
To send a notification from a server:
```
curl -X 'POST' \
  'https://localhost:7254/addNotification' \
  -H 'accept: */*' \
  -d '{"PushToCookies":["abc"],"Meta":{"A":"B","C":{},"D":[1,2,3]}}' \
```

To subscribe a client:
wss://localhost:7254/wssubscribe

First subscription message schema:
```json
{
  "FromCookie": "abc"
}
```

Subsequent response schema:
```json
{
  "ResourceType": "nullable string",
  "Meta": {
    "any": {
      "object": ["no matter how nested!"]
      }, 
    "whatever": "works for you"
    }
}
```