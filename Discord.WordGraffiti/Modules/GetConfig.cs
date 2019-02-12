using Discord.Commands;
using System.Threading.Tasks;
using Discord.WordGraffiti.DAL.Repositories;

namespace Discord.WordGraffiti.Modules
{
    public class GetConfig : ModuleBase<SocketCommandContext>
    {
        private IConfigRepository _configRepository;

        public GetConfig(IConfigRepository repository)
        {
            _configRepository = repository;
        }


        [Command("getconfig")]
        public async Task GetValueAsync(string name)
        {
            var res = await _configRepository.GetByName(name);

            if (res != null)
            {
                await ReplyAsync(res.Value.ToString());
            }

            else
            {
                await ReplyAsync("balls!");
            }
        }
    }
}
