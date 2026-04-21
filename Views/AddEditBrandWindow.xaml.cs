using BasketballBallBrandsCMS.Helpers;
using BasketballBallBrandsCMS.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BasketballBallBrandsCMS.Views
{
    public partial class AddEditBrandWindow : Window
    {
        private List<BasketballBallBrand> _allBrands;

        public BasketballBallBrand ResultBrand { get; private set; }

        private BasketballBallBrand _existingBrand;   
        private int _nextId;
        private bool _suppressFormatEvents = false;  

       

        public AddEditBrandWindow(int nextId, List<BasketballBallBrand> allBrands)
        {
            InitializeComponent();
            _nextId = nextId;
            _allBrands = allBrands;
            WindowTitleTextBlock.Text = "Add New Brand";
            BrandIdTextBox.Text = nextId.ToString();
            PopulateToolbarControls();
        }

        public AddEditBrandWindow(BasketballBallBrand existingBrand, List<BasketballBallBrand> allBrands)
        {
            InitializeComponent();
            _existingBrand = existingBrand;
            _allBrands = allBrands;
            WindowTitleTextBlock.Text = "Edit Brand";
            PopulateToolbarControls();
            FillFieldsFromBrand(existingBrand);
        }

        

        private void PopulateToolbarControls()
        {
             
            var fonts = Fonts.SystemFontFamilies.OrderBy(f => f.Source).ToList();
            FontFamilyComboBox.ItemsSource = fonts;
            FontFamilyComboBox.SelectedItem = fonts.FirstOrDefault(f => f.Source == "Segoe UI")
                                              ?? fonts.First();

             
            int[] sizes = { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72 };
            FontSizeComboBox.ItemsSource = sizes;
            FontSizeComboBox.SelectedItem = 13;

             
            var colorProperties = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(p => (Color)p.GetValue(null))
                .Select(c => new SolidColorBrush(c))
                .ToList();

             
            var namedColors = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(p => new NamedColor(p.Name, (Color)p.GetValue(null)))
                .ToList();

            FontColorComboBox.ItemsSource = namedColors;
            FontColorComboBox.SelectedIndex = namedColors.FindIndex(c => c.Name == "Black");
        }

        private void FillFieldsFromBrand(BasketballBallBrand brand)
        {
            BrandNameTextBox.Text = brand.BrandName;
            BrandIdTextBox.Text = brand.Id.ToString();
            ImagePathTextBox.Text = brand.ImagePath;
            RtfPathTextBox.Text = brand.RtfPath;

            if (!string.IsNullOrEmpty(brand.ImagePath))
            {
                string absImage = AppPaths.ToAbsolute(brand.ImagePath);
                if (File.Exists(absImage))
                    SetImagePreview(absImage);
            }

            if (!string.IsNullOrEmpty(brand.RtfPath))
            {
                string absRtf = AppPaths.ToAbsolute(brand.RtfPath);
                if (File.Exists(absRtf))
                {
                    using var fs = new FileStream(absRtf, FileMode.Open, FileAccess.Read);
                    var range = new TextRange(DescriptionRichTextBox.Document.ContentStart,
                                              DescriptionRichTextBox.Document.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);
                }
            }
        }

      
        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select Image",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp"
            };
            if (dlg.ShowDialog() != true) return;

             string destFolder = AppPaths.ImagesFolder;
            string destFile = Path.Combine(destFolder, Path.GetFileName(dlg.FileName));
            if (dlg.FileName != destFile)
                File.Copy(dlg.FileName, destFile, overwrite: true);

            string relativePath = Path.Combine("Resources", "Images", Path.GetFileName(dlg.FileName));
            ImagePathTextBox.Text = relativePath;

            SetImagePreview(destFile);
            HideError(ImageError);
        }

        private void BrowseRtfButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select RTF File",
                Filter = "RTF Files|*.rtf"
            };
            if (dlg.ShowDialog() != true) return;

            string destFolder = AppPaths.RtfFolder;
            string destFile = Path.Combine(destFolder, Path.GetFileName(dlg.FileName));
            if (dlg.FileName != destFile)
                File.Copy(dlg.FileName, destFile, overwrite: true);

            string relativePath = Path.Combine("Resources", "Rtf", Path.GetFileName(dlg.FileName));
            RtfPathTextBox.Text = relativePath;

             using var fs = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
            var range = new TextRange(DescriptionRichTextBox.Document.ContentStart,
                                      DescriptionRichTextBox.Document.ContentEnd);
            range.Load(fs, DataFormats.Rtf);
        }

        private void SetImagePreview(string absolutePath)
        {
            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(absolutePath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;  
                bmp.EndInit();
                ImagePreview.Source = bmp;
                NoImageLabel.Visibility = Visibility.Collapsed;
            }
            catch
            {
                ImagePreview.Source = null;
                NoImageLabel.Visibility = Visibility.Visible;
            }
        }

        // ──────────────────────────────────────────────────────────────
        // RTF TOOLBAR — Bold / Italic / Underline
        // ──────────────────────────────────────────────────────────────

        private void BoldToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_suppressFormatEvents) return;
            DescriptionRichTextBox.Focus();
            EditingCommands.ToggleBold.Execute(null, DescriptionRichTextBox);
        }

        private void ItalicToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_suppressFormatEvents) return;
            DescriptionRichTextBox.Focus();
            EditingCommands.ToggleItalic.Execute(null, DescriptionRichTextBox);
        }

        private void UnderlineToggle_Click(object sender, RoutedEventArgs e)
        {
            if (_suppressFormatEvents) return;
            DescriptionRichTextBox.Focus();
            EditingCommands.ToggleUnderline.Execute(null, DescriptionRichTextBox);
        }

        // ──────────────────────────────────────────────────────────────
        // RTF TOOLBAR — Font / Size / Color
        // ──────────────────────────────────────────────────────────────

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressFormatEvents || FontFamilyComboBox.SelectedItem == null) return;
            if (!DescriptionRichTextBox.Selection.IsEmpty)
                DescriptionRichTextBox.Selection.ApplyPropertyValue(
                    TextElement.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            DescriptionRichTextBox.Focus();
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressFormatEvents || FontSizeComboBox.SelectedItem == null) return;
            if (!DescriptionRichTextBox.Selection.IsEmpty)
                DescriptionRichTextBox.Selection.ApplyPropertyValue(
                    TextElement.FontSizeProperty, (double)(int)FontSizeComboBox.SelectedItem);
            DescriptionRichTextBox.Focus();
        }

        private void FontColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressFormatEvents || FontColorComboBox.SelectedItem == null) return;
            if (!DescriptionRichTextBox.Selection.IsEmpty)
            {
                var named = (NamedColor)FontColorComboBox.SelectedItem;
                DescriptionRichTextBox.Selection.ApplyPropertyValue(
                    TextElement.ForegroundProperty, new SolidColorBrush(named.Color));
            }
            DescriptionRichTextBox.Focus();
        }

        // ──────────────────────────────────────────────────────────────
        // SELECTION CHANGED — update toolbar to reflect current format
        // ──────────────────────────────────────────────────────────────

        private void DescriptionRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _suppressFormatEvents = true;
            try
            {
                // Bold
                var boldVal = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
                BoldToggle.IsChecked = boldVal != DependencyProperty.UnsetValue
                                       && boldVal is FontWeight fw
                                       && fw == FontWeights.Bold;

                // Italic
                var italicVal = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
                ItalicToggle.IsChecked = italicVal != DependencyProperty.UnsetValue
                                         && italicVal is FontStyle fs
                                         && fs == FontStyles.Italic;

                // Underline
                var decorVal = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                UnderlineToggle.IsChecked = decorVal != DependencyProperty.UnsetValue
                                            && decorVal is TextDecorationCollection decorations
                                            && decorations == TextDecorations.Underline;

                // Font family
                var familyVal = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
                if (familyVal != DependencyProperty.UnsetValue && familyVal is FontFamily selectedFamily)
                {
                    var match = FontFamilyComboBox.Items
                        .OfType<FontFamily>()
                        .FirstOrDefault(f => f.Source == selectedFamily.Source);

                    if (match != null)
                        FontFamilyComboBox.SelectedItem = match;
                }

                // Font size
                var sizeVal = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
                if (sizeVal != DependencyProperty.UnsetValue)
                {
                    double size = Convert.ToDouble(sizeVal);
                    int roundedSize = (int)Math.Round(size);

                    var match = FontSizeComboBox.Items
                        .OfType<int>()
                        .FirstOrDefault(s => s == roundedSize);

                    if (match != 0)
                        FontSizeComboBox.SelectedItem = match;
                }

                // Font color
                var colorVal = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.ForegroundProperty);
                if (colorVal != DependencyProperty.UnsetValue && colorVal is SolidColorBrush brush)
                {
                    var match = FontColorComboBox.Items
                        .OfType<NamedColor>()
                        .FirstOrDefault(c => c.Color == brush.Color);

                    if (match != null)
                        FontColorComboBox.SelectedItem = match;
                }
            }
            finally
            {
                _suppressFormatEvents = false;
            }

            UpdateWordCount();
        }

        private void DescriptionRichTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateWordCount();
            var text = new TextRange(
            DescriptionRichTextBox.Document.ContentStart,
            DescriptionRichTextBox.Document.ContentEnd).Text.Trim();
                if (!string.IsNullOrWhiteSpace(text))
                    HideError(DescriptionError);
        }

        private void UpdateWordCount()
        {
            var text = new TextRange(
                DescriptionRichTextBox.Document.ContentStart,
                DescriptionRichTextBox.Document.ContentEnd).Text.Trim();

            int wordCount = string.IsNullOrWhiteSpace(text)
                ? 0
                : text.Split(new[] { ' ', '\n', '\r', '\t' },
                              StringSplitOptions.RemoveEmptyEntries).Length;

            WordCountTextBlock.Text = $"Words: {wordCount}";
        }

         

        private void BrandNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(BrandNameTextBox.Text))
                HideError(BrandNameError);
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void ShowError(TextBlock error, string message)
        {
            error.Text = message;
            error.Visibility = Visibility.Visible;
        }

        private void HideError(TextBlock error)
        {
            error.Visibility = Visibility.Collapsed;
        }

        private bool ValidateAll()
        {
            bool valid = true;

             
            if (string.IsNullOrWhiteSpace(BrandNameTextBox.Text))
            { ShowError(BrandNameError, "Brand name is required."); valid = false; }
            else HideError(BrandNameError);


            if (!int.TryParse(BrandIdTextBox.Text, out int parsedId))
            {
                ShowError(BrandIdError, "Brand ID must be a valid whole number.");
                valid = false;
            }
            else if (_allBrands.Any(b => b.Id == parsedId && (_existingBrand == null || b.Id != _existingBrand.Id)))
            {
                ShowError(BrandIdError, "Brand ID must be unique. This ID already exists.");
                valid = false;
            }
            else
            {
                HideError(BrandIdError);
            }


            if (string.IsNullOrWhiteSpace(ImagePathTextBox.Text))
            { ShowError(ImageError, "Please select an image for this brand."); valid = false; }
            else HideError(ImageError);

           
            var rtfText = new TextRange(
                DescriptionRichTextBox.Document.ContentStart,
                DescriptionRichTextBox.Document.ContentEnd).Text.Trim();
            if (string.IsNullOrWhiteSpace(rtfText))
            { ShowError(DescriptionError, "Description cannot be empty."); valid = false; }
            else HideError(DescriptionError);

            return valid;
        }

         

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAll()) return;

            int id = int.Parse(BrandIdTextBox.Text);

             
            string rtfFileName = $"brand_{id}_{DateTime.Now:yyyyMMddHHmmss}.rtf";
            string rtfAbsPath = Path.Combine(AppPaths.RtfFolder, rtfFileName);

             
            if (_existingBrand != null && !string.IsNullOrEmpty(_existingBrand.RtfPath))
            {
                string oldRtfAbs = AppPaths.ToAbsolute(_existingBrand.RtfPath);
                if (File.Exists(oldRtfAbs))
                {
                    try { File.Delete(oldRtfAbs); } catch { /* ignore if locked */ }
                }
            }

            using (var fs = new FileStream(rtfAbsPath, FileMode.Create, FileAccess.Write))
            {
                var range = new TextRange(
                    DescriptionRichTextBox.Document.ContentStart,
                    DescriptionRichTextBox.Document.ContentEnd);
                range.Save(fs, DataFormats.Rtf);
            }

            string rtfRelPath = Path.Combine("Resources", "Rtf", rtfFileName);

            ResultBrand = new BasketballBallBrand
            {
                Id = id,
                BrandName = BrandNameTextBox.Text.Trim(),
                ImagePath = ImagePathTextBox.Text,
                RtfPath = rtfRelPath,
                DateAdded = _existingBrand?.DateAdded ?? DateTime.UtcNow
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }

     
    public class NamedColor
    {
        public string Name { get; }
        public Color Color { get; }
        public NamedColor(string name, Color color) { Name = name; Color = color; }
    }
}