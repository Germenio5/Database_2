using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace StudentSched
{
    public partial class FrmLogin : Form
    {
        private Form parentForm;

        // Event to notify parent form of successful login
        public event Action<string> LoginSuccess;
        public FrmLogin(Form parent)
        {
            InitializeComponent();
            parentForm = parent;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Please enter username and password",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string connStr = "server=localhost;database=pracjoin;uid=root;pwd=;";
            string sql = @"SELECT password_hash, salt
                           FROM users
                           WHERE username = @username
                           AND is_active = 1";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string dbHash = reader.GetString("password_hash");
                        string dbSalt = reader.GetString("salt");

                        if (PasswordHelper.VerifyPassword(password, dbHash, dbSalt))
                        {
                            // Notify parent form about successful login
                            LoginSuccess?.Invoke(username);

                            // Hide login form
                            this.Hide();
                            parentForm?.Hide();

                            // Main dashboard will be opened by Dash or ApplicationContext
                        }
                        else
                        {
                            ShowLoginError();
                        }
                    }
                    else
                    {
                        ShowLoginError();
                    }
                }
            }
        }

        private void ShowLoginError()
        {
            MessageBox.Show(
                "Invalid username or password",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            txtPassword.Clear();
            txtPassword.Focus();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
