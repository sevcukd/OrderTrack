﻿using OrderTrack.Models;
using OrderTrack.ViewModel;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;


namespace OrderTrack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
        
    }
}