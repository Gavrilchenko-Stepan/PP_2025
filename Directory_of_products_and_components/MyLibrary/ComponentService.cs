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
