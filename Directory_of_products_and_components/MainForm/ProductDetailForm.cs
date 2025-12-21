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
    public partial class ProductDetailForm : Form
    {
        private ProductService _productService;
        private ComponentService _componentService;

        private ProductComposition _composition;
        public ProductDetailForm(ProductComposition composition,
            ProductService productService,
            ComponentService componentService)
        {
            _composition = composition;
            _productService = productService;
            _componentService = componentService;
            InitializeComponent();
            LoadComposition();
        }

        private void LoadComposition()
        {
            lblProductInfo.Text = $"📋 Изделие: {_composition.Product.Name} ({_composition.Product.Article})\n" +
                                 $"📦 Всего комплектующих: {_composition.TotalComponents} шт. ({_composition.ComponentTypesCount} позиций)";

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
    }
}
