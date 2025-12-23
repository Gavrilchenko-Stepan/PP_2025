using System.Collections.Generic;
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
