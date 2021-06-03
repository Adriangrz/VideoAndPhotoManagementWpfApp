using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VideoAndPhotoManagementWpfApp.ViewModels
{
    public class PictureWindowViewModel : INotifyPropertyChanged
    {
        public BitmapImage ImageSource { get; set; } = null;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
