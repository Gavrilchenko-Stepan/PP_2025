using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductComponentRepository _productComponentRepository;
        private readonly IComponentRepository _componentRepository;

        public ProductService(
            IProductRepository productRepository,
            IProductComponentRepository productComponentRepository,
            IComponentRepository componentRepository)
        {
            _productRepository = productRepository;
            _productComponentRepository = productComponentRepository;
            _componentRepository = componentRepository;
        }

        public Product GetProductById(int id) => _productRepository.GetById(id);
        public List<Product> GetAllProducts() => _productRepository.GetAll();
        public Product GetProductByArticle(string article) => _productRepository.GetByArticle(article);
        public List<Product> SearchProducts(string searchTerm) => _productRepository.Search(searchTerm);

        public ProductComposition GetProductComposition(int productId)
        {
            var product = _productRepository.GetById(productId);
            if (product == null)
                throw new ArgumentException($"Изделие с ID {productId} не найдено");

            var componentLinks = _productComponentRepository.GetComponentsByProduct(productId);
            var compositionItems = new List<CompositionItem>();

            foreach (var link in componentLinks)
            {
                var component = _componentRepository.GetById(link.ComponentId);
                if (component != null)
                {
                    compositionItems.Add(new CompositionItem
                    {
                        Component = component,
                        Quantity = link.Quantity
                    });
                }
            }

            return new ProductComposition
            {
                Product = product,
                Components = compositionItems
            };
        }

        public List<ProductComposition> GetWhereComponentUsed(int componentId)
        {
            var productLinks = _productComponentRepository.GetProductsByComponent(componentId);
            var result = new List<ProductComposition>();

            var uniqueProductIds = productLinks
                .Select(l => l.ProductId)
                .Distinct()
                .ToList();

            foreach (var productId in uniqueProductIds)
            {
                try
                {
                    var composition = GetProductComposition(productId);
                    if (composition.Components.Any(c => c.Component.Id == componentId))
                    {
                        result.Add(composition);
                    }
                }
                catch
                {
                    // Игнорируем ошибки загрузки
                }
            }

            return result.OrderBy(c => c.Product.Name).ToList();
        }
    }
}
