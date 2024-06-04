using MafiaAPI.Models;
using MafiaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MafiaAPI.Hub
{
    public class SignalRHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private static List<PlayerState> connectedPlayers = new();
        private static Dictionary<string, string> connectionsIds = new();
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly IUserRepository _userRepository;

        public SignalRHub(IPlayerStateRepository playerStateRepository, IUserRepository userRepository)
        {
            _playerStateRepository = playerStateRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        public async Task Join(string roomid) {
            var ps = await _playerStateRepository.GetByUserMatchIds(Context.User.Identity.Name, roomid);
            if(ps == null)
            {
                return;
            }
            connectedPlayers.Add(ps);
            connectionsIds.Add(ps.User.Id, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomid);
            List<string> connectedIds = [];
            connectedPlayers.FindAll(x => x.MatchId == roomid).ForEach(x => connectedIds.Add(x.UserId));
            await Clients.Caller.SendAsync("Connected", connectedIds);
        }

        [Authorize]
        public async Task Offer(string targetId, string message)
        {
            var invokerUser = await _userRepository.Get(Context.User.Identity.Name);
            await Clients.Client(connectionsIds[targetId]).SendAsync("IncomingOffer", invokerUser.Id, message);
        }

        [Authorize]
        public async Task Candidate(string targetId, string message)
        {
            var invokerUser = await _userRepository.Get(Context.User.Identity.Name);
            await Clients.Client(connectionsIds[targetId]).SendAsync("CandidateOffer", invokerUser.Id, message);
        }

        [Authorize]
        public async Task Answer(string targetId, string message)
        {
            var invokerUser = await _userRepository.Get(Context.User.Identity.Name);
            await Clients.Client(connectionsIds[targetId]).SendAsync("IncomingAnswer", invokerUser.Id, message);
        }

        [Authorize]
        public async Task Disconnect() {
            var ps = connectedPlayers.FirstOrDefault(x => x.UserId == Context.User.Identity.Name);
            string roomid = ps.MatchId;
            connectedPlayers.Remove(ps);
            connectionsIds.Remove(ps.User.Id);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomid);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var player = connectedPlayers.FirstOrDefault(x => x.UserId == Context.User?.Identity?.Name);
            if (player != null)
            {
                string roomid = player.MatchId;
                connectedPlayers.Remove(player);
                connectionsIds.Remove(player.User.Id);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomid);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
