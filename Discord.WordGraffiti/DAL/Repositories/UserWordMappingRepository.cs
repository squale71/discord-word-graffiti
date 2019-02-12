using Discord.WordGraffiti.DAL.Database;
using Discord.WordGraffiti.DAL.Models;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.DAL.Repositories
{
    public class UserWordMappingRepository : IUserWordMappingRepository
    {
        public async Task AddUserWords(int userId, IEnumerable<UserWordMapping> mappings)
        {
            using (var db = new PostgresDBProvider())
            {
                var transaction = db.Connection.BeginTransaction();

                using (var cmd = db.Connection.CreateCommand())
                {
                    cmd.CommandText =
                        "insert into user_word_mapping(user_id, word_id, value)" +
                            "values(@userId, :wordIds, :values)";

                    cmd.Transaction = transaction;

                    cmd.Parameters.AddWithValue("userId", userId);
                    cmd.Parameters.Add("wordIds", NpgsqlDbType.Array | NpgsqlDbType.Integer).Value = mappings.Select(x => x.WordId);
                    cmd.Parameters.Add("values", NpgsqlDbType.Array | NpgsqlDbType.Integer).Value = mappings.Select(x => x.Value);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
            }           
        }

        public async Task Delete(UserWordMapping entity)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("DELETE FROM user_word_mapping WHERE id='@id';", db.Connection))
            {
                cmd.Parameters.AddWithValue("@id", entity.Id);

                using (var reader = cmd.ExecuteReader())
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<UserWordMapping>> GetAll()
        {
            var wordCollection = new List<UserWordMapping>();

            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from user_word_mapping;", db.Connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    wordCollection.Add(GetUserWordMappingFromDataReader(reader));
                }
            }

            return wordCollection;
        }

        public async Task<UserWordMapping> GetById(int id)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from user_word_mapping WHERE id='@id';", db.Connection))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetUserWordMappingFromDataReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<UserWordMapping> GetByWordId(int wordId)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from user_word_mapping WHERE word_id='@wordId';", db.Connection))
            {
                cmd.Parameters.AddWithValue("wordId", wordId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetUserWordMappingFromDataReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<UserWordMapping> GetByWordName(string wordName)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT wm.* from user_word_mapping wm JOIN word w ON w.Id = wm.word_id WHERE w.name ='@wordName';", db.Connection))
            {
                cmd.Parameters.AddWithValue("wordName", wordName);

                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetUserWordMappingFromDataReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<UserWordMapping>> GetUserOwnedWords(int userId)
        {
            var collection = new List<UserWordMapping>();

            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand($"SELECT * from user_word_mapping WHERE user_id = @userId;", db.Connection))
            {
                cmd.Parameters.AddWithValue("userId", userId);

                var sql = cmd.CommandText;
                using (var reader = cmd.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        collection.Add(GetUserWordMappingFromDataReader(reader));
                    }
                }
            }

            return collection;
        }

        public async Task<IEnumerable<UserWordMapping>> GetUserOwnedWords(int userId, params string[] words)
        {
            var collection = new List<UserWordMapping>();

            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT wm.* from user_word_mapping wm JOIN word w ON w.Id = wm.word_id WHERE m.user_id = @userId and w.name = ANY(:words);", db.Connection))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.Add("words", NpgsqlDbType.Array | NpgsqlDbType.Varchar).Value = words;
                using (var reader = cmd.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        collection.Add(GetUserWordMappingFromDataReader(reader));
                    }
                }
            }

            return collection;
        }

        public async Task<UserWordMapping> Insert(UserWordMapping entity)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("INSERT INTO user_word_mapping (user_id, word_id, value) VALUES (@user_id, @word_id, @value);", db.Connection))
            {
                cmd.Parameters.AddWithValue("user_id", entity.UserId);
                cmd.Parameters.AddWithValue("word_id", entity.WordId);
                cmd.Parameters.AddWithValue("value", entity.Value);

                using (var reader = cmd.ExecuteReader())
                {
                    await cmd.ExecuteNonQueryAsync();

                    return await GetByWordId(entity.WordId);
                }
            }
        }

        public async Task<UserWordMapping> Update(UserWordMapping entity)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("UPDATE word SET value=@value, user_id=@userId, word_id=@wordId WHERE id=@id;", db.Connection))
            {
                cmd.Parameters.AddWithValue("id", entity.Id);
                cmd.Parameters.AddWithValue("userId", entity.UserId);
                cmd.Parameters.AddWithValue("wordId", entity.WordId);
                cmd.Parameters.AddWithValue("value", entity.Value);

                using (var reader = cmd.ExecuteReader())
                {
                    await cmd.ExecuteNonQueryAsync();

                    return entity;
                }
            }
        }

        public async Task UpdateUserWordOwnership(int userId, IEnumerable<UserWordMapping> mappings)
        {
            using (var db = new PostgresDBProvider())
            {
                var transaction = db.Connection.BeginTransaction();

                var sb = new StringBuilder();
                if (mappings.Any())
                {
                    using (var cmd = db.Connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;

                        // Dynamically building sql string to prevent multiple trips to database.
                        foreach (var mapping in mappings)
                        {
                            sb.Append($"UPDATE word SET value=@{mapping.Value}, user_id=@{mapping.UserId}, word_id=@{mapping.WordId} WHERE id={mapping.Id};");
                        }

                        cmd.CommandText = sb.ToString();
                        using (var reader = cmd.ExecuteReader())
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await transaction.CommitAsync();
            }
        }

        public async Task<UserWordMapping> Upsert(UserWordMapping entity)
        {
            var word = await GetById(entity.Id);

            using (var db = new PostgresDBProvider())
            {
                if (word == null)
                {
                    return await Insert(entity);
                }

                else
                {
                    return await Update(entity);
                }
            }
        }

        private UserWordMapping GetUserWordMappingFromDataReader(NpgsqlDataReader reader)
        {
            return new UserWordMapping
            {
                Id = Convert.ToInt32(reader["id"]),
                UserId = Convert.ToUInt64(reader["user_id"]),
                WordId = Convert.ToInt32(reader["word_id"]),
                Value = Convert.ToInt32(reader["value"])
            };
        }
    }

    public interface IUserWordMappingRepository : IRepository<UserWordMapping>
    {
        Task<UserWordMapping> GetByWordId(int wordId);
        Task<UserWordMapping> GetByWordName(string wordName);
        Task<IEnumerable<UserWordMapping>> GetUserOwnedWords(int userId);
        Task<IEnumerable<UserWordMapping>> GetUserOwnedWords(int userId, params string[] words);
        Task AddUserWords(int userId, IEnumerable<UserWordMapping> mappings);
        Task UpdateUserWordOwnership(int userId, IEnumerable<UserWordMapping> mappings);
    }
}
