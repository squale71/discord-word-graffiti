using Discord.Commands;
using Discord.WebSocket;
using Discord.WordGraffiti.DAL.Database;
using Npgsql;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.Modules
{
    public class GetWordValue : ModuleBase<SocketCommandContext>
    {
        [Command("wv")]
        public async Task GetValueAsync(string word)
        {
            word = word.ToLower();
            int wordValue = 0;
            using (var db = new PostgresDBProvider())
            {
                using (var cmd = new NpgsqlCommand("SELECT value FROM word WHERE name='" + word + "'", db.Connection))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        wordValue = reader.GetInt32(0);
            }


            await ReplyAsync(wordValue.ToString());
        }
    }
}
