using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.Model.Repositories
{
    public class MySqlComponentRepository : IComponentRepository
    {
        private readonly string _connectionString;

        public MySqlComponentRepository()
        {
            _connectionString = IniConfig.ConnectionString;
        }

        public Component GetById(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM components WHERE id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Component
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Article = reader["article"].ToString(),
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["created_at"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Component> GetAll()
        {
            var components = new List<Component>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM components ORDER BY name";

                using (var command = new MySqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        components.Add(new Component
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Article = reader["article"].ToString(),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        });
                    }
                }
            }
            return components;
        }

        public Component GetByArticle(string article)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM components WHERE article = @Article";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Article", article);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Component
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Article = reader["article"].ToString(),
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["created_at"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Component> Search(string searchTerm)
        {
            var components = new List<Component>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"SELECT * FROM components 
                             WHERE article LIKE @SearchTerm 
                                OR name LIKE @SearchTerm 
                                OR description LIKE @SearchTerm
                             ORDER BY name";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            components.Add(new Component
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Article = reader["article"].ToString(),
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["created_at"])
                            });
                        }
                    }
                }
            }
            return components;
        }
    }
}
