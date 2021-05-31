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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VideoAndPhotoManagementWpfApp
{
    /// <summary>
    /// Logika interakcji dla klasy PlayMovieWindow.xaml
    /// </summary>
    public partial class PlayMovieWindow : MetroWindow
    {
        private MovieWindowViewModel movieWindowViewModel = new MovieWindowViewModel();
        private bool userIsDraggingSlider = false;
        DispatcherTimer timer;
        public PlayMovieWindow(string moviePath)
        {
            InitializeComponent();
            
            movieWindowViewModel.MoviePath = new Uri(moviePath).ToString();
            DataContext = movieWindowViewModel;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += new EventHandler(Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((movieMediaElement.Source != null) && (movieMediaElement.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliderSeek.Minimum = 0;
                sliderSeek.Maximum = movieMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                sliderSeek.Value = movieMediaElement.Position.TotalSeconds;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            movieMediaElement.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            movieMediaElement.Pause();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            movieMediaElement.Close();
        }

        private void SliderSeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!userIsDraggingSlider)
            {
                movieMediaElement.Position = TimeSpan.FromSeconds(sliderSeek.Value);
            }
        }

        private void MovieMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSpan ts = movieMediaElement.NaturalDuration.TimeSpan;
            sliderSeek.Maximum = ts.TotalSeconds;
            timer.Start();
        }

        private void sliProgress_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            movieMediaElement.Position = TimeSpan.FromSeconds(sliderSeek.Value);
        }
    }
}
