﻿using System;
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

namespace VideoAndPhotoManagementWpfApp
{
    /// <summary>
    /// Logika interakcji dla klasy ShowPictureWindow.xaml
    /// </summary>
    public partial class ShowPictureWindow : Window
    {
        public ShowPictureWindow(string imagePath)
        {
            InitializeComponent();
            DataContext = imagePath;
        }
    }
}