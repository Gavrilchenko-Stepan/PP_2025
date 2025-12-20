using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public interface IProductRepository
    {
        Product GetById(int id);
        List<Product> GetAll();
        Product GetByArticle(string article);
        List<Product> Search(string searchTerm);
    }
}
