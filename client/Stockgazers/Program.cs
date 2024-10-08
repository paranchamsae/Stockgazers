namespace Stockgazers
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Common common = new();
            Application.Run(new LoginForm(common));

            if (common.IsAppLogin)
            {
#if DEBUG
#else
                string cachePath = Path.Combine(Application.StartupPath, @"Stockgazers.exe.WebView2\EBWebView");
                DirectoryInfo dir = new(cachePath);
                if (dir.Exists)
                    dir.Delete(true);
#endif
                Application.Run(new MainForm(common));
            }
        }
    }
}