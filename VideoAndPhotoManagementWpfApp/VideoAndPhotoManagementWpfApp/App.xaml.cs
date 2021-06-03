using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VideoAndPhotoManagementWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var sc = new ServiceCollection()
                .AddSingleton<MainWindow>()
                .AddSingleton<MainWindowViewModel>();
            var sp = sc.BuildServiceProvider();
            var mv = sp.GetService<MainWindow>();
            mv.Show();
        }
    }
}
