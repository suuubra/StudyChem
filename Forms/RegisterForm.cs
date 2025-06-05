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
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeRegisterUI();
        }

        private void InitializeRegisterUI()
        {
            this.Text = "StudyChem - Register";
            this.Width = 320;
            this.Height = 270;
            //Construct the labels,etc..
            var lblUsername = new Label() { Text = "Username:", Left = 5, Top = 20 };
            var txtUsername = new TextBox() { Left = 105, Top = 20, Width = 180 };

            var lblPassword = new Label() { Text = "Password:", Left = 5, Top = 60 };
            var txtPassword = new TextBox() { Left = 105, Top = 60, Width = 180, UseSystemPasswordChar = true };

            var lblConfirm = new Label() { Text = "Confirm:", Left = 5, Top = 100 };
            var txtConfirm = new TextBox() { Left = 105, Top = 100, Width = 180, UseSystemPasswordChar = true };
            //Check Password Toggle
            var chkShow = new CheckBox() { Text = "Show Password", Left = 100, Top = 130 };
            chkShow.CheckedChanged += (s, e) =>
            {
                bool visible = chkShow.Checked;
                txtPassword.UseSystemPasswordChar = !visible;
                txtConfirm.UseSystemPasswordChar = !visible;
            };
            //Create user system
            var btnCreate = new Button() { Text = "Create", Left = 100, Top = 160 };
            btnCreate.Click += (sender, e) =>
            {
                string user = txtUsername.Text.Trim();
                string pass = txtPassword.Text;
                string confirm = txtConfirm.Text;

                if (user == "" || pass == "")
                {
                    MessageBox.Show("Username and password cannot be empty.");
                    return;
                }

                if (pass != confirm)
                {
                    MessageBox.Show("Passwords do not match.");
                    return;
                }

                if (User.Load(user) != null)
                {
                    MessageBox.Show("User already exists.");
                    return;
                }

                var newUser = new User();
                newUser.Username = user;
                newUser.PasswordHash = User.Hash(pass);
                newUser.Results = new System.Collections.Generic.List<UserResult>();
                newUser.Save();
                MessageBox.Show("User created successfully.");
                this.Close();
            };
            //Adding controls to the form.
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(lblConfirm);
            Controls.Add(txtConfirm);
            Controls.Add(chkShow);
            Controls.Add(btnCreate);
        }
    }
}