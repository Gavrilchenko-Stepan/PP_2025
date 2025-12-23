using System.Collections.Generic;

namespace MyLibrary
{
    public interface IComponentRepository
    {
        Component GetById(int id);
        List<Component> GetAll();
        Component GetByArticle(string article);
        List<Component> Search(string searchTerm);
    }
}
