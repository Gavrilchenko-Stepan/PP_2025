using MyLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MainForm
{
    public partial class MainForm : Form
    {
        private ProductService _productService;
        private ComponentService _componentService;
        private List<Product> _allProducts;

        public MainForm(ProductService productService, ComponentService componentService)
        {
            _productService = productService;
            _componentService = componentService;

            InitializeComponent();

            txtSearch.TextChanged += (s, e) => ApplyFilters();

            LoadProducts();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            Product product = GetSelectedProduct();
            if (product == null)
            {
                MessageBox.Show("Выберите изделие для просмотра состава", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ProductComposition composition = _productService.GetProductComposition(product.Id);
                if (composition != null)
                {
                    using (ProductDetailForm form = new ProductDetailForm(composition))
                    {
                        form.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки состава: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWhereUsed_Click(object sender, EventArgs e)
        {
            using (var searchForm = new ComponentSearchForm(_componentService))
            {
                if (searchForm.ShowDialog() == DialogResult.OK && searchForm.SelectedComponent != null)
                {
                    Component component = searchForm.SelectedComponent;

                    try
                    {
                        List<ProductComposition> compositions = _productService.GetWhereComponentUsed(component.Id);

                        if (compositions.Count > 0)
                        {
                            using (WhereUsedForm resultsForm = new WhereUsedForm(component, compositions))
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

            foreach (Product product in products)
            {
                dgvProducts.Rows.Add(product.Id,product.Article, product.Name, product.Description ?? "", product.CreatedAt);
            }
        }

        private void UpdateProductInfo()
        {
            try
            {
                Product product = GetSelectedProduct();
                if (product != null)
                {
                    ProductComposition composition = _productService.GetProductComposition(product.Id);
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

            int componentTypesCount = GetComponentTypesCount(composition.Components);
            int totalComponents = GetTotalComponents(composition.Components);

            info.AppendLine($"🔢 Позиций в составе: {componentTypesCount}");
            info.AppendLine($"📦 Всего комплектующих: {totalComponents}");
            info.AppendLine();
            info.AppendLine("🔩 СОСТАВ:");

            if (composition.Components != null && composition.Components.Any())
            {
                foreach (var item in composition.Components.OrderByDescending(c => c.Quantity))
                {
                    info.AppendLine($"  • {item.Component.Name} — {item.Quantity} шт.");
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

                // 1. Текстовый поиск
                if (!string.IsNullOrWhiteSpace(txtSearch.Text) &&
                    txtSearch.Text != "Введите артикул или наименование..." &&
                    txtSearch.ForeColor != Color.Gray)
                {
                    string search = txtSearch.Text.Trim().ToLower();
                    filteredProducts = filteredProducts.Where(p =>
                        (p.Name != null && p.Name.ToLower().Contains(search)) ||
                        (p.Article != null && p.Article.ToLower().Contains(search))
                    ).ToList();
                }

                // 2. Отображение
                DisplayProducts(filteredProducts);

                // 3. Автовыделение первой строки
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
                Console.WriteLine($"Фильтрация: {ex.Message}");
            }
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

        private int GetTotalComponents(List<CompositionItem> components)
        {
            return components?.Sum(c => c.Quantity) ?? 0;
        }

        private int GetComponentTypesCount(List<CompositionItem> components)
        {
            return components?.Count ?? 0;
        }
    }
}
