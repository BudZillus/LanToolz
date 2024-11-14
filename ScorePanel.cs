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
using System.Threading;

namespace LanToolz2
{
    public partial class ScorePanel : Form
    {
        string tournamentDataFilePath;
        string dataFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");
        List<List<string>> teams;
        int teamCount;
        int playerPerTeam;
        string headline;
        public static string tempHeadline = "Presented by Stibelsoft ©";
        int numberOfMatchups;
        int totalRounds;
        int playedGames = 0;
        int round = 1;
        bool restored = false;
        private string currentGame;
        
        public List<string> tournamentData = new List<string>();

        public List<string> playedGamesList = new List<string>();

        Dictionary<string, int> finalRanking = new Dictionary<string, int>();

        private List<Label> teamLabels = new List<Label>();
        private List<TextBox> teamTextBoxes = new List<TextBox>();

        public ScorePanel(List<string> tournamenData, List<List<string>> teams, int teamCount, int playerPerTeam, string headline, int numberOfMatchups, int totalRounds, bool restored, string filePath)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            this.tournamentDataFilePath = filePath;
            this.tournamentData = tournamenData;
            this.teams = teams;
            this.playerPerTeam = playerPerTeam;
            this.headline = headline;
            this.numberOfMatchups = numberOfMatchups;
            this.totalRounds = totalRounds;
            this.teamCount = teamCount;
            this.restored = restored;
            Console.WriteLine(filePath);
        }

        private void ScorePanel_Load(object sender, EventArgs e)
        {
            CreateScorePanelLayout(numberOfMatchups, teamCount);

            if (!restored)
            {                
                RevealPlayers();

                string nextGame = SelectNextGame();
                GenerateMatchups();
                List<string> matchups = GetRoundMatchups(tournamentData, nextGame, round);
                UpdateScorePanel(nextGame, matchups, round);
                UpdateScoreScreen(nextGame, matchups, round);
            }
            else
            {                
                RestoreTournamentState();
            }
        }

        private void ScorePanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void CreateScorePanelLayout(int numberOfMatchups, int teamCount)
        {
            int textBoxWidth = 50;
            int textBoxHeight = 30;
            int labelHeight = 20;
            int spacing = 5;

            this.Width = 320;
            this.Height = numberOfMatchups * (textBoxHeight + labelHeight + spacing * 2) + 255;

            // Add game label
            Label gameLabel = new Label();
            gameLabel.Name = "gameLabel";
            gameLabel.Text = "Spiel";
            gameLabel.Width = 100;
            gameLabel.Height = labelHeight;
            gameLabel.AutoSize = true;
            gameLabel.Location = new Point((this.ClientSize.Width - gameLabel.Width) / 2, spacing);
            gameLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            gameLabel.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(gameLabel);

            Label roundLabel = new Label();
            roundLabel.Name = "roundLabel";
            roundLabel.Text = "Runde";
            roundLabel.Width = 100;
            roundLabel.Height = labelHeight;
            roundLabel.Location = new Point((this.ClientSize.Width - roundLabel.Width) / 2, gameLabel.Bottom + spacing);
            roundLabel.TextAlign = ContentAlignment.MiddleCenter;
            if (totalRounds == 1)
            {
                roundLabel.Visible = false;
                roundLabel.Enabled = false;
                roundLabel.Location = gameLabel.Location;
            }

            this.Controls.Add(roundLabel);

            // Create save button
            Button saveButton = new Button();
            saveButton.Text = "Speichern";
            saveButton.Font = new Font("Arial", 10, FontStyle.Bold);
            saveButton.Width = 100;
            saveButton.Height = labelHeight + 5;
            saveButton.Location = new Point((this.ClientSize.Width - saveButton.Width) / 2, this.ClientSize.Height - saveButton.Height - 10);
            saveButton.Click += SaveButton_Click;
            saveButton.BackColor = Color.Red;

            this.Controls.Add(saveButton);

            // Create option button
            Button changeScore = new Button();
            changeScore.Text = "C";
            changeScore.Font = new Font("Arial", 10);
            changeScore.Width = 25;
            changeScore.Height = labelHeight + 5;
            changeScore.Location = new Point(saveButton.Right + spacing, saveButton.Top);
            changeScore.Click += ChangeScore_Click;

            this.Controls.Add(changeScore);

            int currentY = roundLabel.Bottom + spacing;

            for (int i = 0; i < numberOfMatchups; i++)
            {
                // Create the labels and TextBoxes for the first team
                Label team1Label = new Label();
                team1Label.Text = $"Team {i * 2 + 1}";
                team1Label.Width = textBoxWidth;
                team1Label.Height = labelHeight;
                team1Label.Location = new Point((this.ClientSize.Width - 2 * textBoxWidth - spacing) / 2, currentY);

                TextBox team1TextBox = new TextBox();
                team1TextBox.Width = textBoxWidth;
                team1TextBox.Height = textBoxHeight;
                team1TextBox.TextAlign = HorizontalAlignment.Center;
                team1TextBox.Location = new Point(team1Label.Left, team1Label.Bottom + spacing);

                // Create the labels and TextBoxes for the second team
                Label team2Label = new Label();
                team2Label.Text = $"Team {i * 2 + 2}";
                team2Label.Width = textBoxWidth;
                team2Label.Height = labelHeight;
                team2Label.Location = new Point(team1TextBox.Right + spacing, team1Label.Top);

                TextBox team2TextBox = new TextBox();
                team2TextBox.Width = textBoxWidth;
                team2TextBox.Height = textBoxHeight;
                team2TextBox.TextAlign = HorizontalAlignment.Center;
                team2TextBox.Location = new Point(team2Label.Left, team2Label.Bottom + spacing);

                // Center the text in the labels
                team1Label.TextAlign = ContentAlignment.MiddleCenter;
                team2Label.TextAlign = ContentAlignment.MiddleCenter;

                // Add the labels and TextBoxes to the form
                this.Controls.Add(team1Label);
                this.Controls.Add(team1TextBox);
                this.Controls.Add(team2Label);
                this.Controls.Add(team2TextBox);

                // Save the labels and TextBoxes for later use
                teamLabels.Add(team1Label);
                teamLabels.Add(team2Label);
                teamTextBoxes.Add(team1TextBox);
                teamTextBoxes.Add(team2TextBox);

                // Update currentY for the next set of controls
                currentY = team1TextBox.Bottom + spacing;
            }

            // Create the InfoBox
            RichTextBox infoBox = new RichTextBox();
            infoBox.Height = 100;
            infoBox.Width = 300;
            infoBox.SelectionAlignment = HorizontalAlignment.Center;
            infoBox.Location = new Point((this.ClientSize.Width - infoBox.Width) / 2, currentY + spacing);

            this.Controls.Add(infoBox);

            // Cerate saveInfoButton
            Button saveInfoButton = new Button();
            saveInfoButton.Text = "Info Speichern";
            saveInfoButton.Width = 100;
            saveInfoButton.Height = labelHeight;
            saveInfoButton.Location = new Point((this.ClientSize.Width - (saveInfoButton.Width * 2 + spacing)) / 2, infoBox.Bottom + spacing);
            saveInfoButton.Click += SaveInfoButton_Click;

            this.Controls.Add(saveInfoButton);

            // Create resetInfoButton
            Button resetButton = new Button();
            resetButton.Text = "Zurücksetzen";
            resetButton.Width = 100;
            resetButton.Height = labelHeight;
            resetButton.Location = new Point(saveInfoButton.Right + spacing, saveInfoButton.Top);
            resetButton.Click += (s, e) => ClearInfo();

            this.Controls.Add(resetButton);

            ScoreScreen scoreScreen = new ScoreScreen(teams, teamCount, playerPerTeam, headline);
            scoreScreen.Show();
        }

