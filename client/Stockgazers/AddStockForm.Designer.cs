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
            materialTextBoxEdit2 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialTextBoxEdit3 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialTextBoxEdit4 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialTextBoxEdit5 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            listView1 = new ListView();
            materialComboBox1 = new ReaLTaiizor.Controls.MaterialComboBox();
            materialTextBoxEdit6 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
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
            materialButton1.Location = new Point(289, 694);
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
            materialButton2.Location = new Point(373, 694);
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
            materialTextBoxEdit5.Location = new Point(221, 623);
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
            materialTextBoxEdit5.Size = new Size(216, 48);
            materialTextBoxEdit5.TabIndex = 7;
            materialTextBoxEdit5.TabStop = false;
            materialTextBoxEdit5.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit5.TrailingIcon = null;
            materialTextBoxEdit5.UseSystemPasswordChar = false;
            // 
            // listView1
            // 
            listView1.Location = new Point(8, 140);
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.Size = new Size(429, 147);
            listView1.TabIndex = 9;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.List;
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
            // 
            // materialComboBox1
            // 
            materialComboBox1.AutoResize = false;
            materialComboBox1.BackColor = Color.FromArgb(255, 255, 255);
            materialComboBox1.Depth = 0;
            materialComboBox1.DrawMode = DrawMode.OwnerDrawVariable;
            materialComboBox1.DropDownHeight = 174;
            materialComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            materialComboBox1.DropDownWidth = 121;
            materialComboBox1.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            materialComboBox1.ForeColor = Color.FromArgb(222, 0, 0, 0);
            materialComboBox1.FormattingEnabled = true;
            materialComboBox1.Hint = "Size";
            materialComboBox1.IntegralHeight = false;
            materialComboBox1.ItemHeight = 43;
            materialComboBox1.Location = new Point(8, 425);
            materialComboBox1.MaxDropDownItems = 4;
            materialComboBox1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialComboBox1.Name = "materialComboBox1";
            materialComboBox1.Size = new Size(429, 49);
            materialComboBox1.StartIndex = 0;
            materialComboBox1.TabIndex = 10;
            materialComboBox1.SelectedIndexChanged += materialComboBox1_SelectedIndexChanged;
            // 
            // materialTextBoxEdit6
            // 
            materialTextBoxEdit6.AnimateReadOnly = false;
            materialTextBoxEdit6.AutoCompleteMode = AutoCompleteMode.None;
            materialTextBoxEdit6.AutoCompleteSource = AutoCompleteSource.None;
            materialTextBoxEdit6.BackgroundImageLayout = ImageLayout.None;
            materialTextBoxEdit6.CharacterCasing = CharacterCasing.Normal;
            materialTextBoxEdit6.Depth = 0;
            materialTextBoxEdit6.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBoxEdit6.HideSelection = true;
            materialTextBoxEdit6.Hint = "Price";
            materialTextBoxEdit6.LeadingIcon = Properties.Resources.icons8_수색_24;
            materialTextBoxEdit6.Location = new Point(8, 623);
            materialTextBoxEdit6.Margin = new Padding(4);
            materialTextBoxEdit6.MaxLength = 32767;
            materialTextBoxEdit6.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialTextBoxEdit6.Name = "materialTextBoxEdit6";
            materialTextBoxEdit6.PasswordChar = '\0';
            materialTextBoxEdit6.PrefixSuffixText = null;
            materialTextBoxEdit6.ReadOnly = false;
            materialTextBoxEdit6.RightToLeft = RightToLeft.No;
            materialTextBoxEdit6.SelectedText = "";
            materialTextBoxEdit6.SelectionLength = 0;
            materialTextBoxEdit6.SelectionStart = 0;
            materialTextBoxEdit6.ShortcutsEnabled = true;
            materialTextBoxEdit6.Size = new Size(205, 48);
            materialTextBoxEdit6.TabIndex = 11;
            materialTextBoxEdit6.TabStop = false;
            materialTextBoxEdit6.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit6.TrailingIcon = null;
            materialTextBoxEdit6.UseSystemPasswordChar = false;
            // 
            // button1
            // 
            button1.Location = new Point(8, 554);
            button1.Name = "button1";
            button1.Size = new Size(140, 55);
            button1.TabIndex = 12;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(154, 554);
            button2.Name = "button2";
            button2.Size = new Size(140, 55);
            button2.TabIndex = 13;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(297, 554);
            button3.Name = "button3";
            button3.Size = new Size(140, 55);
            button3.TabIndex = 14;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // AddStockForm
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(445, 742);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(materialTextBoxEdit6);
            Controls.Add(materialComboBox1);
            Controls.Add(listView1);
            Controls.Add(materialTextBoxEdit5);
            Controls.Add(materialTextBoxEdit4);
            Controls.Add(materialTextBoxEdit3);
            Controls.Add(materialTextBoxEdit2);
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit1;
        private ReaLTaiizor.Controls.MaterialButton materialButton1;
        private ReaLTaiizor.Controls.MaterialButton materialButton2;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit2;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit3;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit4;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit5;
        private ListView listView1;
        private ReaLTaiizor.Controls.MaterialComboBox materialComboBox1;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit6;
        private Button button1;
        private Button button2;
        private Button button3;
    }
}