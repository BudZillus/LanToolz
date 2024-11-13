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
    public partial class Setup : Form
    {
        string lanToolzPath = Assembly.GetExecutingAssembly().Location;
        bool restored = false;
        List<string> tournamentData = new List<string>();

        public Setup()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void okayButton_Click(object sender, EventArgs e)
        {
            if (headlineInput.Text != "" && !FolderNameIsAlreadyTaken(headlineInput.Text))
            {
                string headline = headlineInput.Text;
                int selectedTeams = Convert.ToInt32(teamNumberInput.Text);
                int selectedPlayersPerTeam = Convert.ToInt32(playerPerTeamInput.Text);
                tournamentData.Add($"Überschrift: {headline}");
                tournamentData.Add($"Team Anzahl: {selectedTeams.ToString()}");
                tournamentData.Add($"Spieler pro Team: {selectedPlayersPerTeam.ToString()}");

                // Create folder for the tournament data
                string folderPath = Path.Combine(Path.GetDirectoryName(lanToolzPath), headline);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Save tournament data to a file in the new folder
                string filePath = Path.Combine(folderPath, "TournamentData.txt");
                File.WriteAllLines(filePath, tournamentData);

                TeamBuilding teams = new TeamBuilding(tournamentData, selectedTeams, selectedPlayersPerTeam, headline, filePath);
                teams.Show();
                this.Hide();
            }
            else
            {
                if (FolderNameIsAlreadyTaken(headlineInput.Text))
                {
                    MessageBox.Show("Der Name wird bereits benutzt.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Bitte Überschrift eingeben.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }                
            }
        }

        private bool FolderNameIsAlreadyTaken(string folderName)
        {
            string folderPath = Path.Combine(Path.GetDirectoryName(lanToolzPath), folderName);
            return Directory.Exists(folderPath);
        }

        private void Setup_Load(object sender, EventArgs e)
        {

        }

        private void restorButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Title = "Wählen der TournamentData.txt";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    List<string> fileLines = System.IO.File.ReadAllLines(filePath).ToList();

                    if (fileLines.Count > 0)
                    {
                        string headline = fileLines[0].Replace("Überschrift: ", "").Trim();
                        int selectedTeams = Convert.ToInt32(fileLines[1].Replace("Team Anzahl: ", "").Trim());                        
                        int selectedPlayersPerTeam = Convert.ToInt32(fileLines[2].Replace("Spieler pro Team:", "").Trim());

                        List<List<string>> teams = new List<List<string>>();
                        for (int i = 4; i < 4 + selectedTeams; i++)
                        {
                            string teamLine = fileLines[i];
                            string[] teamData = teamLine.Split(new string[] { ": " }, StringSplitOptions.None);
                            List<string> players = teamData[1].Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                            teams.Add(players);
                        }

                        // Update the form fields or use the data as needed
                        headlineInput.Text = headline;
                        teamNumberInput.Text = selectedTeams.ToString();
                        playerPerTeamInput.Text = selectedPlayersPerTeam.ToString();
                        tournamentData = fileLines;

                        MessageBox.Show("Tournament Daten geladen.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        int numberOfMatchups = selectedTeams / 2; // Define numberOfMatchups here
                        int totalRounds = CalculateTotalRounds(selectedTeams); // Calculate total rounds
                        restored = true;

                        ScorePanel scorePanel = new ScorePanel(tournamentData, teams, selectedTeams, selectedPlayersPerTeam, headline, numberOfMatchups, totalRounds, restored, filePath);
                        scorePanel.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Datei ist leer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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
