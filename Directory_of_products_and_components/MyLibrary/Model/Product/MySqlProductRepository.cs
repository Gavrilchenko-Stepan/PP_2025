using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace MyLibrary.Model.Repositories
{
    public class MySqlProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public MySqlProductRepository()
        {
            _connectionString = IniConfig.ConnectionString;
        }

        public Product GetById(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM products WHERE id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
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

        public List<Product> GetAll()
        {
            var products = new List<Product>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM products ORDER BY name";

                using (var command = new MySqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
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
            return products;
        }

        public Product GetByArticle(string article)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM products WHERE article = @Article";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Article", article);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
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

        public List<Product> Search(string searchTerm)
        {
            var products = new List<Product>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"SELECT * FROM products 
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
                            products.Add(new Product
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
            return products;
        }
    }
}
