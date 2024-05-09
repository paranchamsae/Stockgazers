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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            textBox_id = new TextBox();
            textBox_passwd = new TextBox();
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            panel1 = new Panel();
            button2 = new Button();
            SuspendLayout();
            // 
            // textBox_id
            // 
            textBox_id.Location = new Point(83, 231);
            textBox_id.Name = "textBox_id";
            textBox_id.Size = new Size(187, 25);
            textBox_id.TabIndex = 0;
            // 
            // textBox_passwd
            // 
            textBox_passwd.Location = new Point(83, 273);
            textBox_passwd.Name = "textBox_passwd";
            textBox_passwd.PasswordChar = '●';
            textBox_passwd.Size = new Size(187, 25);
            textBox_passwd.TabIndex = 1;
            // 
            // button1
            // 
            button1.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button1.Location = new Point(90, 332);
            button1.Name = "button1";
            button1.Size = new Size(75, 26);
            button1.TabIndex = 2;
            button1.Text = "로그인";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(26, 234);
            label1.Name = "label1";
            label1.Size = new Size(51, 19);
            label1.TabIndex = 3;
            label1.Text = "아이디";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 276);
            label2.Name = "label2";
            label2.Size = new Size(65, 19);
            label2.TabIndex = 4;
            label2.Text = "패스워드";
            // 
            // panel1
            // 
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Location = new Point(6, 67);
            panel1.Name = "panel1";
            panel1.Size = new Size(264, 126);
            panel1.TabIndex = 5;
            // 
            // button2
            // 
            button2.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button2.Location = new Point(90, 364);
            button2.Name = "button2";
            button2.Size = new Size(75, 26);
            button2.TabIndex = 6;
            button2.Text = "회원가입";
            button2.UseVisualStyleBackColor = true;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(276, 410);
            Controls.Add(button2);
            Controls.Add(panel1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(textBox_passwd);
            Controls.Add(textBox_id);
            Font = new Font("맑은 고딕", 10F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "LoginForm";
            Text = "STOCKGAZERS";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox_id;
        private TextBox textBox_passwd;
        private Button button1;
        private Label label1;
        private Label label2;
        private Panel panel1;
        private Button button2;
    }
}