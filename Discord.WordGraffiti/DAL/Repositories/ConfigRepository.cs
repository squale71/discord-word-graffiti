using Discord.WordGraffiti.DAL.Database;
using Discord.WordGraffiti.DAL.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.DAL.Repositories
{
    /// <summary>
    /// Database layer for creating, reading, updating, and deleting words.
    /// </summary>
    public class ConfigRepository : IConfigRepository
    {
  
        /// <summary>
        /// Gets all words from the word table.
        /// </summary>
        /// <returns>A collection of words.</returns>
        public async Task<IEnumerable<Config>> GetAll()
        {
            var configCollection = new List<Config>();

            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from config;", db.Connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    configCollection.Add(GetConfigFromDataReader(reader));
                }
            }
            return configCollection;
        }

        public async Task<Config> GetById(int id)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from config WHERE id='@id';", db.Connection))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetConfigFromDataReader(reader);
                    }
                }
            }
                
            return null;
        }

        public async Task<Config> GetByName(string name)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from config WHERE name = @name", db.Connection))
            {
                cmd.Parameters.AddWithValue("name", name);
                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetConfigFromDataReader(reader);
                    }
                }
            }
                       
            return null;
        }

        public async Task<Config> Update(Config entity)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("UPDATE config SET name=@name, value=@value WHERE id='@id';", db.Connection))
            {
                cmd.Parameters.AddWithValue("@id", entity.Id);
                cmd.Parameters.AddWithValue("@name", entity.Name);
                cmd.Parameters.AddWithValue("@value", entity.Value);

                using (var reader = cmd.ExecuteReader())
                {
                    await cmd.ExecuteNonQueryAsync();

                    return entity;
                }
            }
        }

        private Config GetConfigFromDataReader(NpgsqlDataReader reader)
        {
            return new Config
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = reader["name"].ToString(),
                Value = reader["value"].ToString()
            };
        }

        public Task<Config> Insert(Config entity)
        {
            throw new NotImplementedException();
        }

        public Task<Config> SetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Config> Upsert(Config entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Config entity)
        {
            throw new NotImplementedException();
        }

    }

    public interface IConfigRepository : IRepository<Config>
    {
        Task<Config> GetByName(string name);
        //Task<Config> SetByName(string name);
    }
}
