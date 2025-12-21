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

        public MainForm(ProductService productService, ComponentService componentService)
        {
            _productService = productService;
            _componentService = componentService;

            InitializeComponent();
            LoadProducts();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadProducts();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchProducts();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchProducts();
                e.Handled = true;
            }
        }

        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            UpdateProductInfo();
        }

        private void dgvProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnDetails_Click(sender, e);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = _productService.GetAllProducts();
                DisplayProducts(products);
                ClearProductInfo();
                txtSearch.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изделий: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchProducts()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadProducts();
                }
                else
                {
                    var products = _productService.SearchProducts(txtSearch.Text);
                    DisplayProducts(products);

                    if (products.Count == 0)
                    {
                        ClearProductInfo();
                        txtInfo.Text = "По вашему запросу ничего не найдено.";
                        txtInfo.ForeColor = Color.FromArgb(200, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayProducts(List<Product> products)
        {
            dgvProducts.Rows.Clear();

            if (products.Count == 0)
            {
                dgvProducts.Rows.Add("", "Нет данных", "Список изделий пуст", "", "");
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
            info.AppendLine($"🏷️  Артикул: {composition.Product.Article}");

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
    }
}
