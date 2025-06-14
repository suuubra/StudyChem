using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using StudyChem.Models;

namespace StudyChem.Forms
{
    public partial class MainForm : Form
    {
        // Declarations
        private User currentUser;
        private Dictionary<string, Quiz> allQuizzes;
        private Quiz currentQuiz;
        private int currentIndex;
        private int earnedPoints;
        private int attemptLimit = 0; // Number of questions user wants to attempt

        // UI elements
        private List<RadioButton> optionButtons = new List<RadioButton>();
        private ComboBox cmbTopics;
        private Button btnStart;
        private Button btnSubmit;
        private Button btnPlayAgain;
        private Button btnExit;
        private Button btnExportStats;
        private Button btnAttemptEnter;
        private Label lblQuestion;
        private Label lblStats;
        private Label lblQuestions;
        private TextBox txtStats;
        private TextBox txtQuestions;

        // Constants
        private const int MIN_QUESTIONS = 1;
        private const int MAX_QUESTIONS = 15;

        public MainForm(User user)
        {
            currentUser = user;
            InitializeComponent();
            InitializeQuizUI();
        }

        // Set up all UI controls
        private void InitializeQuizUI()
        {
            this.Text = $"StudyChem - Welcome {currentUser.Username}";
            this.Width = 750;
            this.Height = 740;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Topic selector
            var lblSelectTopic = new Label { Text = "Select Topic:", Left = 5, Top = 10 };
            cmbTopics = new ComboBox { Left = 105, Top = 10, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            btnStart = new Button { Text = "Begin Quiz", Left = 370, Top = 10, Width = 100 };

            // Question attempt input
            lblQuestions = new Label { Text = "Enter No. Of Questions:", Left = 105, Top = 40, Width = 400, Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) };
            txtQuestions = new TextBox { Left = 250, Top = 40, Width = 100 };
            btnAttemptEnter = new Button { Text = "Enter", Left = 360, Top = 40, Width = 70 };

            // Hide these until quiz is selected
            lblQuestions.Visible = txtQuestions.Visible = btnAttemptEnter.Visible = false;
            lblQuestions.Enabled = txtQuestions.Enabled = btnAttemptEnter.Enabled = false;

            // Question display
            lblQuestion = new Label { Top = 80, Left = 10, Width = 700, Height = 60, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };
            btnSubmit = new Button { Text = "Submit", Top = 270, Left = 10, Enabled = false, Width = 100 };
            btnPlayAgain = new Button { Text = "Play Again", Top = 270, Left = 120, Width = 100, Visible = false };
            btnExit = new Button { Text = "Exit", Top = 10, Left = 650, Width = 70 };

            // Stats display
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

            // Export stats
            btnExportStats = new Button { Text = "Export Stats", Top = 630, Left = 10, Width = 120 };
            btnExportStats.Click += ExportStats;

            // Add loaded quizzes to dropdown
            allQuizzes = Quiz.LoadAllPreloadedQuizzes();
            foreach (var key in allQuizzes.Keys)
                cmbTopics.Items.Add(key);
            if (cmbTopics.Items.Count > 0)
                cmbTopics.SelectedIndex = 0;

            // Add controls to form
            Controls.AddRange(new Control[]
            {
                lblSelectTopic, cmbTopics, btnStart, btnExit,
                lblQuestions, txtQuestions, btnAttemptEnter,
                lblQuestion, btnSubmit, btnPlayAgain,
                lblStats, txtStats, btnExportStats
            });

            // Hook up events
            btnStart.Click += StartQuizPrompt;
            btnAttemptEnter.Click += ConfirmQuestionAttempt;
            btnSubmit.Click += SubmitAnswer;
            btnPlayAgain.Click += ResetQuiz;
            btnExit.Click += (s, e) => Application.Exit();

            DisplayStats();
        }
        /// <summary>
        /// Starts the quiz by prompting for number of questions to attempt
        /// </summary>
        private void StartQuizPrompt(object sender, EventArgs e)
        {
            var selected = cmbTopics.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selected) || !allQuizzes.ContainsKey(selected))
                return;

            currentQuiz = allQuizzes[selected];

            // Validate quiz has questions
            if (currentQuiz.Questions.Count == 0)
            {
                MessageBox.Show("Selected quiz has no questions.");
                return;
            }

