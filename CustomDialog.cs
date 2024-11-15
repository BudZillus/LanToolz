using System.Windows.Forms;

public partial class CustomDialog : Form
{
    public CustomDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
            this.btnNextGame = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnEndTournament = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnNextGame
            // 
            this.btnNextGame.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnNextGame.Location = new System.Drawing.Point(12, 12);
            this.btnNextGame.Name = "btnNextGame";
            this.btnNextGame.Size = new System.Drawing.Size(75, 23);
            this.btnNextGame.TabIndex = 0;
            this.btnNextGame.Text = "Weiter";
            this.btnNextGame.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            this.btnPause.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnPause.Location = new System.Drawing.Point(93, 12);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 1;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            // 
            // btnEndTournament
            // 
            this.btnEndTournament.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnEndTournament.Location = new System.Drawing.Point(174, 12);
            this.btnEndTournament.Name = "btnEndTournament";
            this.btnEndTournament.Size = new System.Drawing.Size(75, 23);
            this.btnEndTournament.TabIndex = 2;
            this.btnEndTournament.Text = "Beenden";
            this.btnEndTournament.UseVisualStyleBackColor = true;
            // 
            // CustomDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 47);
            this.Controls.Add(this.btnEndTournament);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnNextGame);
            this.Name = "CustomDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose wisely";
            this.ResumeLayout(false);

    }

    private System.Windows.Forms.Button btnNextGame;
    private System.Windows.Forms.Button btnPause;
    private System.Windows.Forms.Button btnEndTournament;
}
