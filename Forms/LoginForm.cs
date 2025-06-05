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

        private void InitializeLoginUI()
        {
            this.Text = "StudyChem - Login";
            this.Width = 300;
            this.Height = 200;

            var lblUsername = new Label() { Text = "Username:", Left = 5, Top = 20 };
            var txtUsername = new TextBox() { Left = 105, Top = 20, Width = 150 };
            var lblPassword = new Label() { Text = "Password:", Left = 5, Top = 60 };
            var txtPassword = new TextBox() { Left = 105, Top = 60, Width = 150, UseSystemPasswordChar = true };
            //Login system
            var btnLogin = new Button() { Text = "Login", Left = 100, Top = 100 };
            btnLogin.Click += (sender, e) =>
            {
                var user = User.Load(txtUsername.Text);
                if (user != null && user.VerifyPassword(txtPassword.Text))
                {
                    //Opening new form whilst also closing this form.
                    LoggedInUser = user;
                    this.Hide();
                    var main = new MainForm(LoggedInUser);
                    main.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid credentials.");
                }
            };
            //Register referral.
            var btnRegister = new Button() { Text = "Register", Left = 180, Top = 100 };
            btnRegister.Click += (sender, e) =>
            {
                var reg = new RegisterForm();
                reg.ShowDialog();

            };
            //Comment
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
