using ProjectAlpha.ViewModels;
using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;

namespace ProjectAlpha.Views
{
    public sealed partial class SearchPage : Page
    {
        public SearchViewModel ViewModel { get; set; }

        public SearchPage()
        {
            this.InitializeComponent();

            ViewModel = new SearchViewModel();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (TopCommandBarVisualStateGroup.CurrentState == SmallWidth)
            {
                SearchBox.Width = (e.NewSize.Width - 1.5 * 2) - 48;
            }
            else
            {
                SearchBox.Width = 248;
            }
        }

        private void BackgroundImage_ImageOpened(object sender, RoutedEventArgs e)
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

        private void SearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.UI.Xaml.FrameworkElement", "AllowFocusOnInteraction"))
            {
                SearchBox.AllowFocusOnInteraction = true;
            }
        }

        private async void SetAsDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            if (await ViewModel.SetAsDefaultLocation())
            {
                FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            }
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
