using MyLibrary;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MainForm
{
    public partial class ProductDetailForm : Form
    {
        private ProductComposition _composition;
        public ProductDetailForm(ProductComposition composition)
        {
            _composition = composition;
            InitializeComponent();
            LoadComposition();
        }

        private void LoadComposition()
        {
            int totalComponents = _composition.Components?.Sum(c => c.Quantity) ?? 0;
            int componentTypesCount = _composition.Components?.Count ?? 0;
            lblProductInfo.Text = $"📋 Изделие: {_composition.Product.Name} ({_composition.Product.Article})\n" +
                         $"📦 Всего комплектующих: {totalComponents} шт. ({componentTypesCount} позиций)";

            DisplayComposition();
        }

        private void DisplayComposition()
        {
            dgvComponents.Rows.Clear();

            if (_composition.Components != null && _composition.Components.Any())
            {
                foreach (var item in _composition.Components.OrderBy(c => c.Component.Name))
                {
                    dgvComponents.Rows.Add(
                        item.Component.Article,
                        item.Component.Name,
                        $"{item.Quantity} шт.",
                        item.Component.Description ?? ""
                    );
                }
            }
            else
            {
                dgvComponents.Rows.Add("", "Состав не задан", "", "");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
