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

        public MainPresenter(IMainView view, ProductService productService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            _view.LoadEvent += OnViewLoad;
            _view.SearchEvent += OnSearch;
            _view.AddProductEvent += OnAddProduct;
            _view.EditProductEvent += OnEditProduct;
            _view.DeleteProductEvent += OnDeleteProduct;
            _view.ViewDetailsEvent += OnViewDetails;
            _view.WhereUsedEvent += OnWhereUsed;
            _view.RefreshEvent += OnRefresh;
        }

        /// <summary>
        /// Обработчик загрузки формы
        /// </summary>
        private void OnViewLoad(object sender, EventArgs e)
        {
            LoadProducts();
        }

        /// <summary>
        /// Обработчик обновления данных
        /// </summary>
        private void OnRefresh(object sender, EventArgs e)
        {
            LoadProducts();
        }

        /// <summary>
        /// Обработчик поиска
        /// </summary>
        private void OnSearch(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_view.SearchText))
                {
                    LoadProducts();
                }
                else
                {
                    var products = _productService.SearchProducts(_view.SearchText);
                    if (products != null)
                    {
                        _view.DisplayProducts(products);
                    }
                    _view.ClearProductInfo();
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка поиска: {ex.Message}", "Ошибка");
            }
        }

        /// <summary>
        /// Загрузка списка изделий
        /// </summary>
        private void LoadProducts()
        {
            try
            {
                var products = _productService.GetAllProducts();
                if (products != null)
                {
                    _view.DisplayProducts(products);
                }
                _view.ClearProductInfo();
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка загрузки: {ex.Message}", "Ошибка");
            }
        }

        /// <summary>
        /// Обработчик добавления изделия
        /// </summary>
        private void OnAddProduct(object sender, EventArgs e)
        {
            try
            {
                _view.ShowProductForm();
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        /// <summary>
        /// Обработчик редактирования изделия
        /// </summary>
        private void OnEditProduct(object sender, EventArgs e)
        {
            try
            {
                if (_view.SelectedProduct == null)
                {
                    _view.ShowMessage("Выберите изделие для редактирования", "Внимание");
                    return;
                }

                _view.ShowProductForm(_view.SelectedProduct);
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        /// <summary>
        /// Обработчик удаления изделия
        /// </summary>
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
                    bool success = _productService.DeleteProduct(product.Id);

                    if (success)
                    {
                        _view.ShowMessage("Изделие удалено", "Успех");
                        LoadProducts();
                    }
                    else
                    {
                        _view.ShowMessage("Не удалось удалить изделие", "Ошибка");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка удаления: {ex.Message}", "Ошибка");
            }
        }

        /// <summary>
        /// Обработчик просмотра состава изделия
        /// </summary>
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
                {
                    _view.ShowMessage("Не удалось загрузить состав изделия", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        /// <summary>
        /// Обработчик функции "Где используется"
        /// </summary>
        private void OnWhereUsed(object sender, EventArgs e)
        {
            try
            {
                // Просто вызываем метод View, который сам откроет нужную форму
                _view.ShowWhereUsedForm();
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        /// <summary>
        /// Сохранение изделия (вызывается из формы редактирования)
        /// </summary>
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
                    bool success = _productService.UpdateProduct(product);
                    if (success)
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
            catch (InvalidOperationException ex)
            {
                _view.ShowMessage(ex.Message, "Ошибка операции");
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка сохранения: {ex.Message}", "Ошибка");
            }
        }
    }
}
