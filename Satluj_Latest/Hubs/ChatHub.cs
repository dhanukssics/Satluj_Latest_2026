namespace Satluj_Latest.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Satluj_Latest.Models.Temp;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ChatHub : Hub
    {
        private readonly TempDbContext _context;

        public ChatHub(TempDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(int conversationId, string message, int chatUserId)
        {
            var chatUser = await _context.TbChatUsers
                .FirstOrDefaultAsync(x => x.ChatUserId == chatUserId);

            if (chatUser == null) return;

            var msg = new TbChatMessage
            {
                ConversationId = conversationId,
                FromChatUserId = chatUserId,
                MessageText = message,
                CreatedOn = DateTime.Now,
                IsRead = false
            };

            _context.TbChatMessages.Add(msg);
            await _context.SaveChangesAsync();

            await Clients.Group("conv_" + conversationId)
                .SendAsync("receiveMessage", chatUser.DisplayName, message, chatUser.ChatUserId, conversationId);

            await Clients.All
                .SendAsync("updateChatList", conversationId, message, DateTime.Now);
        }
        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "conv_" + conversationId);
        }

        public async Task UserTyping(int conversationId)
        {
            await Clients.Group("conv_" + conversationId)
                .SendAsync("showTyping");
        }

        public async Task StopTyping(int conversationId)
        {
            await Clients.Group("conv_" + conversationId)
                .SendAsync("hideTyping");
        }

        public async Task MessageSeen(int conversationId)
        {
            await Clients.Group("conv_" + conversationId)
                .SendAsync("messageSeen");
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            if (httpContext == null)
            {
                await base.OnConnectedAsync();
                return;
            }

            var chatUserIdStr = httpContext.Request.Query["chatUserId"];

            if (!int.TryParse(chatUserIdStr, out int chatUserId))
            {
                await base.OnConnectedAsync();
                return;
            }

            var user = await _context.TbChatUsers.FindAsync(chatUserId);
            if (user != null)
            {
                user.IsOnline = true;
                await _context.SaveChangesAsync();
            }

            await Clients.All.SendAsync("userOnline", chatUserId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();

            if (httpContext == null)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            var chatUserIdStr = httpContext.Request.Query["chatUserId"];

            if (!int.TryParse(chatUserIdStr, out int chatUserId))
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            var user = await _context.TbChatUsers.FindAsync(chatUserId);
            if (user != null)
            {
                user.IsOnline = false;
                await _context.SaveChangesAsync();
            }

            await Clients.All.SendAsync("userOffline", chatUserId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
