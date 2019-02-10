using Discord.WordGraffiti.DAL.Database;
using Discord.WordGraffiti.DAL.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.DAL.Repositories
{
    /// <summary>
    /// Database layer for creating, reading, updating, and deleting words.
    /// </summary>
    public class WordRepository : IWordRepository
    {
        /// <summary>
        /// Give a word entity, deletes from database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task Delete(Word entity)
        {
            using (var db = new PostgresDBProvider())
            {
                using (var cmd = new NpgsqlCommand("DELETE FROM word WHERE id='@id';", db.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    cmd.Parameters.AddWithValue("@id", entity.Id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets all words from the word table.
        /// </summary>
        /// <returns>A collection of words.</returns>
        public async Task<IEnumerable<Word>> GetAll()
        {
            var wordCollection = new List<Word>();

            using (var db = new PostgresDBProvider())
            {
                using (var cmd = new NpgsqlCommand("SELECT * from word;", db.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        wordCollection.Add(GetWordFromDataReader(reader));
                    }
                }
            }

            return wordCollection;
        }

        /// <summary>
        /// Given an Id, returns a single word.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A word, or null if not found.</returns>
        public async Task<Word> GetByID(int id)
        {
            using (var db = new PostgresDBProvider())
            {
                using (var cmd = new NpgsqlCommand("SELECT * from word WHERE id='@id';", db.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    if (await reader.ReadAsync())
                    {
                        return GetWordFromDataReader(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Give a string, returns a word that matches the value.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>A word, or null if not found.</returns>
        public async Task<Word> GetWord(string word)
        {
            using (var db = new PostgresDBProvider())
            {
                using (var cmd = new NpgsqlCommand("SELECT * from word WHERE name='@word';", db.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    cmd.Parameters.AddWithValue("@word", word);

                    if (await reader.ReadAsync())
                    {
                        return GetWordFromDataReader(reader);         
                    }
                }               
            }

            return null;
        }

        /// <summary>
        /// Given a collection of strings, returns a group of word entities
        /// </summary>
        /// <param name="words"></param>
        /// <returns>A collection of words. Will be empty if no matches.</returns>
        public async Task<IEnumerable<Word>> GetWords(IEnumerable<string> words)
        {
            var wordCollection = new List<Word>();

            using (var db = new PostgresDBProvider())
            {
                using (var cmd = new NpgsqlCommand($"SELECT * from word WHERE name in (@words);", db.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    cmd.Parameters.AddWithValue("@words", string.Join(",", words));

                    while (await reader.ReadAsync())
                    {
                        wordCollection.Add(GetWordFromDataReader(reader));
                    }
                }
            }

            return wordCollection;
        }

        /// <summary>
        /// Given a word entity, either updates the word if it exists, or inserts it if not.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The newly updated word.</returns>
        public async Task<Word> Upsert(Word entity)
        {
            var word = await GetByID(entity.Id);

            if (word == null)
            {
                using (var db = new PostgresDBProvider())
                {
                    using (var cmd = new NpgsqlCommand("INSERT INTO word (name, value) VALUES (@name, @value);", db.Connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        cmd.Parameters.AddWithValue("@name", entity.Name);
                        cmd.Parameters.AddWithValue("@value", entity.Value);

                        await cmd.ExecuteNonQueryAsync();

                        return await GetWord(entity.Name);
                    }
                }
            }

            else
            {
                using (var db = new PostgresDBProvider())
                {
                    using (var cmd = new NpgsqlCommand("UPDATE word SET name=@name, value=@value WHERE id='@id';", db.Connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        cmd.Parameters.AddWithValue("@name", entity.Name);
                        cmd.Parameters.AddWithValue("@value", entity.Value);

                        await cmd.ExecuteNonQueryAsync();

                        return entity;
                    }
                }
            }
        }

        private Word GetWordFromDataReader(NpgsqlDataReader reader)
        {
            return new Word
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = reader["name"].ToString(),
                Value = Convert.ToInt32(reader["value"])
            };
        }
    }

    public interface IWordRepository : IRepository<Word>
    {
        Task<Word> GetWord(string word);
        Task<IEnumerable<Word>> GetWords(IEnumerable<string> word);
    }
}
