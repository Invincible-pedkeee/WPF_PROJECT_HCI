using BasketballBallBrandsCMS.Helpers;
using BasketballBallBrandsCMS.Models;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BasketballBallBrandsCMS.Views
{
    public partial class BrandDetailsWindow : Window
    {
        public BrandDetailsWindow(BasketballBallBrand brand)
        {
            InitializeComponent();
            PopulateFields(brand);
        }

        private void PopulateFields(BasketballBallBrand brand)
        {
            BrandNameText.Text = brand.BrandName;
            BrandIdText.Text = brand.Id.ToString();
            DateAddedText.Text = brand.DateAdded.ToLocalTime().ToString("dd.MM.yyyy HH:mm");

             string absImage = AppPaths.ToAbsolute(brand.ImagePath);
            if (!string.IsNullOrEmpty(absImage) && File.Exists(absImage))
            {
                try
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new System.Uri(absImage, System.UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;  
                    bmp.EndInit();
                    BrandImage.Source = bmp;
                }
                catch { /* ignore broken image */ }
            }

             string absRtf = AppPaths.ToAbsolute(brand.RtfPath);
            if (!string.IsNullOrEmpty(absRtf) && File.Exists(absRtf))
            {
                try
                {
                    var doc = new FlowDocument();
                    using var fs = new FileStream(absRtf, FileMode.Open, FileAccess.Read);
                    var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);
                    doc.Background = System.Windows.Media.Brushes.Transparent;
                    doc.Foreground = System.Windows.Media.Brushes.White;
                    doc.FontSize = 13;
                    DescriptionViewer.Document = doc;
                }
                catch { /* ignore broken rtf */ }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
