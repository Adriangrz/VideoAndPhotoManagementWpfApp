using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoAndPhotoManagementWpfApp.ViewModels
{
    public class CustomUserInputDialogViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CategoryViewModel> CategoryViewModels { get; set; }
        public CategoryViewModel CategoryName { get; set; } = null;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
