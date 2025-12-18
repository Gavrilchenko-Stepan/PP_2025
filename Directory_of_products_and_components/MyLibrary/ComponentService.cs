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
            _componentRepository = componentRepository ?? throw new ArgumentNullException(nameof(componentRepository));
            _productComponentRepository = productComponentRepository ?? throw new ArgumentNullException(nameof(productComponentRepository));
        }

        public Component GetComponentById(int id) => _componentRepository.GetById(id);
        public List<Component> GetAllComponents() => _componentRepository.GetAll();
        public Component GetComponentByArticle(string article) => _componentRepository.GetByArticle(article);

        public List<Component> SearchComponents(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllComponents();
            return _componentRepository.Search(searchTerm);
        }

        public int AddComponent(Component component)
        {
            ValidateComponent(component);
            if (_componentRepository.CheckArticleExists(component.Article))
                throw new ArgumentException($"Артикул '{component.Article}' уже существует");
            return _componentRepository.Add(component);
        }

        public bool UpdateComponent(Component component)
        {
            ValidateComponent(component);
            if (_componentRepository.CheckArticleExists(component.Article, component.Id))
                throw new ArgumentException($"Артикул '{component.Article}' уже используется другим комплектующим");
            return _componentRepository.Update(component);
        }

        public bool DeleteComponent(int id)
        {
            if (_productComponentRepository.CheckComponentUsed(id))
                throw new InvalidOperationException(
                    "Нельзя удалить комплектующее, которое используется в изделиях. " +
                    "Сначала удалите его из всех изделий.");
            return _componentRepository.Delete(id);
        }

        public bool IsComponentUsed(int componentId) =>
            _productComponentRepository.CheckComponentUsed(componentId);

        public int GetComponentUsageCount(int componentId) =>
            _productComponentRepository.GetProductsByComponent(componentId).Count;

        public int GetTotalComponentQuantity(int componentId)
        {
            var productLinks = _productComponentRepository.GetProductsByComponent(componentId);
            return productLinks.Sum(link => link.Quantity);
        }

        public bool CheckArticleExists(string article, int? excludeId = null) =>
            _componentRepository.CheckArticleExists(article, excludeId);

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
    }
}
