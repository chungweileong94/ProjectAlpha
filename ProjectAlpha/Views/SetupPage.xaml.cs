using ProjectAlpha.Services;
using ProjectAlpha.ViewModels;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace ProjectAlpha.Views
{
    public sealed partial class SetupPage : Page
    {
        public SetupViewModel ViewModel { get; set; }

        public SetupPage()
        {
            this.InitializeComponent();
            ViewModel = new SetupViewModel();
        }

        private async void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SetupSettingsAsync();
            Frame.Navigate(typeof(MainPage));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Compositor _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var welcomeTextBlock_Visual = ElementCompositionPreview.GetElementVisual(WelcomeTextBlock);
            var setupPanel_Visual = ElementCompositionPreview.GetElementVisual(SetupPanel);

            welcomeTextBlock_Visual.Opacity = 0;
            setupPanel_Visual.Opacity = 0;
            setupPanel_Visual.CenterPoint = new Vector3((float)SetupPanel.RenderSize.Width / 2, (float)SetupPanel.RenderSize.Height / 2, 0);
            setupPanel_Visual.Offset = new Vector3(0, welcomeTextBlock_Visual.Offset.Y + 200, 0);

            var welcomeFadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            welcomeFadeAnimation.Target = "Opacity";
            welcomeFadeAnimation.InsertKeyFrame(1f, 1f);
            welcomeFadeAnimation.Duration = TimeSpan.FromMilliseconds(2000);
            welcomeFadeAnimation.DelayTime = TimeSpan.FromMilliseconds(0);

            var welcomeOffsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            welcomeOffsetAnimation.Target = "Offset.Y";
            welcomeOffsetAnimation.InsertKeyFrame(1f, -180f);
            welcomeOffsetAnimation.Duration = TimeSpan.FromMilliseconds(1500);
            welcomeOffsetAnimation.DelayTime = TimeSpan.FromMilliseconds(2200);

            var setupPanelFadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            setupPanelFadeAnimation.Target = "Opacity";
            setupPanelFadeAnimation.InsertKeyFrame(1f, 1f);
            setupPanelFadeAnimation.Duration = TimeSpan.FromMilliseconds(2000);
            setupPanelFadeAnimation.DelayTime = TimeSpan.FromMilliseconds(2200);

            var setupPanelOffsetAnimation = _compositor.CreateExpressionAnimation("welcome.Offset.Y + y");
            setupPanelOffsetAnimation.Target = "Offset.Y";
            setupPanelOffsetAnimation.SetReferenceParameter("welcome", welcomeTextBlock_Visual);
            setupPanelOffsetAnimation.SetScalarParameter("y", 200f);

            var welcomeAnimationGroup = _compositor.CreateAnimationGroup();
            welcomeAnimationGroup.Add(welcomeFadeAnimation);
            welcomeAnimationGroup.Add(welcomeOffsetAnimation);

            var setupPanleAnimationGroup = _compositor.CreateAnimationGroup();
            setupPanleAnimationGroup.Add(setupPanelFadeAnimation);
            setupPanleAnimationGroup.Add(setupPanelOffsetAnimation);

            welcomeTextBlock_Visual.StartAnimationGroup(welcomeAnimationGroup);
            setupPanel_Visual.StartAnimationGroup(setupPanleAnimationGroup);
        }
    }
}
