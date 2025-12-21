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
    public partial class WhereUsedDetailForm : Form
    {
        private readonly ProductComposition _composition;
        private readonly Component _targetComponent;

        public WhereUsedDetailForm(ProductComposition composition, Component targetComponent)
        {
            _composition = composition;
            _targetComponent = targetComponent;

            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            // Информация об изделии
            lblProductInfo.Text = $"{_composition.Product.Name} ({_composition.Product.Article})";

            // Заполнение таблицы с подсветкой искомого компонента
            dgvComponents.Rows.Clear();
            foreach (var item in _composition.Components.OrderBy(c => c.Component.Name))
            {
                int rowIndex = dgvComponents.Rows.Add(
                    item.Component.Article,
                    item.Component.Name,
                    $"{item.Quantity} шт."
                );

                // Подсвечиваем искомый компонент
                if (item.Component.Id == _targetComponent.Id)
                {
                    var row = dgvComponents.Rows[rowIndex];
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 204); // Светло-желтый
                    row.DefaultCellStyle.Font = new Font(dgvComponents.Font, FontStyle.Bold);
                }
            }
        }
    }
}
