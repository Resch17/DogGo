using DogGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;
        public DogRepository(IConfiguration config)
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

        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT d.Id AS Id, d.Name AS Name, d.Breed as Breed, d.Notes, d.ImageUrl, o.Name AS Owner, o.Id AS OwnerId
                            FROM Dog d
                            LEFT JOIN Owner o ON o.Id = d.OwnerId";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Dog> dogs = new List<Dog>();
                    while (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Owner = new Owner()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Name = reader.GetString(reader.GetOrdinal("Owner"))
                            },
                            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ?
                                    "No Notes Found" : reader.GetString(reader.GetOrdinal("Notes")),
                            ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ?
                                    null : reader.GetString(reader.GetOrdinal("ImageUrl"))

                        };
                        dogs.Add(dog);
                    }
                    reader.Close();
                    return dogs;
                }
            }
        }
        public Dog GetDogById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                          SELECT Id, Name, OwnerId, Breed, Notes, ImageUrl
                         FROM Dog
                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            //Notes = reader.GetString(reader.GetOrdinal("Notes")),
                            //ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"))
                        };

                        reader.Close();

                        return dog;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }

                }
            }
        }


    }
}
