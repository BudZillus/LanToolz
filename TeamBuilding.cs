using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace LanToolz2
{
    public partial class TeamBuilding : Form
    {
        string filePath;
        List<string> tournamentData = new List<string>();
        private int teamCount;
        private int playerPerTeam;
        private string headline;
        bool restored = false;  

        public TeamBuilding(List<string> tournamentData, int teamCount, int playerPerTeam, string headline, string filePath)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            this.filePath = filePath;
            this.tournamentData = tournamentData;
            this.teamCount = teamCount;
            this.playerPerTeam = playerPerTeam;
            this.headline = headline;

            CreateInputLayout(teamCount, playerPerTeam);
        }

        private void Teams_Load(object sender, EventArgs e)
        {

        }

        private void CreateInputLayout(int teamCount, int playerPerTeam)
        {
            // Calculate the size of the window based on the number of teams and players
            int labelHeight = 30;
            int textBoxHeight = 30;
            int verticalSpacing = 1;
            int horizontalSpacing = 5; // Decrease the horizontal spacing
            int textBoxWidth = 125;

            int formWidth = (textBoxWidth + horizontalSpacing) * playerPerTeam + horizontalSpacing;
            int formHeight = (labelHeight + (textBoxHeight + verticalSpacing) * teamCount) + verticalSpacing + 90; // Increase the height for the buttons

            this.ClientSize = new Size(formWidth, formHeight);

            // Create the headline label
            Label headlineLabel = new Label();
            headlineLabel.Text = headline;
            headlineLabel.Size = new Size(formWidth, 30);
            headlineLabel.TextAlign = ContentAlignment.MiddleCenter; // Center the text in the label
            headlineLabel.Font = new Font("Arial", 12);
            headlineLabel.Location = new Point(0, 0);
            this.Controls.Add(headlineLabel);
            // Create the labels for the pots
            for (int i = 0; i < playerPerTeam; i++)
            {
                Label potLabel = new Label();
                potLabel.Text = $"Pot {i + 1}";
                potLabel.Size = new Size(textBoxWidth, labelHeight);
                potLabel.TextAlign = ContentAlignment.MiddleCenter; // Center the text in the label
                potLabel.Location = new Point(horizontalSpacing + i * (textBoxWidth + horizontalSpacing), headlineLabel.Bottom + verticalSpacing);
                this.Controls.Add(potLabel);

                // Create the text boxes for the players
                for (int j = 0; j < teamCount; j++)
                {
                    TextBox playerTextBox = new TextBox();
                    playerTextBox.Name = $"pot{i + 1}Player{j + 1}";
                    playerTextBox.Text = $"Pot {i + 1} Player {j + 1}";
                    playerTextBox.TextAlign = HorizontalAlignment.Center; // Set the text alignment to center
                    playerTextBox.Location = new Point(horizontalSpacing + i * (textBoxWidth + horizontalSpacing), potLabel.Bottom + verticalSpacing + j * (textBoxHeight + verticalSpacing));
                    playerTextBox.Size = new Size(textBoxWidth, textBoxHeight);
                    this.Controls.Add(playerTextBox);
                }
            }

            
            Button saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Size = new Size(70, 25);
            
            int saveButtonX = (formWidth - saveButton.Width) / 2;
            int saveButtonY = this.Controls.OfType<TextBox>().Max(tb => tb.Bottom) + 10;

            saveButton.Location = new Point(saveButtonX, saveButtonY);
            saveButton.Click += new EventHandler(saveButton_Click);
            this.Controls.Add(saveButton);

            
            Button backButton = new Button();
            backButton.Text = "Back";
            backButton.Size = new Size(70, 25);
            backButton.Location = new Point((formWidth - backButton.Width) / 2, saveButton.Bottom + 5); 
            backButton.Click += new EventHandler(backButton_Click);
            this.Controls.Add(backButton);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            Setup setup = new Setup();
            setup.Show();
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Dictionary<int, List<string>> pots = new Dictionary<int, List<string>>();
             
            for (int i = 0; i < playerPerTeam; i++)
            {
                List<string> players = new List<string>();
                for (int j = 0; j < teamCount; j++)
                {
                    TextBox textBox = this.Controls.Find($"pot{i + 1}Player{j + 1}", true).FirstOrDefault() as TextBox;
                    if (textBox != null)
                    {
                        players.Add(textBox.Text);
                    }
                }
                pots.Add(i + 1, players);
            }

            // Create a list to store the teams
            List<List<string>> teams = new List<List<string>>();

            // Randomly assign players from each pot to the teams
            Random random = new Random();
            for (int i = 0; i < teamCount; i++)
            {
                List<string> team = new List<string>();
                foreach (var pot in pots)
                {
                    int randomIndex = random.Next(pot.Value.Count);
                    string player = pot.Value[randomIndex];
                    team.Add(player);
                    pot.Value.RemoveAt(randomIndex);
                }
                teams.Add(team);
            }

            // randomize player order in each team
            foreach (var team in teams)
            {
                team.Sort((a, b) => random.Next(-1, 2));
            }

            // Add the teams to the tournament data, adding a separator between each team
            tournamentData.Add("Teams:");
            for (int i = 0; i < teamCount; i++)
            {
                tournamentData.Add($"Team {i + 1}: {string.Join(", ", teams[i])}");
            }

            int numberOfMatchups = teamCount / 2; // Define numberOfMatchups here
            int totalRounds = CalculateTotalRounds(teamCount); // Calculate total rounds

            // Save tournament data to a file
            File.AppendAllLines(filePath, tournamentData);


            ScorePanel scorePanel = new ScorePanel(tournamentData, teams, teamCount, playerPerTeam, headline, numberOfMatchups, totalRounds, restored, filePath);
            scorePanel.Show();
            this.Close();
        }

        private int CalculateTotalRounds(int teamCount)
        {
            if (teamCount % 2 == 0)
            {
                return teamCount - 1;
            }
            else
            {
                return teamCount;
            }
        }
    }
}
