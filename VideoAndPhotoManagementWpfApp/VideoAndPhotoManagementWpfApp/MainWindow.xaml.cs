using DatabaseManager;
using DatabaseManager.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VideoAndPhotoManagementWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public string CategoryName { get; set; } = null;
        private MainWindowViewModel _mainWindowViewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            _mainWindowViewModel.CategoryViewModels = LoadCategories();
            _mainWindowViewModel.PictureViewModels = new ObservableCollection<PictureViewModel>();
            _mainWindowViewModel.MovieViewModels = new ObservableCollection<MovieViewModel>();
            DataContext = _mainWindowViewModel;
        }

        private ObservableCollection<CategoryViewModel> LoadCategories()
        {
            using var context = new VideoAndPhotoManagementContext();
            var categories = context.Categories.Select(x => new CategoryViewModel
            {
                CategoryName = x.CategoryName
            }
            );
            return new ObservableCollection<CategoryViewModel>(categories);
        }

        private void ShowElement_Click(object sender, RoutedEventArgs e)
        {
            //PlayMovieWindow playMovieWindow = new PlayMovieWindow(@"");
            //playMovieWindow.ShowDialog();
            //ShowPictureWindow showPictureWindow = new ShowPictureWindow(@"");
            //showPictureWindow.ShowDialog();
        }

        private async void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var categoryName = await this.ShowInputAsync("Podaj nazwe kategorii", "Kategoria:");
            if (categoryName is null)
                return;
            if (_mainWindowViewModel.CategoryViewModels.Any(x => x.CategoryName == categoryName))
            {
                await this.ShowMessageAsync("Uwaga", "Dana kategoria już istnieje");
                return;
            }
            using var context = new VideoAndPhotoManagementContext();
            var category = new Category()
            {
                CategoryName = categoryName
            };
            context.Add(category);
            context.SaveChanges();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var folder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryName}");
            Directory.CreateDirectory(folder);
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            categoryViewModel.CategoryName = categoryName;
            _mainWindowViewModel.CategoryViewModels.Add(categoryViewModel);
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            using var context = new VideoAndPhotoManagementContext();
            string categoryNameToDelete = ((Button)sender).Tag.ToString();
            Category category = context.Categories.Where(x => x.CategoryName == categoryNameToDelete).SingleOrDefault();
            context.Categories.Remove(category);
            context.SaveChanges();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var folder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryNameToDelete}");
            DirectoryInfo di = new DirectoryInfo(folder);
            di.Delete(true);
            int categoryViewModelId = _mainWindowViewModel.CategoryViewModels.IndexOf(_mainWindowViewModel.CategoryViewModels.Where(x=>x.CategoryName == categoryNameToDelete).SingleOrDefault());
            _mainWindowViewModel.CategoryViewModels.RemoveAt(categoryViewModelId);
        }

        private void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            bool? result = dlg.ShowDialog();
            if (result == false)
                return;
            var path = dlg.FileName;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            return;
        }
    }
}
