using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// Получение всех изделий
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

        /// Получение изделия по артикулу
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

        /// Поиск изделий по тексту
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

        /// Добавление нового изделия
        public int Add(Product product)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"INSERT INTO products (article, name, description, created_at) 
                             VALUES (@Article, @Name, @Description, @CreatedAt);
                             SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Article", product.Article);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Description", product.Description ?? "");
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// Обновление изделия
        public bool Update(Product product)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"UPDATE products 
                             SET article = @Article, 
                                 name = @Name, 
                                 description = @Description
                             WHERE id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Article", product.Article);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Description", product.Description ?? "");
                    command.Parameters.AddWithValue("@Id", product.Id);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// Удаление изделия
        public bool Delete(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM products WHERE id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// Проверка существования артикула
        public bool CheckArticleExists(string article, int? excludeId = null)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM products WHERE article = @Article";

                if (excludeId.HasValue)
                {
                    sql += " AND id != @ExcludeId";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Article", article);

                    if (excludeId.HasValue)
                    {
                        command.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
                    }

                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
    }
}
