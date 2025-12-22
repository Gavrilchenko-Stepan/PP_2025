namespace MainForm
{
    partial class ProductDetailForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblProductInfo = new System.Windows.Forms.Label();
            this.dgvComponents = new System.Windows.Forms.DataGridView();
            this.colCompArticle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCompName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCompQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCompDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComponents)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProductInfo
            // 
            this.lblProductInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.lblProductInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblProductInfo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblProductInfo.Location = new System.Drawing.Point(15, 15);
            this.lblProductInfo.Name = "lblProductInfo";
            this.lblProductInfo.Padding = new System.Windows.Forms.Padding(10);
            this.lblProductInfo.Size = new System.Drawing.Size(650, 60);
            this.lblProductInfo.TabIndex = 0;
            this.lblProductInfo.Text = "📋 Изделие: [Название изделия]\\n📦 Всего комплектующих: [Количество] шт. ([Позици" +
    "й] позиций)";
            // 
            // dgvComponents
            // 
            this.dgvComponents.AllowUserToAddRows = false;
            this.dgvComponents.AllowUserToDeleteRows = false;
            this.dgvComponents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvComponents.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvComponents.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvComponents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvComponents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCompArticle,
            this.colCompName,
            this.colCompQuantity,
            this.colCompDescription});
            this.dgvComponents.Location = new System.Drawing.Point(15, 90);
            this.dgvComponents.Name = "dgvComponents";
            this.dgvComponents.ReadOnly = true;
            this.dgvComponents.RowHeadersVisible = false;
            this.dgvComponents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvComponents.Size = new System.Drawing.Size(650, 280);
            this.dgvComponents.TabIndex = 1;
            // 
            // colCompArticle
            // 
            this.colCompArticle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colCompArticle.HeaderText = "Артикул";
            this.colCompArticle.Name = "colCompArticle";
            this.colCompArticle.ReadOnly = true;
            this.colCompArticle.Width = 79;
            // 
            // colCompName
            // 
            this.colCompName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCompName.FillWeight = 118.2796F;
            this.colCompName.HeaderText = "Наименование";
            this.colCompName.Name = "colCompName";
            this.colCompName.ReadOnly = true;
            // 
            // colCompQuantity
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colCompQuantity.DefaultCellStyle = dataGridViewCellStyle2;
            this.colCompQuantity.FillWeight = 118.2796F;
            this.colCompQuantity.HeaderText = "Количество";
            this.colCompQuantity.MinimumWidth = 100;
            this.colCompQuantity.Name = "colCompQuantity";
            this.colCompQuantity.ReadOnly = true;
            // 
            // colCompDescription
            // 
            this.colCompDescription.FillWeight = 45.16129F;
            this.colCompDescription.HeaderText = "Описание";
            this.colCompDescription.MinimumWidth = 200;
            this.colCompDescription.Name = "colCompDescription";
            this.colCompDescription.ReadOnly = true;
            this.colCompDescription.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(585, 380);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 25);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ProductDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(684, 411);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgvComponents);
            this.Controls.Add(this.lblProductInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Состав изделия";
            ((System.ComponentModel.ISupportInitialize)(this.dgvComponents)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblProductInfo;
        private System.Windows.Forms.DataGridView dgvComponents;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCompArticle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCompName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCompQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCompDescription;
        private System.Windows.Forms.Button btnClose;
    }
}