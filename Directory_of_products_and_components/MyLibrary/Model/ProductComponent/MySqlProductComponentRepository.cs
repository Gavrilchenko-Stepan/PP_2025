using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.Model.Repositories
{
    public class MySqlProductComponentRepository : IProductComponentRepository
    {
        private readonly string _connectionString;

        public MySqlProductComponentRepository()
        {
            _connectionString = IniConfig.ConnectionString;
        }

        /// Получение комплектующих для изделия
        public List<ProductComponent> GetComponentsByProduct(int productId)
        {
            var components = new List<ProductComponent>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"SELECT product_id, component_id, quantity 
                             FROM product_components 
                             WHERE product_id = @ProductId
                             ORDER BY component_id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            components.Add(new ProductComponent
                            {
                                ProductId = Convert.ToInt32(reader["product_id"]),
                                ComponentId = Convert.ToInt32(reader["component_id"]),
                                Quantity = Convert.ToInt32(reader["quantity"])
                            });
                        }
                    }
                }
            }
            return components;
        }

        /// Получение изделий для комплектующего
        public List<ProductComponent> GetProductsByComponent(int componentId)
        {
            var products = new List<ProductComponent>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"SELECT product_id, component_id, quantity 
                             FROM product_components 
                             WHERE component_id = @ComponentId
                             ORDER BY product_id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ComponentId", componentId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new ProductComponent
                            {
                                ProductId = Convert.ToInt32(reader["product_id"]),
                                ComponentId = Convert.ToInt32(reader["component_id"]),
                                Quantity = Convert.ToInt32(reader["quantity"])
                            });
                        }
                    }
                }
            }
            return products;
        }

        /// Добавление комплектующего к изделию
        public bool AddComponentToProduct(int productId, int componentId, int quantity)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string checkSql = @"SELECT COUNT(*) FROM product_components 
                                  WHERE product_id = @ProductId AND component_id = @ComponentId";

                using (var checkCmd = new MySqlCommand(checkSql, connection))
                {
                    checkCmd.Parameters.AddWithValue("@ProductId", productId);
                    checkCmd.Parameters.AddWithValue("@ComponentId", componentId);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                        return false;
                }

                string sql = @"INSERT INTO product_components (product_id, component_id, quantity) 
                             VALUES (@ProductId, @ComponentId, @Quantity)";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@ComponentId", componentId);
                    command.Parameters.AddWithValue("@Quantity", quantity);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// Удаление комплектующего из изделия
        public bool RemoveComponentFromProduct(int productId, int componentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"DELETE FROM product_components 
                             WHERE product_id = @ProductId AND component_id = @ComponentId";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@ComponentId", componentId);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// Обновление количества комплектующего в изделии
        public bool UpdateComponentQuantity(int productId, int componentId, int quantity)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = @"UPDATE product_components 
                             SET quantity = @Quantity
                             WHERE product_id = @ProductId AND component_id = @ComponentId";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@ComponentId", componentId);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// Проверка использования комплектующего в изделиях
        public bool CheckComponentUsed(int componentId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM product_components WHERE component_id = @ComponentId";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ComponentId", componentId);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
    }
}
