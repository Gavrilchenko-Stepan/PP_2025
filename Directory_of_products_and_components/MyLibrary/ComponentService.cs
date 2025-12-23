using System.Collections.Generic;

namespace MyLibrary
{
    public class ComponentService
    {
        private readonly IComponentRepository _componentRepository;

        public ComponentService(IComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
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
    }
}
