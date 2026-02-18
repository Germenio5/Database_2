namespace StudentSched
{
	partial class Dash
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			menuStrip1 = new MenuStrip();
			registerToolStripMenuItem = new ToolStripMenuItem();
			loginToolStripMenuItem = new ToolStripMenuItem();
			menuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.ImageScalingSize = new Size(20, 20);
			menuStrip1.Items.AddRange(new ToolStripItem[] { registerToolStripMenuItem, loginToolStripMenuItem });
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new Size(800, 28);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// registerToolStripMenuItem
			// 
			registerToolStripMenuItem.Name = "registerToolStripMenuItem";
			registerToolStripMenuItem.Size = new Size(77, 24);
			registerToolStripMenuItem.Text = "Register";
			registerToolStripMenuItem.Click += registerToolStripMenuItem_Click;
			// 
			// loginToolStripMenuItem
			// 
			loginToolStripMenuItem.Name = "loginToolStripMenuItem";
			loginToolStripMenuItem.Size = new Size(60, 24);
			loginToolStripMenuItem.Text = "Login";
			loginToolStripMenuItem.Click += loginToolStripMenuItem_Click;
			// 
			// Dash
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(menuStrip1);
			MainMenuStrip = menuStrip1;
			Name = "Dash";
			Text = "Dashboard";
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menuStrip1;
		private ToolStripMenuItem registerToolStripMenuItem;
		private ToolStripMenuItem loginToolStripMenuItem;
	}
}
