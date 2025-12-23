using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLibrary
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductComponentRepository _productComponentRepository;
        private readonly IComponentRepository _componentRepository;

        public ProductService(IProductRepository productRepository, IProductComponentRepository productComponentRepository, IComponentRepository componentRepository)
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
            Product product = _productRepository.GetById(productId)
        ?? throw new ArgumentException($"Изделие с ID {productId} не найдено");

            var compositionItems = _productComponentRepository.GetComponentsByProduct(productId).Select(link => new
                {
                    Link = link,
                    Component = _componentRepository.GetById(link.ComponentId)
                }).Where(x => x.Component != null).Select(x => new CompositionItem
                {
                    Component = x.Component,
                    Quantity = x.Link.Quantity
                }).ToList();

            return new ProductComposition
            {
                Product = product,
                Components = compositionItems
            };
        }

        public List<ProductComposition> GetWhereComponentUsed(int componentId)
        {
            List<ProductComponent> productLinks = _productComponentRepository.GetProductsByComponent(componentId);
            List<ProductComposition> result = new List<ProductComposition>();

            var uniqueProductIds = productLinks.Select(l => l.ProductId).Distinct().ToList();

            foreach (var productId in uniqueProductIds)
            {
                try
                {
                    ProductComposition composition = GetProductComposition(productId);
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
