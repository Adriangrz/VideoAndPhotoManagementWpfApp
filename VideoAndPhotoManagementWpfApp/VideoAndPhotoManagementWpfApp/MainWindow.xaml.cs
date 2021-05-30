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
        private MainWindowViewModel _mainWindowViewModel = new MainWindowViewModel();
        public string MoveToCategory { get; set; } = null;

        public MainWindow()
        {
            InitializeComponent();
            _mainWindowViewModel.CategoryViewModels = LoadCategories();
            _mainWindowViewModel.PictureViewModels = LoadPicturies();
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

        private ObservableCollection<PictureViewModel> LoadPicturies()
        {
            if (_mainWindowViewModel.CategoryName is not null)
            {
                using var context = new VideoAndPhotoManagementContext();
                var pictures = context.Pictures.Where(x => x.Category.CategoryName == _mainWindowViewModel.CategoryName.CategoryName).Select(x => new PictureViewModel
                {
                    PictureId = x.PictureId,
                    Title = x.Title,
                    Path = x.Path
                }
                );
                return new ObservableCollection<PictureViewModel>(pictures);
            }
            return null;
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
            var metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ok",
                NegativeButtonText = "Anuluj"
            };
            var categoryName = await this.ShowInputAsync("Podaj nazwe kategorii", "Kategoria:", metroDialogSettings);
            if (categoryName is null)
                return;
            if (_mainWindowViewModel.CategoryViewModels.Any(x => x.CategoryName == categoryName))
            {
                await this.ShowMessageAsync("Uwaga", "Dana kategoria już istnieje");
                return;
            }
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var mainFolder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryName}");
            Directory.CreateDirectory(mainFolder);
            var pictureFolder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryName}\Zdjęcia");
            Directory.CreateDirectory(pictureFolder);
            var movieFolder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryName}\Filmy");
            Directory.CreateDirectory(movieFolder);
            using var context = new VideoAndPhotoManagementContext();
            var category = new Category()
            {
                CategoryName = categoryName
            };
            context.Add(category);
            context.SaveChanges();
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            categoryViewModel.CategoryName = categoryName;
            _mainWindowViewModel.CategoryViewModels.Add(categoryViewModel);
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryViewModel categoryViewModelRow = ((Button)sender).DataContext as CategoryViewModel;
            string categoryNameToDelete = categoryViewModelRow.CategoryName;
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var folder = Path.Combine(path, $@"VideoAndPhotoApp\{categoryNameToDelete}");
            DirectoryInfo di = new DirectoryInfo(folder);
            di.Delete(true);
            using var context = new VideoAndPhotoManagementContext();
            Category category = context.Categories.Where(x => x.CategoryName == categoryNameToDelete).SingleOrDefault();
            context.Categories.Remove(category);
            context.SaveChanges();
            int categoryViewModelId = _mainWindowViewModel.CategoryViewModels.IndexOf(_mainWindowViewModel.CategoryViewModels.Where(x=>x.CategoryName == categoryNameToDelete).SingleOrDefault());
            _mainWindowViewModel.CategoryViewModels.RemoveAt(categoryViewModelId);
        }

        private void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|All files (*.*)|*.*";

            bool? result = dlg.ShowDialog();
            if (result == false)
                return;
            var pathSource = dlg.FileName;
            var pathMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathDestination = Path.Combine(pathMyDocuments, $@"VideoAndPhotoApp\{_mainWindowViewModel.CategoryName.CategoryName}\Zdjęcia\{Path.GetFileName(pathSource)}");
            File.Move(pathSource, pathDestination);
            using var context = new VideoAndPhotoManagementContext();
            Picture picture = new Picture()
            {
                Title = Path.GetFileNameWithoutExtension(pathDestination),
                Path = pathDestination
            };
            picture.Category = context.Categories.Where(x => x.CategoryName == _mainWindowViewModel.CategoryName.CategoryName).SingleOrDefault();
            context.Add(picture);
            context.SaveChanges();
            PictureViewModel pictureViewModel = new PictureViewModel();
            pictureViewModel.PictureId = context.Pictures.Where(x => x.Title == Path.GetFileNameWithoutExtension(pathDestination)).Select(x => x.PictureId).SingleOrDefault();
            pictureViewModel.Title = picture.Title;
            pictureViewModel.Path = picture.Path;
            _mainWindowViewModel.PictureViewModels.Add(pictureViewModel);
        }

        private void CategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _mainWindowViewModel.CategorySelect = true;
            _mainWindowViewModel.PictureViewModels = LoadPicturies();
        }

        private void ShowPictureButton_Click(object sender, RoutedEventArgs e)
        {
            PictureViewModel categoryViewModelRow = ((Button)sender).DataContext as PictureViewModel;
            ShowPictureWindow showPictureWindow = new ShowPictureWindow(categoryViewModelRow.Path);
            showPictureWindow.ShowDialog();
        }

        private void DeletePictureButton_Click(object sender, RoutedEventArgs e)
        {
            PictureViewModel pictureViewModel = ((Button)sender).DataContext as PictureViewModel;
            File.Delete(pictureViewModel.Path);
            _mainWindowViewModel.PictureViewModels.Remove(pictureViewModel);
            using var context = new VideoAndPhotoManagementContext();
            Picture picture = context.Pictures.Where(x => x.PictureId == pictureViewModel.PictureId).SingleOrDefault();
            context.Pictures.Remove(picture);
            context.SaveChanges();
        }

        private async void MovePictureButton_Click(object sender, RoutedEventArgs e)
        {
            PictureViewModel pictureViewModel = ((Button)sender).DataContext as PictureViewModel;
            CustomUserInputDialog customUserInputDialog = new CustomUserInputDialog(_mainWindowViewModel.CategoryViewModels,this);
            await this.ShowMetroDialogAsync(customUserInputDialog);
            await customUserInputDialog.WaitUntilUnloadedAsync();
            if (MoveToCategory is null)
                return;
            var pathMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathDestination = Path.Combine(pathMyDocuments, $@"VideoAndPhotoApp\{MoveToCategory}\Zdjęcia\{Path.GetFileName(pictureViewModel.Path)}");
            File.Move(pictureViewModel.Path, pathDestination);
            using var context = new VideoAndPhotoManagementContext();
            Picture picture = context.Pictures.SingleOrDefault(x => x.PictureId == pictureViewModel.PictureId);
            picture.Path = pathDestination;
            picture.Category = context.Categories.SingleOrDefault(x => x.CategoryName == MoveToCategory);
            context.Update(picture);
            context.SaveChanges();
            _mainWindowViewModel.PictureViewModels.Remove(pictureViewModel);
        }
    }
}
