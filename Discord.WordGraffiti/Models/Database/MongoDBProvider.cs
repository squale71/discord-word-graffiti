using Discord.WordGraffiti.App_Start;
using MongoDB.Driver;

namespace Discord.WordGraffiti.Models.Database
{
    public class MongoDBProvider : DBProvider
    {
        private MongoClient _client;

        public IMongoDatabase Context { get; }

        public MongoDBProvider()
        {
            _client = new MongoClient(Configuration.Instance.Get("MongoDatabaseConnectionString"));
            Context = _client.GetDatabase(Configuration.Instance.Get("MongoDatabaseName"));
        }

        // Doesn't actually have any way to dispose, but keeping this here for now.
        public override void Dispose()
        {
        }
    }
}
