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

        /// Получение полного состава изделия
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

        /// Получение списка изделий, где используется компонент
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка загрузки изделия {productId}: {ex.Message}");
                }
            }

            return result.OrderBy(c => c.Product.Name).ToList();
        }

        /// Получение доступных для добавления комплектующих
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

        /// Получение изделия по ID
        public Product GetProductById(int id) => _productRepository.GetById(id);

        /// Получение всех изделий
        public List<Product> GetAllProducts() => _productRepository.GetAll();

        /// Поиск изделий по тексту
        public List<Product> SearchProducts(string searchTerm) => _productRepository.Search(searchTerm);

        /// Добавление нового изделия
        public int AddProduct(Product product)
        {
            ValidateProduct(product);

            if (_productRepository.CheckArticleExists(product.Article))
                throw new ArgumentException($"Артикул '{product.Article}' уже существует");

            return _productRepository.Add(product);
        }

        /// Обновление изделия
        public bool UpdateProduct(Product product)
        {
            ValidateProduct(product);

            if (_productRepository.CheckArticleExists(product.Article, product.Id))
                throw new ArgumentException($"Артикул '{product.Article}' уже используется другим изделием");

            return _productRepository.Update(product);
        }

        /// Удаление изделия
        public bool DeleteProduct(int id)
        {
            return _productRepository.Delete(id);
        }

        /// Добавление комплектующего к изделию
        public bool AddComponentToProduct(int productId, int componentId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть больше 0");

            return _productComponentRepository.AddComponentToProduct(productId, componentId, quantity);
        }

        /// Удаление комплектующего из изделия
        public bool RemoveComponentFromProduct(int productId, int componentId)
        {
            return _productComponentRepository.RemoveComponentFromProduct(productId, componentId);
        }

        /// Обновление количества комплектующего в изделии
        public bool UpdateComponentQuantity(int productId, int componentId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть больше 0");

            return _productComponentRepository.UpdateComponentQuantity(productId, componentId, quantity);
        }

        /// Получение всех комплектующих
        public List<Component> GetAllComponents()
        {
            return _componentRepository.GetAll();
        }

        /// Получение комплектующего по ID
        public Component GetComponentById(int id)
        {
            return _componentRepository.GetById(id);
        }

        /// Получение комплектующего по артикулу
        public Component GetComponentByArticle(string article)
        {
            return _componentRepository.GetByArticle(article);
        }

        /// Поиск комплектующих по тексту
        public List<Component> SearchComponents(string searchTerm)
        {
            return _componentRepository.Search(searchTerm);
        }

        /// Валидация данных изделия
        private void ValidateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Article))
                throw new ArgumentException("Артикул обязателен");

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Наименование обязательно");
        }
    }
}
