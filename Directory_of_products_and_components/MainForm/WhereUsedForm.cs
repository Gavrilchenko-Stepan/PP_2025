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
using Component = MyLibrary.Component;

namespace MainForm
{
    public partial class WhereUsedForm : Form
    {
        private readonly Component _component;
        private readonly List<ProductComposition> _compositions;

        public WhereUsedForm(Component component, List<ProductComposition> compositions)
        {
            _component = component ?? throw new ArgumentNullException(nameof(component));
            _compositions = compositions ?? new List<ProductComposition>();

            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            // Информация о компоненте
            lblComponentInfo.Text = $"{_component.Name} ({_component.Article})";

            if (!string.IsNullOrEmpty(_component.Description))
                lblComponentInfo.Text += $"\n{_component.Description}";

            // Количество результатов
            lblResultsCount.Text = $"Найдено изделий: {_compositions.Count}";

            // Заполнение таблицы
            dgvProducts.Rows.Clear();
            foreach (var composition in _compositions.OrderBy(c => c.Product.Name))
            {
                var componentInProduct = composition.Components
                    .FirstOrDefault(c => c.Component.Id == _component.Id);
                var quantity = componentInProduct?.Quantity ?? 0;

                dgvProducts.Rows.Add(
                    composition.Product.Id,
                    composition.Product.Article,
                    composition.Product.Name,
                    $"{quantity} шт."
                );
            }
        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            ViewSelectedProduct();
        }

        private void dgvProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ViewSelectedProduct();
            }
        }

        private void ViewSelectedProduct()
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var row = dgvProducts.SelectedRows[0];
                int productId = Convert.ToInt32(row.Cells["colProductId"].Value);

                var composition = _compositions.FirstOrDefault(c => c.Product.Id == productId);
                if (composition != null)
                {
                    ShowProductDetail(composition);
                }
            }
        }

        private void ShowProductDetail(ProductComposition composition)
        {
            try
            {
                // Правильный вызов формы WhereUsedDetailForm
                using (var detailForm = new WhereUsedDetailForm(composition, _component))
                {
                    detailForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии состава: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
