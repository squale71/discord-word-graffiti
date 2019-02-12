using Discord.Commands;
using Discord.WordGraffiti.DAL.Database;
using Npgsql;
using System.Threading.Tasks;
using Discord.WordGraffiti.DAL.Repositories;

namespace Discord.WordGraffiti.Modules
{
    public class GetRankings : ModuleBase<SocketCommandContext>
    {
        
        [Command("getrankings")]
        public async Task GetRankingsAsync()
        {
            
            await ReplyAsync("These are the rankings: ");
            //await ReplyAsync($"You are user {x.Username} with id {x.Id}");
        }
    }
}
