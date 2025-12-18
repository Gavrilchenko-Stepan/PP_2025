using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class ComponentService
    {
        private readonly IComponentRepository _componentRepository;
        private readonly IProductComponentRepository _productComponentRepository;

        public ComponentService(
            IComponentRepository componentRepository,
            IProductComponentRepository productComponentRepository)
        {
            _componentRepository = componentRepository;
            _productComponentRepository = productComponentRepository;
        }

        /// Получение комплектующего по ID
        public Component GetComponentById(int id)
        {
            return _componentRepository.GetById(id);
        }

        /// Получение всех комплектующих
        public List<Component> GetAllComponents()
        {
            return _componentRepository.GetAll();
        }

        /// Поиск комплектующих по тексту
        public List<Component> SearchComponents(string searchTerm)
        {
            return _componentRepository.Search(searchTerm);
        }

        /// Добавление нового комплектующего
        public int AddComponent(Component component)
        {
            ValidateComponent(component);

            if (_componentRepository.CheckArticleExists(component.Article))
                throw new ArgumentException($"Артикул '{component.Article}' уже существует");

            return _componentRepository.Add(component);
        }

        /// Обновление комплектующего
        public bool UpdateComponent(Component component)
        {
            ValidateComponent(component);

            if (_componentRepository.CheckArticleExists(component.Article, component.Id))
                throw new ArgumentException($"Артикул '{component.Article}' уже используется другим комплектующим");

            return _componentRepository.Update(component);
        }

        /// Удаление комплектующего
        public bool DeleteComponent(int id)
        {
            // Проверяем, используется ли в изделиях
            if (_productComponentRepository.CheckComponentUsed(id))
                throw new InvalidOperationException(
                    "Нельзя удалить комплектующее, которое используется в изделиях. " +
                    "Сначала удалите его из всех изделий.");

            return _componentRepository.Delete(id);
        }

        /// Получение списка изделий, где используется комплектующее
        public List<Product> GetProductsUsingComponent(int componentId)
        {
            var productLinks = _productComponentRepository.GetProductsByComponent(componentId);
            var products = new List<Product>();

            // В реальном приложении нужно будет добавить ProductRepository
            // для получения полных данных об изделиях

            return products;
        }

        /// Проверка использования комплектующего в изделиях
        public bool IsComponentUsed(int componentId)
        {
            return _productComponentRepository.CheckComponentUsed(componentId);
        }

        /// Получение количества изделий, где используется комплектующее
        public int GetComponentUsageCount(int componentId)
        {
            var productLinks = _productComponentRepository.GetProductsByComponent(componentId);
            return productLinks.Count;
        }

        /// Получение общего количества комплектующего в изделиях
        public int GetTotalComponentQuantity(int componentId)
        {
            var productLinks = _productComponentRepository.GetProductsByComponent(componentId);
            int total = 0;

            foreach (var link in productLinks)
            {
                total += link.Quantity;
            }

            return total;
        }

        /// Валидация данных комплектующего
        private void ValidateComponent(Component component)
        {
            if (string.IsNullOrWhiteSpace(component.Article))
                throw new ArgumentException("Артикул обязателен");

            if (string.IsNullOrWhiteSpace(component.Name))
                throw new ArgumentException("Наименование обязательно");

            if (component.Article.Length > 50)
                throw new ArgumentException("Артикул не должен превышать 50 символов");

            if (component.Name.Length > 200)
                throw new ArgumentException("Наименование не должно превышать 200 символов");
        }

        /// Получение комплектующего по артикулу
        public Component GetComponentByArticle(string article)
        {
            return _componentRepository.GetByArticle(article);
        }

        /// Проверка существования артикула
        public bool CheckArticleExists(string article, int? excludeId = null)
        {
            return _componentRepository.CheckArticleExists(article, excludeId);
        }
    }
}
