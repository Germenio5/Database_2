using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MySqlConnector;

class Program
{
    static string connectionString =
        "Server=localhost;Database=pracjoin;User ID=root;Password=;";

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            PrintHeader();
            PrintMenuTitle("MAIN MENU");
            PrintMenuItem("1", "Register");
            PrintMenuItem("2", "Login");
            PrintMenuItem("0", "Exit");
            PrintFooter();
            Console.Write("  Choose: ");
            string choice = Console.ReadLine()?.Trim() ?? "";

            if (choice == "1") Register();
            else if (choice == "2")
            {
                var user = Login();
                if (user != null) ShowDashboard(user);
            }
            else if (choice == "0") return;
            else PrintError("Invalid choice.");
        }
    }

    // ===================== DASHBOARD =====================
    static void ShowDashboard(LoggedInUser user)
    {
        while (true)
        {
            Console.Clear();
            PrintHeader();
            PrintMenuTitle("DASHBOARD");
            PrintInfoLine("User", user.FullName + $" (@{user.Username})");
            PrintInfoLine("Email", user.Email);
            PrintSeparatorLine();
            PrintMenuItem("1", "View All Tables");
            PrintMenuItem("2", "Search Records");
            PrintMenuItem("0", "Logout");
            PrintFooter();
            Console.Write("  Choose: ");
            string choice = Console.ReadLine()?.Trim() ?? "";

            if (choice == "1") ShowAllTables();
            else if (choice == "2") ShowSearchMenu();
            else if (choice == "0") return;
            else PrintError("Invalid choice.");
        }
    }

    static void ShowAllTables()
    {
        Console.Clear();
        PrintHeader();

        DrawTable("CUSTOMERS", GetCustomers(),
            new[] { "ID", "First Name", "Last Name", "Email", "Created At" },
            new[] { 6, 15, 15, 30, 12 });

        DrawTable("PRODUCTS", GetProducts(),
            new[] { "ID", "Product Name", "Price", "Stock" },
            new[] { 6, 28, 12, 10 });

        DrawTable("SALES", GetSales(),
            new[] { "Sale ID", "Cust ID", "Prod ID", "Qty", "Sale Date", "Total" },
            new[] { 8, 9, 9, 5, 12, 12 });

        DrawTable("ORDERS", GetOrders(),
            new[] { "Order ID", "Cust ID", "Order Date", "Total" },
            new[] { 10, 9, 12, 12 });

        DrawTable("USERS", GetUsers(),
            new[] { "ID", "Username", "Full Name", "Email", "Active", "Created At" },
            new[] { 6, 15, 22, 30, 8, 20 });

        DrawTable("ORDER ITEMS", GetOrderItems(),
            new[] { "Item ID", "Order ID", "Prod ID", "Qty", "Price" },
            new[] { 8, 10, 9, 5, 12 });

        DrawTable("LOGS", GetLogs(),
            new[] { "Log ID", "User ID", "Action", "Log Time" },
            new[] { 8, 9, 36, 20 });

        DrawTable("CUSTOMERS + ORDERS (INNER JOIN)", GetJoinedData(),
            new[] { "Order ID", "First Name", "Last Name", "Email", "Order Date", "Total" },
            new[] { 10, 14, 14, 30, 12, 12 });

        Console.WriteLine();
        PrintPrompt("Press any key to go back...");
        Console.ReadKey();
    }

    static void ShowSearchMenu()
    {
        while (true)
        {
            Console.Clear();
            PrintHeader();
            PrintMenuTitle("SEARCH RECORDS");
            PrintMenuItem("1", "Customers");
            PrintMenuItem("2", "Products");
            PrintMenuItem("3", "Sales");
            PrintMenuItem("4", "Orders");
            PrintMenuItem("5", "Users");
            PrintMenuItem("6", "Order Items");
            PrintMenuItem("7", "Logs");
            PrintMenuItem("0", "Back");
            PrintFooter();
            Console.Write("  Choose: ");
            string choice = Console.ReadLine()?.Trim() ?? "";

            if (choice == "0") return;

            Console.Write("  Search keyword: ");
            string keyword = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                PrintError("Search keyword is required.");
                continue;
            }

            Console.Clear();
            PrintHeader();

            if (choice == "1")
                DrawTable($"SEARCH CUSTOMERS — \"{keyword}\"", SearchCustomers(keyword),
                    new[] { "ID", "First Name", "Last Name", "Email", "Created At" },
                    new[] { 6, 15, 15, 30, 12 });
            else if (choice == "2")
                DrawTable($"SEARCH PRODUCTS — \"{keyword}\"", SearchProducts(keyword),
                    new[] { "ID", "Product Name", "Price", "Stock" },
                    new[] { 6, 28, 12, 10 });
            else if (choice == "3")
                DrawTable($"SEARCH SALES — \"{keyword}\"", SearchSales(keyword),
                    new[] { "Sale ID", "Cust ID", "Prod ID", "Qty", "Sale Date", "Total" },
                    new[] { 8, 9, 9, 5, 12, 12 });
            else if (choice == "4")
                DrawTable($"SEARCH ORDERS — \"{keyword}\"", SearchOrders(keyword),
                    new[] { "Order ID", "Cust ID", "Order Date", "Total" },
                    new[] { 10, 9, 12, 12 });
            else if (choice == "5")
                DrawTable($"SEARCH USERS — \"{keyword}\"", SearchUsers(keyword),
                    new[] { "ID", "Username", "Full Name", "Email", "Active", "Created At" },
                    new[] { 6, 15, 22, 30, 8, 20 });
            else if (choice == "6")
                DrawTable($"SEARCH ORDER ITEMS — \"{keyword}\"", SearchOrderItems(keyword),
                    new[] { "Item ID", "Order ID", "Prod ID", "Qty", "Price" },
                    new[] { 8, 10, 9, 5, 12 });
            else if (choice == "7")
                DrawTable($"SEARCH LOGS — \"{keyword}\"", SearchLogs(keyword),
                    new[] { "Log ID", "User ID", "Action", "Log Time" },
                    new[] { 8, 9, 36, 20 });
            else
            {
                PrintError("Invalid choice.");
                continue;
            }

            Console.WriteLine();
            PrintPrompt("Press any key to go back...");
            Console.ReadKey();
        }
    }

    // ===================== AUTH =====================
    static void Register()
    {
        Console.Clear();
        PrintHeader();
        PrintMenuTitle("REGISTER");

        Console.Write("  Username        : ");
        string username = (Console.ReadLine() ?? "").Trim();

        Console.Write("  Full name       : ");
        string fullName = (Console.ReadLine() ?? "").Trim();

        Console.Write("  Email           : ");
        string email = (Console.ReadLine() ?? "").Trim();

        Console.Write("  Password        : ");
        string password = ReadPassword();

        Console.Write("  Confirm password: ");
        string confirm = ReadPassword();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirm))
        {
            PrintError("All fields are required.");
            return;
        }

        if (!email.Contains("@") || !email.Contains(".") || email.IndexOf("@") > email.LastIndexOf("."))
        {
            PrintError("Invalid email format.");
            return;
        }

        if (password.Length < 8)
        {
            PrintError("Password must be at least 8 characters.");
            return;
        }

        if (password != confirm) { PrintError("Passwords do not match."); return; }
        if (UsernameExists(username)) { PrintError("Username already exists."); return; }
        if (EmailExists(email)) { PrintError("Email already exists."); return; }

        string saltB64 = GenerateSaltBase64(16);
        string hashB64 = HashPasswordPbkdf2Base64(password, saltB64);

        using var conn = new MySqlConnection(connectionString);
        conn.Open();
        using var cmd = new MySqlCommand(@"
            INSERT INTO users (username, password_hash, salt, full_name, email, is_active)
            VALUES (@username, @hash, @salt, @fullname, @email, 1);", conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@hash", hashB64);
        cmd.Parameters.AddWithValue("@salt", saltB64);
        cmd.Parameters.AddWithValue("@fullname", fullName);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.ExecuteNonQuery();

        PrintSuccess("Registration successful!");
    }

    static LoggedInUser? Login()
    {
        Console.Clear();
        PrintHeader();
        PrintMenuTitle("LOGIN");

        Console.Write("  Username or Email: ");
        string loginValue = (Console.ReadLine() ?? "").Trim();

        Console.Write("  Password         : ");
        string password = ReadPassword();

        if (string.IsNullOrWhiteSpace(loginValue) || string.IsNullOrWhiteSpace(password))
        {
            PrintError("Both fields are required.");
            return null;
        }

        using var conn = new MySqlConnection(connectionString);
        conn.Open();
        using var cmd = new MySqlCommand(@"
            SELECT user_id, username, full_name, email, password_hash, salt, is_active
            FROM users WHERE username = @v OR email = @v LIMIT 1;", conn);
        cmd.Parameters.AddWithValue("@v", loginValue);
        using var r = cmd.ExecuteReader();

        if (!r.Read())
        {
            PrintError("Invalid username/email or password.");
            return null;
        }

        if (Convert.ToInt32(r["is_active"]) != 1)
        {
            PrintError("Account is inactive.");
            return null;
        }

        string storedSalt = r["salt"]?.ToString() ?? "";
        string storedHash = r["password_hash"]?.ToString() ?? "";

        if (!VerifyPasswordPbkdf2(password, storedSalt, storedHash))
        {
            PrintError("Invalid username/email or password.");
            return null;
        }

        return new LoggedInUser
        {
            UserId = Convert.ToInt32(r["user_id"]),
            Username = r["username"]?.ToString() ?? "",
            FullName = r["full_name"]?.ToString() ?? "",
            Email = r["email"]?.ToString() ?? ""
        };
    }

    static bool UsernameExists(string username)
    {
        using var conn = new MySqlConnection(connectionString);
        conn.Open();
        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE username=@u", conn);
        cmd.Parameters.AddWithValue("@u", username);
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    static bool EmailExists(string email)
    {
        using var conn = new MySqlConnection(connectionString);
        conn.Open();
        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE email=@e", conn);
        cmd.Parameters.AddWithValue("@e", email);
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    // ===================== PASSWORD (PBKDF2) =====================
    static string GenerateSaltBase64(int bytes)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(salt);
    }

    static string HashPasswordPbkdf2Base64(string password, string saltBase64)
    {
        byte[] salt = Convert.FromBase64String(saltBase64);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password, salt: salt, iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256, outputLength: 32);
        return Convert.ToBase64String(hash);
    }

    static bool VerifyPasswordPbkdf2(string password, string saltBase64, string expectedHashBase64)
    {
        string actual = HashPasswordPbkdf2Base64(password, saltBase64);
        byte[] a = Convert.FromBase64String(actual);
        byte[] b = Convert.FromBase64String(expectedHashBase64);
        return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
    }

    static string ReadPassword()
    {
        var sb = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter) { Console.WriteLine(); break; }
            if (key.Key == ConsoleKey.Backspace) { if (sb.Length > 0) { sb.Length--; Console.Write("\b \b"); } continue; }
            if (!char.IsControl(key.KeyChar)) { sb.Append(key.KeyChar); Console.Write("*"); }
        }
        return sb.ToString();
    }

    class LoggedInUser
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
    }

    // ===================== DATA METHODS =====================
    static List<string[]> GetCustomers()
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT customer_id, first_name, last_name, email, created_at FROM customers ORDER BY customer_id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["customer_id"]?.ToString()??"", r["first_name"]?.ToString()??"",
                             r["last_name"]?.ToString()??"",   r["email"]?.ToString()??"",
                             FormatDate(r["created_at"]) });
        return rows;
    }

    static List<string[]> GetProducts()
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT product_id, product_name, price, stock_quantity FROM products ORDER BY product_id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["product_id"]?.ToString()??"", r["product_name"]?.ToString()??"",
                             Convert.ToDecimal(r["price"]).ToString("0.00"),
                             r["stock_quantity"]?.ToString()??"" });
        return rows;
    }

    static List<string[]> GetSales()
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT sale_id, customer_id, product_id, quantity, sale_date, total_amount FROM sales ORDER BY sale_id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["sale_id"]?.ToString()??"",    r["customer_id"]?.ToString()??"",
                             r["product_id"]?.ToString()??"", r["quantity"]?.ToString()??"",
                             FormatDate(r["sale_date"]),       Convert.ToDecimal(r["total_amount"]).ToString("0.00") });
        return rows;
    }

    static List<string[]> GetOrders()
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT order_id, customer_id, order_date, total FROM orders ORDER BY order_id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["order_id"]?.ToString()??"", r["customer_id"]?.ToString()??"",
                             FormatDate(r["order_date"]),    Convert.ToDecimal(r["total"]).ToString("0.00") });
        return rows;
    }

    static List<string[]> GetUsers()
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT user_id, username, full_name, email, is_active, created_at FROM users ORDER BY user_id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["user_id"]?.ToString()??"",   r["username"]?.ToString()??"",
                             r["full_name"]?.ToString()??"", r["email"]?.ToString()??"",
                             Convert.ToInt32(r["is_active"]) == 1 ? "Yes" : "No",
                             FormatDateTime(r["created_at"]) });
        return rows;
    }

    static List<string[]> GetJoinedData()
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(@"
            SELECT o.order_id, c.first_name, c.last_name, c.email, o.order_date, o.total
            FROM orders o INNER JOIN customers c ON o.customer_id = c.customer_id
            ORDER BY o.order_date, o.order_id;", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["order_id"]?.ToString()??"",  r["first_name"]?.ToString()??"",
                             r["last_name"]?.ToString()??"", r["email"]?.ToString()??"",
                             FormatDate(r["order_date"]),    Convert.ToDecimal(r["total"]).ToString("0.00") });
        return rows;
    }

    static List<string[]> GetOrderItems()
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT order_item_id, order_id, product_id, quantity, price FROM order_items ORDER BY order_item_id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["order_item_id"]?.ToString()??"", r["order_id"]?.ToString()??"",
                             r["product_id"]?.ToString()??"",    r["quantity"]?.ToString()??"",
                             Convert.ToDecimal(r["price"]).ToString("0.00") });
        return rows;
    }

    static List<string[]> GetLogs()
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT log_id, user_id, action, log_time FROM logs ORDER BY log_id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["log_id"]?.ToString()??"", r["user_id"]?.ToString()??"",
                             r["action"]?.ToString()??"", FormatDateTime(r["log_time"]) });
        return rows;
    }

    // ===================== SEARCH METHODS =====================
    static List<string[]> SearchCustomers(string kw)
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(@"
            SELECT customer_id, first_name, last_name, email, created_at FROM customers
            WHERE CAST(customer_id AS CHAR) LIKE @q OR first_name LIKE @q OR last_name LIKE @q
               OR email LIKE @q OR CAST(created_at AS CHAR) LIKE @q ORDER BY customer_id;", conn);
        cmd.Parameters.AddWithValue("@q", "%" + kw + "%");
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["customer_id"]?.ToString()??"", r["first_name"]?.ToString()??"",
                             r["last_name"]?.ToString()??"",   r["email"]?.ToString()??"",
                             FormatDate(r["created_at"]) });
        return EnsureRows(rows);
    }

    static List<string[]> SearchProducts(string kw)
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(@"
            SELECT product_id, product_name, price, stock_quantity FROM products
            WHERE CAST(product_id AS CHAR) LIKE @q OR product_name LIKE @q
               OR CAST(price AS CHAR) LIKE @q OR CAST(stock_quantity AS CHAR) LIKE @q ORDER BY product_id;", conn);
        cmd.Parameters.AddWithValue("@q", "%" + kw + "%");
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["product_id"]?.ToString()??"", r["product_name"]?.ToString()??"",
                             Convert.ToDecimal(r["price"]).ToString("0.00"),
                             r["stock_quantity"]?.ToString()??"" });
        return EnsureRows(rows);
    }

    static List<string[]> SearchSales(string kw)
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(@"
            SELECT sale_id, customer_id, product_id, quantity, sale_date, total_amount FROM sales
            WHERE CAST(sale_id AS CHAR) LIKE @q OR CAST(customer_id AS CHAR) LIKE @q
               OR CAST(product_id AS CHAR) LIKE @q OR CAST(quantity AS CHAR) LIKE @q
               OR CAST(sale_date AS CHAR) LIKE @q OR CAST(total_amount AS CHAR) LIKE @q ORDER BY sale_id;", conn);
        cmd.Parameters.AddWithValue("@q", "%" + kw + "%");
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["sale_id"]?.ToString()??"",    r["customer_id"]?.ToString()??"",
                             r["product_id"]?.ToString()??"", r["quantity"]?.ToString()??"",
                             FormatDate(r["sale_date"]),       Convert.ToDecimal(r["total_amount"]).ToString("0.00") });
        return EnsureRows(rows);
    }

    static List<string[]> SearchOrders(string kw)
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(@"
            SELECT order_id, customer_id, order_date, total FROM orders
            WHERE CAST(order_id AS CHAR) LIKE @q OR CAST(customer_id AS CHAR) LIKE @q
               OR CAST(order_date AS CHAR) LIKE @q OR CAST(total AS CHAR) LIKE @q ORDER BY order_id;", conn);
        cmd.Parameters.AddWithValue("@q", "%" + kw + "%");
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["order_id"]?.ToString()??"", r["customer_id"]?.ToString()??"",
                             FormatDate(r["order_date"]),    Convert.ToDecimal(r["total"]).ToString("0.00") });
        return EnsureRows(rows);
    }

    static List<string[]> SearchUsers(string kw)
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(@"
            SELECT user_id, username, full_name, email, is_active, created_at FROM users
            WHERE CAST(user_id AS CHAR) LIKE @q OR username LIKE @q OR full_name LIKE @q
               OR email LIKE @q OR CAST(is_active AS CHAR) LIKE @q
               OR CAST(created_at AS CHAR) LIKE @q ORDER BY user_id;", conn);
        cmd.Parameters.AddWithValue("@q", "%" + kw + "%");
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["user_id"]?.ToString()??"",   r["username"]?.ToString()??"",
                             r["full_name"]?.ToString()??"", r["email"]?.ToString()??"",
                             Convert.ToInt32(r["is_active"]) == 1 ? "Yes" : "No",
                             FormatDateTime(r["created_at"]) });
        return EnsureRows(rows);
    }

    static List<string[]> SearchOrderItems(string kw)
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(@"
            SELECT order_item_id, order_id, product_id, quantity, price FROM order_items
            WHERE CAST(order_item_id AS CHAR) LIKE @q OR CAST(order_id AS CHAR) LIKE @q
               OR CAST(product_id AS CHAR) LIKE @q OR CAST(quantity AS CHAR) LIKE @q
               OR CAST(price AS CHAR) LIKE @q ORDER BY order_item_id;", conn);
        cmd.Parameters.AddWithValue("@q", "%" + kw + "%");
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["order_item_id"]?.ToString()??"", r["order_id"]?.ToString()??"",
                             r["product_id"]?.ToString()??"",    r["quantity"]?.ToString()??"",
                             Convert.ToDecimal(r["price"]).ToString("0.00") });
        return EnsureRows(rows);
    }

    static List<string[]> SearchLogs(string kw)
    {
        var rows = new List<string[]>();
        using var conn = new MySqlConnection(connectionString); conn.Open();
        using var cmd = new MySqlCommand(@"
            SELECT log_id, user_id, action, log_time FROM logs
            WHERE CAST(log_id AS CHAR) LIKE @q OR CAST(user_id AS CHAR) LIKE @q
               OR action LIKE @q OR CAST(log_time AS CHAR) LIKE @q ORDER BY log_id;", conn);
        cmd.Parameters.AddWithValue("@q", "%" + kw + "%");
        using var r = cmd.ExecuteReader();
        while (r.Read())
            rows.Add(new[] { r["log_id"]?.ToString()??"", r["user_id"]?.ToString()??"",
                             r["action"]?.ToString()??"", FormatDateTime(r["log_time"]) });
        return EnsureRows(rows);
    }

    // ===================== HELPERS =====================
    static List<string[]> EnsureRows(List<string[]> rows)
    {
        if (rows.Count == 0) rows.Add(new[] { "No records found." });
        return rows;
    }

    static string FormatDate(object? value)
    {
        if (value == null || value == DBNull.Value) return "";
        return Convert.ToDateTime(value).ToString("yyyy-MM-dd");
    }

    static string FormatDateTime(object? value)
    {
        if (value == null || value == DBNull.Value) return "";
        return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm");
    }

    // ===================== UI / DESIGN HELPERS =====================
    static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine("  ╔══════════════════════════════════════╗");
        Console.Write("  ║");
        Console.ForegroundColor = ConsoleColor.White;
        string title = "DB MANAGER";
        int boxInner = 38;
        string centered = title.PadLeft((boxInner + title.Length) / 2).PadRight(boxInner);
        Console.Write(centered);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("║");
        Console.WriteLine("  ╚══════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void PrintMenuTitle(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  ┌─ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(title);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  │");
        Console.ResetColor();
    }

    static void PrintMenuItem(string key, string label)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  │  [");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(key);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("]  ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(label);
        Console.ResetColor();
    }

    static void PrintInfoLine(string label, string value)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"  │  {label,-8}: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(value);
        Console.ResetColor();
    }

    static void PrintSeparatorLine()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  │");
        Console.ResetColor();
    }

    static void PrintFooter()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  │");
        Console.WriteLine("  └────────────────────────────────────");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  [!] {message}");
        Console.ResetColor();
        Console.WriteLine("      Press any key...");
        Console.ReadKey();
    }

    static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  [+] {message}");
        Console.ResetColor();
        Console.WriteLine("      Press any key...");
        Console.ReadKey();
    }

    static void PrintPrompt(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"  {message}");
        Console.ResetColor();
    }

    static void DrawTable(string title, List<string[]> rows, string[] headers, int[] widths)
    {
        int totalInner = widths.Sum() + widths.Length - 1;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  ╔");
        Console.Write(new string('═', totalInner));
        Console.WriteLine("╗");

        string centeredTitle = title.Length >= totalInner
            ? title.Substring(0, totalInner)
            : title.PadLeft((totalInner + title.Length) / 2).PadRight(totalInner);

        Console.Write("  ║");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(centeredTitle);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("║");

        PrintBorder(widths, "╠", "╦", "╣");
        PrintRow(headers, widths);
        PrintBorder(widths, "╠", "╬", "╣");

        if (rows.Count == 1 && rows[0].Length == 1)
        {
            string msg = rows[0][0].PadRight(totalInner);
            if (msg.Length > totalInner) msg = msg.Substring(0, totalInner);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  ║");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("║");
        }
        else
        {
            foreach (var row in rows)
                PrintRow(row, widths);
        }

        PrintBorder(widths, "╚", "╩", "╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void PrintBorder(int[] widths, string left, string mid, string right)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  " + left);
        for (int i = 0; i < widths.Length; i++)
        {
            Console.Write(new string('═', widths[i]));
            Console.Write(i == widths.Length - 1 ? right : mid);
        }
        Console.WriteLine();
    }

    static void PrintRow(string[] cols, int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  ║");
        for (int i = 0; i < widths.Length; i++)
        {
            string cell = i < cols.Length ? cols[i] ?? "" : "";
            string value = cell.Length > widths[i]
                ? cell.Substring(0, widths[i] - 1) + "…"
                : cell.PadRight(widths[i]);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(value);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("║");
        }
        Console.WriteLine();
    }
}