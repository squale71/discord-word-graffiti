using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            var x = (Context.Message.Author);

            
            await ReplyAsync($"You are user {x.Username} with id {x.Id}");
        }
    }
}
