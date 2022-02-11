using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ReactProjectsAuthApi.Data;
using ReactProjectsAuthApi.Models;
using System.Text.Json;

namespace ReactProjectsAuthApi.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MainHub : Hub
    {
        private readonly IMapper _mapper;
        private readonly ChatDbContext _ctx;

        public MainHub(IMapper mapper,ChatDbContext ctx)
        {
            _mapper = mapper;
            _ctx = ctx;
        }

        public override async Task OnConnectedAsync()
        {
            string email = Context.User.Identity.Name;
            if (Connections.ConnectionMap.ContainsKey(email))
            {
                Connections.ConnectionMap.Remove(email);
            }
            Connections.ConnectionMap.Add(email,Context.ConnectionId);
            await Clients.Caller.SendAsync("ConnectionEstablished");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string email = Context.User.Identity.Name;
            if (Connections.ConnectionMap.ContainsKey(email))
            {
                Connections.ConnectionMap.Remove(email);
            }
            await Clients.Caller.SendAsync("Disconnected");
        }

        public async Task GetActiveUsers(string name)
        {
            try
            {
                string[] activeUsers = Connections.ConnectionMap.Where(c => c.Key.StartsWith(name)).Select(c => c.Key).ToArray();

                await Clients.Caller.SendAsync("ActiveUsers", JsonSerializer.Serialize(new { active = activeUsers }));
            }
            catch
            {

            }
        }

        public async Task GetMyFriends()
        {
            try
            {
                List<string> allPrevFriends = _ctx.Relations.FirstOrDefault(r => r.UserEmail == Context.User.Identity.Name).Friends.Split(',').ToList();

                await Clients.Caller.SendAsync("MyFriends", JsonSerializer.Serialize(allPrevFriends));
            }
            catch(Exception ex)
            {
                
            }
        }

        public async Task AddToFriends(string name)
        {
            try
            {
                List<string> allPrevFriends = _ctx.Relations.FirstOrDefault(r => r.UserEmail == Context.User.Identity.Name).Friends.Split(',').ToList();
                if (!allPrevFriends.Contains(name))
                {
                    RelationshipsModel md = _ctx.Relations.FirstOrDefault(r => r.UserEmail == Context.User.Identity.Name);
                    allPrevFriends.Add(name);
                    md.Friends = String.Join(',', allPrevFriends);
                    await _ctx.SaveChangesAsync();
                }
                await Clients.Caller.SendAsync("FriendAdded");
            }
            catch
            {

            }
            
        }

        public async Task RemoveFromFriends(string msg)
        {
            try
            {
                List<string> allPrevFriends = _ctx.Relations.FirstOrDefault(r => r.UserEmail == Context.User.Identity.Name).Friends.Split(',').ToList();
                if (allPrevFriends.Contains(msg))
                {
                    dynamic md = JsonSerializer.Deserialize<dynamic>(msg);
                    allPrevFriends.Remove(md.Email);
                    _ctx.SaveChanges();
                }

                await Clients.Caller.SendAsync("FriendDeleted");
            }
            catch
            {

            }
        }

        public async Task SendMessage(string msg)
        {
            MessageDTO message = JsonSerializer.Deserialize<MessageDTO>(msg);
            var valMessage = _mapper.Map<MessageModel>(message);
            _ctx.Messages.Add(valMessage);
            await _ctx.SaveChangesAsync();

            Connections.ConnectionMap.TryGetValue(message.To, out string cd);
            if (!string.IsNullOrEmpty(cd))
            {
                await Clients.Client(cd).SendAsync("ReceiveMessage",JsonSerializer.Serialize(valMessage));
            }
        }
    }
}
