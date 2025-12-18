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
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _productComponentRepository = productComponentRepository ?? throw new ArgumentNullException(nameof(productComponentRepository));
            _componentRepository = componentRepository ?? throw new ArgumentNullException(nameof(componentRepository));
        }

        public Product GetProductById(int id) => _productRepository.GetById(id);
        public List<Product> GetAllProducts() => _productRepository.GetAll();
        public Product GetProductByArticle(string article) => _productRepository.GetByArticle(article);
        public List<Product> SearchProducts(string searchTerm) => _productRepository.Search(searchTerm);

        public int AddProduct(Product product)
        {
            ValidateProduct(product);
            if (_productRepository.CheckArticleExists(product.Article))
                throw new ArgumentException($"Артикул '{product.Article}' уже существует");
            return _productRepository.Add(product);
        }

        public bool UpdateProduct(Product product)
        {
            ValidateProduct(product);
            if (_productRepository.CheckArticleExists(product.Article, product.Id))
                throw new ArgumentException($"Артикул '{product.Article}' уже используется другим изделием");
            return _productRepository.Update(product);
        }

        public bool DeleteProduct(int id) => _productRepository.Delete(id);

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
                    // Игнорируем ошибки загрузки отдельных изделий
                }
            }

            return result.OrderBy(c => c.Product.Name).ToList();
        }

        public List<Component> GetAvailableComponents(int productId)
        {
            var allComponents = _componentRepository.GetAll();
            var usedComponents = _productComponentRepository.GetComponentsByProduct(productId);
            var usedIds = usedComponents.Select(c => c.ComponentId).ToHashSet();

            return allComponents
                .Where(c => !usedIds.Contains(c.Id))
                .OrderBy(c => c.Name)
                .ToList();
        }

        public bool AddComponentToProduct(int productId, int componentId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть больше 0");
            return _productComponentRepository.AddComponentToProduct(productId, componentId, quantity);
        }

        public bool RemoveComponentFromProduct(int productId, int componentId) =>
            _productComponentRepository.RemoveComponentFromProduct(productId, componentId);

        public bool UpdateComponentQuantity(int productId, int componentId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть больше 0");
            return _productComponentRepository.UpdateComponentQuantity(productId, componentId, quantity);
        }

        public bool CheckProductArticleExists(string article, int? excludeId = null) =>
            _productRepository.CheckArticleExists(article, excludeId);

        private void ValidateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Article))
                throw new ArgumentException("Артикул обязателен");
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Наименование обязательно");
            if (product.Article.Length > 50)
                throw new ArgumentException("Артикул не должен превышать 50 символов");
            if (product.Name.Length > 200)
                throw new ArgumentException("Наименование не должно превышать 200 символов");
        }
    }
}
