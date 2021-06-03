using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoAndPhotoManagementWpfApp.FileManagement
{
    public static class FileManagementClass
    {
        public static void CreateCategoryStructure(string categoryName)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var mainFolder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryName}");
            Directory.CreateDirectory(mainFolder);
            var pictureFolder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryName}\Zdjęcia");
            Directory.CreateDirectory(pictureFolder);
            var movieFolder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryName}\Filmy");
            Directory.CreateDirectory(movieFolder);
        }
        public static void RemoveCategoryStructure(string categoryName)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var folder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryName}");
            DirectoryInfo di = new DirectoryInfo(folder);
            di.Delete(true);
        }
        public static string DisplayFileDialog(string defaultExt,string filter)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = defaultExt;
            dlg.Filter = filter;

            bool? result = dlg.ShowDialog();
            if (result == false)
                return null;
            return dlg.FileName;
        }
        public static string GetPathDestination(string pathSource, string categoryName, string type)
        {
            var pathMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathDestination = Path.Combine(pathMyDocuments, $@"VideoAndPhotoApp\{categoryName}\{type}\{Path.GetFileName(pathSource)}");
            return pathDestination;
        }
        public static void FileMove(string pathSource, string pathDestination)
        {
            File.Move(pathSource, pathDestination);
        }
        public static void FileDelete(string path)
        {
            File.Delete(path);
        }
    }
}
