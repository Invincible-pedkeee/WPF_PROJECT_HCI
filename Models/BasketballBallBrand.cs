using System;
using System.ComponentModel;
using System.IO;

namespace BasketballBallBrandsCMS.Models
{
    [Serializable]
    public class BasketballBallBrand : INotifyPropertyChanged
    {
        private bool _isSelected;

        public int Id { get; set; }
        public string BrandName { get; set; }
        public string ImagePath { get; set; }
        public string RtfPath { get; set; }
        public DateTime DateAdded { get; set; }
        public string SafeImagePath
        {
            get
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(ImagePath))
                    {
                        // ImagePath is relative (e.g. "Resources\Images\foto.png")
                        // Must resolve from project root, same as AppPaths.ToAbsolute
                        string root = Path.GetFullPath(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
                        string full = Path.GetFullPath(Path.Combine(root, ImagePath));
                        if (File.Exists(full))
                            return full;
                    }
                }
                catch { /* ignore any path resolution errors */ }
                return null; // return null so Image control shows nothing instead of crashing
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public BasketballBallBrand() { }

        public BasketballBallBrand(int id, string brandName, string imagePath, string rtfPath, DateTime dateAdded)
        {
            Id = id;
            BrandName = brandName;
            ImagePath = imagePath;
            RtfPath = rtfPath;
            DateAdded = dateAdded;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
