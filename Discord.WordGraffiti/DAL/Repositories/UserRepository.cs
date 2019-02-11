using Discord.WordGraffiti.DAL.Database;
using Discord.WordGraffiti.DAL.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.DAL.Repositories
{
    /// <summary>
    /// Database layer for creating, reading, updating, and deleting words.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        public Task Delete(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetById(ulong id)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from public.user WHERE id=@id;", db.Connection))
            {
                cmd.Parameters.AddWithValue("id",  NpgsqlTypes.NpgsqlDbType.Bigint, Convert.ToInt64(id));

                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetUserFromDataReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<User> GetById(int id)
        {
            return await GetById(Convert.ToUInt64(id));
        }

        public async Task<User> Insert(User entity)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("INSERT INTO public.user (id, username) VALUES (@id, @username);", db.Connection))
            {
                cmd.Parameters.AddWithValue("@id", Convert.ToInt64(entity.Id));
                cmd.Parameters.AddWithValue("@username", entity.Username);

                using (var reader = cmd.ExecuteReader())
                {
                    await cmd.ExecuteNonQueryAsync();
                    return entity;
                }
            }
        }

        public async Task<User> Update(User entity)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("UPDATE public.user SET username=@username WHERE id='@id';", db.Connection))
            {
                cmd.Parameters.AddWithValue("@id", Convert.ToInt64(entity.Id));
                cmd.Parameters.AddWithValue("@username", entity.Username);

                using (var reader = cmd.ExecuteReader())
                {
                    await cmd.ExecuteNonQueryAsync();

                    return entity;
                }
            }
        }

        public async Task<User> Upsert(User entity)
        {
            var user = await GetById(entity.Id);

            using (var db = new PostgresDBProvider())
            {
                if (user == null)
                {
                    return await Insert(entity);
                }

                else
                {
                    return await Update(entity);
                }
            }
        }

        private User GetUserFromDataReader(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = Convert.ToUInt64(reader["id"]),
                Username = reader["username"].ToString(),
            };
        }
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetById(ulong id);
    }
}
