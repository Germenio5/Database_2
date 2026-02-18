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
	public partial class RegisterForm : Form
	{
		public RegisterForm()
		{
			InitializeComponent();
		}

		private void btnRegister_Click(object sender, EventArgs e)
		{
			string username = txtUsername.Text.Trim();
			string password = txtPassword.Text;
			string confirmPassword = txtConfirm.Text;
			string fullName = txtFullName.Text.Trim();
			string email = txtEmail.Text.Trim();

			// Basic validation
			if (string.IsNullOrEmpty(username) ||
				string.IsNullOrEmpty(password) ||
				string.IsNullOrEmpty(confirmPassword))
			{
				MessageBox.Show(
					"Please fill all required fields",
					"Registration",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);
				return;
			}

			if (password != confirmPassword)
			{
				MessageBox.Show(
					"Passwords do not match",
					"Registration",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);
				return;
			}

			// Create hash + salt
			PasswordHelper.CreatePasswordHash(
				password,
				out string passwordHash,
				out string salt
			);

			string connStr = "server=localhost;database=pracjoin;uid=root;pwd=;";
			string sql = @"INSERT INTO users
                           (username, password_hash, salt, full_name, email)
                           VALUES (@u, @ph, @s, @fn, @em)";

			try
			{
				using (MySqlConnection conn = new MySqlConnection(connStr))
				using (MySqlCommand cmd = new MySqlCommand(sql, conn))
				{
					cmd.Parameters.AddWithValue("@u", username);
					cmd.Parameters.AddWithValue("@ph", passwordHash);
					cmd.Parameters.AddWithValue("@s", salt);
					cmd.Parameters.AddWithValue("@fn", fullName);
					cmd.Parameters.AddWithValue("@em", email);

					conn.Open();
					cmd.ExecuteNonQuery();
				}

				MessageBox.Show(
					"Registration successful!",
					"Success",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				);

				this.Close();
			}
			catch (MySqlException ex)
			{
				if (ex.Number == 1062) // Duplicate username
				{
					MessageBox.Show(
						"Username already exists",
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
				else
				{
					MessageBox.Show(
						"Database error: " + ex.Message,
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
