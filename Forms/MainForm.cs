using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using StudyChem.Models;

namespace StudyChem.Forms
{
    public partial class MainForm : Form
    {
        //Declarations
        private User currentUser;
        private Dictionary<string, Quiz> allQuizzes;
        private Quiz currentQuiz;
        private int currentIndex;
        private int earnedPoints;
        private List<RadioButton> optionButtons = new List<RadioButton>();
        private ComboBox cmbTopics;
        private Button btnStart;
        private Button btnSubmit;
        private Button btnPlayAgain;
        private Button btnExit;
        private Button btnExportStats;
        private Label lblQuestion;
        private Label lblStats;
        private TextBox txtStats;

        public MainForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            InitializeQuizUI();
        }

        private void InitializeQuizUI()
        {
            this.Text = $"StudyChem - Welcome {currentUser.Username}";
            this.Width = 750;
            this.Height = 740;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;


            //Creation of labels and buttons and textboxes
            var lblSelectTopic = new Label { Text = "Select Topic:", Left = 5, Top = 10 };
            cmbTopics = new ComboBox { Left = 105, Top = 10, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            btnStart = new Button { Text = "Begin Quiz", Left = 370, Top = 10, Width = 100 };

            lblQuestion = new Label { Top = 60, Left = 10, Width = 700, Height = 60, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };
            btnSubmit = new Button { Text = "Submit", Top = 270, Left = 10, Enabled = false, Width = 100 };

            btnPlayAgain = new Button { Text = "Play Again", Top = 270, Left = 120, Width = 100, Visible = false };
            btnExit = new Button { Text = "Exit", Top = 10, Left = 650, Width = 70, Visible = true };

            lblStats = new Label { Text = "Your Performance:", Top = 310, Left = 10, Width = 300, Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) };
            txtStats = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Top = 340,
                Left = 10,
                Width = 700,
                Height = 280,
                Font = new System.Drawing.Font("Consolas", 9)
            };
            //Export system.
            btnExportStats = new Button { Text = "Export Stats", Top = 630, Left = 10, Width = 120 };
            btnExportStats.Click += (s, e) =>
            {
                if (currentUser.Results.Count == 0)
                {
                    MessageBox.Show("No results to export.");
                    return;
                }

                Directory.CreateDirectory(AppConstants.StatsDataFolder);
                string statsPath = Path.Combine(AppConstants.StatsDataFolder, currentUser.Username + "_results.json");
                File.WriteAllText(statsPath, Newtonsoft.Json.JsonConvert.SerializeObject(currentUser.Results, Newtonsoft.Json.Formatting.Indented));
                MessageBox.Show("Stats exported to: " + statsPath);
            };

            //Loading quizzes with foreach loop.
            allQuizzes = Quiz.LoadAllPreloadedQuizzes();
            foreach (var key in allQuizzes.Keys)
                cmbTopics.Items.Add(key);

            if (cmbTopics.Items.Count > 0)
                cmbTopics.SelectedIndex = 0;


            //Start Quizzing system
            btnStart.Click += (s, e) =>
            {
                var selected = cmbTopics.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(selected) || !allQuizzes.ContainsKey(selected)) return;

                currentQuiz = allQuizzes[selected];
                if (currentQuiz.Questions.Count == 0)
                {
                    MessageBox.Show("Selected quiz has no questions.");
                    return;
                }

                currentQuiz.Shuffle();
                currentIndex = 0;
                earnedPoints = 0;
                btnStart.Enabled = false;
                cmbTopics.Visible = false;
                btnPlayAgain.Visible = false;
                ShowQuestion();
            };
            //Submit question, improved by disabling the submit questions.
            btnSubmit.Click += (s, e) =>
            {
                if (currentQuiz == null || currentIndex >= currentQuiz.Questions.Count) return;

                var selected = optionButtons.FirstOrDefault(rb => rb.Checked);
                if (selected == null)
                {
                    MessageBox.Show("Please select an answer before submitting.");
                    return;
                }

                var currentQuestion = currentQuiz.Questions[currentIndex];
                if (selected.Text == currentQuestion.Answer)
                {
                    earnedPoints += currentQuestion.Points;
                    MessageBox.Show("Correct!");
                }
                else
                {
                    MessageBox.Show("Incorrect. Correct answer: " + currentQuestion.Answer);
                }

                currentIndex++;
                if (currentIndex < currentQuiz.Questions.Count)
                {
                    ShowQuestion();
                }
                else
                {
                    int totalPoints = currentQuiz.Questions.Sum(q => q.Points);
                    double percent = (double)earnedPoints / totalPoints * 100;
                    MessageBox.Show($"Quiz complete! Score: {percent:F1}%");
                    currentUser.Results.Add(new UserResult { Score = percent, Timestamp = DateTime.Now });
                    currentUser.Save();
                    DisplayStats();
                    btnSubmit.Enabled = false;
                    btnPlayAgain.Visible = true;
                }
            };
            //Replay button
            btnPlayAgain.Click += (s, e) =>
            {
                cmbTopics.Visible = true;
                btnStart.Enabled = true;
                btnPlayAgain.Visible = false;
                lblQuestion.Text = "";
                foreach (var rb in optionButtons)
                    Controls.Remove(rb);
                optionButtons.Clear();
            };
            //Exit button.
            btnExit.Click += (s, e) => Application.Exit();
            //Adding Controls to the form.
            Controls.Add(lblSelectTopic);
            Controls.Add(cmbTopics);
            Controls.Add(btnStart);
            Controls.Add(lblQuestion);
            Controls.Add(btnSubmit);
            Controls.Add(btnPlayAgain);
            Controls.Add(btnExit);
            Controls.Add(lblStats);
            Controls.Add(txtStats);
            Controls.Add(btnExportStats);
            //Display statistics.
            DisplayStats();
        }
        /// <summary>
        /// Shows questions and loads the type TF or MCQ.
        /// </summary>
        private void ShowQuestion()
        {
            var currentQuestion = currentQuiz.Questions[currentIndex];
            lblQuestion.Text = $"Q{currentIndex + 1}: {currentQuestion.Prompt}";

            foreach (var rb in optionButtons)
            {
                rb.CheckedChanged -= Option_CheckedChanged;
                Controls.Remove(rb);
            }
            optionButtons.Clear();

            int top = 130;
            List<string> opts = currentQuestion.Type == "TF" ? new List<string> { "True", "False" } : currentQuestion.Options;
            foreach (var option in opts)
            {
                var rb = new RadioButton { Text = option, Left = 10, Top = top, Width = 700 };
                rb.CheckedChanged += Option_CheckedChanged;
                optionButtons.Add(rb);
                Controls.Add(rb);
                top += 30;
            }

            btnSubmit.Enabled = false;
        }


        //If the option selected then the button gets enabled.
        private void Option_CheckedChanged(object sender, EventArgs e)
        {
            btnSubmit.Enabled = optionButtons.Any(rb => rb.Checked);
        }
        //Display stats.
        private void DisplayStats()
        {
            txtStats.Clear();
            txtStats.AppendText("Your past quiz results:\r\n\r\n");
            foreach (var r in currentUser.Results.OrderByDescending(r => r.Timestamp))
            {
                txtStats.AppendText($"{r.Timestamp:g} - {r.Score:F1}%\r\n");
            }
        }
    }
}
