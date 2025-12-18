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
        private readonly ComponentService _componentService;

        public ProductDetailPresenter(
            IProductDetailView view,
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
            _view.AddComponentEvent += OnAddComponent;
            _view.EditComponentEvent += OnEditComponent;
            _view.RemoveComponentEvent += OnRemoveComponent;
            _view.SaveEvent += OnSave;
            _view.CloseEvent += OnClose;
            _view.RefreshEvent += OnRefresh;
        }

        private void OnViewLoad(object sender, EventArgs e) => RefreshComposition();

        private void OnRefresh(object sender, EventArgs e) => RefreshComposition();

        private void RefreshComposition()
        {
            try
            {
                if (_view.Composition?.Product == null)
                {
                    _view.ShowMessage("Не задано изделие для просмотра состава", "Внимание");
                    return;
                }

                var composition = _productService.GetProductComposition(_view.Composition.Product.Id);
                if (composition != null)
                {
                    _view.DisplayComposition(composition);
                    _view.Composition = composition;
                }
                else
                {
                    _view.ShowMessage("Не удалось загрузить состав изделия", "Ошибка");
                }
            }
            catch (ArgumentException ex)
            {
                _view.ShowMessage(ex.Message, "Ошибка");
            }
            catch (InvalidOperationException ex)
            {
                _view.ShowMessage(ex.Message, "Ошибка операции");
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка обновления: {ex.Message}", "Ошибка");
            }
        }

        private void OnAddComponent(object sender, EventArgs e)
        {
            try
            {
                if (_view.Composition?.Product == null)
                {
                    _view.ShowMessage("Не задано изделие", "Внимание");
                    return;
                }

                var availableComponents = _productService.GetAvailableComponents(_view.Composition.Product.Id);
                if (availableComponents != null && availableComponents.Any())
                {
                    _view.ShowComponentSelectionForm(availableComponents);
                }
                else
                {
                    _view.ShowMessage("Нет доступных комплектующих для добавления", "Внимание");
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void OnEditComponent(object sender, EventArgs e)
        {
            try
            {
                if (_view.SelectedComponent == null)
                {
                    _view.ShowMessage("Выберите комплектующее для редактирования", "Внимание");
                    return;
                }

                // Используем ComponentService для получения информации
                var component = _componentService.GetComponentById(_view.SelectedComponent.Component.Id);
                if (component != null)
                {
                    _view.ShowQuantityForm(_view.SelectedComponent, _view.SelectedComponent.Quantity);
                }
                else
                {
                    _view.ShowMessage("Комплектующее не найдено", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

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
                    // Проверяем, используется ли компонент где-то еще
                    bool isComponentUsed = _componentService.IsComponentUsed(component.Component.Id);

                    if (isComponentUsed)
                    {
                        int usageCount = _componentService.GetComponentUsageCount(component.Component.Id);
                        if (usageCount > 1)
                        {
                            var confirm = _view.ConfirmRemove(
                                $"Комплектующее используется в {usageCount} изделиях. Удалить его из текущего изделия?",
                                "Внимание");

                            if (!confirm)
                                return;
                        }
                    }

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
                _view.ShowMessage($"Ошибка удаления: {ex.Message}", "Ошибка");
            }
        }

        private void OnSave(object sender, EventArgs e)
        {
            try
            {
                _view.ShowMessage("Изменения сохранены", "Успех");
                RefreshComposition();
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка сохранения: {ex.Message}", "Ошибка");
            }
        }

        private void OnClose(object sender, EventArgs e)
        {
            try
            {
                _view.Close();
            }
            catch (Exception ex)
            {
                // Логируем ошибку закрытия
                System.Diagnostics.Debug.WriteLine($"Ошибка при закрытии формы: {ex.Message}");
            }
        }

        public void AddComponentToProduct(int componentId, int quantity)
        {
            try
            {
                if (_view.Composition?.Product == null)
                {
                    _view.ShowMessage("Не задано изделие", "Ошибка");
                    return;
                }

                var component = _componentService.GetComponentById(componentId);
                if (component == null)
                {
                    _view.ShowMessage("Комплектующее не найдено", "Ошибка");
                    return;
                }

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
            catch (InvalidOperationException ex)
            {
                _view.ShowMessage(ex.Message, "Ошибка операции");
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        public void UpdateComponentQuantity(int componentId, int quantity)
        {
            try
            {
                if (_view.Composition?.Product == null)
                {
                    _view.ShowMessage("Не задано изделие", "Ошибка");
                    return;
                }

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
            catch (InvalidOperationException ex)
            {
                _view.ShowMessage(ex.Message, "Ошибка операции");
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        public string GetComponentInfo(int componentId)
        {
            try
            {
                var component = _componentService.GetComponentById(componentId);
                if (component != null)
                {
                    int usageCount = _componentService.GetComponentUsageCount(componentId);
                    int totalQuantity = _componentService.GetTotalComponentQuantity(componentId);

                    return $"{component.Name} ({component.Article}) - " +
                           $"используется в {usageCount} изделиях, " +
                           $"всего {totalQuantity} шт.";
                }
                return "Комплектующее не найдено";
            }
            catch (Exception)
            {
                return "Ошибка при получении информации";
            }
        }
    }
}
