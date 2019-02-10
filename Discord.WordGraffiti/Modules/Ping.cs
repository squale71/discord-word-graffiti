using Discord.Commands;
using Discord.WordGraffiti.DAL.Database;
using Npgsql;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            var x = (Context.Message.Author);

            long count = 0;
            using (var db = new PostgresDBProvider())
            {
                using (var cmd = new NpgsqlCommand("SELECT count(*) from word", db.Connection))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        count = reader.GetInt64(0);
            }


            await ReplyAsync($"I currently know about {count} total words!");
            //await ReplyAsync($"You are user {x.Username} with id {x.Id}");
        }
    }
}
