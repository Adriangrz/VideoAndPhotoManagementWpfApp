using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoAndPhotoManagementWpfApp.DisplayElement
{
    public static class DisplayElementClass
    {
        public static void DisplayPicture(PictureViewModel categoryViewModelRow, MainWindow mainWindow)
        {
            ShowPictureWindow showPictureWindow = new ShowPictureWindow() { Owner = mainWindow };
            showPictureWindow.SetPicture(categoryViewModelRow.Path);
            showPictureWindow.ShowDialog();
        }
        public static void DisplayMovie(MovieViewModel movieViewModel, MainWindow mainWindow)
        {
            PlayMovieWindow playMovieWindow = new PlayMovieWindow() { Owner = mainWindow };
            playMovieWindow.SetMovie(movieViewModel.Path);
            playMovieWindow.ShowDialog();
        }
    }
}
