using System;

namespace GetLyrics
{
    public partial class App : System.Windows.Application
    {
        [STAThread()]
        public static void Main()
        {
            App app = new App();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            app.InitializeComponent();
            app.Run();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Helper.Log(e.ExceptionObject as Exception);
        }
    }
}
