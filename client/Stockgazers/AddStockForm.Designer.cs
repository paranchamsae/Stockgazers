namespace Stockgazers
{
    partial class AddStockForm
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
            materialTextBoxEdit1 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialButton1 = new ReaLTaiizor.Controls.MaterialButton();
            materialButton2 = new ReaLTaiizor.Controls.MaterialButton();
            materialListView1 = new ReaLTaiizor.Controls.MaterialListView();
            columnHeader1 = new ColumnHeader();
            materialTextBoxEdit2 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialTextBoxEdit3 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialTextBoxEdit4 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialTextBoxEdit5 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialListBox1 = new ReaLTaiizor.Controls.MaterialListBox();
            SuspendLayout();
            // 
            // materialTextBoxEdit1
            // 
            materialTextBoxEdit1.AnimateReadOnly = false;
            materialTextBoxEdit1.AutoCompleteMode = AutoCompleteMode.None;
            materialTextBoxEdit1.AutoCompleteSource = AutoCompleteSource.None;
            materialTextBoxEdit1.BackgroundImageLayout = ImageLayout.None;
            materialTextBoxEdit1.CharacterCasing = CharacterCasing.Normal;
            materialTextBoxEdit1.Depth = 0;
            materialTextBoxEdit1.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBoxEdit1.HelperText = "Model";
            materialTextBoxEdit1.HideSelection = true;
            materialTextBoxEdit1.Hint = "모델명으로 검색";
            materialTextBoxEdit1.LeadingIcon = Properties.Resources.icons8_수색_24;
            materialTextBoxEdit1.Location = new Point(8, 85);
            materialTextBoxEdit1.Margin = new Padding(4);
            materialTextBoxEdit1.MaxLength = 32767;
            materialTextBoxEdit1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialTextBoxEdit1.Name = "materialTextBoxEdit1";
            materialTextBoxEdit1.PasswordChar = '\0';
            materialTextBoxEdit1.PrefixSuffixText = null;
            materialTextBoxEdit1.ReadOnly = false;
            materialTextBoxEdit1.RightToLeft = RightToLeft.No;
            materialTextBoxEdit1.SelectedText = "";
            materialTextBoxEdit1.SelectionLength = 0;
            materialTextBoxEdit1.SelectionStart = 0;
            materialTextBoxEdit1.ShortcutsEnabled = true;
            materialTextBoxEdit1.Size = new Size(429, 48);
            materialTextBoxEdit1.TabIndex = 0;
            materialTextBoxEdit1.TabStop = false;
            materialTextBoxEdit1.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit1.TrailingIcon = null;
            materialTextBoxEdit1.UseSystemPasswordChar = false;
            materialTextBoxEdit1.KeyDown += materialTextBoxEdit1_KeyDown;
            // 
            // materialButton1
            // 
            materialButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            materialButton1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton1.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton1.Depth = 0;
            materialButton1.HighEmphasis = true;
            materialButton1.Icon = null;
            materialButton1.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            materialButton1.Location = new Point(299, 632);
            materialButton1.Margin = new Padding(5, 8, 5, 8);
            materialButton1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialButton1.Name = "materialButton1";
            materialButton1.NoAccentTextColor = Color.Empty;
            materialButton1.Size = new Size(64, 36);
            materialButton1.TabIndex = 1;
            materialButton1.Text = "등록";
            materialButton1.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton1.UseAccentColor = false;
            materialButton1.UseVisualStyleBackColor = true;
            materialButton1.Click += materialButton1_Click;
            // 
            // materialButton2
            // 
            materialButton2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            materialButton2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton2.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton2.Depth = 0;
            materialButton2.HighEmphasis = true;
            materialButton2.Icon = null;
            materialButton2.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            materialButton2.Location = new Point(373, 632);
            materialButton2.Margin = new Padding(5, 8, 5, 8);
            materialButton2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialButton2.Name = "materialButton2";
            materialButton2.NoAccentTextColor = Color.Empty;
            materialButton2.Size = new Size(64, 36);
            materialButton2.TabIndex = 2;
            materialButton2.Text = "취소";
            materialButton2.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton2.UseAccentColor = false;
            materialButton2.UseVisualStyleBackColor = true;
            materialButton2.Click += materialButton2_Click;
            // 
            // materialListView1
            // 
            materialListView1.AutoSizeTable = false;
            materialListView1.BackColor = Color.FromArgb(255, 255, 255);
            materialListView1.BorderStyle = BorderStyle.None;
            materialListView1.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
            materialListView1.Depth = 0;
            materialListView1.FullRowSelect = true;
            materialListView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            materialListView1.Location = new Point(8, 140);
            materialListView1.MinimumSize = new Size(200, 100);
            materialListView1.MouseLocation = new Point(-1, -1);
            materialListView1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialListView1.Name = "materialListView1";
            materialListView1.OwnerDraw = true;
            materialListView1.Size = new Size(429, 147);
            materialListView1.TabIndex = 3;
            materialListView1.UseCompatibleStateImageBehavior = false;
            materialListView1.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "검색결과";
            columnHeader1.Width = 429;
            // 
            // materialTextBoxEdit2
            // 
            materialTextBoxEdit2.AnimateReadOnly = false;
            materialTextBoxEdit2.AutoCompleteMode = AutoCompleteMode.None;
            materialTextBoxEdit2.AutoCompleteSource = AutoCompleteSource.None;
            materialTextBoxEdit2.BackgroundImageLayout = ImageLayout.None;
            materialTextBoxEdit2.CharacterCasing = CharacterCasing.Normal;
            materialTextBoxEdit2.Depth = 0;
            materialTextBoxEdit2.Enabled = false;
            materialTextBoxEdit2.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBoxEdit2.HideSelection = true;
            materialTextBoxEdit2.Hint = "Model";
            materialTextBoxEdit2.LeadingIcon = Properties.Resources.icons8_수색_24;
            materialTextBoxEdit2.Location = new Point(8, 294);
            materialTextBoxEdit2.Margin = new Padding(4);
            materialTextBoxEdit2.MaxLength = 32767;
            materialTextBoxEdit2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialTextBoxEdit2.Name = "materialTextBoxEdit2";
            materialTextBoxEdit2.PasswordChar = '\0';
            materialTextBoxEdit2.PrefixSuffixText = null;
            materialTextBoxEdit2.ReadOnly = false;
            materialTextBoxEdit2.RightToLeft = RightToLeft.No;
            materialTextBoxEdit2.SelectedText = "";
            materialTextBoxEdit2.SelectionLength = 0;
            materialTextBoxEdit2.SelectionStart = 0;
            materialTextBoxEdit2.ShortcutsEnabled = true;
            materialTextBoxEdit2.Size = new Size(429, 48);
            materialTextBoxEdit2.TabIndex = 4;
            materialTextBoxEdit2.TabStop = false;
            materialTextBoxEdit2.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit2.TrailingIcon = null;
            materialTextBoxEdit2.UseSystemPasswordChar = false;
            // 
            // materialTextBoxEdit3
            // 
            materialTextBoxEdit3.AnimateReadOnly = false;
            materialTextBoxEdit3.AutoCompleteMode = AutoCompleteMode.None;
            materialTextBoxEdit3.AutoCompleteSource = AutoCompleteSource.None;
            materialTextBoxEdit3.BackgroundImageLayout = ImageLayout.None;
            materialTextBoxEdit3.CharacterCasing = CharacterCasing.Normal;
            materialTextBoxEdit3.Depth = 0;
            materialTextBoxEdit3.Enabled = false;
            materialTextBoxEdit3.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBoxEdit3.HideSelection = true;
            materialTextBoxEdit3.Hint = "Name";
            materialTextBoxEdit3.LeadingIcon = Properties.Resources.icons8_수색_24;
            materialTextBoxEdit3.Location = new Point(8, 360);
            materialTextBoxEdit3.Margin = new Padding(4);
            materialTextBoxEdit3.MaxLength = 32767;
            materialTextBoxEdit3.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialTextBoxEdit3.Name = "materialTextBoxEdit3";
            materialTextBoxEdit3.PasswordChar = '\0';
            materialTextBoxEdit3.PrefixSuffixText = null;
            materialTextBoxEdit3.ReadOnly = false;
            materialTextBoxEdit3.RightToLeft = RightToLeft.No;
            materialTextBoxEdit3.SelectedText = "";
            materialTextBoxEdit3.SelectionLength = 0;
            materialTextBoxEdit3.SelectionStart = 0;
            materialTextBoxEdit3.ShortcutsEnabled = true;
            materialTextBoxEdit3.Size = new Size(429, 48);
            materialTextBoxEdit3.TabIndex = 5;
            materialTextBoxEdit3.TabStop = false;
            materialTextBoxEdit3.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit3.TrailingIcon = null;
            materialTextBoxEdit3.UseSystemPasswordChar = false;
            // 
            // materialTextBoxEdit4
            // 
            materialTextBoxEdit4.AnimateReadOnly = false;
            materialTextBoxEdit4.AutoCompleteMode = AutoCompleteMode.None;
            materialTextBoxEdit4.AutoCompleteSource = AutoCompleteSource.None;
            materialTextBoxEdit4.BackgroundImageLayout = ImageLayout.None;
            materialTextBoxEdit4.CharacterCasing = CharacterCasing.Normal;
            materialTextBoxEdit4.Depth = 0;
            materialTextBoxEdit4.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBoxEdit4.HideSelection = true;
            materialTextBoxEdit4.Hint = "BuyPrice(KRW)";
            materialTextBoxEdit4.LeadingIcon = Properties.Resources.icons8_수색_24;
            materialTextBoxEdit4.Location = new Point(8, 489);
            materialTextBoxEdit4.Margin = new Padding(4);
            materialTextBoxEdit4.MaxLength = 32767;
            materialTextBoxEdit4.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialTextBoxEdit4.Name = "materialTextBoxEdit4";
            materialTextBoxEdit4.PasswordChar = '\0';
            materialTextBoxEdit4.PrefixSuffixText = null;
            materialTextBoxEdit4.ReadOnly = false;
            materialTextBoxEdit4.RightToLeft = RightToLeft.No;
            materialTextBoxEdit4.SelectedText = "";
            materialTextBoxEdit4.SelectionLength = 0;
            materialTextBoxEdit4.SelectionStart = 0;
            materialTextBoxEdit4.ShortcutsEnabled = true;
            materialTextBoxEdit4.Size = new Size(429, 48);
            materialTextBoxEdit4.TabIndex = 6;
            materialTextBoxEdit4.TabStop = false;
            materialTextBoxEdit4.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit4.TrailingIcon = null;
            materialTextBoxEdit4.UseSystemPasswordChar = false;
            // 
            // materialTextBoxEdit5
            // 
            materialTextBoxEdit5.AnimateReadOnly = false;
            materialTextBoxEdit5.AutoCompleteMode = AutoCompleteMode.None;
            materialTextBoxEdit5.AutoCompleteSource = AutoCompleteSource.None;
            materialTextBoxEdit5.BackgroundImageLayout = ImageLayout.None;
            materialTextBoxEdit5.CharacterCasing = CharacterCasing.Normal;
            materialTextBoxEdit5.Depth = 0;
            materialTextBoxEdit5.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBoxEdit5.HideSelection = true;
            materialTextBoxEdit5.Hint = "Limit(USD)";
            materialTextBoxEdit5.LeadingIcon = Properties.Resources.icons8_수색_24;
            materialTextBoxEdit5.Location = new Point(8, 555);
            materialTextBoxEdit5.Margin = new Padding(4);
            materialTextBoxEdit5.MaxLength = 32767;
            materialTextBoxEdit5.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialTextBoxEdit5.Name = "materialTextBoxEdit5";
            materialTextBoxEdit5.PasswordChar = '\0';
            materialTextBoxEdit5.PrefixSuffixText = null;
            materialTextBoxEdit5.ReadOnly = false;
            materialTextBoxEdit5.RightToLeft = RightToLeft.No;
            materialTextBoxEdit5.SelectedText = "";
            materialTextBoxEdit5.SelectionLength = 0;
            materialTextBoxEdit5.SelectionStart = 0;
            materialTextBoxEdit5.ShortcutsEnabled = true;
            materialTextBoxEdit5.Size = new Size(429, 48);
            materialTextBoxEdit5.TabIndex = 7;
            materialTextBoxEdit5.TabStop = false;
            materialTextBoxEdit5.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit5.TrailingIcon = null;
            materialTextBoxEdit5.UseSystemPasswordChar = false;
            // 
            // materialListBox1
            // 
            materialListBox1.BackColor = Color.White;
            materialListBox1.BorderColor = Color.LightGray;
            materialListBox1.Depth = 0;
            materialListBox1.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialListBox1.Location = new Point(8, 424);
            materialListBox1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialListBox1.Name = "materialListBox1";
            materialListBox1.SelectedIndex = -1;
            materialListBox1.SelectedItem = null;
            materialListBox1.Size = new Size(429, 48);
            materialListBox1.TabIndex = 8;
            // 
            // AddStockForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(445, 680);
            Controls.Add(materialListBox1);
            Controls.Add(materialTextBoxEdit5);
            Controls.Add(materialTextBoxEdit4);
            Controls.Add(materialTextBoxEdit3);
            Controls.Add(materialTextBoxEdit2);
            Controls.Add(materialListView1);
            Controls.Add(materialButton2);
            Controls.Add(materialButton1);
            Controls.Add(materialTextBoxEdit1);
            Font = new Font("Roboto", 12F, FontStyle.Bold, GraphicsUnit.Point);
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddStockForm";
            Padding = new Padding(4, 81, 4, 4);
            StartPosition = FormStartPosition.CenterParent;
            Text = "재고추가";
            MouseDoubleClick += materialListView1_MouseDoubleClick;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit1;
        private ReaLTaiizor.Controls.MaterialButton materialButton1;
        private ReaLTaiizor.Controls.MaterialButton materialButton2;
        private ReaLTaiizor.Controls.MaterialListView materialListView1;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit2;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit3;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit4;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit5;
        private ReaLTaiizor.Controls.MaterialListBox materialListBox1;
        private ColumnHeader columnHeader1;
    }
}