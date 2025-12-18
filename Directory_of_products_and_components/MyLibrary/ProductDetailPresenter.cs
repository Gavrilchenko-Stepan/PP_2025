using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class ProductDetailPresenter
    {
        private readonly IProductDetailView _view;
        private readonly ProductService _productService;

        public ProductDetailPresenter(
            IProductDetailView view,
            ProductService productService)
        {
            _view = view;
            _productService = productService;

            _view.LoadEvent += OnViewLoad;
            _view.AddComponentEvent += OnAddComponent;
            _view.EditComponentEvent += OnEditComponent;
            _view.RemoveComponentEvent += OnRemoveComponent;
            _view.SaveEvent += OnSave;
            _view.CloseEvent += OnClose;
            _view.RefreshEvent += OnRefresh;
        }

        /// Обработчик загрузки формы
        private void OnViewLoad(object sender, EventArgs e)
        {
            RefreshComposition();
        }

        /// Обработчик обновления данных
        private void OnRefresh(object sender, EventArgs e)
        {
            RefreshComposition();
        }

        /// Обновление состава изделия
        private void RefreshComposition()
        {
            try
            {
                if (_view.Composition?.Product == null) return;

                var composition = _productService.GetProductComposition(_view.Composition.Product.Id);
                _view.DisplayComposition(composition);
                _view.Composition = composition;
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка обновления: {ex.Message}", "Ошибка");
            }
        }

        /// Обработчик добавления комплектующего
        private void OnAddComponent(object sender, EventArgs e)
        {
            try
            {
                if (_view.Composition?.Product == null) return;

                var availableComponents = _productService.GetAvailableComponents(_view.Composition.Product.Id);
                _view.ShowComponentSelectionForm(availableComponents);
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        /// Обработчик редактирования количества комплектующего
        private void OnEditComponent(object sender, EventArgs e)
        {
            try
            {
                if (_view.SelectedComponent == null)
                {
                    _view.ShowMessage("Выберите комплектующее для редактирования", "Внимание");
                    return;
                }

                _view.ShowQuantityForm(_view.SelectedComponent, _view.SelectedComponent.Quantity);
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        /// Обработчик удаления комплектующего
        private void OnRemoveComponent(object sender, EventArgs e)
        {
            try
            {
                if (_view.SelectedComponent == null)
                {
                    _view.ShowMessage("Выберите комплектующее для удаления", "Внимание");
                    return;
                }

                var component = _view.SelectedComponent;
                if (_view.ConfirmRemove($"Удалить '{component.ComponentName}' из состава?", "Подтверждение удаления"))
                {
                    bool success = _productService.RemoveComponentFromProduct(
                        _view.Composition.Product.Id,
                        component.Component.Id);

                    if (success)
                    {
                        _view.ShowMessage("Комплектующее удалено из состава", "Успех");
                        RefreshComposition();
                    }
                    else
                    {
                        _view.ShowMessage("Не удалось удалить комплектующее", "Ошибка");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка удаления: {ex.Message}", "Ошибка");
            }
        }

        /// Обработчик сохранения изменений
        private void OnSave(object sender, EventArgs e)
        {
            _view.ShowMessage("Изменения сохранены", "Успех");
        }

        /// Обработчик закрытия формы
        private void OnClose(object sender, EventArgs e)
        {
            _view.Close();
        }

        /// Добавление комплектующего к изделию
        public void AddComponentToProduct(int componentId, int quantity)
        {
            try
            {
                if (_view.Composition?.Product == null) return;

                bool success = _productService.AddComponentToProduct(
                    _view.Composition.Product.Id,
                    componentId,
                    quantity);

                if (success)
                {
                    _view.ShowMessage("Комплектующее добавлено в состав", "Успех");
                    RefreshComposition();
                }
                else
                {
                    _view.ShowMessage("Не удалось добавить комплектующее", "Ошибка");
                }
            }
            catch (ArgumentException ex)
            {
                _view.ShowMessage(ex.Message, "Ошибка валидации");
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        /// Обновление количества комплектующего в изделии
        public void UpdateComponentQuantity(int componentId, int quantity)
        {
            try
            {
                if (_view.Composition?.Product == null) return;

                bool success = _productService.UpdateComponentQuantity(
                    _view.Composition.Product.Id,
                    componentId,
                    quantity);

                if (success)
                {
                    _view.ShowMessage("Количество обновлено", "Успех");
                    RefreshComposition();
                }
                else
                {
                    _view.ShowMessage("Не удалось обновить количество", "Ошибка");
                }
            }
            catch (ArgumentException ex)
            {
                _view.ShowMessage(ex.Message, "Ошибка валидации");
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }
    }
}
