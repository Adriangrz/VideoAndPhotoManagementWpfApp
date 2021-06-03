using DatabaseManager;
using DatabaseManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoAndPhotoManagementWpfApp.DatatbaseManagement
{
    public static class DatabaseManagementClass
    {
        public static async Task AddPicture(string title,string path,string categoryName)
        {
            using var context = new VideoAndPhotoManagementContext();
            Picture picture = new Picture()
            {
                Title = title,
                Path = path
            };
            picture.Category = await context.Categories.SingleOrDefaultAsync(x => x.CategoryName == categoryName);
            await context.AddAsync(picture);
            await context.SaveChangesAsync();
        }
        public static async Task<Picture> GetPicture(string title)
        {
            using var context = new VideoAndPhotoManagementContext();
            return await context.Pictures.SingleOrDefaultAsync(x => x.Title == title);
        }
        public static async Task DeletePicture(Guid pictureId)
        {
            using var context = new VideoAndPhotoManagementContext();
            Picture picture = await context.Pictures.SingleOrDefaultAsync(x => x.PictureId == pictureId);
            context.Pictures.Remove(picture);
            await context.SaveChangesAsync();
        }
        public static async Task UpdatePicture(Guid pictureId, string path, string moveToCategory)
        {
            using var context = new VideoAndPhotoManagementContext();
            Picture picture = await context.Pictures.SingleOrDefaultAsync(x => x.PictureId == pictureId);
            picture.Path = path;
            picture.Category = await context.Categories.SingleOrDefaultAsync(x => x.CategoryName == moveToCategory);
            context.Update(picture);
            await context.SaveChangesAsync();
        }
        public static async Task<ObservableCollection<PictureViewModel>> LoadPicturies(CategoryViewModel categoryViewModel){
            if (categoryViewModel is not null)
            {
                using var context = new VideoAndPhotoManagementContext();
                var pictures = await context.Pictures.Where(x => x.Category.CategoryName == categoryViewModel.CategoryName).Select(x => new PictureViewModel
                {
                    PictureId = x.PictureId,
                    Title = x.Title,
                    Path = x.Path
                }
                ).ToListAsync();
                return new ObservableCollection<PictureViewModel>(pictures);
            }
            return null;
        }

        public static async Task AddMovie(string title, string path, string categoryName)
        {
            using var context = new VideoAndPhotoManagementContext();
            Movie movie = new Movie()
            {
                Title = title,
                Path = path
            };
            movie.Category = await context.Categories.SingleOrDefaultAsync(x => x.CategoryName == categoryName);
            await context.AddAsync(movie);
            await context.SaveChangesAsync();
        }
        public static async Task<Movie> GetMovie(string title)
        {
            using var context = new VideoAndPhotoManagementContext();
            return await context.Movies.SingleOrDefaultAsync(x => x.Title == title);
        }
        public static async Task DeleteMovie(Guid movieId)
        {
            using var context = new VideoAndPhotoManagementContext();
            Movie movie = await context.Movies.SingleOrDefaultAsync(x => x.MovieId == movieId);
            context.Movies.Remove(movie);
            await context.SaveChangesAsync();
        }
        public static async Task UpdateMovie(Guid movieId, string path, string moveToCategory)
        {
            using var context = new VideoAndPhotoManagementContext();
            Movie movie = await context.Movies.SingleOrDefaultAsync(x => x.MovieId == movieId);
            movie.Path = path;
            movie.Category = await context.Categories.SingleOrDefaultAsync(x => x.CategoryName == moveToCategory);
            context.Update(movie);
            await context.SaveChangesAsync();
        }
        public static async Task<ObservableCollection<MovieViewModel>> LoadMovies(CategoryViewModel categoryViewModel)
        {
            if (categoryViewModel is not null)
            {
                using var context = new VideoAndPhotoManagementContext();
                var movies = await context.Movies.Where(x => x.Category.CategoryName == categoryViewModel.CategoryName).Select(x => new MovieViewModel
                {
                    MovieId = x.MovieId,
                    Title = x.Title,
                    Path = x.Path
                }
                ).ToListAsync();
                return new ObservableCollection<MovieViewModel>(movies);
            }
            return null;
        }

        public static async Task AddCategory(string categoryName)
        {
            using var context = new VideoAndPhotoManagementContext();
            var category = new Category()
            {
                CategoryName = categoryName
            };
            await context.AddAsync(category);
            await context.SaveChangesAsync();
        }
        public static async Task DeleteCategory(string categoryName)
        {
            using var context = new VideoAndPhotoManagementContext();
            Category category = await context.Categories.SingleOrDefaultAsync(x => x.CategoryName == categoryName);
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }
        public static async Task<ObservableCollection<CategoryViewModel>> LoadCategories()
        {
            using var context = new VideoAndPhotoManagementContext();
            var categories = await context.Categories.Select(x => new CategoryViewModel
            {
                CategoryName = x.CategoryName
            }
            ).ToListAsync();
            return new ObservableCollection<CategoryViewModel>(categories);
        }
    }
}
