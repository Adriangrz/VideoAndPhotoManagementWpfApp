using DatabaseManager.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VideoAndPhotoManagementWpfApp.DatabaseManagement;
using VideoAndPhotoManagementWpfApp.DisplayElement;
using VideoAndPhotoManagementWpfApp.FileManagement;

namespace VideoAndPhotoManagementWpfApp
{
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel _mainWindowViewModel;
        public string MoveToCategory { get; set; } = null;

        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            _mainWindowViewModel = mainWindowViewModel;
            LoadCategories();
            LoadPicturies();
            LoadMovies();
            DataContext = _mainWindowViewModel;
        }

        private async void LoadCategories()
        {
            try
            {
                _mainWindowViewModel.CategoryViewModels = await DatabaseManagementClass.LoadCategories();
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void LoadMovies()
        {
            try
            {
                _mainWindowViewModel.MovieViewModels = await DatabaseManagementClass.LoadMovies(_mainWindowViewModel.CategoryName);
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void LoadPicturies()
        {
            try
            {
                _mainWindowViewModel.PictureViewModels =  await DatabaseManagementClass.LoadPicturies(_mainWindowViewModel.CategoryName);
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
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
                await DatabaseManagementClass.AddCategory(categoryName);
                try
                {
                    _mainWindowViewModel.AddCategory(categoryName);
                }
                catch
                {
                    await DatabaseManagementClass.DeleteCategory(categoryName);
                    throw new Exception();
                }
                try
                {
                    FileManagementClass.CreateCategoryStructure(categoryName);
                }
                catch
                {
                    await DatabaseManagementClass.DeleteCategory(categoryName);
                    _mainWindowViewModel.RemoveCategory(categoryName);
                    await this.ShowMessageAsync("Uwaga", "Nie udało się wykonać tego zadania.Sprawdź czy folder o tej nazwie istnieje w folderze aplikacji na dysku.");
                    return;
                }
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
                CategoryViewModel categoryViewModel = ((Button)sender).DataContext as CategoryViewModel;
                string categoryNameToDelete = categoryViewModel.CategoryName;
                await DatabaseManagementClass.DeleteCategory(categoryNameToDelete);
                try
                {
                    _mainWindowViewModel.RemoveCategory(categoryViewModel);
                }
                catch
                {
                    await DatabaseManagementClass.AddCategory(categoryNameToDelete);
                    throw new Exception();
                }
                try
                {
                    FileManagementClass.RemoveCategoryStructure(categoryNameToDelete);
                }
                catch
                {
                    await DatabaseManagementClass.AddCategory(categoryNameToDelete);
                    _mainWindowViewModel.AddCategory(categoryViewModel.CategoryName);
                    await this.ShowMessageAsync("Uwaga", "Nie udało się wykonać tego zadania.Sprawdź czy folder o tej nazwie istnieje w folderze aplikacji na dysku.");
                    return;
                }
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Nie udało się wykonać tego zadania.");
            }
        }

        private async void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pathSource = FileManagementClass.DisplayFileDialog(".png", "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|All files (*.*)|*.*");
                if (pathSource == null)
                    return;
                if (_mainWindowViewModel.PictureViewModels.Any(x => x.Title == Path.GetFileNameWithoutExtension(pathSource)))
                {
                    await this.ShowMessageAsync("Uwaga", "Zdjęcie o tej nazwie już istnieje w danej kategorii");
                    return;
                }
                var pathDestination = FileManagementClass.GetPathDestination(pathSource, _mainWindowViewModel.CategoryName.CategoryName, "Zdjęcia");
                await DatabaseManagementClass.AddPicture(Path.GetFileNameWithoutExtension(pathDestination), pathDestination, _mainWindowViewModel.CategoryName.CategoryName);
                Picture picture = await DatabaseManagementClass.GetPicture(Path.GetFileNameWithoutExtension(pathDestination), _mainWindowViewModel.CategoryName.CategoryName);
                try
                {
                    _mainWindowViewModel.AddPicture(picture);
                }
                catch
                {
                    await DatabaseManagementClass.DeletePicture(picture.PictureId);
                    throw new Exception();
                }
                try
                {
                    FileManagementClass.FileMove(pathSource, pathDestination);
                }
                catch
                {
                    await DatabaseManagementClass.DeletePicture(picture.PictureId);
                    _mainWindowViewModel.RemovePicture(picture);
                    await this.ShowMessageAsync("Uwaga", "Nie udało się wykonać tego zadania.Sprawdź czy plik o tej nazwie istnieje w folderze na dysku.");
                    return;
                }
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private void CategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_mainWindowViewModel.CategoryName == null)
            {
                _mainWindowViewModel.CategorySelect = false;
                return;
            }
            _mainWindowViewModel.CategorySelect = true;
            LoadPicturies();
            LoadMovies();
        }

        private async void ShowPictureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayElementClass.DisplayPicture(((Button)sender).DataContext as PictureViewModel, this);
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void DeletePictureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PictureViewModel pictureViewModel = ((Button)sender).DataContext as PictureViewModel;
                await DatabaseManagementClass.DeletePicture(pictureViewModel.PictureId);
                try
                {
                    _mainWindowViewModel.RemovePicture(pictureViewModel);
                }
                catch
                {
                    await DatabaseManagementClass.AddPicture(pictureViewModel.Title, pictureViewModel.Path, _mainWindowViewModel.CategoryName.CategoryName);
                    throw new Exception();
                }
                try
                {
                    FileManagementClass.FileDelete(pictureViewModel.Path);
                }
                catch
                {
                    await DatabaseManagementClass.AddPicture(pictureViewModel.Title, pictureViewModel.Path, _mainWindowViewModel.CategoryName.CategoryName);
                    _mainWindowViewModel.AddPicture(pictureViewModel);
                    await this.ShowMessageAsync("Uwaga", "Nie udało się wykonać tego zadania.Sprawdź czy plik o tej nazwie istnieje w folderze na dysku.");
                    return;
                }
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
                if (_mainWindowViewModel.CategoryViewModels.Count() <= 1)
                {
                    await this.ShowMessageAsync("Uwaga", "Aby przenieść zdjęcie musisz posiadać więcej niż jedną kategorie");
                    return;
                }
                CustomUserInputDialog customUserInputDialog = new CustomUserInputDialog(_mainWindowViewModel.CategoryViewModels.ToList(), this, _mainWindowViewModel.CategoryName);
                await this.ShowMetroDialogAsync(customUserInputDialog);
                await customUserInputDialog.WaitUntilUnloadedAsync();
                if (MoveToCategory is null)
                    return;
                var picturesInCategory = await DatabaseManagementClass.LoadPicturies(_mainWindowViewModel.CategoryViewModels.FirstOrDefault(x=>x.CategoryName == MoveToCategory));
                if (picturesInCategory.Any(x => x.Title == pictureViewModel.Title))
                {
                    await this.ShowMessageAsync("Uwaga", "Zdjęcie o tej nazwie już istnieje w danej kategorii");
                    return;
                }
                var pathDestination = FileManagementClass.GetPathDestination(pictureViewModel.Path, MoveToCategory, "Zdjęcia");
                await DatabaseManagementClass.UpdatePicture(pictureViewModel.PictureId, pathDestination, MoveToCategory);
                try
                {
                    _mainWindowViewModel.RemovePicture(pictureViewModel);
                }
                catch
                {
                    await DatabaseManagementClass.UpdatePicture(pictureViewModel.PictureId, pictureViewModel.Path, _mainWindowViewModel.CategoryName.CategoryName);
                    throw new Exception();
                }
                try
                {
                    FileManagementClass.FileMove(pictureViewModel.Path, pathDestination);
                }
                catch
                {
                    await DatabaseManagementClass.UpdatePicture(pictureViewModel.PictureId, pictureViewModel.Path, _mainWindowViewModel.CategoryName.CategoryName);
                    _mainWindowViewModel.AddPicture(pictureViewModel);
                    await this.ShowMessageAsync("Uwaga", "Nie udało się wykonać tego zadania.Sprawdź czy plik o tej nazwie istnieje w folderze na dysku.");
                    return;
                }
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void ShowMovieButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayElementClass.DisplayMovie(((Button)sender).DataContext as MovieViewModel, this);
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }

        private async void MoveMovieButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MovieViewModel movieViewModel = ((Button)sender).DataContext as MovieViewModel;
                if (_mainWindowViewModel.CategoryViewModels.Count() <= 1)
                {
                    await this.ShowMessageAsync("Uwaga", "Aby przenieść film musisz posiadać więcej niż jedną kategorie");
                    return;
                }
                CustomUserInputDialog customUserInputDialog = new CustomUserInputDialog(_mainWindowViewModel.CategoryViewModels.ToList(), this, _mainWindowViewModel.CategoryName);
                await this.ShowMetroDialogAsync(customUserInputDialog);
                await customUserInputDialog.WaitUntilUnloadedAsync();
                if (MoveToCategory is null)
                    return;
                var moviesInCategory = await DatabaseManagementClass.LoadMovies(_mainWindowViewModel.CategoryViewModels.FirstOrDefault(x => x.CategoryName == MoveToCategory));
                if (moviesInCategory.Any(x => x.Title == movieViewModel.Title))
                {
                    await this.ShowMessageAsync("Uwaga", "Film o tej nazwie już istnieje w danej kategorii");
                    return;
                }
                var pathDestination = FileManagementClass.GetPathDestination(movieViewModel.Path, MoveToCategory, "Filmy");
                await DatabaseManagementClass.UpdateMovie(movieViewModel.MovieId, pathDestination, MoveToCategory);
                try
                {
                    _mainWindowViewModel.RemoveMovie(movieViewModel);
                }
                catch
                {
                    await DatabaseManagementClass.UpdateMovie(movieViewModel.MovieId, movieViewModel.Path, _mainWindowViewModel.CategoryName.CategoryName);
                    throw new Exception();
                }
                try
                {
                    FileManagementClass.FileMove(movieViewModel.Path, pathDestination);
                }
                catch
                {
                    await DatabaseManagementClass.UpdateMovie(movieViewModel.MovieId, movieViewModel.Path, _mainWindowViewModel.CategoryName.CategoryName);
                    _mainWindowViewModel.AddMovie(movieViewModel);
                    await this.ShowMessageAsync("Uwaga", "Nie udało się wykonać tego zadania.Sprawdź czy plik o tej nazwie istnieje w folderze na dysku.");
                    return;
                }
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
                await DatabaseManagementClass.DeleteMovie(movieViewModel.MovieId);
                try
                {
                    _mainWindowViewModel.RemoveMovie(movieViewModel);
                }
                catch
                {
                    await DatabaseManagementClass.AddMovie(movieViewModel.Title, movieViewModel.Path, _mainWindowViewModel.CategoryName.CategoryName);
                    throw new Exception();
                }
                try
                {
                    FileManagementClass.FileDelete(movieViewModel.Path);
                }
                catch
                {
                    await DatabaseManagementClass.AddMovie(movieViewModel.Title, movieViewModel.Path, _mainWindowViewModel.CategoryName.CategoryName);
                    _mainWindowViewModel.AddMovie(movieViewModel);
                    await this.ShowMessageAsync("Uwaga", "Nie udało się wykonać tego zadania.Sprawdź czy plik o tej nazwie istnieje w folderze na dysku.");
                    return;
                }
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
                var pathSource = FileManagementClass.DisplayFileDialog(".mp4", "MP4 Files (*.mp4)|*.mp4|All files (*.*)|*.*");
                if (pathSource == null)
                    return;
                if (_mainWindowViewModel.MovieViewModels.Any(x => x.Title == Path.GetFileNameWithoutExtension(pathSource)))
                {
                    await this.ShowMessageAsync("Uwaga", "Film o tej nazwie już istnieje w danej kategorii");
                    return;
                }
                var pathDestination = FileManagementClass.GetPathDestination(pathSource, _mainWindowViewModel.CategoryName.CategoryName, "Filmy");
                await DatabaseManagementClass.AddMovie(Path.GetFileNameWithoutExtension(pathDestination), pathDestination, _mainWindowViewModel.CategoryName.CategoryName);
                Movie movie = await DatabaseManagementClass.GetMovie(Path.GetFileNameWithoutExtension(pathDestination), _mainWindowViewModel.CategoryName.CategoryName);
                try
                {
                    _mainWindowViewModel.AddMovie(movie);
                }
                catch
                {
                    await DatabaseManagementClass.DeleteMovie(movie.MovieId);
                    throw new Exception();
                }
                try
                {
                    FileManagementClass.FileMove(pathSource, pathDestination);
                }
                catch
                {
                    await DatabaseManagementClass.DeleteMovie(movie.MovieId);
                    _mainWindowViewModel.RemoveMovie(movie);
                    await this.ShowMessageAsync("Uwaga", "Nie udało się wykonać tego zadania.Sprawdź czy plik o tej nazwie istnieje w folderze na dysku.");
                    return;
                }
            }
            catch
            {
                await this.ShowMessageAsync("Uwaga", "Coś poszło nie tak");
            }
        }
    }
}
