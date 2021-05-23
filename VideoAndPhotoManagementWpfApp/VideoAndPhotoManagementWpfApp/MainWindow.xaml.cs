using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoAndPhotoManagementWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowElement_Click(object sender, RoutedEventArgs e)
        {
            //PlayMovieWindow playMovieWindow = new PlayMovieWindow(@"C:\Users\adi\Downloads\RM-1080p.mp4");
            //playMovieWindow.ShowDialog();
            ShowPictureWindow showPictureWindow = new ShowPictureWindow(@"C:\Users\adi\Pictures\Z02.02_Grzebieniak Adrian.png");
            showPictureWindow.ShowDialog();
        }
    }
}
