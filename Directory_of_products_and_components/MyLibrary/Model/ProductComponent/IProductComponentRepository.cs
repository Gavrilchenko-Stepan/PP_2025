using System.Collections.Generic;

namespace MyLibrary
{
    public interface IProductComponentRepository
    {
        List<ProductComponent> GetComponentsByProduct(int productId);
        List<ProductComponent> GetProductsByComponent(int componentId);
    }
}
