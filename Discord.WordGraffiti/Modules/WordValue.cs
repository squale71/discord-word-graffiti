using Discord.Commands;
using Discord.WordGraffiti.DAL.Repositories;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.Modules
{
    public class WordValue : ModuleBase<SocketCommandContext>
    {
        private IWordRepository _repository;

        public WordValue(IWordRepository repository)
        {
            _repository = repository;
        }

        [Command("wv")]
        public async Task GetValueAsync(string word)
        {
            var res = await _repository.GetByWord(word.ToLower());

            if (res != null)
            {
                await ReplyAsync(res.Value.ToString());
            }

            else
            {
                await ReplyAsync("That word doesn't exist!");
            }
        }
    }
}
