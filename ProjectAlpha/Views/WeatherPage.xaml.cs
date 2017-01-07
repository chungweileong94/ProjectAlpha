using ProjectAlpha.ViewModels;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;

namespace ProjectAlpha.Views
{
    public sealed partial class WeatherPage : Page
    {
        public WeatherViewModel ViewModel { get; set; }

        public WeatherPage()
        {
            this.InitializeComponent();
            ViewModel = new WeatherViewModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadCurrentWeatherAsync();
        }

        private async void LocationPermissionButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
        }

        private void BackgroundImage_ImageOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var s = sender as Image;

            Compositor _compositor = ElementCompositionPreview.GetElementVisual(s).Compositor;
            var visual = ElementCompositionPreview.GetElementVisual(s);

            visual.Opacity = 0f;

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, 1f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);
            fadeAnimation.DelayTime = TimeSpan.FromMilliseconds(0);
            visual.StartAnimation("Opacity", fadeAnimation);
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var propSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(sender as ScrollViewer);
            var _compositor = propSet.Compositor;

            var offsetExpressionAnimation = _compositor.CreateExpressionAnimation("ScrollViewer.Translation.Y * m");
            offsetExpressionAnimation.SetReferenceParameter("ScrollViewer", propSet);
            offsetExpressionAnimation.SetScalarParameter("m", 0.3f);

            var backgroundVisual = ElementCompositionPreview.GetElementVisual(BackgroundImage);
            backgroundVisual.StartAnimation("Offset.Y", offsetExpressionAnimation);
        }
    }
}
