using Discord.WordGraffiti.App_Start;
using Npgsql;

namespace Discord.WordGraffiti.DAL.Database
{
    public class PostgresDBProvider : DBProvider
    {
        public NpgsqlConnection Connection { get;  }

        public PostgresDBProvider()
        {
            var connectionString = Configuration.Instance.Get("PostgresDatabaseConnectionString");

            Connection = new NpgsqlConnection(connectionString);

            Connection.Open();
        }

        public override void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}
