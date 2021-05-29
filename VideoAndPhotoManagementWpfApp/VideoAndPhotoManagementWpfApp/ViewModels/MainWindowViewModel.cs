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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
