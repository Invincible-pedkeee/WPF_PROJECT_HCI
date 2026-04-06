using System;
using System.IO;

namespace BasketballBallBrandsCMS.Helpers
{
    public static class AppPaths
    {
        private static readonly string ProjectRoot =
            Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));

        public static readonly string ResourcesFolder = Path.Combine(ProjectRoot, "Resources");
        public static readonly string DataFolder = Path.Combine(ResourcesFolder, "Data");
        public static readonly string ImagesFolder = Path.Combine(ResourcesFolder, "Images");
        public static readonly string RtfFolder = Path.Combine(ResourcesFolder, "Rtf");

        public static readonly string UsersFilePath = Path.Combine(DataFolder, "Users.xml");
        public static readonly string BasketballBallBrandsFilePath = Path.Combine(DataFolder, "BasketballBallBrands.xml");

        public static void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(ResourcesFolder);
            Directory.CreateDirectory(DataFolder);
            Directory.CreateDirectory(ImagesFolder);
            Directory.CreateDirectory(RtfFolder);
        }
    }
}