        private void SaveInfoButton_Click(object sender, EventArgs e)
        {
            // Output infoBox.Text to TeamOverview´s infoBox
            foreach (Control control in this.Controls)
            {
                if (control is RichTextBox infoBox)
                {
                    foreach (Control control2 in Application.OpenForms["ScoreScreen"].Controls)
                    {
                        if (control2 is RichTextBox infoBox2)
                        {
                            // Add TeamOverview RichTextBox text and center alignment
                            infoBox2.Text = infoBox.Text;
                            infoBox2.SelectAll();
                            infoBox2.SelectionAlignment = HorizontalAlignment.Center;
                            infoBox2.DeselectAll();
                        }
                    }
                }
            }
        }

        private void ClearInfo()
        {
            foreach (Control control in this.Controls)
            {
                if (control is RichTextBox infoBox)
                {
                    infoBox.Text = string.Empty;
                }
            }
            SaveInfoButton_Click(null, EventArgs.Empty);
        }

        private void ChangeScore_Click(object sender, EventArgs e)
        {
            ChangeScores changeScores = new ChangeScores(tournamentData);
            changeScores.FormClosed += (s, args) =>
            {
                SaveFile();
                UpdateResults(tournamentData);
                
            };
            changeScores.Show();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveScores();
            SaveFile();
            UpdateResults(tournamentData);

            // Show custom dialog
            using (var dialog = new CustomDialog())
            {
                var result = dialog.ShowDialog();

                if (result == DialogResult.Yes)
                {
                    // Start next round
                    if (round < totalRounds)
                    {
                        ClearInput();
                        round++;
                        
                        List<string> matchups = GetRoundMatchups(tournamentData, currentGame, round);
                        
                        UpdateScorePanel(currentGame, matchups, round);
                        UpdateScoreScreen(currentGame, matchups, round);
                    }
                    else
                    {                        
                        ClearInput();
                        CountWinPoints(tournamentData);
                        round = 1;
                        playedGames++;
                        if (playedGames < 6)
                        {
                            string nextGame = SelectNextGame();
                            GenerateMatchups();
                            List<string> matchups = GetRoundMatchups(tournamentData, nextGame, round);
                            UpdateScorePanel(nextGame, matchups, round);
                            UpdateScoreScreen(nextGame, matchups, round);
                        }
                        else
                        {
                            WinnerScreen winnerScreen = new WinnerScreen(finalRanking);
                            winnerScreen.Show();
                        }
                    }
                }
                else if (result == DialogResult.No)
                {                    
                    MessageBox.Show("Das Turnier wird pausiert. Drücken Sie OK, um fortzufahren.", "Pause", MessageBoxButtons.OK, MessageBoxIcon.Information);                   
                    if (round < totalRounds)
                    {
                        ClearInput();
                        round++;
                        
                        List<string> matchups = GetRoundMatchups(tournamentData, currentGame, round);
                       
                        UpdateScorePanel(currentGame, matchups, round);
                        UpdateScoreScreen(currentGame, matchups, round);
                    }
                    else
                    {                        
                        ClearInput();
                        CountWinPoints(tournamentData);
                        round = 1;
                        playedGames++;
                        if (playedGames < 6)
                        {
                            string nextGame = SelectNextGame();
                            GenerateMatchups();
                            List<string> matchups = GetRoundMatchups(tournamentData, nextGame, round);
                            UpdateScorePanel(nextGame, matchups, round);
                            UpdateScoreScreen(nextGame, matchups, round);
                        }
                        else
                        {
                            WinnerScreen winnerScreen = new WinnerScreen(finalRanking);
                            winnerScreen.Show();
                        }
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    // End the tournament
                    MessageBox.Show("Das Turnier wird beendet.", "Turnier beendet", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    WinnerScreen winnerScreen = new WinnerScreen(finalRanking);
                    winnerScreen.Show();
                }
            }
        }

        private void SaveFile()
        {            
            File.WriteAllText(tournamentDataFilePath, string.Join(Environment.NewLine, tournamentData));
        }

        private void SaveScores()
        {
            // Find the index of the current game in the tournamentData
            int gameIndex = tournamentData.FindIndex(line => line.Contains($"Spiel: {currentGame}"));

            // Find the index of the current round
            int roundIndex = tournamentData.FindIndex(gameIndex, line => line.Contains($"Runde {round}:"));

            // Iterate over the TextBoxes and save the results
            for (int i = 0; i < teamTextBoxes.Count; i += 2)
            {
                string team1Result = teamTextBoxes[i].Text;
                string team2Result = teamTextBoxes[i + 1].Text;

                // Find the corresponding matchup in the tournamentData
                int matchupIndex = roundIndex + 1 + (i / 2);
                if (matchupIndex < tournamentData.Count && tournamentData[matchupIndex].Contains(" vs "))
                {
                    // Extract the team names from the matchup
                    var matchup = tournamentData[matchupIndex].Split(new[] { " vs " }, StringSplitOptions.None);
                    string team1 = matchup[0];
                    string team2 = matchup[1];

                    // Add the results
                    if (tournamentData[matchupIndex].Contains("-"))
                    {
                        // If results are already present, add the new results
                        tournamentData[matchupIndex] += $", {team1Result}:{team2Result}";
                    }
                    else
                    {
                        // If no results are present, add the first results
                        tournamentData[matchupIndex] += $" - {team1Result}:{team2Result}";
                    }
                }
            }
        }

        private void ClearInput()
        {
            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
            }
        }

        private string SelectNextGame()
        {
            string gameListFilePath = Path.Combine(dataFolder, "games.csv");

            List<List<string>> allGamesLists = new List<List<string>>();
            List<string> games1 = new List<string>();
            List<string> games2 = new List<string>();
            List<string> games3 = new List<string>();
            
            foreach (var game in playedGamesList)
            {
                games1.Remove(game);
                games2.Remove(game);
                games3.Remove(game);
            }

            if (File.Exists(gameListFilePath))
            {
                var lines = File.ReadAllLines(gameListFilePath);
                if (lines.Length >= 3)
                {
                    games1 = lines[0].Split(';').ToList();
                    games2 = lines[1].Split(';').ToList();
                    games3 = lines[2].Split(';').ToList();
                }
            }

            string nextGame = string.Empty;
            Random random = new Random();

            List<string> selectedGamesList = null;
            switch (playedGames)
            {
                case 0:
                    selectedGamesList = games1;
                    break;
                case 1:
                    selectedGamesList = games3;
                    break;
                case 2:
                    selectedGamesList = games2;
                    break;
                case 3:
                    selectedGamesList = games3;
                    break;
                case 4:
                    selectedGamesList = games1;
                    break;
                case 5:
                    selectedGamesList = games2;
                    break;
            }

            if (selectedGamesList != null && selectedGamesList.Count > 0)
            {
                nextGame = selectedGamesList[random.Next(selectedGamesList.Count)];
                selectedGamesList.Remove(nextGame);
            }
            else
            {
                nextGame = PromptUserForGame();
            }

            tournamentData.Add($"Spiel: {nextGame}");
            currentGame = nextGame;
            playedGamesList.Add(nextGame);

            return nextGame;
        }

        // Prompt the user to enter a game if no games are left or no List is provided
        private string PromptUserForGame()
        {
            string userInput = string.Empty;
            using (Form inputForm = new Form())
            {
                inputForm.Width = 120;
                inputForm.Height = 150;
                inputForm.Text = "Spiel eingeben:";
                inputForm.StartPosition = FormStartPosition.CenterScreen;

                Label label = new Label() { Left = 45, Top = 20, Text = "Spiel:" };
                TextBox textBox = new TextBox() { Left = 10, Top = 50, Width = 100, TextAlign = HorizontalAlignment.Center };
                Button confirmation = new Button() { Text = "OK", Left = 25, Width = 70, Top = 80, DialogResult = DialogResult.OK };

                inputForm.Controls.Add(label);
                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(confirmation);
                inputForm.AcceptButton = confirmation;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    userInput = textBox.Text;
                }
            }
            return userInput;
        }

