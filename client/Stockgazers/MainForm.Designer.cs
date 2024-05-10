namespace Stockgazers
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "", "1", "2", "3", "4", "5" }, -1);
            materialTabControl1 = new ReaLTaiizor.Controls.MaterialTabControl();
            tabPage1 = new TabPage();
            parrotGauge1 = new ReaLTaiizor.Controls.ParrotGauge();
            tabPage2 = new TabPage();
            materialListView1 = new ReaLTaiizor.Controls.MaterialListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            tabPage3 = new TabPage();
            materialButton2 = new ReaLTaiizor.Controls.MaterialButton();
            materialButton1 = new ReaLTaiizor.Controls.MaterialButton();
            materialTextBoxEdit1 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialRadioButton2 = new ReaLTaiizor.Controls.MaterialRadioButton();
            materialRadioButton1 = new ReaLTaiizor.Controls.MaterialRadioButton();
            materialLabel3 = new ReaLTaiizor.Controls.MaterialLabel();
            materialLabel2 = new ReaLTaiizor.Controls.MaterialLabel();
            materialLabel1 = new ReaLTaiizor.Controls.MaterialLabel();
            imageList1 = new ImageList(components);
            materialTabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // materialTabControl1
            // 
            materialTabControl1.Controls.Add(tabPage1);
            materialTabControl1.Controls.Add(tabPage2);
            materialTabControl1.Controls.Add(tabPage3);
            materialTabControl1.Depth = 0;
            materialTabControl1.Dock = DockStyle.Fill;
            materialTabControl1.ImageList = imageList1;
            materialTabControl1.Location = new Point(3, 64);
            materialTabControl1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialTabControl1.Multiline = true;
            materialTabControl1.Name = "materialTabControl1";
            materialTabControl1.SelectedIndex = 0;
            materialTabControl1.Size = new Size(794, 383);
            materialTabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(parrotGauge1);
            tabPage1.ImageKey = "icons8-집-16.png";
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(786, 355);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Home";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // parrotGauge1
            // 
            parrotGauge1.BarColor = (List<Color>)resources.GetObject("parrotGauge1.BarColor");
            parrotGauge1.ConsumptionColor = Color.Black;
            parrotGauge1.DialColor = Color.Gray;
            parrotGauge1.DialThickness = 5;
            parrotGauge1.FilledColor = Color.FromArgb(0, 162, 250);
            parrotGauge1.GaugeStyle = ReaLTaiizor.Controls.ParrotGauge.Style.Material;
            parrotGauge1.Location = new Point(114, 80);
            parrotGauge1.Name = "parrotGauge1";
            parrotGauge1.Percentage = 35;
            parrotGauge1.Size = new Size(140, 70);
            parrotGauge1.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            parrotGauge1.TabIndex = 0;
            parrotGauge1.Text = "parrotGauge1";
            parrotGauge1.Thickness = 20;
            parrotGauge1.UnfilledColor = Color.Gray;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(materialListView1);
            tabPage2.ImageKey = "icons8-돈-가방-16.png";
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(786, 355);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "SellStatus";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // materialListView1
            // 
            materialListView1.AutoSizeTable = true;
            materialListView1.BackColor = Color.FromArgb(255, 255, 255);
            materialListView1.BorderStyle = BorderStyle.None;
            materialListView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5 });
            materialListView1.Depth = 0;
            materialListView1.Dock = DockStyle.Fill;
            materialListView1.FullRowSelect = true;
            materialListView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listViewItem1.StateImageIndex = 0;
            materialListView1.Items.AddRange(new ListViewItem[] { listViewItem1 });
            materialListView1.Location = new Point(3, 3);
            materialListView1.MinimumSize = new Size(200, 100);
            materialListView1.MouseLocation = new Point(-1, -1);
            materialListView1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialListView1.Name = "materialListView1";
            materialListView1.OwnerDraw = true;
            materialListView1.Scrollable = false;
            materialListView1.Size = new Size(300, 100);
            materialListView1.TabIndex = 0;
            materialListView1.UseCompatibleStateImageBehavior = false;
            materialListView1.View = View.Details;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(materialButton2);
            tabPage3.Controls.Add(materialButton1);
            tabPage3.Controls.Add(materialTextBoxEdit1);
            tabPage3.Controls.Add(materialRadioButton2);
            tabPage3.Controls.Add(materialRadioButton1);
            tabPage3.Controls.Add(materialLabel3);
            tabPage3.Controls.Add(materialLabel2);
            tabPage3.Controls.Add(materialLabel1);
            tabPage3.ImageKey = "icons8-설정-3-16.png";
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(786, 355);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Settings";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // materialButton2
            // 
            materialButton2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton2.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton2.Depth = 0;
            materialButton2.HighEmphasis = true;
            materialButton2.Icon = null;
            materialButton2.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            materialButton2.Location = new Point(624, 313);
            materialButton2.Margin = new Padding(4, 6, 4, 6);
            materialButton2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialButton2.Name = "materialButton2";
            materialButton2.NoAccentTextColor = Color.Empty;
            materialButton2.Size = new Size(158, 36);
            materialButton2.TabIndex = 7;
            materialButton2.Text = "materialButton2";
            materialButton2.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton2.UseAccentColor = false;
            materialButton2.UseVisualStyleBackColor = true;
            // 
            // materialButton1
            // 
            materialButton1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton1.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton1.Depth = 0;
            materialButton1.HighEmphasis = true;
            materialButton1.Icon = null;
            materialButton1.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            materialButton1.Location = new Point(458, 313);
            materialButton1.Margin = new Padding(4, 6, 4, 6);
            materialButton1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialButton1.Name = "materialButton1";
            materialButton1.NoAccentTextColor = Color.Empty;
            materialButton1.Size = new Size(158, 36);
            materialButton1.TabIndex = 6;
            materialButton1.Text = "materialButton1";
            materialButton1.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton1.UseAccentColor = false;
            materialButton1.UseVisualStyleBackColor = true;
            // 
            // materialTextBoxEdit1
            // 
            materialTextBoxEdit1.AnimateReadOnly = false;
            materialTextBoxEdit1.AutoCompleteMode = AutoCompleteMode.None;
            materialTextBoxEdit1.AutoCompleteSource = AutoCompleteSource.None;
            materialTextBoxEdit1.BackgroundImageLayout = ImageLayout.None;
            materialTextBoxEdit1.CharacterCasing = CharacterCasing.Normal;
            materialTextBoxEdit1.Depth = 0;
            materialTextBoxEdit1.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBoxEdit1.HideSelection = true;
            materialTextBoxEdit1.LeadingIcon = null;
            materialTextBoxEdit1.Location = new Point(305, 238);
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
            materialTextBoxEdit1.Size = new Size(250, 48);
            materialTextBoxEdit1.TabIndex = 5;
            materialTextBoxEdit1.TabStop = false;
            materialTextBoxEdit1.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit1.TrailingIcon = null;
            materialTextBoxEdit1.UseSystemPasswordChar = false;
            // 
            // materialRadioButton2
            // 
            materialRadioButton2.AutoSize = true;
            materialRadioButton2.Depth = 0;
            materialRadioButton2.Location = new Point(510, 133);
            materialRadioButton2.Margin = new Padding(0);
            materialRadioButton2.MouseLocation = new Point(-1, -1);
            materialRadioButton2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialRadioButton2.Name = "materialRadioButton2";
            materialRadioButton2.Ripple = true;
            materialRadioButton2.Size = new Size(190, 37);
            materialRadioButton2.TabIndex = 4;
            materialRadioButton2.TabStop = true;
            materialRadioButton2.Text = "materialRadioButton2";
            materialRadioButton2.UseAccentColor = false;
            materialRadioButton2.UseVisualStyleBackColor = true;
            // 
            // materialRadioButton1
            // 
            materialRadioButton1.AutoSize = true;
            materialRadioButton1.Checked = true;
            materialRadioButton1.Depth = 0;
            materialRadioButton1.Location = new Point(304, 133);
            materialRadioButton1.Margin = new Padding(0);
            materialRadioButton1.MouseLocation = new Point(-1, -1);
            materialRadioButton1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialRadioButton1.Name = "materialRadioButton1";
            materialRadioButton1.Ripple = true;
            materialRadioButton1.Size = new Size(190, 37);
            materialRadioButton1.TabIndex = 3;
            materialRadioButton1.TabStop = true;
            materialRadioButton1.Text = "materialRadioButton1";
            materialRadioButton1.UseAccentColor = false;
            materialRadioButton1.UseVisualStyleBackColor = true;
            // 
            // materialLabel3
            // 
            materialLabel3.AutoSize = true;
            materialLabel3.Depth = 0;
            materialLabel3.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel3.Location = new Point(128, 232);
            materialLabel3.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialLabel3.Name = "materialLabel3";
            materialLabel3.Size = new Size(107, 19);
            materialLabel3.TabIndex = 2;
            materialLabel3.Text = "materialLabel3";
            // 
            // materialLabel2
            // 
            materialLabel2.AutoSize = true;
            materialLabel2.Depth = 0;
            materialLabel2.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel2.Location = new Point(128, 143);
            materialLabel2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialLabel2.Name = "materialLabel2";
            materialLabel2.Size = new Size(107, 19);
            materialLabel2.TabIndex = 1;
            materialLabel2.Text = "materialLabel2";
            // 
            // materialLabel1
            // 
            materialLabel1.AutoSize = true;
            materialLabel1.Depth = 0;
            materialLabel1.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel1.Location = new Point(128, 61);
            materialLabel1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialLabel1.Name = "materialLabel1";
            materialLabel1.Size = new Size(107, 19);
            materialLabel1.TabIndex = 0;
            materialLabel1.Text = "materialLabel1";
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "icons8-집-16.png");
            imageList1.Images.SetKeyName(1, "icons8-게스트-남성-16.png");
            imageList1.Images.SetKeyName(2, "icons8-도움-16.png");
            imageList1.Images.SetKeyName(3, "icons8-돈-가방-16.png");
            imageList1.Images.SetKeyName(4, "icons8-설정-3-16.png");
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(materialTabControl1);
            DrawerShowIconsWhenHidden = true;
            DrawerTabControl = materialTabControl1;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Stockgazers";
            Load += Form1_Load;
            materialTabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.MaterialTabControl materialTabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private ImageList imageList1;
        private ReaLTaiizor.Controls.ParrotGauge parrotGauge1;
        private TabPage tabPage3;
        private ReaLTaiizor.Controls.MaterialListView materialListView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ReaLTaiizor.Controls.MaterialLabel materialLabel3;
        private ReaLTaiizor.Controls.MaterialLabel materialLabel2;
        private ReaLTaiizor.Controls.MaterialLabel materialLabel1;
        private ReaLTaiizor.Controls.MaterialButton materialButton2;
        private ReaLTaiizor.Controls.MaterialButton materialButton1;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit1;
        private ReaLTaiizor.Controls.MaterialRadioButton materialRadioButton2;
        private ReaLTaiizor.Controls.MaterialRadioButton materialRadioButton1;
    }
}