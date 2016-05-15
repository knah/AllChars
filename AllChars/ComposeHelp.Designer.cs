namespace AllCharsNS
{
    partial class ComposeHelp
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.categoryList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.composeGrid = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.fontChooser = new System.Windows.Forms.ComboBox();
            this.characterColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sequenceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.composeGrid)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // categoryList
            // 
            this.categoryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoryList.FormattingEnabled = true;
            this.categoryList.Location = new System.Drawing.Point(8, 32);
            this.categoryList.Margin = new System.Windows.Forms.Padding(8, 3, 3, 3);
            this.categoryList.Name = "categoryList";
            this.categoryList.Size = new System.Drawing.Size(142, 391);
            this.categoryList.TabIndex = 0;
            this.categoryList.SelectedIndexChanged += new System.EventHandler(this.categoryList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(8, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 11, 3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Category:";
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.closeButton.Location = new System.Drawing.Point(548, 429);
            this.closeButton.Margin = new System.Windows.Forms.Padding(3, 3, 8, 8);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(78, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // composeGrid
            // 
            this.composeGrid.AllowUserToAddRows = false;
            this.composeGrid.AllowUserToDeleteRows = false;
            this.composeGrid.AllowUserToResizeRows = false;
            this.composeGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.composeGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.characterColumn,
            this.sequenceColumn,
            this.nameColumn});
            this.tableLayoutPanel1.SetColumnSpan(this.composeGrid, 3);
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.composeGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.composeGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.composeGrid.Location = new System.Drawing.Point(156, 32);
            this.composeGrid.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
            this.composeGrid.MultiSelect = false;
            this.composeGrid.Name = "composeGrid";
            this.composeGrid.ReadOnly = true;
            this.composeGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.composeGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.composeGrid.Size = new System.Drawing.Size(470, 391);
            this.composeGrid.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.closeButton, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.composeGrid, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.categoryList, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.fontChooser, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(634, 460);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(153, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Font:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fontChooser
            // 
            this.fontChooser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontChooser.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.fontChooser.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.fontChooser.DisplayMember = "Name";
            this.fontChooser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontChooser.FormattingEnabled = true;
            this.fontChooser.Location = new System.Drawing.Point(191, 5);
            this.fontChooser.Name = "fontChooser";
            this.fontChooser.Size = new System.Drawing.Size(351, 21);
            this.fontChooser.TabIndex = 5;
            this.fontChooser.SelectedIndexChanged += new System.EventHandler(this.fontChooser_SelectedIndexChanged);
            // 
            // characterColumn
            // 
            this.characterColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.characterColumn.DataPropertyName = "Character";
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.characterColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.characterColumn.FillWeight = 10F;
            this.characterColumn.HeaderText = "Character";
            this.characterColumn.Name = "characterColumn";
            this.characterColumn.ReadOnly = true;
            this.characterColumn.Width = 78;
            // 
            // sequenceColumn
            // 
            this.sequenceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.sequenceColumn.DataPropertyName = "FancySequence";
            this.sequenceColumn.FillWeight = 20F;
            this.sequenceColumn.HeaderText = "Sequence";
            this.sequenceColumn.Name = "sequenceColumn";
            this.sequenceColumn.ReadOnly = true;
            this.sequenceColumn.Width = 81;
            // 
            // nameColumn
            // 
            this.nameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.nameColumn.DataPropertyName = "Name";
            this.nameColumn.HeaderText = "Name";
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.ReadOnly = true;
            this.nameColumn.Width = 60;
            // 
            // ComposeHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 460);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ComposeHelp";
            this.Text = "Compose Sequences";
            this.Load += new System.EventHandler(this.ComposeHelp_Load);
            ((System.ComponentModel.ISupportInitialize)(this.composeGrid)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox categoryList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.DataGridView composeGrid;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox fontChooser;
        private System.Windows.Forms.DataGridViewTextBoxColumn characterColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sequenceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
    }
}