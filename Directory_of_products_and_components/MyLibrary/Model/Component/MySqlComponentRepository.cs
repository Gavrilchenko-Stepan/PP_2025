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

        /// Получение комплектующего по идентификатору
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

        /// Получение всех комплектующих
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

        /// Получение комплектующего по артикулу
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

        /// Поиск комплектующих по тексту
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

        /// Добавление нового комплектующего
        public int Add(Component component)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"INSERT INTO components (article, name, description, created_at) 
                             VALUES (@Article, @Name, @Description, @CreatedAt);
                             SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Article", component.Article);
                    command.Parameters.AddWithValue("@Name", component.Name);
                    command.Parameters.AddWithValue("@Description", component.Description ?? "");
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// Обновление комплектующего
        public bool Update(Component component)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"UPDATE components 
                             SET article = @Article, 
                                 name = @Name, 
                                 description = @Description
                             WHERE id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Article", component.Article);
                    command.Parameters.AddWithValue("@Name", component.Name);
                    command.Parameters.AddWithValue("@Description", component.Description ?? "");
                    command.Parameters.AddWithValue("@Id", component.Id);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// Удаление комплектующего
        public bool Delete(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM components WHERE id = @Id";

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
                string sql = "SELECT COUNT(*) FROM components WHERE article = @Article";

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
