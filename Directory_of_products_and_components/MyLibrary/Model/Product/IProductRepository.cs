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
        int Add(Product product);
        bool Update(Product product);
        bool Delete(int id);
        bool CheckArticleExists(string article, int? excludeId = null);
    }
}
