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
            _mainWindowViewModel.MovieViewModels = LoadMovies();
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

        private ObservableCollection<MovieViewModel> LoadMovies()
        {
            if (_mainWindowViewModel.CategoryName is not null)
            {
                using var context = new VideoAndPhotoManagementContext();
                var movies = context.Movies.Where(x => x.Category.CategoryName == _mainWindowViewModel.CategoryName.CategoryName).Select(x => new MovieViewModel
                {
                    MovieId = x.MovieId,
                    Title = x.Title,
                    Path = x.Path
                }
                );
                return new ObservableCollection<MovieViewModel>(movies);
            }
            return null;
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

        private async void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
                int categoryViewModelId = _mainWindowViewModel.CategoryViewModels.IndexOf(_mainWindowViewModel.CategoryViewModels.Where(x => x.CategoryName == categoryNameToDelete).SingleOrDefault());
                _mainWindowViewModel.CategoryViewModels.RemoveAt(categoryViewModelId);
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private void CategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _mainWindowViewModel.CategorySelect = true;
            _mainWindowViewModel.PictureViewModels = LoadPicturies();
            _mainWindowViewModel.MovieViewModels = LoadMovies();
        }

        private void ShowPictureButton_Click(object sender, RoutedEventArgs e)
        {
            PictureViewModel categoryViewModelRow = ((Button)sender).DataContext as PictureViewModel;
            ShowPictureWindow showPictureWindow = new ShowPictureWindow(categoryViewModelRow.Path);
            showPictureWindow.ShowDialog();
        }

        private async void DeletePictureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PictureViewModel pictureViewModel = ((Button)sender).DataContext as PictureViewModel;
                File.Delete(pictureViewModel.Path);
                _mainWindowViewModel.PictureViewModels.Remove(pictureViewModel);
                using var context = new VideoAndPhotoManagementContext();
                Picture picture = context.Pictures.Where(x => x.PictureId == pictureViewModel.PictureId).SingleOrDefault();
                context.Pictures.Remove(picture);
                context.SaveChanges();
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void MovePictureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PictureViewModel pictureViewModel = ((Button)sender).DataContext as PictureViewModel;
                CustomUserInputDialog customUserInputDialog = new CustomUserInputDialog(_mainWindowViewModel.CategoryViewModels, this);
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
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private void ShowMovieButton_Click(object sender, RoutedEventArgs e)
        {
            MovieViewModel movieViewModel = ((Button)sender).DataContext as MovieViewModel;
            PlayMovieWindow playMovieWindow = new PlayMovieWindow(movieViewModel.Path);
            playMovieWindow.ShowDialog();
        }

        private async void MoveMovieButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MovieViewModel movieViewModel = ((Button)sender).DataContext as MovieViewModel;
                CustomUserInputDialog customUserInputDialog = new CustomUserInputDialog(_mainWindowViewModel.CategoryViewModels, this);
                await this.ShowMetroDialogAsync(customUserInputDialog);
                await customUserInputDialog.WaitUntilUnloadedAsync();
                if (MoveToCategory is null)
                    return;
                var pathMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var pathDestination = Path.Combine(pathMyDocuments, $@"VideoAndPhotoApp\{MoveToCategory}\Filmy\{Path.GetFileName(movieViewModel.Path)}");
                File.Move(movieViewModel.Path, pathDestination);
                using var context = new VideoAndPhotoManagementContext();
                Movie movie = context.Movies.SingleOrDefault(x => x.MovieId == movieViewModel.MovieId);
                movie.Path = pathDestination;
                movie.Category = context.Categories.SingleOrDefault(x => x.CategoryName == MoveToCategory);
                context.Update(movie);
                context.SaveChanges();
                _mainWindowViewModel.MovieViewModels.Remove(movieViewModel);
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void DeleteMovieButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MovieViewModel movieViewModel = ((Button)sender).DataContext as MovieViewModel;
                File.Delete(movieViewModel.Path);
                _mainWindowViewModel.MovieViewModels.Remove(movieViewModel);
                using var context = new VideoAndPhotoManagementContext();
                Movie movie = context.Movies.Where(x => x.MovieId == movieViewModel.MovieId).SingleOrDefault();
                context.Movies.Remove(movie);
                context.SaveChanges();
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void AddMovieButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();

                dlg.DefaultExt = ".mp4";
                dlg.Filter = "MP4 Files (*.mp4)|*.mp4|All files (*.*)|*.*";

                bool? result = dlg.ShowDialog();
                if (result == false)
                    return;
                var pathSource = dlg.FileName;
                var pathMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var pathDestination = Path.Combine(pathMyDocuments, $@"VideoAndPhotoApp\{_mainWindowViewModel.CategoryName.CategoryName}\Filmy\{Path.GetFileName(pathSource)}");
                File.Move(pathSource, pathDestination);
                using var context = new VideoAndPhotoManagementContext();
                Movie movie = new Movie()
                {
                    Title = Path.GetFileNameWithoutExtension(pathDestination),
                    Path = pathDestination
                };
                movie.Category = context.Categories.Where(x => x.CategoryName == _mainWindowViewModel.CategoryName.CategoryName).SingleOrDefault();
                context.Add(movie);
                context.SaveChanges();
                MovieViewModel movieViewModel = new MovieViewModel();
                movieViewModel.MovieId = context.Movies.Where(x => x.Title == Path.GetFileNameWithoutExtension(pathDestination)).Select(x => x.MovieId).SingleOrDefault();
                movieViewModel.Title = movie.Title;
                movieViewModel.Path = movie.Path;
                _mainWindowViewModel.MovieViewModels.Add(movieViewModel);
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }
    }
}
