using MyLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainForm
{
    public partial class MainForm : Form
    {
        private ProductService _productService;
        private ComponentService _componentService;
        private List<Product> _allProducts;
        private string _sortedBy = "";
        private bool _sortAscending = true;

        public MainForm(ProductService productService, ComponentService componentService)
        {
            _productService = productService;
            _componentService = componentService;

            InitializeComponent();

            txtSearch.TextChanged += (s, e) => ApplyFilters();

            dtpDateFrom.ValueChanged += (s, e) => ApplyFilters();
            dtpDateTo.ValueChanged += (s, e) => ApplyFilters();

            LoadProducts();

            dtpDateFrom.Value = DateTime.Today.AddMonths(-1);
            dtpDateTo.Value = DateTime.Today;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            var product = GetSelectedProduct();
            if (product == null)
            {
                MessageBox.Show("Выберите изделие для просмотра состава", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var composition = _productService.GetProductComposition(product.Id);
                if (composition != null)
                {
                    using (var form = new ProductDetailForm(composition, _productService, _componentService))
                    {
                        form.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки состава: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWhereUsed_Click(object sender, EventArgs e)
        {
            using (var searchForm = new ComponentSearchForm(_componentService))
            {
                if (searchForm.ShowDialog() == DialogResult.OK && searchForm.SelectedComponent != null)
                {
                    var component = searchForm.SelectedComponent;

                    try
                    {
                        var compositions = _productService.GetWhereComponentUsed(component.Id);

                        if (compositions.Count > 0)
                        {
                            using (var resultsForm = new WhereUsedForm(component, compositions))
                            {
                                resultsForm.ShowDialog();
                            }
                        }
                        else
                        {
                            MessageBox.Show(
                                $"Комплектующее '{component.Name}' не используется ни в одном изделии.",
                                "Результаты поиска",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            UpdateProductInfo();
        }

        private void LoadProducts()
        {
            try
            {
                _allProducts = _productService.GetAllProducts();
                ApplyFilters();
                ClearProductInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изделий: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayProducts(List<Product> products)
        {
            dgvProducts.Rows.Clear();

            if (products.Count == 0)
            {
                dgvProducts.Rows.Add("", "Нет данных", "По вашему запросу ничего не найдено", "", "");
                return;
            }

            foreach (var product in products)
            {
                dgvProducts.Rows.Add(
                    product.Id,
                    product.Article,
                    product.Name,
                    product.Description ?? "",
                    product.CreatedAt
                );
            }
        }

        private void UpdateProductInfo()
        {
            try
            {
                var product = GetSelectedProduct();
                if (product != null)
                {
                    var composition = _productService.GetProductComposition(product.Id);
                    DisplayProductInfo(composition);
                }
            }
            catch
            {
                // Игнорируем ошибки при выделении
            }
        }

        private void DisplayProductInfo(ProductComposition composition)
        {
            var info = new System.Text.StringBuilder();
            info.AppendLine($"📋 ИЗДЕЛИЕ: {composition.Product.Name}");
            info.AppendLine($"🏷️ Артикул: {composition.Product.Article}");

            if (!string.IsNullOrEmpty(composition.Product.Description))
                info.AppendLine($"📝 Описание: {composition.Product.Description}");

            info.AppendLine($"📅 Дата создания: {composition.Product.CreatedAt:dd.MM.yyyy HH:mm}");
            info.AppendLine();
            info.AppendLine($"🔢 Позиций в составе: {composition.ComponentTypesCount}");
            info.AppendLine($"📦 Всего комплектующих: {composition.TotalComponents}");
            info.AppendLine();
            info.AppendLine("🔩 СОСТАВ:");

            if (composition.Components != null && composition.Components.Any())
            {
                foreach (var item in composition.Components.OrderByDescending(c => c.Quantity))
                {
                    info.AppendLine($"  • {item.ComponentName} — {item.Quantity} шт.");
                }
            }
            else
            {
                info.AppendLine("  Состав не задан");
            }

            txtInfo.Text = info.ToString();
            txtInfo.ForeColor = Color.Black;
        }

        private void ClearProductInfo()
        {
            txtInfo.Text = "Выберите изделие из списка для просмотра подробной информации...";
            txtInfo.ForeColor = Color.FromArgb(120, 120, 120);
        }

        private Product GetSelectedProduct()
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var row = dgvProducts.SelectedRows[0];

                // Проверяем, что это не строка "Нет данных"
                if (row.Cells["colId"].Value?.ToString() == "")
                    return null;

                return new Product
                {
                    Id = Convert.ToInt32(row.Cells["colId"].Value),
                    Article = row.Cells["colArticle"].Value?.ToString() ?? "",
                    Name = row.Cells["colName"].Value?.ToString() ?? "",
                    Description = row.Cells["colDescription"].Value?.ToString() ?? "",
                    CreatedAt = Convert.ToDateTime(row.Cells["colCreatedAt"].Value)
                };
            }
            return null;
        }

        private void dgvProducts_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var column = dgvProducts.Columns[e.ColumnIndex];

            // Сортируем только по имени и дате
            if (column.Name == "colName" || column.Name == "colCreatedAt")
            {
                if (_sortedBy == column.Name)
                {
                    // Меняем направление сортировки
                    _sortAscending = !_sortAscending;
                }
                else
                {
                    // Новая колонка для сортировки
                    _sortedBy = column.Name;
                    _sortAscending = true;
                }

                // Обновляем заголовки
                UpdateSortHeaders();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            try
            {
                // Если список продуктов пуст, загружаем его
                if (_allProducts == null || _allProducts.Count == 0)
                {
                    LoadProducts();
                    return;
                }

                // Начинаем со всех продуктов
                List<Product> filteredProducts = _allProducts;

                // 1. Текстовый поиск (если есть текст и это не placeholder)
                if (!string.IsNullOrWhiteSpace(txtSearch.Text) &&
                    txtSearch.Text != "Введите артикул или наименование..." &&
                    txtSearch.ForeColor != Color.Gray)
                {
                    string search = txtSearch.Text.Trim().ToLower();
                    filteredProducts = filteredProducts.Where(p =>
                        (p.Name != null && p.Name.ToLower().Contains(search)) ||          // ← ТОЛЬКО по НАИМЕНОВАНИЮ
                        (p.Article != null && p.Article.ToLower().Contains(search))       // ← ТОЛЬКО по АРТИКУЛУ
                                                                                          // Убрали поиск по описанию: (p.Description != null && p.Description.ToLower().Contains(search))
                    ).ToList();
                }

                // 2. Фильтрация по дате (ВСЕГДА применяется)
                DateTime fromDate = dtpDateFrom.Value.Date;
                DateTime toDate = dtpDateTo.Value.Date;

                filteredProducts = filteredProducts
                    .Where(p => p.CreatedAt.Date >= fromDate && p.CreatedAt.Date <= toDate)
                    .ToList();

                // 3. Сортировка
                filteredProducts = SortProducts(filteredProducts);

                // 4. Отображение
                DisplayProducts(filteredProducts);

                // 5. Автовыделение первой строки, если есть результаты
                if (filteredProducts.Count > 0 && dgvProducts.Rows.Count > 0)
                {
                    if (dgvProducts.SelectedRows.Count == 0 ||
                        dgvProducts.SelectedRows[0].Cells["colId"].Value?.ToString() == "")
                    {
                        dgvProducts.Rows[0].Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Игнорируем ошибки при быстрой фильтрации
                Console.WriteLine($"Фильтрация: {ex.Message}");
            }
        }

        private List<Product> SortProducts(List<Product> products)
        {
            if (string.IsNullOrEmpty(_sortedBy))
                return products;

            if (_sortedBy == "colName")
            {
                return _sortAscending
                    ? products.OrderBy(p => p.Name).ToList()
                    : products.OrderByDescending(p => p.Name).ToList();
            }
            else if (_sortedBy == "colCreatedAt")
            {
                return _sortAscending
                    ? products.OrderBy(p => p.CreatedAt).ToList()
                    : products.OrderByDescending(p => p.CreatedAt).ToList();
            }

            return products;
        }

        private void UpdateSortHeaders()
        {
            // Сбрасываем все заголовки
            dgvProducts.Columns["colName"].HeaderText = "Наименование";
            dgvProducts.Columns["colCreatedAt"].HeaderText = "Дата создания";

            // Добавляем стрелку к активной колонке
            if (!string.IsNullOrEmpty(_sortedBy))
            {
                string arrow = _sortAscending ? " ▲" : " ▼";

                if (_sortedBy == "colName")
                    dgvProducts.Columns["colName"].HeaderText = "Наименование" + arrow;
                else if (_sortedBy == "colCreatedAt")
                    dgvProducts.Columns["colCreatedAt"].HeaderText = "Дата создания" + arrow;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "Введите артикул или наименование...";
            txtSearch.ForeColor = Color.Gray;

            // Сбрасываем даты на последний месяц
            dtpDateFrom.Value = DateTime.Today.AddMonths(-1);
            dtpDateTo.Value = DateTime.Today;

            // Сбрасываем сортировку
            _sortedBy = "";
            _sortAscending = true;
            UpdateSortHeaders();

            // Обновляем фильтры
            ApplyFilters();
        }

        private void dgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Проверяем, что это не строка "Нет данных"
                if (dgvProducts.Rows[e.RowIndex].Cells["colId"].Value?.ToString() != "")
                {
                    btnDetails_Click(sender, e);
                }
            }
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Введите артикул или наименование...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Введите артикул или наименование...";
                txtSearch.ForeColor = Color.Gray;
            }
        }
    }
}
