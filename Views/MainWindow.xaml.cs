using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsTweaks
{
    public partial class MainWindow : Window
    {
        private readonly TweakEngine tweakEngine;
        private readonly Dictionary<int, Action> contentLoaders;

        public MainWindow()
        {
            tweakEngine = new TweakEngine();

            contentLoaders = new Dictionary<int, Action>
            {
                { 0, LoadPerformanceContent },
                { 1, LoadPrivacyContent },
                { 2, LoadNetworkContent },
                { 3, LoadAppearanceContent },
                { 4, LoadServicesContent },
                { 5, LoadAdministrationContent },
                { 6, LoadUtilitiesContent }
            };

            InitializeComponent();
            NavigationList.SelectedIndex = 0;
            LoadPerformanceContent();
        }

        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        private void NavigationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contentLoaders == null || NavigationList.SelectedIndex < 0)
                return;

            if (contentLoaders.ContainsKey(NavigationList.SelectedIndex))
                contentLoaders[NavigationList.SelectedIndex]();
        }

    }
}
