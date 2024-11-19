using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LanToolz2
{
    public partial class WinnerScreen : Form
    {
        string rootFolder = Assembly.GetExecutingAssembly().Location;
        string dataFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");

        Dictionary<string, int> finalRanking;
        public WinnerScreen(Dictionary<string, int> finalRanking)
        {
            InitializeComponent();

            // Check if a second screen is available
            if (Screen.AllScreens.Length > 1)
            {
                // Get the bounds of the second screen
                Screen secondScreen = Screen.AllScreens[1];
                this.Location = secondScreen.Bounds.Location;
                this.Size = secondScreen.Bounds.Size; // Set the size to the second screen's size
                this.StartPosition = FormStartPosition.Manual; // Set to manual to control the position
                this.FormBorderStyle = FormBorderStyle.None; // Remove the border
                this.WindowState = FormWindowState.Maximized; // Maximize the window
            }
            else
            {
                // If no second screen, center on the primary screen
                this.StartPosition = FormStartPosition.CenterScreen;
            }

            this.finalRanking = finalRanking;
            createRanking(finalRanking);
        }

        private void WinnerScreen_Load(object sender, EventArgs e)
        {
                        
        }

        private void createRanking(Dictionary<string, int> finalRanking)
        {
            // Sort the teams by their points in descending order
            var sortedTeams = finalRanking.OrderByDescending(kvp => kvp.Value).ToList();

            int maxWidth = this.ClientSize.Width;
            int maxHeight = this.ClientSize.Height;

            // Calculate the maximum height based on the width to maintain the aspect ratio 16:9
            int calculatedHeight = (int)(maxWidth / 16.0 * 9.0);

            // If the calculated height is greater than the maximum height, adjust the width
            if (calculatedHeight > maxHeight)
            {
                calculatedHeight = maxHeight;
                maxWidth = (int)(calculatedHeight / 9.0 * 16.0);
            }

            // Create a PictureBox for placing the labels
            PictureBox rankingPictureBox = new PictureBox
            {
                Name = "rankingPictureBox",
                Size = new Size(maxWidth, calculatedHeight),
                BorderStyle = BorderStyle.None,
                Image = Image.FromFile(Path.Combine(dataFolder, "rankingBackground.png")),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Center the PictureBox in the form
            rankingPictureBox.Location = new Point((this.ClientSize.Width - rankingPictureBox.Width) / 2, 10);
            this.Controls.Add(rankingPictureBox);

            // Create labels for the top three teams
            for (int i = 0; i < sortedTeams.Count && i < 3; i++)
            {
                Label teamLabel = new Label
                {
                    Text = $"{sortedTeams[i].Key}",
                    AutoSize = true,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Font = new Font("Arial", 18, FontStyle.Bold)
                };

                if (i == 0)
                {
                    // First place at the bottom center of the PictureBox
                    teamLabel.Location = new Point((rankingPictureBox.Width - teamLabel.PreferredWidth) / 2, rankingPictureBox.Height - teamLabel.Height - 250);
                    teamLabel.BackColor = Color.Gold;
                }
                else if (i == 1)
                {
                    // Second place at the bottom left of the PictureBox
                    teamLabel.Location = new Point(415, rankingPictureBox.Height - teamLabel.Height - 175);
                    teamLabel.BackColor = Color.Silver;
                }
                else if (i == 2)
                {
                    // Third place at the bottom right of the PictureBox
                    teamLabel.Location = new Point(rankingPictureBox.Width - teamLabel.PreferredWidth - 415, rankingPictureBox.Height - teamLabel.Height - 100);
                    teamLabel.BackColor = Color.SaddleBrown;
                }
                rankingPictureBox.Controls.Add(teamLabel);
            }

            // Create small PictureBoxes for the teams from the fourth place onwards
            if (sortedTeams.Count > 3)
            {
                int smallPictureBoxCount = sortedTeams.Count - 3;
                int smallPictureBoxWidth = 80;
                int smallPictureBoxHeight = 80;
                int spacing = 30; // More space between the PictureBoxes
                int totalWidth = smallPictureBoxCount * smallPictureBoxWidth + (smallPictureBoxCount - 1) * spacing;
                int startX = (this.ClientSize.Width - totalWidth) / 2;

                // Adjust the height of the rankingPictureBox to make room for the small PictureBoxes
                rankingPictureBox.Height -= (smallPictureBoxHeight + spacing + 50);

                // Adjust the position of the labels of the top three teams
                for (int i = 0; i < 3; i++)
                {
                    Control teamLabel = rankingPictureBox.Controls[i];
                    teamLabel.Location = new Point(teamLabel.Location.X, teamLabel.Location.Y - (smallPictureBoxHeight + spacing + 50));
                }

                for (int i = 3; i < sortedTeams.Count; i++)
                {
                    PictureBox smallPictureBox = new PictureBox
                    {
                        Size = new Size(smallPictureBoxWidth, smallPictureBoxHeight),
                        Location = new Point(startX + (i - 3) * (smallPictureBoxWidth + spacing), rankingPictureBox.Location.Y + rankingPictureBox.Height + 20),
                        BorderStyle = BorderStyle.None,
                        Image = Image.FromFile(Path.Combine(dataFolder, "poop.png")),
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    this.Controls.Add(smallPictureBox);

                    // Create a label for the team name under the PictureBox
                    Label teamLabel = new Label
                    {
                        Text = sortedTeams[i].Key,
                        AutoSize = true,
                        Font = new Font("Arial", 10, FontStyle.Regular),
                        TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                    };
                    teamLabel.Location = new Point(smallPictureBox.Location.X + (smallPictureBox.Width - teamLabel.PreferredWidth) / 2, smallPictureBox.Location.Y + smallPictureBox.Height + 5);
                    this.Controls.Add(teamLabel);
                }
            }
        }
    }
}