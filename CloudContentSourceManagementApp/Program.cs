namespace CloudContentSourceManagementApp
{
    internal static class Program
    {
        public static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CloudContentSourceManagementApp");
        public static readonly string DbFile = Path.Combine(AppFolder, "users.db");
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            if (!Directory.Exists(AppFolder))
                Directory.CreateDirectory(AppFolder);

            // Đảm bảo database và bảng đã tạo
            using (var db = new SqlDbContext.DriveDbContext())
            {
                db.Database.EnsureCreated();
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
            ApplicationConfiguration.Initialize();
            Application.Run(new RegisterForm());
        }
    }
}