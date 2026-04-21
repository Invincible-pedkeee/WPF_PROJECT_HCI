using BasketballBallBrandsCMS.Helpers;
using BasketballBallBrandsCMS.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace BasketballBallBrandsCMS.Views
{
    public partial class MainWindow : Window
    {
        private User _loggedUser;
        private ObservableCollection<BasketballBallBrand> _brands;
        private DataIO _dataIO;

        public MainWindow(User user)
        {
            InitializeComponent();
            _loggedUser = user;
            _dataIO = new DataIO();
            _brands = new ObservableCollection<BasketballBallBrand>();

            AppPaths.EnsureDirectoriesExist();
            ConfigureUIForUser();
            LoadBrands();
        }

        // ──────────────────────────────────────────────────────────────
        // UI SETUP
        // ──────────────────────────────────────────────────────────────

        private void ConfigureUIForUser()
        {
            LoggedUserTextBlock.Text = $"Logged in as: {_loggedUser.Username}  ({_loggedUser.Role})";

            bool isAdmin = _loggedUser.Role == UserRole.Admin;
            AddButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            DeleteButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            SelectAllCheckBox.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            // First column (checkbox) hidden for Visitor
            if (!isAdmin)
                BrandsDataGrid.Columns[0].Visibility = Visibility.Collapsed;
        }

        // ──────────────────────────────────────────────────────────────
        // DATA
        // ──────────────────────────────────────────────────────────────

        private void LoadBrands()
        {
            var loaded = _dataIO.DeSerializeObject<List<BasketballBallBrand>>(AppPaths.BasketballBallBrandsFilePath);
            _brands = new ObservableCollection<BasketballBallBrand>(loaded ?? new List<BasketballBallBrand>());
            BrandsDataGrid.ItemsSource = _brands;
            UpdateItemCount();
            StatusBarTextBlock.Text = $"Loaded {_brands.Count} brand(s).";
        }

        private void SaveBrands()
        {
            _dataIO.SerializeObject(_brands.ToList(), AppPaths.BasketballBallBrandsFilePath);
        }

        private void UpdateItemCount()
        {
            ItemCountTextBlock.Text = $"Total: {_brands.Count} brand(s)";
        }

        // ──────────────────────────────────────────────────────────────
        // BUTTON HANDLERS
        // ──────────────────────────────────────────────────────────────

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditBrandWindow(
                _brands.Count > 0 ? _brands.Max(b => b.Id) + 1 : 1,
                _brands.ToList()); addWindow.Owner = this;

            bool? result = addWindow.ShowDialog();

            if (result == true && addWindow.ResultBrand != null)
            {
                _brands.Add(addWindow.ResultBrand);
                SaveBrands();
                UpdateItemCount();
                StatusBarTextBlock.Text = $"Brand '{addWindow.ResultBrand.BrandName}' added successfully.";

                MessageBox.Show(
                    $"Brand '{addWindow.ResultBrand.BrandName}' was added successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var toDelete = _brands.Where(b => b.IsSelected).ToList();

            if (toDelete.Count == 0)
            {
                MessageBox.Show(
                    "No brands are selected for deletion.\nPlease check the checkbox next to the brands you want to delete.",
                    "Nothing Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                $"Are you sure you want to delete {toDelete.Count} selected brand(s)?\nThis action cannot be undone.",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            foreach (var brand in toDelete)
            {
                DeleteAssociatedFiles(brand, toDelete);
                _brands.Remove(brand);
            }

            SelectAllCheckBox.IsChecked = false;
            SaveBrands();
            UpdateItemCount();
            StatusBarTextBlock.Text = $"Deleted {toDelete.Count} brand(s).";

            MessageBox.Show(
                $"{toDelete.Count} brand(s) deleted successfully.",
                "Deleted",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Hide();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Exit Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        // ──────────────────────────────────────────────────────────────
        // HYPERLINK CLICK — Admin → Edit, Visitor → Details
        // ──────────────────────────────────────────────────────────────

        private void BrandHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink)
            {
                var cell = hyperlink.Parent as TextBlock;
                var row = BrandsDataGrid.ContainerFromElement(cell as DependencyObject) as DataGridRow;
                if (row == null) return;

                var brand = row.Item as BasketballBallBrand;
                if (brand == null) return;

                if (_loggedUser.Role == UserRole.Admin)
                {
                    var editWindow = new AddEditBrandWindow(brand, _brands.ToList());
                    editWindow.Owner = this;
                    bool? result = editWindow.ShowDialog();

                    if (result == true && editWindow.ResultBrand != null)
                    {
                        int index = _brands.IndexOf(brand);
                        if (index >= 0)
                            _brands[index] = editWindow.ResultBrand;

                        SaveBrands();
                        UpdateItemCount();
                        StatusBarTextBlock.Text = $"Brand '{editWindow.ResultBrand.BrandName}' updated.";

                        MessageBox.Show(
                            $"Brand '{editWindow.ResultBrand.BrandName}' was updated successfully.",
                            "Updated",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                else
                {
                    var detailsWindow = new BrandDetailsWindow(brand);
                    detailsWindow.Owner = this;
                    detailsWindow.ShowDialog();
                }
            }
        }

        // ──────────────────────────────────────────────────────────────
        // SELECT ALL CHECKBOX
        // ──────────────────────────────────────────────────────────────

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var brand in _brands)
                brand.IsSelected = true;

            BrandsDataGrid.Items.Refresh();
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var brand in _brands)
                brand.IsSelected = false;

            BrandsDataGrid.Items.Refresh();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        private void DeleteAssociatedFiles(BasketballBallBrand brand, List<BasketballBallBrand> brandsBeingDeleted)
        {
            DeleteFileIfExists(brand.RtfPath);

            bool imageUsedByAnotherBrand = _brands.Any(b =>
                !ReferenceEquals(b, brand)
                && !brandsBeingDeleted.Contains(b)
                && !string.IsNullOrEmpty(b.ImagePath)
                && !string.IsNullOrEmpty(brand.ImagePath)
                && string.Equals(b.ImagePath, brand.ImagePath, StringComparison.OrdinalIgnoreCase));

            if (!imageUsedByAnotherBrand)
                DeleteFileIfExists(brand.ImagePath);
        }

        private void DeleteFileIfExists(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            try
            {
                string absolutePath = AppPaths.ToAbsolute(relativePath);
                if (absolutePath != null && File.Exists(absolutePath))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(absolutePath);
                }
            }
            catch (Exception ex)
            {
                 System.Diagnostics.Debug.WriteLine($"[Delete] Could not delete file '{relativePath}': {ex.Message}");
            }
        }

    }
}
