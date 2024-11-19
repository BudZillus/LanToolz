using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LanToolz2
{
    public partial class ScoreScreen : Form
    {
        List<List<string>> teams = new List<List<string>>();
        int teamCount;
        int playerPerTeam;
        string headline;
        int numberOfMatchups;

        public ScoreScreen(List<List<string>> teams, int teamCount, int playerPerTeam, string headline)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual; // Set to manual to control the position
            this.FormBorderStyle = FormBorderStyle.None; // Remove the border
            this.WindowState = FormWindowState.Maximized; // Maximize the window

            // Check if a second screen is available
            if (Screen.AllScreens.Length > 1)
            {
                // Get the bounds of the second screen
                Screen secondScreen = Screen.AllScreens[1];
                this.Location = secondScreen.Bounds.Location;
                this.Size = secondScreen.Bounds.Size; // Set the size to the second screen's size
            }
            else
            {
                // If no second screen, center on the primary screen
                this.StartPosition = FormStartPosition.CenterScreen;
            }

            this.teams = teams;
            this.teamCount = teamCount;
            this.playerPerTeam = playerPerTeam;
            this.headline = headline;

            CreateScoreboardLayout();
        }

        private void TeamOverview_Load(object sender, EventArgs e)
        {

        }

        private void CreateScoreboardLayout()
        {
            int screenWidth = 1920; //Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = 1080; // Screen.PrimaryScreen.Bounds.Height;

            int rows = (teamCount + 1) / 2; // Calculate the number of rows needed, rounding up
            int maxTeamsPerRow = 2;

            int teamIndex = 0;
            int panelMidSpace = 800;

            int totalHeight = screenHeight / rows; // Calculate the total height of each team panel

            Random random = new Random();
            HashSet<Color> usedColors = new HashSet<Color>();

            int formHeight = 0; // Variable to track the total height of the form

            for (int row = 0; row < rows; row++)
            {
                int teamsInRow = Math.Min(maxTeamsPerRow, teamCount - teamIndex); // Calculate the number of teams in the current row

                for (int col = 0; col < teamsInRow; col++)
                {
                    Panel teamPanel = new Panel();
                    teamPanel.Name = "teamPanel" + (teamIndex + 1);
                    teamPanel.Size = new Size((screenWidth - panelMidSpace) / maxTeamsPerRow, totalHeight); // Adjust the height based on the number of rows

                    int panelLeft;
                    if (col == 0) // Left side panels
                    {
                        panelLeft = col * (teamPanel.Width + panelMidSpace); // Position the panel on the left side of the screen
                    }
                    else // Right side panels
                    {
                        panelLeft = screenWidth - teamPanel.Width; // Position the panel on the right side of the screen
                    }
                    int panelTop = row * totalHeight; // Position the panel in the grid

                    teamPanel.Location = new Point(panelLeft, panelTop);

                    // Set random neon background color for team panel ensuring no duplicates
                    Color randomColor;
                    do
                    {
                        int r = random.Next(128, 256);
                        int g = random.Next(128, 256);
                        int b = random.Next(128, 256);
                        randomColor = Color.FromArgb(r, g, b);
                    } while (usedColors.Contains(randomColor) || IsSimilarColor(randomColor, Color.White) || IsTooBright(randomColor) || IsTooLight(randomColor));

                    usedColors.Add(randomColor);
                    teamPanel.BackColor = randomColor;

                    Label teamLabel = new Label();
                    teamLabel.Name = "teamLabel" + (teamIndex + 1);
                    teamLabel.Text = "Team " + (teamIndex + 1);
                    teamLabel.AutoSize = true;
                    teamLabel.Font = new Font("Arial", 20, FontStyle.Bold);

                    if (col == 0) // Left side panels
                    {
                        teamLabel.Location = new Point(0, 10); // Position the team label on the left side of the panel                        
                    }
                    else // Right side panels
                    {
                        teamLabel.Location = new Point(teamPanel.Width - teamLabel.PreferredWidth , 10); // Position the team label on the right side of the panel                        
                    }

                    teamPanel.Controls.Add(teamLabel);

                    // Add opponent labels directly to the team panel
                    int opponentLabelTop = 13; // Position just below the team label
                    int opponentLabelLeft; // Initial left position for opponent labels
                    int labelCounter = 0; // Counter to track the number of labels created

                    for (int opponentIndex = 0; opponentIndex < teamCount; opponentIndex++)
                    {
                        if (opponentIndex != teamIndex)
                        {
                            Label opponentLabel = new Label();
                            opponentLabel.Name = "opponentLabel" + (teamIndex + 1) + "_" + (opponentIndex + 1);
                            opponentLabel.Text = $"VS{opponentIndex + 1}";
                            opponentLabel.AutoSize = true;
                            opponentLabel.Font = new Font("Arial", 14, FontStyle.Bold);

                            opponentLabel.TextAlign = ContentAlignment.MiddleCenter; // Align the opponent name to the center

                            // Adjust the left position based on the column
                            if (col == 0) // Left side panels
                            {
                                opponentLabelLeft = teamPanel.Width - (labelCounter + 1) * (opponentLabel.PreferredWidth + 10); // Position the labels on the right side of the panel
                            }
                            else // Right side panels
                            {
                                opponentLabelLeft = labelCounter * (opponentLabel.PreferredWidth + 10); // Position the labels on the left side of the panel
                            }

                            opponentLabel.Location = new Point(opponentLabelLeft + 5, opponentLabelTop);                            

                            teamPanel.Controls.Add(opponentLabel);

                            labelCounter++; // Increment the label counter
                        }
                    }



                    // Add score label
                    Label scoreLabel = new Label();
                    scoreLabel.Name = "winPointScore" + (teamIndex + 1);
                    scoreLabel.Text = "Punkte: 0";
                    scoreLabel.AutoSize = true;
                    scoreLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                    if (col == 0) // Left side panels
                    {
                        scoreLabel.Location = new Point(5, teamPanel.Height - scoreLabel.PreferredHeight - 10); // Position at the bottom left with additional margin
                    }
                    else // Right side panels
                    {
                        scoreLabel.Location = new Point(teamPanel.Width - scoreLabel.PreferredWidth - 5, teamPanel.Height - scoreLabel.PreferredHeight - 10); // Position at the bottom right with additional margin
                    }
                    teamPanel.Controls.Add(scoreLabel);

                    // Add rank label
                    Label rankLabel = new Label();
                    rankLabel.Name = "rankLabel" + (teamIndex + 1);
                    rankLabel.Text = "Platz: 0";
                    rankLabel.AutoSize = true;
                    rankLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                    if (col == 0) // Left side panels
                    {
                        rankLabel.Location = new Point(teamPanel.Width - rankLabel.PreferredWidth, teamPanel.Height - rankLabel.PreferredHeight - 10); // Position at the bottom right with additional margin
                    }
                    else // Right side panels
                    {
                        rankLabel.Location = new Point(5, teamPanel.Height - rankLabel.PreferredHeight - 10); // Position at the bottom left with additional margin
                    }
                    teamPanel.Controls.Add(rankLabel);



                    // Add player names to the team panel
                    List<string> players = teams[teamIndex];
                    int playerLabelHeight = 20; // or any appropriate value
                    int playerLabelSpacing = 20; // Add spacing between player labels

                    // Calculate the total height of all player labels including spacing
                    int totalPlayerLabelsHeight = players.Count * (playerLabelHeight + playerLabelSpacing) - playerLabelSpacing;

                    // Calculate the top position to center the player labels vertically within the team panel
                    int playerLabelTop = (teamPanel.Height - totalPlayerLabelsHeight) / 2;

                    for (int i = 0; i < players.Count; i++)
                    {
                        Label playerLabel = new Label();
                        playerLabel.Name = "playerLabel" + (i + 1);
                        playerLabel.Text = players[i];
                        playerLabel.AutoSize = true;
                        playerLabel.Font = new Font("Arial", 14, FontStyle.Bold);

                        int labelWidth = playerLabel.PreferredWidth;
                        int labelHeight = playerLabel.PreferredHeight;

                        int labelLeft;
                        if (col == 0) // Left side panels
                        {
                            playerLabel.TextAlign = ContentAlignment.MiddleLeft; // Align the player name to the left
                            labelLeft = 5;
                        }
                        else // Right side panels
                        {
                            playerLabel.TextAlign = ContentAlignment.MiddleRight; // Align the player name to the right
                            labelLeft = teamPanel.Width - labelWidth - 5;
                        }

                        int labelTop = playerLabelTop + i * (playerLabelHeight + playerLabelSpacing);

                        playerLabel.Location = new Point(labelLeft, labelTop);

                        teamPanel.Controls.Add(playerLabel);
                    }
                    this.Controls.Add(teamPanel);
                    teamIndex++;

                }
            }

            // Add headline
            Label headLine = new Label();
            headLine.Name = "headLine";
            headLine.Text = headline;
            headLine.AutoSize = true;
            headLine.Font = new Font("Arial", 32, FontStyle.Bold);
            headLine.Location = new Point((screenWidth - headLine.PreferredWidth) / 2, 20);
            this.Controls.Add(headLine);

            // Add clock
            Label clockLabel = new Label();
            clockLabel.Name = "clockLabel";
            clockLabel.AutoSize = true;
            clockLabel.Font = new Font("Arial", 18, FontStyle.Bold);
            clockLabel.Location = new Point((screenWidth - clockLabel.PreferredWidth) / 2, headLine.Bottom + 20);
            this.Controls.Add(clockLabel);

            // Create timer to update the clock
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // Refresh every second
            timer.Tick += (s, args) =>
            {
                clockLabel.Text = DateTime.Now.ToString("HH:mm:ss"); // Refresh the text
                clockLabel.Location = new Point((screenWidth - clockLabel.PreferredWidth) / 2, headLine.Bottom + 20); // Refresh the location
            };
            timer.Start(); // Start the timer

            // Add current game label
            Label currentGameLabel = new Label();
            currentGameLabel.Name = "gameLabel";
            currentGameLabel.Text = "Aktuelles Spiel";
            currentGameLabel.AutoSize = true;
            currentGameLabel.Font = new Font("Arial", 30, FontStyle.Bold);
            currentGameLabel.TextAlign = ContentAlignment.MiddleCenter;
            currentGameLabel.Location = new Point((screenWidth - currentGameLabel.PreferredWidth) / 2, clockLabel.Bottom + 175);
            this.Controls.Add(currentGameLabel);

            // If more than 2 Teams, add Round label
            if (teams.Count > 2)
            {
                Label roundLabel = new Label();
                roundLabel.Name = "roundLabel";
                roundLabel.Text = "Runde: 1";
                roundLabel.AutoSize = true;
                roundLabel.Font = new Font("Arial", 16, FontStyle.Bold);
                roundLabel.TextAlign = ContentAlignment.MiddleCenter;
                roundLabel.Location = new Point((screenWidth - roundLabel.PreferredWidth) / 2, currentGameLabel.Bottom + 10);
                this.Controls.Add(roundLabel);
            }

            // Add matchup labels
            int matchupCount = teamCount / 2; // Calculate the number of matchup labels needed
            int matchupLabelTop = currentGameLabel.Bottom + 120; // Initial top position for the first matchup label

            numberOfMatchups = matchupCount;

            for (int i = 0; i < matchupCount; i++)
            {
                Label matchupLabel = new Label();
                matchupLabel.Name = "matchupLabel" + (i + 1);
                matchupLabel.Text = "Team X vs Team X";
                matchupLabel.AutoSize = true;
                matchupLabel.Font = new Font("Arial", 26, FontStyle.Bold);
                matchupLabel.TextAlign = ContentAlignment.MiddleCenter;
                matchupLabel.Location = new Point((screenWidth - matchupLabel.PreferredWidth) / 2, matchupLabelTop);
                this.Controls.Add(matchupLabel);

                matchupLabelTop = matchupLabel.Bottom + 20; // Update the top position for the next matchup label
            }

            // Add info Box
            RichTextBox infoBox = new RichTextBox();
            infoBox.Name = "infobox";
            infoBox.Font = new Font("Arial", 18, FontStyle.Bold);
            infoBox.ReadOnly = true;
            infoBox.TabStop = false;
            infoBox.BorderStyle = BorderStyle.None;
            infoBox.Width = 300;
            infoBox.Height = 300;
            infoBox.SelectionProtected = true;

            // Set the location of the RichTextBox
            infoBox.Location = new Point((screenWidth - infoBox.Width) / 2, matchupLabelTop + 80);
            this.Controls.Add(infoBox);

            // Adjust the form height to fit all controls
            formHeight = infoBox.Bottom + 40; // Add some padding at the bottom
            this.ClientSize = new Size(screenWidth, screenHeight);

            //create pause timer on ScoreScreen form
            Panel pausePanel = new Panel();
            pausePanel.Name = "pausePanel";
            pausePanel.Width = panelMidSpace;
            pausePanel.Height = 900;
            pausePanel.Location = new Point((this.ClientSize.Width - pausePanel.Width) / 2, clockLabel.Location.Y + clockLabel.Height + 10);            
            this.Controls.Add(pausePanel);

            Label pauseLabel = new Label();
            pauseLabel.Name = "pauseLabel";
            pauseLabel.Text = "Pause";
            pauseLabel.AutoSize = true;
            pauseLabel.Font = new Font("Arial", 36, FontStyle.Bold);
            pauseLabel.Location = new Point((pausePanel.Width - pauseLabel.PreferredWidth) / 2, 250);
            pausePanel.Controls.Add(pauseLabel);

            Label pauseTimeLabel = new Label();
            pauseTimeLabel.Name = "pauseTimeLabel";
            pauseTimeLabel.Text = "00:00";
            pauseTimeLabel.AutoSize = true;
            pauseTimeLabel.Font = new Font("Arial", 26, FontStyle.Bold);
            pauseTimeLabel.Location = new Point((pausePanel.Width - pauseTimeLabel.PreferredWidth) / 2, (pausePanel.Height - pauseTimeLabel.Height) / 2);
            pausePanel.Controls.Add(pauseTimeLabel); 

            pausePanel.Hide();
        }

        private bool IsTooBright(Color color)
        {
            // Calculate the brightness of the color
            double brightness = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return brightness > 0.95; // Adjust the threshold as needed
        }

        private bool IsTooLight(Color color)
        {
            // Check if the color is too light (e.g., very light yellow)
            return color.R > 200 && color.G > 200 && color.B < 150;
        }

        private bool IsSimilarColor(Color color1, Color color2)
        {
            int threshold = 50; // Adjust this value as needed
            int rDiff = Math.Abs(color1.R - color2.R);
            int gDiff = Math.Abs(color1.G - color2.G);
            int bDiff = Math.Abs(color1.B - color2.B);
            return rDiff < threshold && gDiff < threshold && bDiff < threshold;
        }       
    }
}
