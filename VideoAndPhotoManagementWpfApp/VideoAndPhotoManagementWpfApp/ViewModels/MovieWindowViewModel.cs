using System.ComponentModel;

namespace VideoAndPhotoManagementWpfApp
{
    public class MovieWindowViewModel : INotifyPropertyChanged
    {
        public string MoviePath { get; set; }
        public string LengthOfMovie { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
