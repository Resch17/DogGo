using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;

        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walk> GetAllWalks()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id, w.Date, w.Duration, w.WalkerId, w.DogId, o.Name As OwnerName, d.Name As DogName
                        FROM Walks w
                        LEFT JOIN Dog d on w.DogId = d.Id
                        LEFT JOIN Owner o on d.OwnerId = o.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Walk> walks = new List<Walk>();
                    while (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            ClientName = reader.GetString(reader.GetOrdinal("OwnerName")),
                            DogName = reader.GetString(reader.GetOrdinal("DogName"))
                        };
                        walks.Add(walk);
                    }
                    reader.Close();
                    return walks;
                }
            }
        }
        
        public List<Walk> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id, w.Date, w.Duration, w.WalkerId, w.DogId, o.Name As OwnerName
                        FROM Walks w
                        LEFT JOIN Dog d on w.DogId = d.Id
                        LEFT JOIN Owner o on d.OwnerId = o.Id
                        WHERE w.WalkerId = @walkerId";
                    cmd.Parameters.AddWithValue("@walkerId", walkerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();
                    while (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            ClientName = reader.GetString(reader.GetOrdinal("OwnerName"))
                        };

                        walks.Add(walk);
                    }

                    reader.Close();
                    return walks;
                }
            }
        }

        public void AddWalk(Walk walk)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Walks (Date, Duration, WalkerId, DogId)
                    OUTPUT INSERTED.ID
                    VALUES (@date, @duration, @walkerId, @dogId)";

                    cmd.Parameters.AddWithValue("@date", walk.Date);
                    cmd.Parameters.AddWithValue("@duration", walk.Duration);
                    cmd.Parameters.AddWithValue("@walkerId", walk.WalkerId);
                    cmd.Parameters.AddWithValue("@dogId", walk.DogId);

                    int id = (int)cmd.ExecuteScalar();

                    walk.Id = id;
                }
            }
        }
    }
}
