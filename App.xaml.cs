using System.Configuration;
using System.Data;
using System.Windows;

namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private int CurrentUserId;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
         
            Window1 loginWindow = new Window1();
            if (loginWindow.ShowDialog() == true) // If login is successful
            {
                // Show the main window
                MainWindow mainWindow = new MainWindow(CurrentUserId);
                mainWindow.Show();
            }
        
        }
    }

}
