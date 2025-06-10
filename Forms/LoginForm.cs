using System;
using System.Windows.Forms;
using StudyChem.Models;

namespace StudyChem.Forms
{
    public partial class LoginForm : Form
    {
        public User LoggedInUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeLoginUI();
        }
        //Setting up UI for Login
        private void InitializeLoginUI()
        {
            //Form setup
            this.Text = "StudyChem - Login";
            this.Width = 300;
            this.Height = 200;
            //Draw/Create Componets
            //Username
            var lblUsername = new Label() { Text = "Username:", Left = 5, Top = 20 };
            var txtUsername = new TextBox() { Left = 105, Top = 20, Width = 150 };
            //Password
            var lblPassword = new Label() { Text = "Password:", Left = 5, Top = 60 };
            var txtPassword = new TextBox() { Left = 105, Top = 60, Width = 150, UseSystemPasswordChar = true };
            //Login system
            var btnLogin = new Button() { Text = "Login", Left = 100, Top = 100 };
            // Login button logic
            btnLogin.Click += (sender, e) =>
            {
                try
                {
                    var user = User.Load(txtUsername.Text);
                    // Verifying credentials
                    if (user != null && user.VerifyPassword(txtPassword.Text))
                    {
                        // Set active user and move to main form
                        LoggedInUser = user;
                        this.Hide();
                        var main = new MainForm(LoggedInUser);
                        main.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        // Log failed login attempt (invalid credentials)
                        ErrorLogger.LogMessage($"Login failed for user: {txtUsername.Text}");
                        MessageBox.Show("Invalid credentials.");
                    }
                }
                catch (Exception ex)
                {
                    // Log unexpected errors in login
                    ErrorLogger.Log("LoginForm -> Login Click", ex);
                    MessageBox.Show("An error occurred during login.");
                }
            };
            //Register referral.
            var btnRegister = new Button() { Text = "Register", Left = 180, Top = 100 };
            btnRegister.Click += (sender, e) =>
            {
                var reg = new RegisterForm();
                reg.ShowDialog();

            };
            //Adding controls to the form.
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(btnRegister);
        }
    }
}
