using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
