using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Logika interakcji dla klasy CustomUserInputDialog.xaml
    /// </summary>
    public partial class CustomUserInputDialog : CustomDialog
    {
        private CustomUserInputDialogViewModel _customUserInputDialogViewModel = new CustomUserInputDialogViewModel();
        private MainWindow _mainWindow;
        public CustomUserInputDialog(List<CategoryViewModel> categoryViewModels,MainWindow mainWindow,CategoryViewModel categoryViewModel)
        {
            InitializeComponent();
            categoryViewModels.Remove(categoryViewModel);
            _customUserInputDialogViewModel.CategoryViewModels = new ObservableCollection<CategoryViewModel>(categoryViewModels);
            _customUserInputDialogViewModel.CategoryName = new CategoryViewModel();
            _customUserInputDialogViewModel.CategoryName.CategoryName = _customUserInputDialogViewModel.CategoryViewModels[0].CategoryName;
            _mainWindow = mainWindow;
            DataContext = _customUserInputDialogViewModel;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MoveToCategory = null;
            _mainWindow.HideMetroDialogAsync(this);
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MoveToCategory = _customUserInputDialogViewModel.CategoryName.CategoryName;
            _mainWindow.HideMetroDialogAsync(this);
        }
    }
}
