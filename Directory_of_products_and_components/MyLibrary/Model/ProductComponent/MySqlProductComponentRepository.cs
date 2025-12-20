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
    }
}
