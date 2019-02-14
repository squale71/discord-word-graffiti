using Discord.WordGraffiti.DAL.Database;
using Discord.WordGraffiti.DAL.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.WordGraffiti.DAL.Repositories
{
    public class LevelThresholdRepository : ILevelThresholdRepository
    {
        public async Task Delete(LevelThreshold entity)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("DELETE FROM level_threshold WHERE id='@id';", db.Connection))
            {
                cmd.Parameters.AddWithValue("@id", entity.Id);

                using (var reader = cmd.ExecuteReader())
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<LevelThreshold>> GetAll()
        {
            var thresholds = new List<LevelThreshold>();

            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from level_threshold;", db.Connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (await reader.ReadAsync())
                {
                    thresholds.Add(GetLevelThresholdFromDataReader(reader));
                }
            }

            return thresholds;
        }

        public async Task<LevelThreshold> GetByExperiencePoints(int points)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from level_threshold WHERE points='@points';", db.Connection))
            {
                cmd.Parameters.AddWithValue("points", points);

                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetLevelThresholdFromDataReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<LevelThreshold> GetById(int ID)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from level_threshold WHERE id='@id';", db.Connection))
            {
                cmd.Parameters.AddWithValue("id", ID);

                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetLevelThresholdFromDataReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<LevelThreshold> GetByLevel(int level)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("SELECT * from level_threshold WHERE level='@level';", db.Connection))
            {
                cmd.Parameters.AddWithValue("level", level);

                using (var reader = cmd.ExecuteReader())
                {
                    if (await reader.ReadAsync())
                    {
                        return GetLevelThresholdFromDataReader(reader);
                    }
                }
            }

            return null;
        }

        public async Task<LevelThreshold> Insert(LevelThreshold entity)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("INSERT INTO level_threshold (level, points) VALUES (@level, @points);", db.Connection))
            {
                cmd.Parameters.AddWithValue("id", entity.Id);
                cmd.Parameters.AddWithValue("level", entity.Level);
                cmd.Parameters.AddWithValue("points", entity.Points);

                using (var reader = cmd.ExecuteReader())
                {
                    await cmd.ExecuteNonQueryAsync();

                    return await GetById(entity.Id);
                }
            }
        }

        public async Task<LevelThreshold> Update(LevelThreshold entity)
        {
            using (var db = new PostgresDBProvider())
            using (var cmd = new NpgsqlCommand("UPDATE level_threshold SET level=@level, points=@points WHERE id=@id;", db.Connection))
            {
                cmd.Parameters.AddWithValue("id", entity.Id);
                cmd.Parameters.AddWithValue("level", entity.Level);
                cmd.Parameters.AddWithValue("points", entity.Points);

                using (var reader = cmd.ExecuteReader())
                {
                    await cmd.ExecuteNonQueryAsync();

                    return entity;
                }
            }
        }

        public async Task<LevelThreshold> Upsert(LevelThreshold entity)
        {
            var threshold = await GetById(entity.Id);

            using (var db = new PostgresDBProvider())
            {
                if (threshold == null)
                {
                    return await Insert(entity);
                }

                else
                {
                    return await Update(entity);
                }
            }
        }

        private LevelThreshold GetLevelThresholdFromDataReader(NpgsqlDataReader reader)
        {
            return new LevelThreshold
            {
                Id = Convert.ToInt32(reader["id"]),
                Level = Convert.ToInt32(reader["level"]),
                Points = Convert.ToInt32(reader["points"]),
            };
        }
    }

    public interface ILevelThresholdRepository : IRepository<LevelThreshold>
    {
        Task<LevelThreshold> GetByExperiencePoints(int points);
        Task<LevelThreshold> GetByLevel(int level);
    }
}
