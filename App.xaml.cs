using System.Windows;

namespace BasketballBallBrandsCMS
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }
}
