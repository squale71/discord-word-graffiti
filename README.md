# discord-word-graffiti
Discord Bot that gives users scores in the discord bot based on words they use.

This application uses PostGres as it's database backend. 
You will also need to create a discord application and supply local configuration with your discord API key. 

Once you pull the project, you'll need to have ready your API key and Postgres connection string so you can run it locally. 
This application uses the .NET Core User Secrets API to store a json file locally on your machine containing any secrets you don't want committed to the repo. 
Obviously you'll not want to do this for your production application, as you'll want to use whatever secrets manager you have available to you there. 
You can look up how to get started creating a Discord app in their documentation, and read up on MongoDB as well, as this will assume you have basic knowledge of both.

Do the following to get going:

1. Open Powershell and 'cd' to your project directory:

Run the following commands:

```dotnet user-secrets set DiscordApiKey "{your-key}"```

```dotnet user-secrets set PostgresDatabaseConnectionString "Host={ip};Username={username};Password={pw};Database=WordGraffiti"```

1. Run the following command to spin up your database:
```docker compose -f .\docker-compose.postgres.yml up -d```
