namespace WinForms
{
    partial class SampleForm
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
            this.Inputs = new System.Windows.Forms.TableLayoutPanel();
            this.LoginButton = new System.Windows.Forms.Button();
            this.LogoutButton = new System.Windows.Forms.Button();
            this.Silent = new System.Windows.Forms.CheckBox();
            this.OtherDataDisplay = new System.Windows.Forms.TextBox();
            this.AccessTokenLabel = new System.Windows.Forms.Label();
            this.IdentityTokenLabel = new System.Windows.Forms.Label();
            this.AccessTokenDisplay = new System.Windows.Forms.TextBox();
            this.CallApiButton = new System.Windows.Forms.Button();
            this.Inputs.SuspendLayout();
            this.SuspendLayout();
            // 
            // Inputs
            // 
            this.Inputs.ColumnCount = 5;
            this.Inputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.Inputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.Inputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.Inputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Inputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.Inputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.Inputs.Controls.Add(this.LoginButton, 1, 0);
            this.Inputs.Controls.Add(this.LogoutButton, 2, 0);
            this.Inputs.Controls.Add(this.Silent, 4, 0);
            this.Inputs.Controls.Add(this.OtherDataDisplay, 1, 2);
            this.Inputs.Controls.Add(this.AccessTokenLabel, 0, 1);
            this.Inputs.Controls.Add(this.IdentityTokenLabel, 0, 2);
            this.Inputs.Controls.Add(this.AccessTokenDisplay, 1, 1);
            this.Inputs.Controls.Add(this.CallApiButton, 3, 0);
            this.Inputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Inputs.Location = new System.Drawing.Point(0, 0);
            this.Inputs.Margin = new System.Windows.Forms.Padding(6);
            this.Inputs.Name = "Inputs";
            this.Inputs.RowCount = 3;
            this.Inputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Inputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Inputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Inputs.Size = new System.Drawing.Size(1248, 848);
            this.Inputs.TabIndex = 0;
            // 
            // LoginButton
            // 
            this.LoginButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoginButton.Location = new System.Drawing.Point(172, 6);
            this.LoginButton.Margin = new System.Windows.Forms.Padding(6);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(188, 56);
            this.LoginButton.TabIndex = 0;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // LogoutButton
            // 
            this.LogoutButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogoutButton.Location = new System.Drawing.Point(372, 6);
            this.LogoutButton.Margin = new System.Windows.Forms.Padding(6);
            this.LogoutButton.Name = "LogoutButton";
            this.LogoutButton.Size = new System.Drawing.Size(188, 56);
            this.LogoutButton.TabIndex = 1;
            this.LogoutButton.Text = "Logout";
            this.LogoutButton.UseVisualStyleBackColor = true;
            this.LogoutButton.Click += new System.EventHandler(this.LogoutButton_Click);
            // 
            // Silent
            // 
            this.Silent.AutoSize = true;
            this.Silent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Silent.Location = new System.Drawing.Point(1144, 6);
            this.Silent.Margin = new System.Windows.Forms.Padding(6);
            this.Silent.Name = "Silent";
            this.Silent.Size = new System.Drawing.Size(98, 56);
            this.Silent.TabIndex = 3;
            this.Silent.Text = "Silent";
            this.Silent.UseVisualStyleBackColor = true;
            // 
            // OtherDataDisplay
            // 
            this.Inputs.SetColumnSpan(this.OtherDataDisplay, 4);
            this.OtherDataDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OtherDataDisplay.Location = new System.Drawing.Point(172, 117);
            this.OtherDataDisplay.Margin = new System.Windows.Forms.Padding(6);
            this.OtherDataDisplay.Multiline = true;
            this.OtherDataDisplay.Name = "OtherDataDisplay";
            this.OtherDataDisplay.ReadOnly = true;
            this.OtherDataDisplay.Size = new System.Drawing.Size(1070, 725);
            this.OtherDataDisplay.TabIndex = 7;
            // 
            // AccessTokenLabel
            // 
            this.AccessTokenLabel.AutoSize = true;
            this.AccessTokenLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AccessTokenLabel.Location = new System.Drawing.Point(6, 68);
            this.AccessTokenLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.AccessTokenLabel.Name = "AccessTokenLabel";
            this.AccessTokenLabel.Size = new System.Drawing.Size(154, 43);
            this.AccessTokenLabel.TabIndex = 0;
            this.AccessTokenLabel.Text = "Access Token:";
            this.AccessTokenLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // IdentityTokenLabel
            // 
            this.IdentityTokenLabel.AutoSize = true;
            this.IdentityTokenLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IdentityTokenLabel.Location = new System.Drawing.Point(6, 111);
            this.IdentityTokenLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.IdentityTokenLabel.Name = "IdentityTokenLabel";
            this.IdentityTokenLabel.Size = new System.Drawing.Size(154, 737);
            this.IdentityTokenLabel.TabIndex = 0;
            this.IdentityTokenLabel.Text = "Other data:";
            this.IdentityTokenLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AccessTokenDisplay
            // 
            this.Inputs.SetColumnSpan(this.AccessTokenDisplay, 4);
            this.AccessTokenDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AccessTokenDisplay.Location = new System.Drawing.Point(172, 74);
            this.AccessTokenDisplay.Margin = new System.Windows.Forms.Padding(6);
            this.AccessTokenDisplay.Name = "AccessTokenDisplay";
            this.AccessTokenDisplay.ReadOnly = true;
            this.AccessTokenDisplay.Size = new System.Drawing.Size(1070, 31);
            this.AccessTokenDisplay.TabIndex = 6;
            // 
            // CallApiButton
            // 
            this.CallApiButton.Location = new System.Drawing.Point(569, 3);
            this.CallApiButton.Name = "CallApiButton";
            this.CallApiButton.Size = new System.Drawing.Size(152, 59);
            this.CallApiButton.TabIndex = 8;
            this.CallApiButton.Text = "Call API";
            this.CallApiButton.UseVisualStyleBackColor = true;
            this.CallApiButton.Click += new System.EventHandler(this.CallApiButton_Click);
            // 
            // SampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1248, 848);
            this.Controls.Add(this.Inputs);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "SampleForm";
            this.Text = "SampleForm";
            this.Inputs.ResumeLayout(false);
            this.Inputs.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel Inputs;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Button LogoutButton;
        private System.Windows.Forms.CheckBox Silent;
        private System.Windows.Forms.TextBox OtherDataDisplay;
        private System.Windows.Forms.Label AccessTokenLabel;
        private System.Windows.Forms.Label IdentityTokenLabel;
        private System.Windows.Forms.TextBox AccessTokenDisplay;
        private System.Windows.Forms.Button CallApiButton;
    }
}