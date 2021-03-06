﻿using ProjectAlpha.Views;
using ProjectAlpha.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ProjectAlpha
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            //AppSettingsService.RemoveSetting(AppSettingsService.DEFAULT_LOCATION_MODE); //testing purpose
            //AppSettingsService.RemoveSetting(AppSettingsService.DEFAULT_LOCATION); //testing purpose
            //AppSettingsService.RemoveSetting(AppSettingsService.DEFAULT_LOCATION_FULL_NAME); //testing purpose
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            //#if DEBUG
            //            if (System.Diagnostics.Debugger.IsAttached)
            //            {
            //                this.DebugSettings.EnableFrameRateCounter = true;
            //            }
            //#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    if (!AppSettingsService.ContainsSetting(AppSettingsService.DEFAULT_LOCATION_MODE))
                    {
                        rootFrame.Navigate(typeof(SetupPage), e.Arguments);
                    }
                    else
                    {
                        if (!LiveTileService.IsTaskExisting()) await LiveTileService.RegisterTaskAsync();

                        rootFrame.Navigate(typeof(MainPage), e.Arguments);
                    }
                }
                // Ensure the current window is active
                Window.Current.Activate();

                var view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
                view.SetPreferredMinSize(new Size(500, 640));
                view.TitleBar.BackgroundColor = Color.FromArgb(255, 40, 40, 40);
                view.TitleBar.InactiveBackgroundColor = Color.FromArgb(255, 40, 40, 40);
                view.TitleBar.ButtonBackgroundColor = Color.FromArgb(255, 40, 40, 40);
                view.TitleBar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 40, 40, 40);

                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    statusBar.BackgroundColor = Color.FromArgb(255, 40, 40, 40);
                    statusBar.BackgroundOpacity = 1;
                    await statusBar.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
