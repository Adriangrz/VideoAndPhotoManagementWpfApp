using DatabaseManager.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VideoAndPhotoManagementWpfApp
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CategoryViewModel> CategoryViewModels { get; set; }
        public ObservableCollection<PictureViewModel> PictureViewModels { get; set; }
        public ObservableCollection<MovieViewModel> MovieViewModels { get; set; }
        public bool CategorySelect { get; set; } = false;
        public CategoryViewModel CategoryName { get; set; } = null;

        public void AddMovie(Movie movie)
        {
            MovieViewModel movieViewModel = new MovieViewModel();
            movieViewModel.MovieId = movie.MovieId;
            movieViewModel.Title = movie.Title;
            movieViewModel.Path = movie.Path;
            MovieViewModels.Add(movieViewModel);
        }
        public void AddMovie(MovieViewModel movieViewModel)
        {
            MovieViewModels.Add(movieViewModel);
        }
        public void RemoveMovie(MovieViewModel movieViewModel)
        {
            MovieViewModels.Remove(movieViewModel);
        }
        public void RemoveMovie(Movie movie)
        {
            MovieViewModel movieViewModel = new MovieViewModel();
            movieViewModel.MovieId = movie.MovieId;
            movieViewModel.Title = movie.Title;
            movieViewModel.Path = movie.Path;
            MovieViewModels.Remove(movieViewModel);
        }
        public void AddPicture(Picture picture)
        {
            PictureViewModel pictureViewModel = new PictureViewModel();
            pictureViewModel.PictureId = picture.PictureId;
            pictureViewModel.Title = picture.Title;
            pictureViewModel.Path = picture.Path;
            PictureViewModels.Add(pictureViewModel);
        }
        public void AddPicture(PictureViewModel pictureViewModel)
        {
            PictureViewModels.Add(pictureViewModel);
        }
        public void RemovePicture(Picture picture)
        {
            PictureViewModel pictureViewModel = new PictureViewModel();
            pictureViewModel.PictureId = picture.PictureId;
            pictureViewModel.Title = picture.Title;
            pictureViewModel.Path = picture.Path;
            PictureViewModels.Remove(pictureViewModel);
        }
        public void RemovePicture(PictureViewModel pictureViewModel)
        {
            PictureViewModels.Remove(pictureViewModel);
        }
        public void AddCategory(string categoryName)
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            categoryViewModel.CategoryName = categoryName;
            CategoryViewModels.Add(categoryViewModel);
        }
        public void RemoveCategory(CategoryViewModel categoryViewModel)
        {
            CategoryViewModels.Remove(categoryViewModel);
        }
        public void RemoveCategory(string categoryName)
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            categoryViewModel.CategoryName = categoryName;
            CategoryViewModels.Remove(categoryViewModel);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
