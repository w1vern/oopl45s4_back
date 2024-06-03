using MafiaAPI.Models;
using MafiaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MafiaAPI.Hub
{
    public class SignalRHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private static List<PlayerState> connectedPlayers = new();
        private readonly IPlayerStateRepository _playerStateRepository;

        public SignalRHub(IPlayerStateRepository playerStateRepository)
        {
            _playerStateRepository = playerStateRepository;
        }

        [Authorize]
        public async Task Join(string roomid) {
            connectedPlayers.Add(await _playerStateRepository.GetByUserMatchIds(Context.User.Identity.Name, roomid));
            await Groups.AddToGroupAsync(Context.ConnectionId, roomid);
            List<string> connectedIds = [];
            connectedPlayers.FindAll(x => x.MatchId == roomid).ForEach(x => connectedIds.Add(x.UserId));
            await Clients.Group(roomid).SendAsync("Connected", connectedIds);
        }

        [Authorize]
        public async Task Disconnect() {
            var ps = connectedPlayers.FirstOrDefault(x => x.UserId == Context.User.Identity.Name);
            string roomid = ps.MatchId;
            connectedPlayers.Remove(ps);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomid);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var player = connectedPlayers.FirstOrDefault(x => x.UserId == Context.User?.Identity?.Name);
            if (player != null)
            {
                string roomid = player.MatchId;
                connectedPlayers.Remove(player);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomid);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