            // Show inputs for attempt selection
            lblQuestions.Visible = txtQuestions.Visible = btnAttemptEnter.Visible = true;
            lblQuestions.Enabled = txtQuestions.Enabled = btnAttemptEnter.Enabled = true;
        }

        /// <summary>
        /// Confirms how many questions the user wants to attempt and starts the quiz
        /// </summary>
        private void ConfirmQuestionAttempt(object sender, EventArgs e)
        {
            if (!int.TryParse(txtQuestions.Text, out int amount))
            {
                MessageBox.Show("Please enter a valid number.");
                return;
            }

            if (amount < MIN_QUESTIONS || amount > MAX_QUESTIONS)
            {
                MessageBox.Show($"Please enter a number between {MIN_QUESTIONS} and {MAX_QUESTIONS}.");
                return;
            }

            attemptLimit = Math.Min(amount, currentQuiz.Questions.Count);
            currentQuiz.Shuffle();
            currentIndex = 0;
            earnedPoints = 0;

            btnStart.Enabled = false;
            cmbTopics.Visible = false;
            btnPlayAgain.Visible = false;

            ShowQuestion();
        }

        /// <summary>
        /// Shows a single question on screen, handles both TF and MCQ types
        /// </summary>
        private void ShowQuestion()
        {
            // End quiz if reached limit
            if (currentIndex >= attemptLimit)
            {
                FinishQuiz();
                return;
            }

            var currentQuestion = currentQuiz.Questions[currentIndex];
            lblQuestion.Text = $"Q{currentIndex + 1}: {currentQuestion.Prompt}";

            foreach (var rb in optionButtons)
            {
                rb.CheckedChanged -= Option_CheckedChanged;
                Controls.Remove(rb);
            }
            optionButtons.Clear();

            int top = 130;
            var opts = currentQuestion.Type == "TF" ? new List<string> { "True", "False" } : currentQuestion.Options;
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

        /// <summary>
        /// Triggered when user selects an answer
        /// </summary>
        private void Option_CheckedChanged(object sender, EventArgs e)
        {
            btnSubmit.Enabled = optionButtons.Any(rb => rb.Checked);
        }

        /// <summary>
        /// Submits the selected answer, gives feedback, moves to next question
        /// </summary>
        private void SubmitAnswer(object sender, EventArgs e)
        {
            if (currentQuiz == null || currentIndex >= currentQuiz.Questions.Count)
                return;

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
                MessageBox.Show($"Correct! +{currentQuestion.Points} point(s).");
            }
            else
            {
                MessageBox.Show($"Incorrect. Correct answer was: {currentQuestion.Answer}");
            }

            currentIndex++;
            ShowQuestion();
        }

        /// <summary>
        /// Finalises the quiz, calculates score, and saves result
        /// </summary>
        private void FinishQuiz()
        {
            int totalPoints = currentQuiz.Questions.Take(attemptLimit).Sum(q => q.Points);
            double percent = totalPoints > 0 ? (double)earnedPoints / totalPoints * 100 : 0;

            MessageBox.Show($"Quiz complete! You scored: {percent:F1}%");

            currentUser.Results.Add(new UserResult { Score = percent, Timestamp = DateTime.Now });
            currentUser.Save();

            DisplayStats();

            btnSubmit.Enabled = false;
            btnPlayAgain.Visible = true;
        }

        /// <summary>
        /// Resets UI to allow replay
        /// </summary>
        private void ResetQuiz(object sender, EventArgs e)
        {
            cmbTopics.Visible = true;
            btnStart.Enabled = true;
            btnPlayAgain.Visible = false;
            lblQuestion.Text = "";
            txtQuestions.Clear();
            attemptLimit = 0;

            foreach (var rb in optionButtons)
                Controls.Remove(rb);
            optionButtons.Clear();
        }

        /// <summary>
        /// Displays quiz history in stats box
        /// </summary>
        private void DisplayStats()
        {
            txtStats.Clear();
            txtStats.AppendText("Your past quiz results:\r\n\r\n");

            foreach (var result in currentUser.Results.OrderByDescending(r => r.Timestamp))
                txtStats.AppendText($"{result.Timestamp:g} - {result.Score:F1}%\r\n");
        }

        /// <summary>
        /// Exports stats to a JSON file in Stats folder
        /// </summary>
        private void ExportStats(object sender, EventArgs e)
        {
            if (currentUser.Results.Count == 0)
            {
                MessageBox.Show("No results to export.");
                return;
            }

            Directory.CreateDirectory(AppConstants.StatsDataFolder);
            string statsPath = Path.Combine(AppConstants.StatsDataFolder, currentUser.Username + "_results.json");

            try
            {
                File.WriteAllText(statsPath, JsonConvert.SerializeObject(currentUser.Results, Formatting.Indented));
            }
            catch (Exception ex)
            {
                ErrorLogger.Log("ExportStats", ex);
                MessageBox.Show("Failed to export stats.");
                return;
            }

            MessageBox.Show($"Stats exported to:\n{statsPath}");
        }
    }
}
