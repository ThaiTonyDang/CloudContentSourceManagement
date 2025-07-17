namespace CloudContentSourceManagementApp
{
    internal static class Program
    {
        public static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CloudContentSourceManagementApp");
        public static readonly string DbUser = Path.Combine(AppFolder, "users.db");
        public static readonly string DbAppProfile = Path.Combine(AppFolder, "AppProfile.db");
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

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
            ApplicationConfiguration.Initialize();
            Application.Run(new RegisterForm());
        }
    }
}