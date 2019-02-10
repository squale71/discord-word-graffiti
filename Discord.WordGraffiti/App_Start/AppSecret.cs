namespace Discord.WordGraffiti.App_Start
{
    public class AppSecret
    {
        public string DiscordApiKey { get; set; }
        public string MongoDatabaseConnectionString { get; set; }
        public string PostgresDatabaseConnectionString { get; set; }
    }
}
