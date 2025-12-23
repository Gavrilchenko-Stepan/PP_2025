using MyLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Component = MyLibrary.Component;

namespace MainForm
{
    public partial class ComponentSearchForm : Form
    {
        private readonly ComponentService _componentService;
        private Component _selectedComponent;
        public Component SelectedComponent => _selectedComponent;

        public ComponentSearchForm(ComponentService componentService)
        {
            _componentService = componentService;
            InitializeComponent();
            LoadComponents();
        }

        private void LoadComponents()
        {
            try
            {
                var components = _componentService.GetAllComponents();
                DisplayComponents(components);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayComponents(List<Component> components)
        {
            dgvComponents.Rows.Clear();
            foreach (var component in components.OrderBy(c => c.Name))
            {
                dgvComponents.Rows.Add(
                    component.Id,
                    component.Article,
                    component.Name,
                    component.Description ?? ""
                );
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var results = _componentService.SearchComponents(txtSearch.Text);
                DisplayComponents(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvComponents_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectComponent();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch_Click(sender, e);
                e.Handled = true;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvComponents.SelectedRows.Count > 0)
            {
                SelectComponent();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Выберите комплектующее", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SelectComponent()
        {
            if (dgvComponents.SelectedRows.Count > 0)
            {
                var row = dgvComponents.SelectedRows[0];
                _selectedComponent = new Component
                {
                    Id = Convert.ToInt32(row.Cells["colId"].Value),
                    Article = row.Cells["colArticle"].Value?.ToString() ?? "",
                    Name = row.Cells["colName"].Value?.ToString() ?? "",
                    Description = row.Cells["colDescription"].Value?.ToString() ?? ""
                };
            }
        }
    }
}
