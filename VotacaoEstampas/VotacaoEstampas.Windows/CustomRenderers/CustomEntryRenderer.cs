
using System.ComponentModel;
using VotacaoEstampas.CustomControls;
using VotacaoEstampas.Windows.CustomRenderers;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace VotacaoEstampas.Windows.CustomRenderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);
            var button = e.NewElement;

            var formsTextBox = Control as FormsTextBox;
           // formsTextBox.KeyDown += FormsTextBox_KeyDown;

            if (Control != null)
            {
                Personalize(button);
                Control.Padding = new Thickness(10);
            }
        }

       /* private void FormsTextBox_KeyDown(object sender, global::Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var formsTextBox = sender as FormsTextBox;
            if (e.Key == VirtualKey.Enter)
            {
                Instrumentation inst = new Instrumentation();
                inst.SendKeyDownUpSync(Android.Views.Keycode.Eisu);
            }
        }*/

        private void Personalize(Xamarin.Forms.Entry button)
        {
            Control.ApplyTemplate();
            var borders = Control.GetVisuals<Border>();

            foreach (var border in borders)
            {
                border.Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
                border.CornerRadius = new CornerRadius(20);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "BorderRadius")
            {
                var borders = Control.GetVisuals<Border>();

                foreach (var border in borders)
                {
                    border.Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
                    border.CornerRadius = new CornerRadius(((Xamarin.Forms.Button)sender).BorderRadius);
                }
            }
        }
    }

}
