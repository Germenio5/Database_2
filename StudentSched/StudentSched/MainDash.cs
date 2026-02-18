using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace StudentSched
{
	public class MainDash : Form
	{
		private Label lblWelcome;
		private DataGridView dgvCustomers;
		private DataGridView dgvCustomerOrders;
		private TableLayoutPanel tableLayout;
		private Panel panelLeft;
		private Panel panelRight;

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MainDash
            // 
            ClientSize = new Size(782, 431);
            Name = "MainDash";
            ResumeLayout(false);

        }

		public MainDash(string username)
		{
			Text = "Main Dashboard";
			StartPosition = FormStartPosition.CenterScreen;
			Size = new Size(800, 600);

			// Welcome label at top
			lblWelcome = new Label
			{
				AutoSize = false,
				Height = 40,
				Text = $"Welcome, {username}",
				TextAlign = ContentAlignment.MiddleLeft,
				Dock = DockStyle.Top,
				Padding = new Padding(10, 0, 0, 0),
				Font = new Font("Segoe UI", 12, FontStyle.Regular)
			};

			// TableLayoutPanel to host two equal-sized boxed areas with padding
			tableLayout = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				RowCount = 1,
				Padding = new Padding(10)
			};
			tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

		panelLeft = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), BorderStyle = BorderStyle.None };
		panelRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), BorderStyle = BorderStyle.None };

			dgvCustomers = new DataGridView
			{
				Dock = DockStyle.Fill,
				ReadOnly = true,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
				BorderStyle = BorderStyle.None
			};

			dgvCustomerOrders = new DataGridView
			{
				Dock = DockStyle.Fill,
				ReadOnly = true,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
				BorderStyle = BorderStyle.None
			};

			panelLeft.Controls.Add(dgvCustomers);
			panelRight.Controls.Add(dgvCustomerOrders);

			tableLayout.Controls.Add(panelLeft, 0, 0);
			tableLayout.Controls.Add(panelRight, 1, 0);

			Controls.Add(tableLayout);
			Controls.Add(lblWelcome);

			// For now populate with sample data. Replace with real DB queries as needed.
			PopulateSampleData();
		}

		private void PopulateSampleData()
		{
			// Sample customers
			var dtCustomers = new DataTable();
			dtCustomers.Columns.Add("CustomerId", typeof(int));
			dtCustomers.Columns.Add("Name", typeof(string));
			dtCustomers.Columns.Add("Email", typeof(string));

			dtCustomers.Rows.Add(1, "Alice Smith", "alice@example.com");
			dtCustomers.Rows.Add(2, "Bob Johnson", "bob@example.com");
			dtCustomers.Rows.Add(3, "Carol Lee", "carol@example.com");

			// Sample orders and an inner-join result of customers and orders
			var dtOrders = new DataTable();
			dtOrders.Columns.Add("OrderId", typeof(int));
			dtOrders.Columns.Add("CustomerId", typeof(int));
			dtOrders.Columns.Add("OrderDate", typeof(DateTime));
			dtOrders.Columns.Add("Amount", typeof(decimal));

			dtOrders.Rows.Add(101, 1, new DateTime(2026, 1, 12), 49.99m);
			dtOrders.Rows.Add(102, 2, new DateTime(2026, 2, 5), 19.5m);
			dtOrders.Rows.Add(103, 1, new DateTime(2026, 3, 3), 5.0m);

			// Create inner join result table
			var dtJoin = new DataTable();
			dtJoin.Columns.Add("CustomerId", typeof(int));
			dtJoin.Columns.Add("Name", typeof(string));
			dtJoin.Columns.Add("OrderId", typeof(int));
			dtJoin.Columns.Add("OrderDate", typeof(DateTime));
			dtJoin.Columns.Add("Amount", typeof(decimal));

			foreach (DataRow c in dtCustomers.Rows)
			{
				int cid = (int)c["CustomerId"];
				var matches = dtOrders.Select($"CustomerId = {cid}");
				foreach (var o in matches)
				{
					dtJoin.Rows.Add(cid, c["Name"], o["OrderId"], o["OrderDate"], o["Amount"]);
				}
			}

			dgvCustomers.DataSource = dtCustomers;
			dgvCustomerOrders.DataSource = dtJoin;
		}
    }
}