using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class MainPresenter
    {
        private readonly IMainView _view;
        private readonly ProductService _productService;
        private readonly ComponentService _componentService;

        public MainPresenter(
            IMainView view,
            ProductService productService,
            ComponentService componentService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _componentService = componentService ?? throw new ArgumentNullException(nameof(componentService));

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _view.LoadEvent += OnViewLoad;
            _view.SearchEvent += OnSearch;
            _view.AddProductEvent += OnAddProduct;
            _view.EditProductEvent += OnEditProduct;
            _view.DeleteProductEvent += OnDeleteProduct;
            _view.ViewDetailsEvent += OnViewDetails;
            _view.WhereUsedEvent += OnWhereUsed;
            _view.RefreshEvent += OnRefresh;
        }

        private void OnViewLoad(object sender, EventArgs e) => LoadProducts();
        private void OnRefresh(object sender, EventArgs e) => LoadProducts();

        private void OnSearch(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_view.SearchText))
                    LoadProducts();
                else
                    _view.DisplayProducts(_productService.SearchProducts(_view.SearchText));
                _view.ClearProductInfo();
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка поиска: {ex.Message}", "Ошибка");
            }
        }

        private void LoadProducts()
        {
            try
            {
                _view.DisplayProducts(_productService.GetAllProducts());
                _view.ClearProductInfo();
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка загрузки: {ex.Message}", "Ошибка");
            }
        }

        private void OnAddProduct(object sender, EventArgs e) => _view.ShowProductForm();

        private void OnEditProduct(object sender, EventArgs e)
        {
            if (_view.SelectedProduct == null)
            {
                _view.ShowMessage("Выберите изделие для редактирования", "Внимание");
                return;
            }
            _view.ShowProductForm(_view.SelectedProduct);
        }

        private void OnDeleteProduct(object sender, EventArgs e)
        {
            try
            {
                if (_view.SelectedProduct == null)
                {
                    _view.ShowMessage("Выберите изделие для удаления", "Внимание");
                    return;
                }

                var product = _view.SelectedProduct;
                if (_view.ConfirmDelete($"Удалить изделие '{product.Name}' ({product.Article})?", "Подтверждение удаления"))
                {
                    if (_productService.DeleteProduct(product.Id))
                    {
                        _view.ShowMessage("Изделие удалено", "Успех");
                        LoadProducts();
                    }
                    else
                        _view.ShowMessage("Не удалось удалить изделие", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка удаления: {ex.Message}", "Ошибка");
            }
        }

        private void OnViewDetails(object sender, EventArgs e)
        {
            try
            {
                if (_view.SelectedProduct == null)
                {
                    _view.ShowMessage("Выберите изделие для просмотра состава", "Внимание");
                    return;
                }

                var composition = _productService.GetProductComposition(_view.SelectedProduct.Id);
                if (composition != null)
                {
                    _view.DisplayProductInfo(composition);
                    _view.ShowProductDetailForm(composition);
                }
                else
                    _view.ShowMessage("Не удалось загрузить состав изделия", "Ошибка");
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void OnWhereUsed(object sender, EventArgs e)
        {
            try
            {
                // 1. Открываем диалог поиска компонента
                var component = _view.ShowComponentSearchDialog(_componentService);
                if (component == null) return;

                // 2. Получаем изделия, где используется компонент
                var compositions = _productService.GetWhereComponentUsed(component.Id);
                if (compositions == null || !compositions.Any())
                {
                    _view.ShowMessage($"Комплектующее '{component.Name}' не используется ни в одном изделии", "Результат");
                    return;
                }

                // 3. Показываем результаты
                _view.ShowWhereUsedResults(component, compositions);
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        public void SaveProduct(Product product)
        {
            if (product == null)
            {
                _view.ShowMessage("Данные изделия не заданы", "Ошибка");
                return;
            }

            try
            {
                if (product.Id == 0)
                {
                    int newId = _productService.AddProduct(product);
                    _view.ShowMessage($"Изделие добавлено (ID: {newId})", "Успех");
                }
                else
                {
                    if (_productService.UpdateProduct(product))
                        _view.ShowMessage("Изделие обновлено", "Успех");
                    else
                        _view.ShowMessage("Не удалось обновить изделие", "Ошибка");
                }
                LoadProducts();
            }
            catch (ArgumentException ex)
            {
                _view.ShowMessage(ex.Message, "Ошибка валидации");
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка сохранения: {ex.Message}", "Ошибка");
            }
        }
    }
}
