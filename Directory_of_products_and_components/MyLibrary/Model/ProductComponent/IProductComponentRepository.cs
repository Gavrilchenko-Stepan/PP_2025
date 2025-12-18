using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public interface IProductComponentRepository
    {
        List<ProductComponent> GetComponentsByProduct(int productId);
        List<ProductComponent> GetProductsByComponent(int componentId);
        bool AddComponentToProduct(int productId, int componentId, int quantity);
        bool RemoveComponentFromProduct(int productId, int componentId);
        bool UpdateComponentQuantity(int productId, int componentId, int quantity);
        bool CheckComponentUsed(int componentId);
    }
}