        private void GenerateMatchups()
        {
            List<string> teamNames = teams.Select((team, index) => $"Team {index + 1}").ToList();
            int totalTeams = teamNames.Count;
            bool hasBye = totalTeams % 2 != 0; // Check if a team has to wait
            if (hasBye)
            {
                teamNames.Add("Bye"); // Add a "Bye" team to account for the waiting team
                totalTeams++; // Increase the number of teams by 1
            }

            int totalRounds = totalTeams - 1; // Round-robin, so n-1 rounds

            List<List<Tuple<string, string>>> rounds = new List<List<Tuple<string, string>>>();
            HashSet<Tuple<string, string>> usedMatchups = new HashSet<Tuple<string, string>>();

            // Shuffle teamNames list
            Random random = new Random();
            teamNames = teamNames.OrderBy(x => random.Next()).ToList();

            for (int round = 0; round < totalRounds; round++)
            {
                List<Tuple<string, string>> matchups = new List<Tuple<string, string>>();
                HashSet<string> teamsInThisRound = new HashSet<string>();

                for (int i = 0; i < totalTeams / 2; i++)
                {
                    string team1 = teamNames[i];
                    string team2 = teamNames[totalTeams - 1 - i];
                    var matchup = new Tuple<string, string>(team1, team2);

                    if (!usedMatchups.Contains(matchup) && !teamsInThisRound.Contains(team1) && !teamsInThisRound.Contains(team2))
                    {
                        matchups.Add(matchup);
                        usedMatchups.Add(matchup);
                        teamsInThisRound.Add(team1);
                        teamsInThisRound.Add(team2);
                    }
                }

                if (matchups.Count > 0)
                {
                    rounds.Add(matchups);
                }

                // Rotate teams for next round
                string lastTeam = teamNames[totalTeams - 1];
                teamNames.RemoveAt(totalTeams - 1);
                teamNames.Insert(1, lastTeam);
            }

            // Add matchups to tournamentData
            for (int round = 0; round < rounds.Count; round++)
            {
                tournamentData.Add($"Runde {round + 1}:");
                foreach (var matchup in rounds[round])
                {
                    if (matchup.Item1 != "Bye" && matchup.Item2 != "Bye")
                    {
                        tournamentData.Add($"{matchup.Item1} vs {matchup.Item2}");
                    }
                }
            }
        }

