using System;
using System.Collections.Generic;
using System.ComponentModel;
using VotacaoEstampas.Windows.CustomRenderers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(VotacaoEstampas.CustomControls.CustomButton), typeof(CustomButtonRenderer))]
namespace VotacaoEstampas.Windows.CustomRenderers
{
    public class CustomButtonRenderer : ButtonRenderer
    {

        private int CorBackgroundCustomRed;
        private int CorBackgroundCustomGreen;
        private int CorBackgroundCustomBlue;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            var button = e.NewElement;

            if (button != null)
            {
                CorBackgroundCustomRed = (button as CustomControls.CustomButton).CorBackgroundCustomRed;
                CorBackgroundCustomGreen = (button as CustomControls.CustomButton).CorBackgroundCustomGreen;
                CorBackgroundCustomBlue = (button as CustomControls.CustomButton).CorBackgroundCustomBlue;
            }
            if (Control != null)
            {
                button.SizeChanged += OnSizeChanged;
            }
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            var button = (Xamarin.Forms.Button)sender;

            Control.ApplyTemplate();
            var borders = Control.GetVisuals<Border>();

            foreach (var border in borders)
            {
                border.Background = new SolidColorBrush(Color.FromArgb(255, (byte)CorBackgroundCustomRed, (byte)CorBackgroundCustomGreen, (byte)CorBackgroundCustomBlue));
                border.CornerRadius = new CornerRadius(button.BorderRadius);
            }

            button.SizeChanged -= OnSizeChanged;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "BorderRadius")
            {
                var borders = Control.GetVisuals<Border>();

                foreach (var border in borders)
                {
                    border.Background = new SolidColorBrush(Color.FromArgb(255, (byte)CorBackgroundCustomRed, (byte)CorBackgroundCustomGreen, (byte)CorBackgroundCustomBlue));
                    border.CornerRadius = new CornerRadius(((Xamarin.Forms.Button)sender).BorderRadius);
                }
            }
        }
    }

    static class DependencyObjectExtensions
    {
        public static IEnumerable<T> GetVisuals<T>(this DependencyObject root)
            where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(root);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);

                if (child is T)
                    yield return child as T;

                foreach (var descendants in child.GetVisuals<T>())
                {
                    yield return descendants;
                }
            }
        }
    }
}
