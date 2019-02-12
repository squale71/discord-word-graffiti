using Discord.Commands;
using Discord.WordGraffiti.DAL.Database;
using Npgsql;
using System.Threading.Tasks;
using Discord.WordGraffiti.DAL.Repositories;

namespace Discord.WordGraffiti.Modules
{
    public class GetRank : ModuleBase<SocketCommandContext>
    {
        private IUserRepository _userRepository; //dependent on user-word mapping repo

        [Command("getrank")]
        public async Task GetRankAsync()
        {
            var id = (Context.Message.Author.Id);

            //await _userRepository.GetById(id); //dependent on user-word mapping repo


            await ReplyAsync($"Your rank is: ");
        }
    }
}
