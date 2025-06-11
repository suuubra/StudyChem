using System;
using System.Windows.Forms;
using StudyChem.Models;

namespace StudyChem.Forms
{
    public partial class RegisterForm : Form
    {
        //Initalise
        public RegisterForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeRegisterUI();
        }

        //Setup for Register UI
        private void InitializeRegisterUI()
        {
            //Form Sizing
            this.Text = "StudyChem - Register";
            this.Width = 320;
            this.Height = 270;
            //Construct the labels,etc..
            //Username
            var lblUsername = new Label() { Text = "Username:", Left = 5, Top = 20 };
            var txtUsername = new TextBox() { Left = 105, Top = 20, Width = 180 };
            //Password
            var lblPassword = new Label() { Text = "Password:", Left = 5, Top = 60 };
            var txtPassword = new TextBox() { Left = 105, Top = 60, Width = 180, UseSystemPasswordChar = true };
            //Confirm
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
            //Create Button setup
            var btnCreate = new Button() { Text = "Create", Left = 100, Top = 160 };
            btnCreate.Click += (sender, e) =>
            {
                //User, Password, and confirmedPassword strings
                string user = txtUsername.Text.Trim();
                string pass = txtPassword.Text;
                string confirm = txtConfirm.Text;

                //Checking if the user or password is empty, if it is displaying an error.
                if (user == "" || pass == "")
                {
                    MessageBox.Show("Username and password cannot be empty.");
                    return;
                }
                //Checking if the confirm password is the same as the orginial entered password, if it isn't then display an error.
                if (pass != confirm)
                {
                    MessageBox.Show("Passwords do not match.");
                    return;
                }
                //Checking if a user with that name already exists if so display an error.
                if (User.Load(user) != null)
                {
                    MessageBox.Show("User already exists.");
                    return;
                }
                //Setup new User Data
                var newUser = new User();
                newUser.Username = user;
                newUser.PasswordHash = User.Hash(pass);
                newUser.Results = new System.Collections.Generic.List<UserResult>();
                newUser.Save();
                MessageBox.Show("User created successfully.");
                this.Close();
            };
            //Adding controls to the form, like buttons, textboxes, etc.
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