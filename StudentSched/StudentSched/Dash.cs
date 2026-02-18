using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StudentSched
{
	public partial class Dash : Form
	{
		// Event to notify login success
		public event Action<string> LoginSucceeded;
		public Dash()
		{
			InitializeComponent();
		}

		private void loginToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Pass 'this' as parent
			FrmLogin loginForm = new FrmLogin(this);

			// Subscribe to FrmLogin event
			loginForm.LoginSuccess += (username) =>
			{
				// Raise Dash's event
				LoginSucceeded?.Invoke(username);
			};

			loginForm.ShowDialog();
		}

		private void registerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Open the registration form as a modal dialog, parented to this form.
			using (var registerForm = new RegisterForm())
			{
				registerForm.ShowDialog(this);
			}
		}
	}
}
