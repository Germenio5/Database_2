using System;
using System.Windows.Forms;

namespace StudentSched
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			AppContext appContext = new AppContext();
			Application.Run(appContext);
		}
	}

	// Custom ApplicationContext
	public class AppContext : ApplicationContext
	{
		public AppContext()
		{
			// Start with Dash
			Dash dashForm = new Dash();
			dashForm.LoginSucceeded += OnLoginSucceeded;

			// Exit if Dash is closed before login
			dashForm.FormClosed += (s, e) => ExitThread();

			dashForm.Show();
		}

		private void OnLoginSucceeded(string username)
		{
			// Use the correct form type name (MainDash)
			MainDash mainDash = new MainDash(username);

			// Exit application when main dashboard closes
			mainDash.FormClosed += (s, e) => ExitThread();

			mainDash.Show();
		}
	}
}