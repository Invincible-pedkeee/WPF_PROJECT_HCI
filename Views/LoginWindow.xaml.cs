using BasketballBallBrandsCMS.Helpers;
using BasketballBallBrandsCMS.Models;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BasketballBallBrandsCMS.Views
{
    public partial class LoginWindow : Window
    {
        private List<User> users;
        private DataIO serializer;

        public LoginWindow()
        {
            InitializeComponent();

            serializer = new DataIO();
            users = new List<User>();

            LoadUsers();
        }

        private void LoadUsers()
        {
            AppPaths.EnsureDirectoriesExist();

            if (!File.Exists(AppPaths.UsersFilePath))
            {
                users = Constants.DefaultUsers;
                serializer.SerializeObject(users, AppPaths.UsersFilePath);
            }
            else
            {
                users = serializer.DeSerializeObject<List<User>>(AppPaths.UsersFilePath);

                if (users == null || users.Count == 0)
                {
                    users = Constants.DefaultUsers;
                    serializer.SerializeObject(users, AppPaths.UsersFilePath);
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ClearValidationMessages();

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordInput.Password.Trim();

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(username))
            {
                UsernameErrorTextBlock.Text = "Username is required.";
                UsernameErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordErrorTextBlock.Text = "Password is required.";
                PasswordErrorTextBlock.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!isValid)
            {
                return;
            }

            User loggedUser = users.FirstOrDefault(user =>
                user.Username == username && user.Password == password);

            if (loggedUser == null)
            {
                LoginStatusTextBlock.Text = "Invalid username or password.";
                LoginStatusTextBlock.Visibility = Visibility.Visible;
                return;
            }

            

            MainWindow mainWindow = new MainWindow(loggedUser);
            mainWindow.Show();
            Hide();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Exit Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void ClearValidationMessages()
        {
            UsernameErrorTextBlock.Text = string.Empty;
            UsernameErrorTextBlock.Visibility = Visibility.Collapsed;

            PasswordErrorTextBlock.Text = string.Empty;
            PasswordErrorTextBlock.Visibility = Visibility.Collapsed;

            LoginStatusTextBlock.Text = string.Empty;
            LoginStatusTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}