        private List<string> GetRoundMatchups(List<string> tournamentData, string game, int round)
        {
            List<string> matchups = new List<string>();

            bool startReading = false;
            bool foundGame = false;
            foreach (string line in tournamentData)
            {
                if (startReading)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Runde"))
                    {
                        break;
                    }
                    matchups.Add(line);
                }
                else if (line.StartsWith($"Spiel: {game}"))
                {
                    // Game found, now look for the round
                    foundGame = true;
                }
                else if (foundGame && line.StartsWith($"Runde {round}:"))
                {
                    startReading = true;
                }
            }

            return matchups;
        }

        private void UpdateScorePanel(string nextGame, List<string> matchups, int round)
        {
            // Update the game label
            foreach (Control control in this.Controls)
            {
                if (control is Label gameLabel)
                {
                    gameLabel.Text = nextGame;
                    gameLabel.TextAlign = ContentAlignment.MiddleCenter;
                    gameLabel.Location = new Point((this.ClientSize.Width - gameLabel.Width) / 2, gameLabel.Location.Y);
                    break;
                }
            }

            // Update the round label
            Label roundLabel = this.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "roundLabel");
            if (roundLabel != null)
            {
                roundLabel.Text = $"Runde {round}";
            }

            // Split each matchup into two teams, ignoring the results
            List<List<string>> matchupsList = new List<List<string>>();

            foreach (var matchup in matchups)
            {
                var teams = matchup.Split(new[] { " vs " }, StringSplitOptions.None);
                if (teams.Length == 2)
                {
                    // Remove any results from the team names
                    var team1 = teams[0].Split(new[] { '-' }, 2)[0].Trim();
                    var team2 = teams[1].Split(new[] { '-' }, 2)[0].Trim();
                    matchupsList.Add(new List<string> { team1, team2 });
                }
            }

            // Update the team labels
            for (int i = 0; i < matchupsList.Count; i++)
            {
                if (i * 2 < teamLabels.Count && matchupsList[i].Count == 2)
                {
                    teamLabels[i * 2].Text = matchupsList[i][0];
                    teamLabels[i * 2 + 1].Text = matchupsList[i][1];
                }
            }

            // Enable saveButton only if all TextBoxes are filled
            Button saveButton = this.Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Speichern");
            if (saveButton != null)
            {
                saveButton.Enabled = false;
                foreach (Control control in this.Controls)
                {
                    if (control is TextBox teamTextBox)
                    {
                        teamTextBox.TextChanged += (s, e) =>
                        {
                            bool allFilled = true;
                            foreach (Control ctrl in this.Controls)
                            {
                                if (ctrl is TextBox txtBox && string.IsNullOrWhiteSpace(txtBox.Text))
                                {
                                    allFilled = false;
                                    break;
                                }
                            }
                            saveButton.Enabled = allFilled;
                        };
                    }
                }
            }
        }

        private void UpdateScoreScreen(string nextGame, List<string> matchups, int round)
        {
            var scoreScreenForm = Application.OpenForms["ScoreScreen"];

            foreach (Control control in scoreScreenForm.Controls)
            {
                if (control is Label label)
                {
                    // Update the game label on ScoreScreen
                    if (label.Name == "gameLabel")
                    {
                        label.Text = nextGame;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Location = new Point((scoreScreenForm.ClientSize.Width - label.Width) / 2, label.Location.Y);
                    }

                    // Update the round label on ScoreScreen
                    else if (label.Name == "roundLabel")
                    {
                        label.Text = $"Runde {round}";
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Location = new Point((scoreScreenForm.ClientSize.Width - label.Width) / 2, label.Location.Y);
                    }

                    // Update the team labels on ScoreScreen
                    else if (label.Name.StartsWith("matchupLabel"))
                    {
                        string matchupName;
                        for (int i = 0; i < matchups.Count; i++)
                        {
                            matchupName = $"matchupLabel{i + 1}";
                            if (label.Name == matchupName)
                            {
                                label.Text = matchups[i];
                                label.TextAlign = ContentAlignment.MiddleCenter;
                                label.Location = new Point((scoreScreenForm.ClientSize.Width - label.Width) / 2, label.Location.Y);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateResults(List<string> tournamentData)
        {
            var scoreScreenForm = Application.OpenForms["ScoreScreen"];

            // Delete old score labels
            foreach (var panel in scoreScreenForm.Controls.OfType<Panel>())
            {
                var oldLabels = panel.Controls.OfType<Label>().Where(l => l.Name.StartsWith("scoreLabel")).ToList();
                foreach (var label in oldLabels)
                {
                    panel.Controls.Remove(label);
                }
            }

            int yOffset = 0; // Offset for the new Labels
            int totalHeight = 0; // Total height of the new Labels
            bool firstGameFound = false; // Checking if the first game is found

            // Calculate the total height of the new Labels
            foreach (var line in tournamentData)
            {
                if (line.Contains("Spiel:"))
                {
                    if (firstGameFound)
                    {
                        yOffset += 25; // Add the offset only after the first game
                        totalHeight += 25;
                    }
                    firstGameFound = true;
                }
                if (line.StartsWith("Runde"))
                {
                    continue; // Ignore lines starting with "Runde"
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
            }

            yOffset = 0; // Reset the offset for the actual placement
            firstGameFound = false; // Reset the first game check

            foreach (var line in tournamentData)
            {
                if (line.Contains("Spiel:"))
                {
                    if (firstGameFound)
                    {
                        yOffset += 25; // Add the offset only after the first game
                    }
                    firstGameFound = true;
                }
                if (line.StartsWith("Runde"))
                {
                    continue; // Ignore lines starting with "Runde"
                }

                if (line.Contains(" vs "))
                {
                    var parts = line.Split(new[] { " vs ", " - " }, StringSplitOptions.None);
                    if (parts.Length == 3)
                    {
                        string team1 = parts[0].Trim();
                        string team2 = parts[1].Trim();
                        string[] scores = parts[2].Split(':');
                        if (scores.Length == 2)
                        {
                            string team1Score = scores[0].Trim();
                            string team2Score = scores[1].Trim();

                            var teamPanel1 = scoreScreenForm.Controls.OfType<Panel>().FirstOrDefault(p => p.Name == "teamPanel" + team1.Replace("Team ", ""));
                            var teamPanel2 = scoreScreenForm.Controls.OfType<Panel>().FirstOrDefault(p => p.Name == "teamPanel" + team2.Replace("Team ", ""));

                            //Create new Labels for the scores
                            var scoreLabel1 = new Label();
                            scoreLabel1.Name = "scoreLabel" + team1.Replace("Team ", "") + "_" + team2.Replace("Team ", "");
                            scoreLabel1.Text = team1Score + ":" + team2Score;
                            scoreLabel1.Font = new Font("Arial", 12, FontStyle.Bold);
                            scoreLabel1.AutoSize = true;
                            scoreLabel1.TextAlign = ContentAlignment.MiddleCenter;

                            var scoreLabel2 = new Label();
                            scoreLabel2.Name = "scoreLabel" + team2.Replace("Team ", "") + "_" + team1.Replace("Team ", "");
                            scoreLabel2.Text = team2Score + ":" + team1Score;
                            scoreLabel2.Font = new Font("Arial", 12, FontStyle.Bold);
                            scoreLabel2.AutoSize = true;
                            scoreLabel2.TextAlign = ContentAlignment.MiddleCenter;

                            // Set the location of the new Labels
                            if (teamPanel1 != null)
                            {
                                var vsLabel1 = teamPanel1.Controls.OfType<Label>().FirstOrDefault(l => l.Text == $"VS{team2.Replace("Team ", "")}");
                                if (vsLabel1 != null)
                                {
                                    scoreLabel1.Location = new Point(vsLabel1.Left + (vsLabel1.Width - scoreLabel1.PreferredWidth) / 2, (teamPanel1.Height - totalHeight) / 2 + yOffset);
                                    teamPanel1.Controls.Add(scoreLabel1);
                                }
                                else
                                {
                                    Console.WriteLine($"VS Label for team {team1} vs {team2} not found.");
                                }
                            }

                            if (teamPanel2 != null)
                            {
                                var vsLabel2 = teamPanel2.Controls.OfType<Label>().FirstOrDefault(l => l.Text == $"VS{team1.Replace("Team ", "")}");
                                if (vsLabel2 != null)
                                {
                                    scoreLabel2.Location = new Point(vsLabel2.Left + (vsLabel2.Width - scoreLabel2.PreferredWidth) / 2, (teamPanel2.Height - totalHeight) / 2 + yOffset);
                                    teamPanel2.Controls.Add(scoreLabel2);
                                }
                                else
                                {
                                    Console.WriteLine($"VS Label for team {team2} vs {team1} not found.");
                                }
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
            }
        }


        private void CountWinPoints(List<string> tournamentData)
        {
            Dictionary<string, int> winPoints = new Dictionary<string, int>();
            Dictionary<string, int> tempPoints = new Dictionary<string, int>();
            Dictionary<string, Dictionary<string, int>> directMatchupScores = new Dictionary<string, Dictionary<string, int>>();

            string currentGame = null;
            int roundCount = 0;
            int expectedRounds = (teamCount % 2 == 0) ? teamCount - 1 : teamCount; // Correct number of rounds for even and odd team counts

            foreach (var line in tournamentData)
            {
                if (line.StartsWith("Spiel:"))
                {
                    if (currentGame != null && roundCount == expectedRounds)
                    {
                        AssignWinPoints(tempPoints, directMatchupScores, winPoints);
                    }

                    currentGame = line;
                    roundCount = 0;
                    tempPoints.Clear();
                }
                else if (line.StartsWith("Runde"))
                {
                    roundCount++;
                }
                else if (line.Contains("vs Team") && line.Contains(" - ") && line.Contains(":"))
                {
                    var parts = line.Split(new[] { " - ", " vs " }, StringSplitOptions.None);
                    var team1 = parts[0].Trim();
                    var team2 = parts[1].Trim();

                    var scoreParts = parts[2].Split(':');

                    var score1 = int.Parse(scoreParts[0].Trim());
                    var score2 = int.Parse(scoreParts[1].Trim());

                    if (!tempPoints.ContainsKey(team1)) tempPoints[team1] = 0;
                    if (!tempPoints.ContainsKey(team2)) tempPoints[team2] = 0;
                    if (!directMatchupScores.ContainsKey(team1)) directMatchupScores[team1] = new Dictionary<string, int>();
                    if (!directMatchupScores.ContainsKey(team2)) directMatchupScores[team2] = new Dictionary<string, int>();

                    if (score1 > score2)
                    {
                        tempPoints[team1] += 3;
                    }
                    else if (score1 < score2)
                    {
                        tempPoints[team2] += 3;
                    }
                    else
                    {
                        tempPoints[team1] += 1;
                        tempPoints[team2] += 1;
                    }

                    if (!directMatchupScores[team1].ContainsKey(team2))
                    {
                        directMatchupScores[team1][team2] = 0;
                        directMatchupScores[team2][team1] = 0;
                    }

                    directMatchupScores[team1][team2] += score1 - score2;
                    directMatchupScores[team2][team1] += score2 - score1;
                }
            }

            if (currentGame != null && roundCount == expectedRounds)
            {
                AssignWinPoints(tempPoints, directMatchupScores, winPoints);
            }

            // Sort the teams by WinPoints and DirectMatchupDifference
            var tempRanking = winPoints.OrderByDescending(t => t.Value)
                                        .ThenByDescending(t => GetDirectMatchupDifference(t.Key, directMatchupScores))
                                        .ToList();

            var scoreScreen = Application.OpenForms["ScoreScreen"];

            // Update the ScoreScreen with the final ranking
            foreach (var panel in scoreScreen.Controls.OfType<Panel>())
            {
                var teamLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name.StartsWith("teamLabel"));
                if (teamLabel != null)
                {
                    var teamName = teamLabel.Text;
                    var rankLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name.StartsWith("rankLabel"));
                    if (rankLabel != null)
                    {
                        var rank = tempRanking.FindIndex(t => t.Key == teamName) + 1;
                        rankLabel.Text = $"Platz: {rank}";
                    }
                }
            }

            // Update the ScoreScreen with the final WinPoints
            foreach (var panel in scoreScreen.Controls.OfType<Panel>())
            {
                var teamLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name.StartsWith("teamLabel"));
                if (teamLabel != null)
                {
                    var teamName = teamLabel.Text;
                    var winPointsLabel = panel.Controls.OfType<Label>().FirstOrDefault(l => l.Name.StartsWith("winPointScore"));
                    if (winPointsLabel != null)
                    {
                        var points = winPoints.FirstOrDefault(t => t.Key == teamName).Value;
                        winPointsLabel.Text = $"Punkte: {points}";

                        // Dynamically adjust the position if the panel is on the right side
                        if (panel.Right > scoreScreen.Width / 2)
                        {
                            winPointsLabel.Left = panel.Width - winPointsLabel.Width; // Adjust the position dynamically
                        }
                    }
                }
            }

            finalRanking = tempRanking.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private void AssignWinPoints(Dictionary<string, int> tempPoints, Dictionary<string, Dictionary<string, int>> directMatchupScores, Dictionary<string, int> winPoints)
        {
            var sortedTeams = tempPoints.OrderByDescending(t => t.Value)
                                        .ThenByDescending(t => GetDirectMatchupDifference(t.Key, directMatchupScores))
                                        .ToList();

            // Check if all teams have the same amount of points
            bool allTeamsEqual = sortedTeams.All(t => t.Value == sortedTeams[0].Value);

            for (int i = 0; i < sortedTeams.Count; i++)
            {
                var team = sortedTeams[i].Key;
                if (!winPoints.ContainsKey(team)) winPoints[team] = 0;

                if (allTeamsEqual)
                {
                    winPoints[team] += 1; // All teams have the same amount of points, so assign 1 point to each team
                }
                else
                {
                    winPoints[team] += (sortedTeams.Count - i);
                }
            }

            // Teams that have not played yet get 0 points
            foreach (var team in winPoints.Keys.Except(tempPoints.Keys).ToList())
            {
                winPoints[team] += 0;
            }
        }

        private int GetDirectMatchupDifference(string team, Dictionary<string, Dictionary<string, int>> directMatchupScores)
        {
            int totalDifference = 0;
            if (directMatchupScores.ContainsKey(team))
            {
                foreach (var opponent in directMatchupScores[team])
                {
                    totalDifference += opponent.Value;
                }
            }
            return totalDifference;
        }

        private void RevealPlayers()
        {
            ScoreScreen scoreScreen = Application.OpenForms["ScoreScreen"] as ScoreScreen;
            if (scoreScreen == null) return;

            // Hide the player labels
            foreach (Control control in scoreScreen.Controls)
            {
                if (control is Panel panel && panel.Name.StartsWith("teamPanel"))
                {
                    foreach (Control panelControl in panel.Controls)
                    {
                        if (panelControl is Label playerLabel && playerLabel.Name.StartsWith("playerLabel"))
                        {
                            playerLabel.Visible = false;
                        }
                    }
                }
                if (control is Label headLine)
                {
                    headLine.Text = tempHeadline;
                    // Correct the position of the Headline label
                    headLine.Location = new Point((scoreScreen.ClientSize.Width - headLine.Width) / 2, headLine.Location.Y);
                }
                if (control is Label matchupLabel && matchupLabel.Name.StartsWith("matchupLabel"))
                {
                    matchupLabel.Visible = false;
                }
            }

            // Hide the MatchupLabel, GameLabel and RoundLabel
            Label gameLabel = scoreScreen.Controls.Find("gameLabel", true).FirstOrDefault() as Label;
            Label roundLabel = scoreScreen.Controls.Find("roundLabel", true).FirstOrDefault() as Label;

            if (gameLabel != null) gameLabel.Visible = false;
            if (roundLabel != null) roundLabel.Visible = false;

            // Create a new Clock label
            Label clockLabel = scoreScreen.Controls.Find("clockLabel", true).FirstOrDefault() as Label;
            if (clockLabel == null) return;

            // Initialize a set to keep track of revealed players
            HashSet<string> revealedPlayers = new HashSet<string>();

            Random rng = new Random();

            for (int i = 0; i < playerPerTeam; i++)
            {
                // Randomize order of teams
                List<int> teamIndices = Enumerable.Range(0, teams.Count).ToList();
                teamIndices = teamIndices.OrderBy(x => rng.Next()).ToList();

                List<Label> newLabels = new List<Label>();
                List<Label> originalLabels = new List<Label>();
                List<Point> endPoints = new List<Point>();

                foreach (int teamIndex in teamIndices)
                {
                    string player = teams[teamIndex][i];

                    // Create a new Label for the player
                    Label newLabel = CreatePlayerLabel(scoreScreen, player, clockLabel);
                    newLabels.Add(newLabel);

                    // Find the origin Label
                    Label originalLabel = FindOriginalLabel(scoreScreen, player);
                    if (originalLabel == null) continue;

                    originalLabels.Add(originalLabel);

                    // Calculate the end point for the animation
                    Point endPoint = CalculateEndPoint(scoreScreen, originalLabel);
                    endPoints.Add(endPoint);
                }

                // Create a MessageBox to reveal the players
                MessageBox.Show("OK zum enthüllen der nächsten Spieler", "Spieler enthüllen", MessageBoxButtons.OK);

                // Animate the labels
                AnimateLabels(newLabels, endPoints);

                // Show the original labels again
                foreach (var originalLabel in originalLabels)
                {
                    originalLabel.Visible = true;
                }

                // Delete the animated labels
                foreach (var newLabel in newLabels)
                {
                    scoreScreen.Controls.Remove(newLabel);
                }

                // Add the revealed players to the set
                revealedPlayers.UnionWith(newLabels.Select(label => label.Text));
            }

            // Show the player labels again
            if (gameLabel != null) gameLabel.Visible = true;
            if (roundLabel != null) roundLabel.Visible = true;

            foreach (Control control in scoreScreen.Controls)
            {
                if (control is Label matchupLabel && matchupLabel.Name.StartsWith("matchupLabel"))
                {
                    matchupLabel.Visible = true;
                }
                if (control is Label headLine)
                {
                    headLine.Text = headline;
                    // Correct the position of the Headline label
                    headLine.Location = new Point((scoreScreen.ClientSize.Width - headLine.Width) / 2, headLine.Location.Y);
                }
            }
        }



        private Label CreatePlayerLabel(Control parent, string player, Label clockLabel)
        {
            Label newLabel = new Label();
            newLabel.Text = player;
            newLabel.AutoSize = true;
            newLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            newLabel.Location = new Point((parent.ClientSize.Width - newLabel.PreferredWidth) / 2, clockLabel.Bottom + 20);
            parent.Controls.Add(newLabel);
            newLabel.BringToFront();
            return newLabel;
        }

        private Label FindOriginalLabel(Control parent, string player)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Panel panel && panel.Name.StartsWith("teamPanel"))
                {
                    foreach (Control panelControl in panel.Controls)
                    {
                        if (panelControl is Label label && label.Text == player)
                        {
                            return label;
                        }
                    }
                }
            }
            return null;
        }

        private Point CalculateEndPoint(Control parent, Label originalLabel)
        {
            Point originalLabelScreenLocation = originalLabel.Parent.PointToScreen(originalLabel.Location);
            Point parentScreenLocation = parent.PointToScreen(Point.Empty);
            return new Point(originalLabelScreenLocation.X - parentScreenLocation.X, originalLabelScreenLocation.Y - parentScreenLocation.Y);
        }

        private void AnimateLabels(List<Label> labels, List<Point> endPoints)
        {
            int duration = 150; // Duration of the animation in milliseconds
            int steps = 100; // Number of steps in the animation
            Random random = new Random();
            int circles = random.Next(3, 9); // Random number of circles (3-9)

            // Move to the center of the form
            Point center = new Point((labels[0].Parent.ClientSize.Width - labels[0].Width) / 2, (labels[0].Parent.ClientSize.Height - labels[0].Height) / 2);
            int radius = 180; // Radius of the circle

            // Distribute the labels evenly around the center
            for (int i = 0; i < labels.Count; i++)
            {
                double angle = 2 * Math.PI * i / labels.Count;
                int x = center.X + (int)(radius * Math.Cos(angle));
                int y = center.Y + (int)(radius * Math.Sin(angle));
                SmoothMove(labels[i], labels[i].Location, new Point(x, y), duration, steps);
            }

            // Circle in the center of the form
            for (int j = 0; j < circles; j++)
            {
                for (int i = 0; i <= steps; i++)
                {
                    double angle = 2 * Math.PI * i / steps;
                    for (int k = 0; k < labels.Count; k++)
                    {
                        int x = center.X + (int)(radius * Math.Cos(angle + (2 * Math.PI * k / labels.Count)));
                        int y = center.Y + (int)(radius * Math.Sin(angle + (2 * Math.PI * k / labels.Count)));
                        labels[k].Location = new Point(x, y);

                        // Change the background color of the labels to rainbow colors
                        labels[k].BackColor = ColorFromHSV(360.0 * i / steps, 1.0, 1.0);
                    }

                    Thread.Sleep(duration / steps);
                    Application.DoEvents();
                }
            }

            // Move to the end position
            for (int k = 0; k < labels.Count; k++)
            {
                Point currentPos = labels[k].Location; // Save the current position
                SmoothMove(labels[k], currentPos, endPoints[k], duration, steps);
            }
        }

        private void SmoothMove(Label label, Point startPoint, Point endPoint, int duration, int steps)
        {
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                int x = (int)(startPoint.X * (1 - t) + endPoint.X * t);
                int y = (int)(startPoint.Y * (1 - t) + endPoint.Y * t);
                label.Location = new Point(x, y);

                // Change the background color of the labels to rainbow colors
                label.BackColor = ColorFromHSV(360.0 * t, 1.0, 1.0);

                Thread.Sleep(duration / steps);
                Application.DoEvents();
            }
        }

        // Method to convert HSV to RGB
        private Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        private void RestoreTournamentState()
        {
            if (tournamentData == null || tournamentData.Count == 0)
            {
                MessageBox.Show("Keine Turnierdaten zum Wiederherstellen gefunden.");
                return;
            }

            currentGame = null;
            round = 0;
            playedGames = 0;
            bool gameInProgress = false;
            bool allGamesCompleted = true;

            // Analyze the tournamentData to determine the current game and round
            for (int i = 0; i < tournamentData.Count; i++)
            {
                var line = tournamentData[i];

                if (line.StartsWith("Spiel:"))
                {
                    currentGame = line.Substring(6).Trim();
                    round = 0;
                    playedGames++;
                    playedGamesList.Add(line.Replace("Spiel: ", "").Trim());
                    gameInProgress = true;
                }
                else if (line.StartsWith("Runde"))
                {
                    round++;
                    bool allResultsEntered = true;
                    int matchupsPerRound = teams.Count / 2;

                    // Check if all results for the current round are entered
                    for (int j = 0; j < matchupsPerRound; j++)
                    {
                        if (i + j + 1 >= tournamentData.Count) break;

                        var matchupLine = tournamentData[i + j + 1]; // +1, to check the next line

                        if (matchupLine.StartsWith("Spiel:") || matchupLine.StartsWith("Runde"))
                        {
                            break;
                        }

                        if (matchupLine.Contains(" vs Team") && !matchupLine.Contains(" - ") && !matchupLine.Contains(":"))
                        {
                            allResultsEntered = false;
                            break;
                        }
                    }

                    if (!allResultsEntered)
                    {
                        gameInProgress = true;
                        allGamesCompleted = false;
                        break; // Correct round found, exit the loop
                    }
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    gameInProgress = false;
                }
            }

            // Check if all games are completed
            if (allGamesCompleted)
            {
                currentGame = SelectNextGame();
                round = 1;
                GenerateMatchups();
            }
            else
            {
                // Find the last round with incomplete results
                for (int i = tournamentData.Count - 1; i >= 0; i--)
                {
                    var line = tournamentData[i];
                    if (line.StartsWith("Runde"))
                    {
                        if (int.TryParse(line.Substring(6).Trim(), out int parsedRound))
                        {
                            round = parsedRound;
                            break;
                        }
                    }
                }
            }

            // Check if totalRounds is reached
            if (round > totalRounds)
            {
                round = 1;
            }

            // Show next Matchups
            var matchups = GetRoundMatchups(tournamentData, currentGame, round);
            UpdateScorePanel(currentGame, matchups, round);
            UpdateScoreScreen(currentGame, matchups, round);
            UpdateResults(tournamentData);

            foreach (var game in playedGamesList)
            {
                Console.WriteLine(game);
            }
        }
    }
}