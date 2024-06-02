using MafiaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace MafiaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebSocketsController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;

        public WebSocketsController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        [Authorize]
        [HttpGet("{id:alpha}")]
        public async Task GetWebSocket(string id)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task Echo(WebSocket webSocket)
        {
            byte[] helloMessage = Encoding.UTF8.GetBytes("Hello!");
            var buffer = new byte[1024 * 4];

            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                var clientMsg = Encoding.UTF8.GetString(buffer);
                switch (clientMsg)
                {
                    case "Hi":
                        await webSocket.SendAsync(new ArraySegment<byte>(helloMessage, 0, helloMessage.Length),
                            result.MessageType, result.EndOfMessage, CancellationToken.None);
                        break;
                    default:
                        webSocket.Abort();
                        //await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Wrong Command!", CancellationToken.None);
                        break;
                }
            }
            if (webSocket.State != WebSocketState.Aborted)
            {
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        }
    }
}
