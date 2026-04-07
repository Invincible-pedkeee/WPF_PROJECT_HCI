using BasketballBallBrandsCMS.Helpers;
using BasketballBallBrandsCMS.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace LoginWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User loggedUser;
        public MainWindow(User user)
        {
            InitializeComponent();
            AppPaths.EnsureDirectoriesExist();
            // MessageBox.Show(AppPaths.ResourcesFolder);
            loggedUser = user;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}