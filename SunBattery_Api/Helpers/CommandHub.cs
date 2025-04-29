using Microsoft.AspNetCore.SignalR;
using SunBattery_Api.Services.Commands;

namespace SunBattery_Api.Helpers
{
    public class CommandHub : Hub
    {
        private readonly IParseCommand _parseCommand;

        public CommandHub(IParseCommand parseCommand)
        {
            _parseCommand = parseCommand;

        }

        public async Task StartCommatd()
        {
            while (true)
            {
                _parseCommand.ParseCommandStrAsync();
                await Task.Delay(20000);
            }
        }

        public override async Task OnConnectedAsync()

        {
            Console.WriteLine(Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            Console.WriteLine(Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }
    }
}
