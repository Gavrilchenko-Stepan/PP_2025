using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public interface IMainView
    {
        string SearchText { get; set; }
        Product SelectedProduct { get; }

        event EventHandler LoadEvent;
        event EventHandler SearchEvent;
        event EventHandler AddProductEvent;
        event EventHandler EditProductEvent;
        event EventHandler DeleteProductEvent;
        event EventHandler ViewDetailsEvent;
        event EventHandler WhereUsedEvent;
        event EventHandler RefreshEvent;

        void Show();
        void Close();
        void DisplayProducts(List<Product> products);
        void DisplayProductInfo(ProductComposition composition);
        void ShowMessage(string message, string title);
        bool ConfirmDelete(string message, string title);
        void ClearProductInfo();
        void ShowProductForm(Product product = null);
        void ShowProductDetailForm(ProductComposition composition);

        Component ShowComponentSearchDialog(ComponentService componentService);
        void ShowWhereUsedResults(Component component, List<ProductComposition> compositions);
    }
}
