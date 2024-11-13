namespace LanToolz2
{
    partial class Setup
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.headlineLabel = new System.Windows.Forms.Label();
            this.headlineInput = new System.Windows.Forms.TextBox();
            this.teamNumberInput = new System.Windows.Forms.ComboBox();
            this.playerPerTeamInput = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.okayButton = new System.Windows.Forms.Button();
            this.restorButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // headlineLabel
            // 
            this.headlineLabel.AutoSize = true;
            this.headlineLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headlineLabel.Location = new System.Drawing.Point(49, 24);
            this.headlineLabel.Name = "headlineLabel";
            this.headlineLabel.Size = new System.Drawing.Size(57, 13);
            this.headlineLabel.TabIndex = 0;
            this.headlineLabel.Text = "Headline";
            this.headlineLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // headlineInput
            // 
            this.headlineInput.Location = new System.Drawing.Point(27, 42);
            this.headlineInput.Name = "headlineInput";
            this.headlineInput.Size = new System.Drawing.Size(100, 20);
            this.headlineInput.TabIndex = 1;
            this.headlineInput.Text = "Turnierlan #4";
            this.headlineInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // teamNumberInput
            // 
            this.teamNumberInput.FormattingEnabled = true;
            this.teamNumberInput.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.teamNumberInput.Location = new System.Drawing.Point(105, 82);
            this.teamNumberInput.Name = "teamNumberInput";
            this.teamNumberInput.Size = new System.Drawing.Size(32, 21);
            this.teamNumberInput.TabIndex = 2;
            this.teamNumberInput.Text = "2";
            // 
            // playerPerTeamInput
            // 
            this.playerPerTeamInput.FormattingEnabled = true;
            this.playerPerTeamInput.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.playerPerTeamInput.Location = new System.Drawing.Point(105, 109);
            this.playerPerTeamInput.Name = "playerPerTeamInput";
            this.playerPerTeamInput.Size = new System.Drawing.Size(32, 21);
            this.playerPerTeamInput.TabIndex = 2;
            this.playerPerTeamInput.Text = "1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Teams:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Player per Team:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // okayButton
            // 
            this.okayButton.Location = new System.Drawing.Point(39, 151);
            this.okayButton.Name = "okayButton";
            this.okayButton.Size = new System.Drawing.Size(75, 23);
            this.okayButton.TabIndex = 3;
            this.okayButton.Text = "Okay";
            this.okayButton.UseVisualStyleBackColor = false;
            this.okayButton.Click += new System.EventHandler(this.okayButton_Click);
            // 
            // restorButton
            // 
            this.restorButton.Location = new System.Drawing.Point(121, 151);
            this.restorButton.Name = "restorButton";
            this.restorButton.Size = new System.Drawing.Size(27, 23);
            this.restorButton.TabIndex = 4;
            this.restorButton.Text = "R";
            this.restorButton.UseVisualStyleBackColor = true;
            this.restorButton.Click += new System.EventHandler(this.restorButton_Click);
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(160, 186);
            this.Controls.Add(this.restorButton);
            this.Controls.Add(this.okayButton);
            this.Controls.Add(this.playerPerTeamInput);
            this.Controls.Add(this.teamNumberInput);
            this.Controls.Add(this.headlineInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.headlineLabel);
            this.Name = "Setup";
            this.Text = "LanToolz2Setup";
            this.Load += new System.EventHandler(this.Setup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label headlineLabel;
        private System.Windows.Forms.TextBox headlineInput;
        private System.Windows.Forms.ComboBox teamNumberInput;
        private System.Windows.Forms.ComboBox playerPerTeamInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button okayButton;
        private System.Windows.Forms.Button restorButton;
    }
}

