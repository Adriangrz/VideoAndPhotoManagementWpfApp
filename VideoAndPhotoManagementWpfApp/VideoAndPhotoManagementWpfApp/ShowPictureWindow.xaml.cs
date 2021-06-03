using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VideoAndPhotoManagementWpfApp.ViewModels;

namespace VideoAndPhotoManagementWpfApp
{

    public partial class ShowPictureWindow : MetroWindow
    {
        private PictureWindowViewModel _showPictureWindowViewModel = new PictureWindowViewModel();
        public ShowPictureWindow()
        {
            InitializeComponent();
            DataContext = _showPictureWindowViewModel;
        }

        public void SetPicture(string Path)
        {
            using var stream = File.OpenRead(Path);
            _showPictureWindowViewModel.ImageSource = new BitmapImage();
            _showPictureWindowViewModel.ImageSource.BeginInit();
            _showPictureWindowViewModel.ImageSource.CacheOption = BitmapCacheOption.OnLoad;
            _showPictureWindowViewModel.ImageSource.StreamSource = stream;
            _showPictureWindowViewModel.ImageSource.EndInit();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _showPictureWindowViewModel.ImageSource = null;
        }
    }
}
