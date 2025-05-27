using System;
using System.Windows.Forms;
using StudyChem.Models;

namespace StudyChem.Forms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            InitializeRegisterUI();
        }

        private void InitializeRegisterUI()
        {
            this.Text = "StudyChem - Register";
            this.Width = 300;
            this.Height = 200;

            var lblUsername = new Label() { Text = "New Username:", Left = 10, Top = 20 };
            var txtUsername = new TextBox() { Left = 120, Top = 20, Width = 150 };
            var lblPassword = new Label() { Text = "Password:", Left = 10, Top = 60 };
            var txtPassword = new TextBox() { Left = 120, Top = 60, Width = 150, UseSystemPasswordChar = true };

            var btnCreate = new Button() { Text = "Create Account", Left = 120, Top = 100 };
            btnCreate.Click += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please enter both fields.");
                    return;
                }

                var existing = User.Load(txtUsername.Text);
                if (existing != null)
                {
                    MessageBox.Show("User already exists.");
                    return;
                }

                var newUser = new User
                {
                    Username = txtUsername.Text,
                    PasswordHash = User.Hash(txtPassword.Text)
                };
                newUser.Save();
                MessageBox.Show("Account created.");
                Close();
            };

            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnCreate);
        }
    }
}