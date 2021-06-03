using System.ComponentModel;

namespace VideoAndPhotoManagementWpfApp
{
    public class MovieWindowViewModel : INotifyPropertyChanged
    {
        public string MoviePath { get; set; } = null;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
