using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LanToolz2
{
    public partial class ChangeScores : Form
    {
        private List<string> tournamentData;

        public ChangeScores(List<string> tournamentData)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;            

            this.tournamentData = tournamentData;
        }

        private void ChangeScores_Load(object sender, EventArgs e)
        {
            CreateLayout();
        }

        private void CreateLayout()
        {
            int textBoxWidth = 50;
            int textBoxHeight = 30;
            int labelHeight = 20;
            int spacing = 5;
            int extraSpacing = 10; // Zusätzlicher Abstand zwischen Spielzeilen und Textboxen
            int currentY = 10;
            int maxWidth = 250; // Verwenden Sie die Breite des Formulars

            // Erstellen eines Panels mit AutoScroll
            Panel scrollPanel = new Panel();
            scrollPanel.AutoScroll = true;
            scrollPanel.Dock = DockStyle.Fill;
            this.Controls.Add(scrollPanel);

            for (int i = 0; i < tournamentData.Count; i++)
            {
                string line = tournamentData[i];

                if (line.Contains("Spiel:"))
                {
                    currentY += extraSpacing; // Zusätzlicher Abstand vor der Spielzeile

                    Label gameLabel = new Label();
                    gameLabel.Text = line.Replace("Spiel: ", "");
                    gameLabel.Width = 200;
                    gameLabel.Height = labelHeight;
                    gameLabel.TextAlign = ContentAlignment.MiddleCenter; // Text mittig ausrichten
                    gameLabel.Location = new Point((maxWidth - gameLabel.Width) / 2, currentY); // Zentrieren
                    scrollPanel.Controls.Add(gameLabel);
                    currentY += gameLabel.Height + spacing;
                }
                else if (line.Contains("vs Team") && line.Contains("-") && line.Contains(":"))
                {
                    string[] parts = line.Split(new[] { " vs ", " - ", ":" }, StringSplitOptions.None);
                    if (parts.Length == 4)
                    {
                        Label team1Label = new Label();
                        team1Label.Text = parts[0];
                        team1Label.Width = textBoxWidth; // Breite der Textbox übernehmen
                        team1Label.Height = labelHeight;
                        team1Label.TextAlign = ContentAlignment.MiddleCenter; // Text mittig ausrichten
                        team1Label.Location = new Point((maxWidth - textBoxWidth * 2 - spacing) / 2, currentY); // Zentrieren

                        TextBox team1TextBox = new TextBox();
                        team1TextBox.Width = textBoxWidth;
                        team1TextBox.Height = textBoxHeight;
                        team1TextBox.Text = parts[2];
                        team1TextBox.TextAlign = HorizontalAlignment.Center; // Text mittig ausrichten
                        team1TextBox.Location = new Point(team1Label.Left, currentY + team1Label.Height + spacing);
                        team1TextBox.Tag = $"team1_{i}"; // Setzen der Tag-Eigenschaft

                        Label team2Label = new Label();
                        team2Label.Text = parts[1];
                        team2Label.Width = textBoxWidth; // Breite der Textbox übernehmen
                        team2Label.Height = labelHeight;
                        team2Label.TextAlign = ContentAlignment.MiddleCenter; // Text mittig ausrichten
                        team2Label.Location = new Point(team1TextBox.Right + spacing, currentY); // Zentrieren

                        TextBox team2TextBox = new TextBox();
                        team2TextBox.Width = textBoxWidth;
                        team2TextBox.Height = textBoxHeight;
                        team2TextBox.Text = parts[3];
                        team2TextBox.TextAlign = HorizontalAlignment.Center; // Text mittig ausrichten
                        team2TextBox.Location = new Point(team2Label.Left, currentY + team2Label.Height + spacing);
                        team2TextBox.Tag = $"team2_{i}"; // Setzen der Tag-Eigenschaft

                        scrollPanel.Controls.Add(team1Label);
                        scrollPanel.Controls.Add(team1TextBox);
                        scrollPanel.Controls.Add(team2Label);
                        scrollPanel.Controls.Add(team2TextBox);

                        currentY += team1Label.Height + team1TextBox.Height + spacing * 2;
                    }
                }
            }

            Button saveButton = new Button();
            saveButton.Text = "Speichern";
            saveButton.Width = 100;
            saveButton.Height = labelHeight + 5;
            saveButton.Location = new Point((maxWidth - saveButton.Width) / 2, currentY + spacing); // Zentrieren
            saveButton.Click += SaveButton_Click;
            scrollPanel.Controls.Add(saveButton);

            // Set the ClientSize to accommodate the scroll panel, but do not exceed a height of 500
            int desiredHeight = saveButton.Bottom + 40;
            this.ClientSize = new Size(maxWidth, Math.Min(desiredHeight, 800));
        }



        private void SaveButton_Click(object sender, EventArgs e)
        {
            Panel scrollPanel = this.Controls.OfType<Panel>().FirstOrDefault();
            if (scrollPanel == null) return;

            for (int i = 0; i < tournamentData.Count; i++)
            {
                if (tournamentData[i].Contains("vs Team ") && tournamentData[i].Contains(" - ") && tournamentData[i].Contains(":"))
                {
                    string[] parts = tournamentData[i].Split(new[] { " vs ", " - ", ":" }, StringSplitOptions.None);
                    if (parts.Length == 4)
                    {
                        // Find the corresponding TextBoxes for the current match within the scrollPanel
                        TextBox team1TextBox = scrollPanel.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Tag?.ToString() == $"team1_{i}");
                        TextBox team2TextBox = scrollPanel.Controls.OfType<TextBox>().FirstOrDefault(tb => tb.Tag?.ToString() == $"team2_{i}");

                        // Update the scores in parts array
                        if (team1TextBox != null && team2TextBox != null)
                        {
                            parts[2] = team1TextBox.Text.Trim(); // Entfernen von führenden und nachfolgenden Leerzeichen
                            parts[3] = team2TextBox.Text.Trim(); // Entfernen von führenden und nachfolgenden Leerzeichen
                        }
                        else
                        {
                            Console.WriteLine("TextBoxes not found for match: " + tournamentData[i]);
                        }

                        // Überprüfen, ob beide Teile gültige Werte enthalten
                        if (!string.IsNullOrEmpty(parts[2]) && !string.IsNullOrEmpty(parts[3]))
                        {
                            tournamentData[i] = $"{parts[0]} vs {parts[1]} - {parts[2]}:{parts[3]}";
                            Console.WriteLine("ChangeScore: " + tournamentData[i]);
                        }
                        else
                        {
                            // Beibehalten der ursprünglichen Zeile, wenn die Werte ungültig sind
                            Console.WriteLine("Ungültige Werte für match: " + tournamentData[i]);
                        }
                    }
                }
            }

            var scorePanel = Application.OpenForms["ScorePanel"] as ScorePanel;
            if (scorePanel != null)
            {
                scorePanel.tournamentData = this.tournamentData;
            }
            else
            {
                Console.WriteLine("kein scorepanel gefunden");
            }
            this.Close();
        }
    }
}
