using ProjectAlpha.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using System;
using System.Numerics;

namespace ProjectAlpha.Views
{
    public sealed partial class WeatherTemplate : UserControl
    {
        public OpenWeatherObj OpenWeatherObj => (DataContext as OpenWeatherObj);
        private bool firstLoad = true;

        public WeatherTemplate()
        {
            this.InitializeComponent();
            DataContextChanged += delegate
            {
                Bindings.Update();

                if (!OpenWeatherObj.IsAnimated && !firstLoad)
                {
                    loadAnimation();
                }

                if (firstLoad) firstLoad = false;
            };
        }

        private void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            loadAnimation();
        }

        private void loadAnimation()
        {
            OpenWeatherObj.IsAnimated = true;
            Compositor _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var mainPanel_Visual = ElementCompositionPreview.GetElementVisual(MainPanel);
            var detailsPanel_Visual = ElementCompositionPreview.GetElementVisual(DetailsPanel);

            mainPanel_Visual.Size = new Vector2((float)MainPanel.RenderSize.Width, (float)MainPanel.RenderSize.Height);
            mainPanel_Visual.Scale = new Vector3(1f, 1f, 1f);
            mainPanel_Visual.Offset = new Vector3(0, 0, 0);
            mainPanel_Visual.CenterPoint = new Vector3((float)RenderSize.Width / 2, (float)RenderSize.Height / 2, 0);
            mainPanel_Visual.Opacity = 0f;
            detailsPanel_Visual.Opacity = 0f;

            var mainPanel_FadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            mainPanel_FadeAnimation.Target = "Opacity";
            mainPanel_FadeAnimation.InsertKeyFrame(1f, 1f);
            mainPanel_FadeAnimation.Duration = TimeSpan.FromMilliseconds(1500);
            mainPanel_FadeAnimation.DelayTime = TimeSpan.FromMilliseconds(600);

            var mainPanel_ScaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
            mainPanel_ScaleAnimation.Target = "Scale";
            mainPanel_ScaleAnimation.InsertKeyFrame(0, new Vector3(0, 0, 0));
            mainPanel_ScaleAnimation.InsertKeyFrame(1f, new Vector3(1f, 1f, 1f));
            mainPanel_ScaleAnimation.Duration = TimeSpan.FromMilliseconds(1500);
            mainPanel_ScaleAnimation.DelayTime = TimeSpan.FromMilliseconds(600);

            var detailsPanel_FadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            detailsPanel_FadeAnimation.InsertKeyFrame(1f, 1f);
            detailsPanel_FadeAnimation.Duration = TimeSpan.FromMilliseconds(1500);
            detailsPanel_FadeAnimation.DelayTime = TimeSpan.FromMilliseconds(700);

            var mainPanelAnimationGroup = _compositor.CreateAnimationGroup();
            mainPanelAnimationGroup.Add(mainPanel_FadeAnimation);
            mainPanelAnimationGroup.Add(mainPanel_ScaleAnimation);

            mainPanel_Visual.StartAnimationGroup(mainPanelAnimationGroup);
            detailsPanel_Visual.StartAnimation("Opacity", detailsPanel_FadeAnimation);
        }
    }
}
