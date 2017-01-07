using ProjectAlpha.Models;
using ProjectAlpha.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ProjectAlpha.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            MainFrame.Navigated += (s, e) =>
            {
                var sender = s as Frame;
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = sender.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

                foreach (var rb in allRadioButtons(this))
                {
                    if (MainFrame.SourcePageType.Name == $"{rb.Content}Page")
                    {
                        rb.IsChecked = true;
                        return;
                    }
                }
            };

            SystemNavigationManager.GetForCurrentView().BackRequested += MainFrame_BackRequested;
        }

        private void MainFrame_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                e.Handled = true;
                MainFrame.GoBack();
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e) => MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;

        private void NavRadioButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as RadioButton;

            switch (s.Content.ToString())
            {
                case "Weather":
                    MainFrame.Navigate(typeof(WeatherPage));
                    break;
                case "Search":
                    MainFrame.Navigate(typeof(SearchPage));
                    break;
                case "Devices":
                    break;
                case "Settings":
                    MainFrame.Navigate(typeof(SettingsPage));
                    break;
            }

            MainSplitView.IsPaneOpen = false;
        }

        #region Helpers
        private List<RadioButton> allRadioButtons(DependencyObject parent)
        {
            var list = new List<RadioButton>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is RadioButton)
                {
                    list.Add(child as RadioButton);
                    continue;
                }

                list.AddRange(allRadioButtons(child));
            }
            return list;
        }
        #endregion
    }
}
