namespace Stockgazers
{
    partial class LoginForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            panel1 = new Panel();
            materialTextBoxEdit1 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            materialTextBoxEdit2 = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            imageList1 = new ImageList(components);
            materialButton1 = new ReaLTaiizor.Controls.MaterialButton();
            materialButton2 = new ReaLTaiizor.Controls.MaterialButton();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Center;
            panel1.Location = new Point(6, 74);
            panel1.Name = "panel1";
            panel1.Size = new Size(386, 155);
            panel1.TabIndex = 5;
            // 
            // materialTextBoxEdit1
            // 
            materialTextBoxEdit1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            materialTextBoxEdit1.AnimateReadOnly = false;
            materialTextBoxEdit1.AutoCompleteMode = AutoCompleteMode.None;
            materialTextBoxEdit1.AutoCompleteSource = AutoCompleteSource.None;
            materialTextBoxEdit1.BackgroundImageLayout = ImageLayout.None;
            materialTextBoxEdit1.CharacterCasing = CharacterCasing.Normal;
            materialTextBoxEdit1.Depth = 0;
            materialTextBoxEdit1.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBoxEdit1.HideSelection = true;
            materialTextBoxEdit1.Hint = "ID";
            materialTextBoxEdit1.LeadingIcon = Properties.Resources.icons8_게스트_남성_16;
            materialTextBoxEdit1.Location = new Point(6, 252);
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
            materialTextBoxEdit1.Size = new Size(386, 48);
            materialTextBoxEdit1.TabIndex = 9;
            materialTextBoxEdit1.TabStop = false;
            materialTextBoxEdit1.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit1.TrailingIcon = null;
            materialTextBoxEdit1.UseSystemPasswordChar = false;
            materialTextBoxEdit1.KeyDown += materialTextBoxEdit1_KeyDown;
            // 
            // materialTextBoxEdit2
            // 
            materialTextBoxEdit2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            materialTextBoxEdit2.AnimateReadOnly = false;
            materialTextBoxEdit2.AutoCompleteMode = AutoCompleteMode.None;
            materialTextBoxEdit2.AutoCompleteSource = AutoCompleteSource.None;
            materialTextBoxEdit2.BackgroundImageLayout = ImageLayout.None;
            materialTextBoxEdit2.CharacterCasing = CharacterCasing.Normal;
            materialTextBoxEdit2.Cursor = Cursors.IBeam;
            materialTextBoxEdit2.Depth = 0;
            materialTextBoxEdit2.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialTextBoxEdit2.HideSelection = true;
            materialTextBoxEdit2.Hint = "Password";
            materialTextBoxEdit2.LeadingIcon = Properties.Resources.icons8_지문_24;
            materialTextBoxEdit2.Location = new Point(6, 336);
            materialTextBoxEdit2.MaxLength = 32767;
            materialTextBoxEdit2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            materialTextBoxEdit2.Name = "materialTextBoxEdit2";
            materialTextBoxEdit2.PasswordChar = '●';
            materialTextBoxEdit2.PrefixSuffixText = null;
            materialTextBoxEdit2.ReadOnly = false;
            materialTextBoxEdit2.RightToLeft = RightToLeft.No;
            materialTextBoxEdit2.SelectedText = "";
            materialTextBoxEdit2.SelectionLength = 0;
            materialTextBoxEdit2.SelectionStart = 0;
            materialTextBoxEdit2.ShortcutsEnabled = true;
            materialTextBoxEdit2.Size = new Size(386, 48);
            materialTextBoxEdit2.TabIndex = 10;
            materialTextBoxEdit2.TabStop = false;
            materialTextBoxEdit2.TextAlign = HorizontalAlignment.Left;
            materialTextBoxEdit2.TrailingIcon = null;
            materialTextBoxEdit2.UseSystemPasswordChar = true;
            materialTextBoxEdit2.KeyDown += materialTextBoxEdit2_KeyDown;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth24Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "icons8-지문-24.png");
            // 
            // materialButton1
            // 
            materialButton1.AutoSize = false;
            materialButton1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton1.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton1.Depth = 0;
            materialButton1.Font = new Font("Roboto", 12F, FontStyle.Bold, GraphicsUnit.Point);
            materialButton1.HighEmphasis = true;
            materialButton1.Icon = null;
            materialButton1.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            materialButton1.Location = new Point(50, 392);
            materialButton1.Margin = new Padding(4, 5, 4, 5);
            materialButton1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialButton1.Name = "materialButton1";
            materialButton1.NoAccentTextColor = Color.Empty;
            materialButton1.Size = new Size(105, 32);
            materialButton1.TabIndex = 11;
            materialButton1.Text = "LOGIN";
            materialButton1.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton1.UseAccentColor = false;
            materialButton1.UseVisualStyleBackColor = true;
            materialButton1.Click += materialButton1_Click;
            // 
            // materialButton2
            // 
            materialButton2.AutoSize = false;
            materialButton2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton2.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton2.Depth = 0;
            materialButton2.Font = new Font("Roboto", 12F, FontStyle.Bold, GraphicsUnit.Point);
            materialButton2.HighEmphasis = true;
            materialButton2.Icon = null;
            materialButton2.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            materialButton2.Location = new Point(189, 392);
            materialButton2.Margin = new Padding(4, 5, 4, 5);
            materialButton2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialButton2.Name = "materialButton2";
            materialButton2.NoAccentTextColor = Color.Empty;
            materialButton2.Size = new Size(105, 32);
            materialButton2.TabIndex = 12;
            materialButton2.Text = "SIGN-UP";
            materialButton2.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton2.UseAccentColor = true;
            materialButton2.UseVisualStyleBackColor = true;
            materialButton2.Click += materialButton2_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(398, 664);
            Controls.Add(materialButton2);
            Controls.Add(materialButton1);
            Controls.Add(materialTextBoxEdit2);
            Controls.Add(materialTextBoxEdit1);
            Controls.Add(panel1);
            Font = new Font("Roboto", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "LoginForm";
            Padding = new Padding(3, 56, 3, 3);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Stockgazers";
            ResumeLayout(false);
        }

        #endregion
        private Panel panel1;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit1;
        private ReaLTaiizor.Controls.MaterialTextBoxEdit materialTextBoxEdit2;
        private ImageList imageList1;
        private ReaLTaiizor.Controls.MaterialButton materialButton1;
        private ReaLTaiizor.Controls.MaterialButton materialButton2;
    }
}